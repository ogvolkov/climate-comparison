package org.climate.comparison;

import com.microsoft.azure.storage.StorageException;
import ucar.ma2.InvalidRangeException;

import java.io.IOException;
import java.net.URISyntaxException;
import java.security.InvalidKeyException;

public class Main {
    public static void main(String[] args) throws URISyntaxException, InvalidKeyException, StorageException, IOException, InvalidRangeException {
        String storageConnectionString = System.getenv("CLIMATE_COMPARISON_STORAGE_ACCOUNT");
        TableStorageService tableStorage = new TableStorageService(storageConnectionString, "radiation");

        try (Cdf cdf = new Cdf(args[0])) {
            int placeId = 105989;
            double[] result = cdf.get(52.15, 5.38);

            for (int i = 0; i < 12; i++) {
                tableStorage.save(placeId, i + 1, result[i]);
            }
            System.out.println("Imported " + placeId);
        }
    }
}
