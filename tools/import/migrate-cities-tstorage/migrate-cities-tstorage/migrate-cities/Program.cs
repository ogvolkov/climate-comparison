using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClimateComparison.Migrate.Cities.Progress;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace ClimateComparison.Migrate.Cities
{
    class Program
    {
        private const string OPERATION = "migrate-cities-tstorage";
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
            var placesTable = tableClient.GetTableReference("places");

            var progressRepository = new ProgressRepository(tableClient);
            int start = await progressRepository.Get(OPERATION) ?? 0;

            Console.WriteLine($"Starting from {start}");

            string sqlConnectionString = Environment.GetEnvironmentVariable("CLIMATE_ConnectionStrings__ConnectionString");
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                sqlConnection.Open();

                for (; ; )
                {
                    var places = GetPlaces(start, PLACES_BATCH_SIZE, sqlConnection).ToList();

                    if (places.Count == 0)
                    {
                        break;
                    }

                    foreach (var place in places)
                    {
                        bool insertedRows = false;

                        var allNames = place.AltNames.Split(",").Union(new[] { place.Name })
                            .Select(SanitizeName)
                            .Select(it => it.ToLowerInvariant())
                            .Where(it => !string.IsNullOrWhiteSpace(it))
                            .Distinct();

                        var tasks = allNames.Select(async altName =>
                        {
                            var placeEntity = new PlaceEntity
                            {
                                PartitionKey = altName,
                                RowKey = place.Id.ToString(),
                                Id = place.Id.ToString(),
                                Name = place.Name,
                                AltName = altName,
                                Latitude = place.Latitude,
                                Longitude = place.Longitude,
                                CountryCode = place.CountryCode,
                                Population = place.Population
                            };

                            var getOperation = TableOperation.Retrieve<PlaceEntity>(placeEntity.PartitionKey, placeEntity.RowKey);

                            var existingRecord = await placesTable.ExecuteAsync(getOperation);

                            if (existingRecord.Result == null)
                            {
                                try
                                {
                                    var insertOperation = TableOperation.Insert(placeEntity);
                                    await placesTable.ExecuteAsync(insertOperation);
                                    insertedRows = true;
                                }
                                catch (Exception exception)
                                {
                                    Console.WriteLine($"Cannot insert {place.Name} {altName}: {exception}");
                                }
                            }
                        });

                        await Task.WhenAll(tasks);

                        if (insertedRows)
                        {
                            Console.WriteLine($"{place.Name} imported");
                        }
                        else
                        {
                            Console.WriteLine($"Skipped {place.Name}");
                        }

                        start = place.Id;
                    }

                    await progressRepository.Set(OPERATION, start);
                    Console.WriteLine($"Saved progress {start}");
                }
            }

            Console.WriteLine("Done");
        }

        private static IEnumerable<PlaceRow> GetPlaces(int start, int count, SqlConnection sqlConnection)
        {
            using (var getPlaces = sqlConnection.CreateCommand())
            {
                getPlaces.CommandText = @"
                    SELECT TOP (@count) Id, Name, AltNames, Location.Long, Location.Lat, Population, CountryCode
                    FROM Cities
                    WHERE Id > @startId
                    ORDER BY Id
                ";
                getPlaces.Parameters.AddWithValue("@count", count);
                getPlaces.Parameters.AddWithValue("@startId", start);

                using (var reader = getPlaces.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return new PlaceRow
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            AltNames = reader.GetString(2),
                            Longitude = (float) reader.GetDouble(3),
                            Latitude = (float) reader.GetDouble(4),
                            Population = reader.GetInt32(5),
                            CountryCode = reader.GetString(6)
                        };
                    }
                }
            }
        }

        private static string SanitizeName(string altName)
        {
            // Table storage is unhappy with some characters in the key fields https://docs.microsoft.com/en-us/rest/api/storageservices/Understanding-the-Table-Service-Data-Model?redirectedfrom=MSDN
            var sanitizedAltName = new StringBuilder();

            foreach (char c in altName)
            {
                if (0 <= c && c <= 0x1f)
                {
                    continue;
                }

                if (0x7f <= c && c <= 0x9f)
                {
                    continue;
                }

                if ("/\\#?%".Contains(c))
                {
                    continue;
                }

                sanitizedAltName.Append(c);
            }

            return sanitizedAltName.ToString();
        }
    }
}
