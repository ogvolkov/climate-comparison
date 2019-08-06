using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Migrate.AvgHigh.TableStorage.AverageHigh
{
    public class AverageHighRepository
    {
        private readonly CloudTableClient _cloudTableClient;

        public AverageHighRepository(CloudTableClient cloudTableClient)
        {
            _cloudTableClient = cloudTableClient ?? throw new ArgumentNullException(nameof(cloudTableClient));
        }

        public async Task Save(int placeId, double[] averageHighs)
        {
            var averageHighTable = _cloudTableClient.GetTableReference("avgHigh");

            var saveTasks = Enumerable.Range(1, averageHighs.Length)
                .Select(month => SaveOne(placeId, month, averageHighs[month - 1], averageHighTable));

            await Task.WhenAll(saveTasks);
        }

        private async Task SaveOne(int placeId, int month, double averageHigh, CloudTable averageHighTable)
        {
            var averageHighEntity = new AverageHighEntity
            {
                PartitionKey = placeId.ToString(),
                RowKey = month.ToString("00"),
                AverageHigh = averageHigh
            };
            var replaceOperation = TableOperation.InsertOrReplace(averageHighEntity);

            await averageHighTable.ExecuteAsync(replaceOperation);
        }
    }
}
