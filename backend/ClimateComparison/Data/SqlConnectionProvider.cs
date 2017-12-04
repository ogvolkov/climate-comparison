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
            return new SqlConnection(_databaseOptions.Value.ConnectionString);
        }
    }
}
