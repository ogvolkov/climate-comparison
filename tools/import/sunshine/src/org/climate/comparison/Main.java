package org.climate.comparison;

import com.microsoft.azure.storage.CloudStorageAccount;
import com.microsoft.azure.storage.StorageException;
import com.microsoft.azure.storage.table.CloudTable;
import com.microsoft.azure.storage.table.CloudTableClient;
import ucar.ma2.InvalidRangeException;

import java.io.IOException;
import java.net.URISyntaxException;
import java.security.InvalidKeyException;

public class Main {
    public static void main(String[] args) throws URISyntaxException, IOException, InvalidKeyException, InvalidRangeException, StorageException {
        String cdfFileName = args[0];
        String placesFileName = args[1];
        String storageConnectionString = System.getenv("CLIMATE_COMPARISON_STORAGE_ACCOUNT");

        CloudStorageAccount account = CloudStorageAccount.parse(storageConnectionString);
        CloudTableClient tableClient = account.createCloudTableClient();
        CloudTable radiationTable = tableClient.getTableReference("radiation");

        try (Cdf cdf = new Cdf(cdfFileName)) {
            Importer importer = new Importer(cdf, radiationTable);

            importer.run(placesFileName, 118540, 118642);
        }
    }
}
