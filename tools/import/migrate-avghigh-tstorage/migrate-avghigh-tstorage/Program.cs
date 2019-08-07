using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Migrate.AvgHigh.TableStorage.AverageHigh;
using Migrate.AvgHigh.TableStorage.Progress;

namespace Migrate.AvgHigh.TableStorage
{
    class Program
    {
        private const string OPERATION = "migrate-avghigh-tstorage";
        private const int PLACES_BATCH_SIZE = 20;

        static async Task Main(string[] args)
        {
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                Console.WriteLine("Cancel event triggered");
                Environment.Exit(-1);
            };

            string storageConnectionString = Environment.GetEnvironmentVariable("CLIMATE_COMPARISON_STORAGE_ACCOUNT");
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();

            var progressRepository = new ProgressRepository(tableClient);
            int start = await progressRepository.Get(OPERATION) ?? 0;

            Console.WriteLine($"Starting from {start}");

            var averageHighRepository = new AverageHighRepository(tableClient);

            string sqlConnectionString = Environment.GetEnvironmentVariable("CLIMATE_ConnectionStrings__ConnectionString");
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                sqlConnection.Open();

                for (; ; )
                {
                    var placeIds = GetPlaceIds(start, PLACES_BATCH_SIZE, sqlConnection).ToList();

                    if (placeIds.Count == 0)
                    {
                        break;
                    }

                    foreach (int id in placeIds)
                    {
                        double[] averageHighs;
                        try
                        {
                            averageHighs = GetAverageHighs(id, sqlConnection).ToArray();
                        }
                        catch (SqlException sqlException) when (sqlException.ErrorCode == -2146232060)
                        {
                            Console.WriteLine($"can't get climate for {id}");
                            Console.WriteLine(sqlException);
                            continue;
                        }

                        await averageHighRepository.Save(id, averageHighs);

                        Console.WriteLine($"Processed {id}");

                        start = id;
                    }

                    await progressRepository.Set(OPERATION, start);
                    Console.WriteLine($"Saved progress {start}");
                }
            }
        }

        private static IEnumerable<int> GetPlaceIds(int start, int count, SqlConnection sqlConnection)
        {
            using (var getPlaces = sqlConnection.CreateCommand())
            {
                getPlaces.CommandText = "SELECT TOP (@count) Id FROM Cities WHERE Id > @startId ORDER BY Id";
                getPlaces.Parameters.AddWithValue("@count", count);
                getPlaces.Parameters.AddWithValue("@startId", start);

                using (var reader = getPlaces.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader.GetInt32(0);
                    }
                }
            }
        }

        public static IEnumerable<double> GetAverageHighs(int placeId, SqlConnection sqlConnection)
        {
            using (var getPlaces = sqlConnection.CreateCommand())
            {
                getPlaces.CommandText = @"
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
                ";
                getPlaces.Parameters.AddWithValue("@Id", placeId);
                getPlaces.Parameters.AddWithValue("@AverageYears", 10);

                using (var reader = getPlaces.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader.GetDouble(0);
                    }
                }
            }
        }
    }
}
