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
                    SELECT TOP(@MaxCount) Id, Name, Country
                    FROM Sites S
                    WHERE Name LIKE @SearchPattern
                    AND EXISTS(
                        SELECT AH.Id FROM AverageHigh AH
                        WHERE AH.SiteId = S.Id
                        AND Year BETWEEN YEAR(getdate()) - @AverageYears AND YEAR(getdate())-1
                    )
                    ",
                    new
                    {
                        MaxCount = maxCount,
                        SearchPattern = searchText + "%",
                        AverageYears = 10
                    }
                );
            }
        }
    }
}
