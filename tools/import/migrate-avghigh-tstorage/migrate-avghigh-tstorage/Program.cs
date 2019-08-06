using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Migrate.AvgHigh.TableStorage.Progress;

namespace Migrate.AvgHigh.TableStorage
{
    class Program
    {
        private const string OPERATION = "migrate-avghigh-tstorage";

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

            for (int id = start; id < 1000; id++)
            {
                await progressRepository.Set(OPERATION, id);
                Console.WriteLine($"Processed {id}");
            }
        }
    }
}
