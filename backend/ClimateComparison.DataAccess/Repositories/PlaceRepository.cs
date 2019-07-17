using System.Collections.Generic;
using ClimateComparison.DataAccess.DTO;
using Dapper;

namespace ClimateComparison.DataAccess.Repositories
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
                    WHERE Id IN
	                    (SELECT DISTINCT CN.CityId FROM CityNames CN WHERE CN.Name LIKE @SearchPattern)
                    ORDER BY Population DESC
                    OPTION (LOOP JOIN)
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
