package org.climate.comparison;

import com.microsoft.azure.storage.StorageException;
import ucar.ma2.InvalidRangeException;

import java.io.BufferedReader;
import java.io.FileReader;
import java.io.IOException;
import java.net.URISyntaxException;
import java.security.InvalidKeyException;

public class Main {
    public static void main(String[] args) throws URISyntaxException, InvalidKeyException, StorageException, IOException, InvalidRangeException {
        String storageConnectionString = System.getenv("CLIMATE_COMPARISON_STORAGE_ACCOUNT");
        TableStorageService tableStorage = new TableStorageService(storageConnectionString, "radiation");

        try (Cdf cdf = new Cdf(args[0])) {
            try (BufferedReader placesFile = new BufferedReader(new FileReader(args[1]))) {
                String line;
                while ((line = placesFile.readLine()) != null) {
                    String[] fields = line.split("\\s+");

                    int placeId = Integer.parseInt(fields[0]);
                    double lat = Double.parseDouble(fields[1]);
                    double lon = Double.parseDouble(fields[2]);

                    double[] result = cdf.get(lat, lon);

                    for (int i = 0; i < 12; i++) {
                        tableStorage.save(placeId, i + 1, result[i]);
                    }
                    System.out.println("Imported " + placeId);
                }
            }
        }
    }
}
