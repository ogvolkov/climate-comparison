package org.climate.comparison;

import com.microsoft.azure.storage.CloudStorageAccount;
import com.microsoft.azure.storage.StorageException;
import com.microsoft.azure.storage.queue.CloudQueue;
import com.microsoft.azure.storage.queue.CloudQueueClient;
import com.microsoft.azure.storage.queue.CloudQueueMessage;
import com.microsoft.azure.storage.table.CloudTable;
import com.microsoft.azure.storage.table.CloudTableClient;

import ucar.ma2.InvalidRangeException;
import java.io.IOException;
import java.net.URISyntaxException;
import java.security.InvalidKeyException;

import com.fasterxml.jackson.databind.ObjectMapper;

public class Main {
    public static void main(String[] args) throws URISyntaxException, IOException, InvalidKeyException, InvalidRangeException, StorageException, InterruptedException {
        String cdfFileName = args[0];
        String placesFileName = args[1];
        String storageConnectionString = System.getenv("CLIMATE_COMPARISON_STORAGE_ACCOUNT");

        CloudStorageAccount account = CloudStorageAccount.parse(storageConnectionString);
        CloudTableClient tableClient = account.createCloudTableClient();
        CloudTable radiationTable = tableClient.getTableReference("radiation");

        CloudQueueClient queueClient = account.createCloudQueueClient();
        CloudQueue workQueue = queueClient.getQueueReference("import-radiation");
        CloudQueue deadLetterQueue = queueClient.getQueueReference("import-radiation-dlq");

        ObjectMapper mapper = new ObjectMapper();

        try (Cdf cdf = new Cdf(cdfFileName)) {
            Importer importer = new Importer(cdf, radiationTable);

            for (;;) {
                try {
                    CloudQueueMessage message = workQueue.retrieveMessage();
                    if (message == null) {
                        Thread.sleep(2000);
                        continue;
                    }

                    String content = message.getMessageContentAsString();
                    System.out.println("Got message " + content);

                    try {
                        ImportMessage importMessage = mapper.readValue(content, ImportMessage.class);
                        importer.run(placesFileName, importMessage.from, importMessage.to);
                    } catch (Exception exception) {
                        exception.printStackTrace();
                        deadLetterQueue.addMessage(new CloudQueueMessage(content));
                    }
                    workQueue.deleteMessage(message);
                } catch (Exception exception) {
                    exception.printStackTrace();
                }
            }
        }
    }
}
