using System;
using System.Linq;
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

        public Temperature GetTemperature(int placeId)
        {
            var avgHighTable = _cloudTableClientProvider.Get().GetTableReference("avgHigh");

            var query = new TableQuery<AverageHighEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, placeId.ToString()));

            TableQuerySegment<AverageHighEntity> resultsSegment = avgHighTable.ExecuteQuerySegmentedAsync(query, null).GetAwaiter().GetResult();

            double[] averageHighs = resultsSegment.OrderBy(it => Convert.ToInt32(it.RowKey))
                .Select(it => it.AverageHigh).ToArray();

            return new Temperature
            {
                MonthlyAverageHighs = averageHighs
            };
        }

        public Precipitation GetPrecipitation(int placeId)
        {
            var precipitationTable = _cloudTableClientProvider.Get().GetTableReference("precipitation");

            var query = new TableQuery<PrecipitationEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, placeId.ToString()));

            TableQuerySegment<PrecipitationEntity> resultsSegment = precipitationTable.ExecuteQuerySegmentedAsync(query, null).GetAwaiter().GetResult();

            double[] averages = resultsSegment.OrderBy(it => Convert.ToInt32(it.RowKey))
                .Select(it => it.Average).ToArray();

            return new Precipitation
            {
                MonthlyAverages = averages
            };
        }
    }
}
