using System;
using System.Linq;
using System.Threading.Tasks;
using ClimateComparison.DataAccess.DTO;
using ClimateComparison.DataAccess.Entities;
using ClimateComparison.DataAccess.Infra;
using Microsoft.WindowsAzure.Storage.Table;

namespace ClimateComparison.DataAccess.Repositories
{
    public class ClimateRepository
    {
        private readonly CloudTableClientProvider _cloudTableClientProvider;

        public ClimateRepository(CloudTableClientProvider cloudTableClientProvider)
        {
            _cloudTableClientProvider = cloudTableClientProvider ?? throw new ArgumentNullException(nameof(cloudTableClientProvider));
        }

        public async Task<Temperature> GetTemperature(int placeId)
        {
            var avgHighTable = _cloudTableClientProvider.Get().GetTableReference("avgHigh");

            var query = new TableQuery<AverageHighEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, placeId.ToString()));

            TableQuerySegment<AverageHighEntity> resultsSegment = await avgHighTable.ExecuteQuerySegmentedAsync(query, null);

            double[] averageHighs = resultsSegment.OrderBy(it => Convert.ToInt32(it.RowKey))
                .Select(it => it.AverageHigh).ToArray();

            return new Temperature
            {
                MonthlyAverageHighs = averageHighs
            };
        }

        public async Task<Precipitation> GetPrecipitation(int placeId)
        {
            var precipitationTable = _cloudTableClientProvider.Get().GetTableReference("precipitation");

            var query = new TableQuery<PrecipitationEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, placeId.ToString()));

            TableQuerySegment<PrecipitationEntity> resultsSegment = await precipitationTable.ExecuteQuerySegmentedAsync(query, null);

            double[] averages = resultsSegment.OrderBy(it => Convert.ToInt32(it.RowKey))
                .Select(it => it.Average).ToArray();

            return new Precipitation
            {
                MonthlyAverages = averages
            };
        }
    }
}
