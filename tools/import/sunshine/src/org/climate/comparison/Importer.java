package org.climate.comparison;

import com.microsoft.azure.storage.StorageException;
import ucar.ma2.InvalidRangeException;

import java.io.BufferedReader;
import java.io.FileReader;
import java.io.IOException;

public class Importer {
    private Cdf cdf;

    private TableStorageService tableStorage;

    public Importer(Cdf cdf, TableStorageService tableStorage) {
        this.cdf = cdf;
        this.tableStorage = tableStorage;
    }

    public void run(String placesFileName, int from, int to) throws IOException, InvalidRangeException, StorageException {
        try (BufferedReader placesFile = new BufferedReader(new FileReader(placesFileName))) {
            String line;
            while ((line = placesFile.readLine()) != null) {
                String[] fields = line.split("\\s+");

                int placeId = Integer.parseInt(fields[0]);
                double lat = Double.parseDouble(fields[1]);
                double lon = Double.parseDouble(fields[2]);

                if (placeId < from) continue;
                if (placeId > to) break;

                double[] result = cdf.get(lat, lon);

                for (int i = 0; i < 12; i++) {
                    tableStorage.save(placeId, i + 1, result[i]);
                }

                System.out.println("Imported " + placeId);
            }
        }
    }
}
