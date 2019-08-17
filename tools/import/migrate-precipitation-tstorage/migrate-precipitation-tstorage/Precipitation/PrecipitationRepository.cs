using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Migrate.Precipitation.TableStorage.Precipitation
{
    public class PrecipitationRepository
    {
        private readonly CloudTableClient _cloudTableClient;

        public PrecipitationRepository(CloudTableClient cloudTableClient)
        {
            _cloudTableClient = cloudTableClient ?? throw new ArgumentNullException(nameof(cloudTableClient));
        }

        public async Task Save(int placeId, double[] monthlyAverages)
        {
            var precipitationTable = _cloudTableClient.GetTableReference("precipitation");

            var saveTasks = Enumerable.Range(1, monthlyAverages.Length)
                .Select(month => SaveOne(placeId, month, monthlyAverages[month - 1], precipitationTable));

            await Task.WhenAll(saveTasks);
        }

        private async Task SaveOne(int placeId, int month, double average, CloudTable precipitationTable)
        {
            var precipitationEntity = new PrecipitationEntity
            {
                PartitionKey = placeId.ToString(),
                RowKey = month.ToString("00"),
                Average = average
            };
            var replaceOperation = TableOperation.InsertOrReplace(precipitationEntity);

            await precipitationTable.ExecuteAsync(replaceOperation);
        }
    }
}
