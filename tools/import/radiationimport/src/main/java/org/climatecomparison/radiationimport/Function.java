package org.climatecomparison.radiationimport;

import com.microsoft.azure.functions.annotation.*;
import com.microsoft.azure.functions.*;
import com.microsoft.azure.storage.CloudStorageAccount;
import com.microsoft.azure.storage.StorageException;
import com.microsoft.azure.storage.table.CloudTable;
import com.microsoft.azure.storage.table.CloudTableClient;
import ucar.ma2.InvalidRangeException;

import java.io.IOException;
import java.net.URISyntaxException;
import java.security.InvalidKeyException;

public class Function {
    @FunctionName("ImportRadiation")
    public void run(
            @QueueTrigger(name = "message", queueName = "import-radiation", connection = "QueueStorageConnection") QueueMessage message,
            final ExecutionContext context) throws IOException, URISyntaxException, InvalidKeyException, StorageException, InvalidRangeException {
        context.getLogger().info("Queue message: " + message.from + " " + message.to);

        String cdfFileName = System.getenv("CDF_FILE_NAME");
        String placesFileName = System.getenv("PLACES_FILE_NAME");
        String storageConnectionString = System.getenv("CLIMATE_COMPARISON_STORAGE_ACCOUNT");

        context.getLogger().info("go");
        CloudStorageAccount account = CloudStorageAccount.parse(storageConnectionString);
        CloudTableClient tableClient = account.createCloudTableClient();
        CloudTable radiationTable = tableClient.getTableReference("radiation");

        context.getLogger().info("before try");

        try (Cdf cdf = new Cdf(cdfFileName)) {
            Importer importer = new Importer(cdf, radiationTable);
            importer.run(placesFileName, message.from, message.to);
        }
        context.getLogger().info("Imported one");
    }
}
