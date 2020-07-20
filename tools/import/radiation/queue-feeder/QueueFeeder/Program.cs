using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using MoreLinq;
using Newtonsoft.Json;

namespace QueueFeeder
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                Console.WriteLine("Cancel event triggered");
                Environment.Exit(-1);
            };

            var fileName = args[0];
            var batchSize = int.Parse(args[1]);

            var connectionString = Environment.GetEnvironmentVariable("CLIMATE_COMPARISON_STORAGE_ACCOUNT");
            QueueClient queueClient = new QueueClient(connectionString, "import-radiation");

            using (var streamReader = new StreamReader(fileName))
            {
                var batches = ListCities().Batch(batchSize);

                foreach (IEnumerable<int> batch in batches)
                {
                    var mBatch = batch.ToList();
                    Console.WriteLine(mBatch.First() + " " + mBatch.Last());

                    var message = new ImportMessage
                    {
                        From = mBatch.First(),
                        To = mBatch.Last()
                    };

                    var json = JsonConvert.SerializeObject(message);
                    await queueClient.SendMessageAsync(json);
                }

                IEnumerable<int> ListCities()
                {
                    for (;;)
                    {
                        var line = streamReader.ReadLine();
                        if (line == null)
                        {
                            yield break;
                        }

                        var parts = line.Split('\t');
                        var id = int.Parse(parts[0].Trim());

                        yield return id;
                    }

                }
            }
        }
    }
}
