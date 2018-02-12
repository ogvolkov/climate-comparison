using ClimateComparison.Models;
using Dapper;
using System.Collections.Generic;

namespace ClimateComparison.Data
{
    public class PlaceRepository
    {
        private readonly SqlConnectionProvider _sqlConnectionProvider;

        public PlaceRepository(SqlConnectionProvider sqlConnectionProvider)
        {
            _sqlConnectionProvider = sqlConnectionProvider ?? throw new System.ArgumentNullException(nameof(sqlConnectionProvider));
        }

        public IEnumerable<Place> Find(string searchText, int maxCount)
        {
            using (var connection = _sqlConnectionProvider.Get())
            {
                return connection.Query<Place>(@"
                    SELECT TOP(@MaxCount) Id, Name, CountryCode AS Country
                    FROM Cities
                    WHERE Name LIKE @SearchPattern
                    ORDER BY Population DESC
                    ",
                    new
                    {
                        MaxCount = maxCount,
                        SearchPattern = searchText + "%",
                    }
                );
            }
        }
    }
}
