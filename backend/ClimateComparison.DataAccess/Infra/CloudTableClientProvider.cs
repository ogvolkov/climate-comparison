using System;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace ClimateComparison.DataAccess.Infra
{
    public class CloudTableClientProvider
    {
        private readonly IConfiguration _configuration;

        public CloudTableClientProvider(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public CloudTableClient Get()
        {
            var storageAccount = CloudStorageAccount.Parse(_configuration.GetSection("STORAGE_ACCOUNT").Value);
            var tableClient = storageAccount.CreateCloudTableClient();
            return tableClient;
        }
    }
}
