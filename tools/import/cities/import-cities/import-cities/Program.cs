using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace ClimateComparison.Import.Cities
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string fileName = args[0];

            using (var streamReader = new StreamReader(fileName))
            {
                string storageConnectionString = Environment.GetEnvironmentVariable("CLIMATE_COMPARISON_STORAGE_ACCOUNT");
                var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
                var tableClient = storageAccount.CreateCloudTableClient();
                var placesTable = tableClient.GetTableReference("places");

                for (; ;)
                {
                    string line = streamReader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }

                    string[] fields = line.Split('\t');

                    string id = fields[0].Trim();
                    string name = fields[1];
                    string altNamesString = fields[3];
                    float latitude = float.Parse(fields[4]);
                    float longitude = float.Parse(fields[5]);
                    string countryCode = fields[8];
                    int population = int.Parse(fields[14]);

                    bool insertedRows = false;

                    var tasks = GetAltNames(altNamesString).Select(async altName =>
                    {
                        var placeEntity = new PlaceEntity
                        {
                            PartitionKey = altName,
                            RowKey = id,
                            Id = id,
                            Name = name,
                            AltName = altName,
                            Latitude = latitude,
                            Longitude = longitude,
                            CountryCode = countryCode,
                            Population = population,
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
                                Console.WriteLine($"Cannot insert {name} {altName}: {exception}");
                            }
                        }
                    });

                    await Task.WhenAll(tasks);

                    if (insertedRows)
                    {
                        Console.WriteLine($"{name} imported");
                    }
                    else
                    {
                        Console.WriteLine($"Skipped {name}");
                    }
                }
            }

            Console.WriteLine("Done");
        }

        private static IEnumerable<string> GetAltNames(string altNamesString)
        {
            return altNamesString.Split(",")
                .Select(SanitizeName)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct();
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
