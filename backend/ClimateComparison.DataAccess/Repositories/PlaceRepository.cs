using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClimateComparison.DataAccess.DTO;
using ClimateComparison.DataAccess.Entities;
using ClimateComparison.DataAccess.Infra;
using Microsoft.WindowsAzure.Storage.Table;

namespace ClimateComparison.DataAccess.Repositories
{
    public class PlaceRepository
    {
        private readonly CloudTableClientProvider _cloudTableClientProvider;

        public PlaceRepository(CloudTableClientProvider cloudTableClientProvider)
        {
            _cloudTableClientProvider = cloudTableClientProvider ?? throw new ArgumentNullException(nameof(cloudTableClientProvider));
        }

        public async Task<IReadOnlyCollection<Place>> Find(string searchText, int maxCount)
        {
            if (searchText == null)
            {
                throw new ArgumentNullException(nameof(searchText));
            }

            if (maxCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxCount));
            }

            if (searchText.Length < 3)
            {
                throw new ArgumentException("Search string is too short", nameof(searchText));
            }

            var searchTextLower = searchText.ToLowerInvariant();
            var placesTable = _cloudTableClientProvider.Get().GetTableReference("places");

            var length = searchTextLower.Length - 1;
            var nextChar = searchTextLower[length] + 1;

            var startWithEnd = searchTextLower.Substring(0, length) + (char)nextChar;
            var filter = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.GreaterThanOrEqual, searchTextLower),
                TableOperators.And,
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.LessThan, startWithEnd)
            );

            var query = new TableQuery<PlaceEntity>();
            query.Where(filter);
            query.TakeCount = 1000;

            TableQuerySegment<PlaceEntity> resultsSegment = await placesTable.ExecuteQuerySegmentedAsync(query, null);

            var results = resultsSegment
                .GroupBy(p => new
                {
                    Id = p.Id,
                    Name = p.Name,
                    Population = p.Population,
                    CountryCode = p.CountryCode
                })
                .Select(g => g.Key)
                .OrderByDescending(it => it.Population)
                .Take(maxCount);

            return results.Select(it => new Place
            {
                Id = int.Parse(it.Id),
                Name = it.Name,
                Country = it.CountryCode
            }).ToList();
        }
    }
}
