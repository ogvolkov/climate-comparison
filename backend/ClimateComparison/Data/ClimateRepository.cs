
using ClimateComparison.Models;
using Dapper;
using System.Linq;

namespace ClimateComparison.Data
{
    public class ClimateRepository
    {
        private readonly SqlConnectionProvider _sqlConnectionProvider;

        public ClimateRepository(SqlConnectionProvider sqlConnectionProvider)
        {
            _sqlConnectionProvider = sqlConnectionProvider ?? throw new System.ArgumentNullException(nameof(sqlConnectionProvider));
        }

        public Climate Get(int placeId)
        {
            using (var connection = _sqlConnectionProvider.Get())
            {
                var averageHigh = connection.Query<double>(@"
                    SELECT ROUND(AVG(Temperature), 1)
                    FROM AverageHigh
                    WHERE SiteId = @Id
                    AND Year > YEAR(getdate()) - @AverageYears
                    AND Year < YEAR(getdate())
                    GROUP BY Month
                    ORDER BY Month
                    ",
                    new
                    {
                        Id = placeId,
                        AverageYears = 10
                    }
                ).ToArray();

                return new Climate
                {
                    AverageHighs = averageHigh
                };
            }
        }
    }
}
