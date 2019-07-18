﻿using System.Linq;
using ClimateComparison.DataAccess.DTO;
using ClimateComparison.DataAccess.Infra;
using Dapper;

namespace ClimateComparison.DataAccess.Repositories
{
    public class ClimateRepository
    {
        private readonly SqlConnectionProvider _sqlConnectionProvider;

        public ClimateRepository(SqlConnectionProvider sqlConnectionProvider)
        {
            _sqlConnectionProvider = sqlConnectionProvider ?? throw new System.ArgumentNullException(nameof(sqlConnectionProvider));
        }

        public Temperature GetTemperature(int placeId)
        {
            using (var connection = _sqlConnectionProvider.Get())
            {
                var result = connection.Query<double>(@"
                    DECLARE @g geography = (SELECT TOP(1) Location FROM Cities WHERE Id = @Id)

                    DECLARE @NearestStations TABLE(Id INT, Name NVARCHAR(255), Distance FLOAT, Weight FLOAT)
                    
                    INSERT INTO @NearestStations
                    SELECT TOP(7) Id, Name, Location.STDistance(@g), POWER(Location.STDistance(@g), -2)
                    FROM Stations
                    WHERE Location.STDistance(@g) IS NOT NULL AND Location.STDistance(@g) < 50000    
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
                );

                return new Temperature
                {
                    MonthlyAverageHighs = result.ToArray(),
                };
            }
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