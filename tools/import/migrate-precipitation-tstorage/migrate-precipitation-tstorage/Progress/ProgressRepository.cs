using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Migrate.Precipitation.TableStorage.Progress
{
    public class ProgressRepository
    {
        private readonly CloudTableClient _cloudTableClient;

        public ProgressRepository(CloudTableClient cloudTableClient)
        {
            _cloudTableClient = cloudTableClient ?? throw new ArgumentNullException(nameof(cloudTableClient));
        }

        public async Task<int?> Get(string operation)
        {
            var progressTable = _cloudTableClient.GetTableReference("progress");
            var getOperation = TableOperation.Retrieve<ProgressEntity>(operation, operation);

            var existingRecord = await progressTable.ExecuteAsync(getOperation);
            return (existingRecord.Result as ProgressEntity)?.State;
        }

        public async Task Set(string operation, int state)
        {
            var progressEntity = new ProgressEntity
            {
                PartitionKey = operation,
                RowKey = operation,
                State = state
            };
            var progressTable = _cloudTableClient.GetTableReference("progress");
            var replaceOperation = TableOperation.InsertOrReplace(progressEntity);

            await progressTable.ExecuteAsync(replaceOperation);
        }
    }
}
