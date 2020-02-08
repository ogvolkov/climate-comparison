using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ClimateComparison.DataAccess.Infra;
using ClimateComparison.DataAccess.Repositories;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace ClimateComparison.IntegrationTests
{
    [TestFixture]
    public class PlaceRepositoryTests
    {
        private PlaceRepository _placeRepository;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var builder = new ConfigurationBuilder();
            builder.AddEnvironmentVariables("CLIMATE_COMPARISON_");
            var configuration = builder.Build();
            var connectionProvider = new CloudTableClientProvider(configuration);
            _placeRepository = new PlaceRepository(connectionProvider);
        }

        [Test]
        public async Task FindsPlaceByNameStart()
        {
            // arrange
            string searchText = "Utr";

            // act
            var results = await _placeRepository.Find(searchText, 10);

            // assert
            Assert.That(results, Is.Not.Empty);
            Assert.That(results.First().Name, Is.EqualTo("Utrecht"));
        }

        [Test]
        public async Task FindsPlaceByAlternativeNameStart()
        {
            // arrange
            string searchText = "Moskva";

            // act
            var results = await _placeRepository.Find(searchText, 10);

            // assert
            Assert.That(results, Is.Not.Empty);
            Assert.That(results.First().Name, Is.EqualTo("Moscow"));
        }

        [Test]
        public async Task PerformsCaseInsensitiveSearch()
        {
            // arrange
            string searchText = "cHicAgO";

            // act
            var results = await _placeRepository.Find(searchText, 10);

            // assert
            Assert.That(results, Is.Not.Empty);
            Assert.That(results.First().Name, Is.EqualTo("Chicago"));
        }

        [Test]
        public async Task SelectsMostRelevantPlace()
        {
            // arrange
            string searchText = "London";

            // act
            var results = await _placeRepository.Find(searchText, 10);

            // assert
            Assert.That(results, Is.Not.Empty);
            Assert.That(results.First().Name, Is.EqualTo("London"));
            Assert.That(results.First().Country, Is.EqualTo("GB"));
        }

        [Test]
        public async Task WorksWithLatinExtraCharacters()
        {
            // arrange
            string searchText = "Mün";

            // act
            var results = await _placeRepository.Find(searchText, 10);

            // assert
            Assert.That(results, Is.Not.Empty);
            Assert.That(results.First().Name, Is.EqualTo("Munich"));
            Assert.That(results.First().Country, Is.EqualTo("DE"));
        }

        [Test]
        public async Task WorksWithCyrillic()
        {
            // arrange
            string searchText = "Моск";

            // act
            var results = await _placeRepository.Find(searchText, 10);

            // assert
            Assert.That(results, Is.Not.Empty);
            Assert.That(results.First().Name, Is.EqualTo("Moscow"));
            Assert.That(results.First().Country, Is.EqualTo("RU"));
        }

        [Test]
        public async Task SelectsNoMoreThanMaximum()
        {
            // arrange
            string searchText = "San";
            int maxResults = 5;

            // act
            var results = await _placeRepository.Find(searchText, maxResults);

            // assert
            Assume.That(results, Is.Not.Empty);
            Assert.That(results.Count, Is.LessThanOrEqualTo(maxResults));
        }

        [Test]
        public async Task ReturnsEmptyResultIfNothingWasFound()
        {
            // arrange
            string searchText = "spkt";

            // act
            var results = await _placeRepository.Find(searchText, 10);

            // assert
            Assert.That(results, Is.Empty);
        }

        [Test]
        public async Task PerformanceIsAcceptable()
        {
            // arrange
            string searchText = "San";
            var stopwatch = new Stopwatch();

            // act
            stopwatch.Start();
            await _placeRepository.Find(searchText, 10);
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            // assert
            Assert.That(elapsedMs, Is.LessThanOrEqualTo(500));
        }

        [TestCase("Am")]
        [TestCase("A")]
        [TestCase("")]
        public void ThrowsIfSearchStringIsTooShort(string searchText)
        {
            // act + assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _placeRepository.Find(searchText, 10));
        }

        [Test]
        public async Task DoesNotReturnFullDuplicates()
        {
            // arrange
            string searchText = "Amers";

            // act
            var results = await _placeRepository.Find(searchText, 10);

            // assert
            var duplicates = results.GroupBy(it => new { it.Id }).Where(g => g.Count() > 1).Select(g => g.Key);
            Assert.That(duplicates, Is.Empty);
        }
    }
}
