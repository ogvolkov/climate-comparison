package org.climatecomparison.radiationimport;

import java.util.*;
import com.microsoft.azure.functions.annotation.*;
import com.microsoft.azure.functions.*;

public class Function {
    @FunctionName("ImportRadiation")
    public void run(
            @QueueTrigger(name = "message", queueName = "import-radiation", connection = "QueueStorageConnection") QueueMessage message,
            final ExecutionContext context) {
        context.getLogger().info("Queue message: " + message.from + " " + message.to);
    }
}
