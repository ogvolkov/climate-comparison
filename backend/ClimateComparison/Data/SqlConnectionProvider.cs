using System;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;

namespace ClimateComparison.Data
{
    public class SqlConnectionProvider
    {
        private readonly IOptions<DatabaseOptions> _databaseOptions;

        public SqlConnectionProvider(IOptions<DatabaseOptions> databaseOptions)
        {
            _databaseOptions = databaseOptions ?? throw new System.ArgumentNullException(nameof(databaseOptions));
        }

        public SqlConnection Get()
        {
            string connectionString = _databaseOptions.Value.ConnectionString;

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string is not present");
            }

            return new SqlConnection(connectionString);
        }
    }
}
