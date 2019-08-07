using System;
using System.Linq;
using ClimateComparison.DataAccess.DTO;
using ClimateComparison.DataAccess.Entities;
using ClimateComparison.DataAccess.Infra;
using Dapper;
using Microsoft.WindowsAzure.Storage.Table;

namespace ClimateComparison.DataAccess.Repositories
{
    public class ClimateRepository
    {
        private readonly SqlConnectionProvider _sqlConnectionProvider;

        private readonly CloudTableClientProvider _cloudTableClientProvider;

        public ClimateRepository(SqlConnectionProvider sqlConnectionProvider, CloudTableClientProvider cloudTableClientProvider)
        {
            _sqlConnectionProvider = sqlConnectionProvider ?? throw new System.ArgumentNullException(nameof(sqlConnectionProvider));
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
            using (var connection = _sqlConnectionProvider.Get())
            {
                var result = connection.Query<double>(@"
                    DECLARE @g geography = (SELECT TOP(1) Location FROM Cities WHERE Id = @Id)

                    SELECT ROUND(
		                    SUM(Precipitation * POWER(Location.STDistance(@g), -2))
		                    / SUM(POWER(Location.STDistance(@g), -2)), 1)
                    FROM Precipitation
                    WHERE Year > YEAR(getdate()) - @AverageYears
                    AND Year < YEAR(getdate())
                    AND Location.STDistance(@g) IS NOT NULL AND Location.STDistance(@g) < 50000
                    GROUP BY Month
                    ORDER BY Month
                    ",
                    new
                    {
                        Id = placeId,
                        AverageYears = 5
                    }
                );

                return new Precipitation
                {
                    MonthlyAverages = result.ToArray()
                };
            }
        }
    }
}
