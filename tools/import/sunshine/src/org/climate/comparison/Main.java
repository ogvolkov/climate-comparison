package org.climate.comparison;

import com.microsoft.azure.storage.CloudStorageAccount;
import com.microsoft.azure.storage.StorageException;
import com.microsoft.azure.storage.table.*;

import java.net.URISyntaxException;
import java.security.InvalidKeyException;

public class Main {
    public static void main(String[] args) throws URISyntaxException, InvalidKeyException, StorageException {
        String storageConnectionString = System.getenv("CLIMATE_COMPARISON_STORAGE_ACCOUNT");
        CloudStorageAccount account = CloudStorageAccount.parse(storageConnectionString);
        CloudTableClient tableClient = account.createCloudTableClient();
        CloudTable table = tableClient.getTableReference("radiation");

        var radiationRecord = new org.climate.comparison.Radiation(105989,7, 19545431.4);
        var replaceOperation = TableOperation.insertOrReplace(radiationRecord);
        table.execute(replaceOperation);
    }
}
