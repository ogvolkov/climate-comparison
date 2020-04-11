package org.climate.comparison;

import com.microsoft.azure.storage.CloudStorageAccount;
import com.microsoft.azure.storage.StorageException;
import com.microsoft.azure.storage.table.CloudTable;
import com.microsoft.azure.storage.table.CloudTableClient;
import com.microsoft.azure.storage.table.TableOperation;

import java.net.URISyntaxException;
import java.security.InvalidKeyException;

public class TableStorageService {
    CloudTable table;

    public TableStorageService(String storageConnectionString, String tableName) throws URISyntaxException, InvalidKeyException, StorageException {
        CloudStorageAccount account = CloudStorageAccount.parse(storageConnectionString);
        CloudTableClient tableClient = account.createCloudTableClient();
        this.table = tableClient.getTableReference(tableName);
    }

    public void save(int placeId, int month, double radiation) throws StorageException {
        var radiationRecord = new org.climate.comparison.RadiationEntity(placeId, month, radiation);
        var replaceOperation = TableOperation.insertOrReplace(radiationRecord);
        table.execute(replaceOperation);
    }
}
