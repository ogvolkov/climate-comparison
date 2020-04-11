package org.climate.comparison;

import com.microsoft.azure.storage.StorageException;
import ucar.ma2.InvalidRangeException;

import java.io.IOException;
import java.net.URISyntaxException;
import java.security.InvalidKeyException;

public class Main {
    public static void main(String[] args) throws URISyntaxException, IOException, InvalidKeyException, InvalidRangeException, StorageException {
        String cdfFileName = args[0];
        String placesFileName = args[1];
        String storageConnectionString = System.getenv("CLIMATE_COMPARISON_STORAGE_ACCOUNT");

        TableStorageService tableStorage = new TableStorageService(storageConnectionString, "radiation");

        try (Cdf cdf = new Cdf(cdfFileName)) {
            Importer importer = new Importer(cdf, tableStorage);

            importer.run(placesFileName, 118540, 118642);
        }
    }
}
