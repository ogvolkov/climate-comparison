
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
                    DECLARE @g geography = (SELECT TOP(1) Location FROM Cities WHERE Id = @Id)

                    DECLARE @NearestStations TABLE(Id INT, Name NVARCHAR(255), Distance FLOAT, Weight FLOAT)
                    
                    INSERT INTO @NearestStations
                    SELECT TOP(7) Id, Name, Location.STDistance(@g), POWER(Location.STDistance(@g), -2)
                    FROM Stations
                    WHERE Location.STDistance(@g) IS NOT NULL  
                    ORDER BY Location.STDistance(@g);
                    
                    SELECT ROUND(SUM(S.Weight * AH.Temperature) / SUM(S.Weight), 1)
                    FROM AverageHigh AH
                    INNER JOIN @NearestStations S
                    ON AH.StationId = S.Id
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
