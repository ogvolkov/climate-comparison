using Microsoft.WindowsAzure.Storage;
using Migrate.Precipitation.TableStorage.Precipitation;
using Migrate.Precipitation.TableStorage.Progress;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Migrate.Precipitation.TableStorage
{
    class Program
    {
        private const string OPERATION = "migrate-precipitation-tstorage";
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

            var repository = new PrecipitationRepository(tableClient);

            string sqlConnectionString = Environment.GetEnvironmentVariable("CLIMATE_COMPARISON_ConnectionStrings__ConnectionString");
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
                        double[] precipitation;
                        try
                        {
                            precipitation = GetPrecipitation(id, sqlConnection).ToArray();
                        }
                        catch (SqlException sqlException) when (sqlException.ErrorCode == -2146232060)
                        {
                            Console.WriteLine($"can't get precipitation for {id}");
                            Console.WriteLine(sqlException);
                            continue;
                        }

                        await repository.Save(id, precipitation);

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

        public static IEnumerable<double> GetPrecipitation(int placeId, SqlConnection sqlConnection)
        {
            using (var getPlaces = sqlConnection.CreateCommand())
            {
                getPlaces.CommandText = @"
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
                ";
                getPlaces.Parameters.AddWithValue("@Id", placeId);
                getPlaces.Parameters.AddWithValue("@AverageYears", 5);

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
