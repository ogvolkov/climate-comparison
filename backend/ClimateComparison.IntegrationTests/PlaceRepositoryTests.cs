using System.Diagnostics;
using System.Linq;
using ClimateComparison.Data;
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
            builder.AddEnvironmentVariables("CLIMATE_");
            var configuration = builder.Build();
            var connectionProvider = new SqlConnectionProvider(configuration);
            _placeRepository = new PlaceRepository(connectionProvider);
        }

        [Test]
        public void FindsPlaceByNameStart()
        {
            // arrange
            string searchText = "Utr";
            
            // act
            var results = _placeRepository.Find(searchText, 10).ToList();

            // assert
            Assert.That(results, Is.Not.Empty);
            Assert.That(results.First().Name, Is.EqualTo("Utrecht"));
        }

        [Test]
        public void FindsPlaceByAlternativeNameStart()
        {
            // arrange
            string searchText = "Moskva";

            // act
            var results = _placeRepository.Find(searchText, 10).ToList();

            // assert
            Assert.That(results, Is.Not.Empty);
            Assert.That(results.First().Name, Is.EqualTo("Moscow"));
        }

        [Test]
        public void PerformsCaseInsensitiveSearch()
        {
            // arrange
            string searchText = "cHicAgO";

            // act
            var results = _placeRepository.Find(searchText, 10).ToList();

            // assert
            Assert.That(results, Is.Not.Empty);
            Assert.That(results.First().Name, Is.EqualTo("Chicago"));
        }


        [Test]
        public void SelectsMostRelevantPlace()
        {
            // arrange
            string searchText = "London";

            // act
            var results = _placeRepository.Find(searchText, 10).ToList();

            // assert
            Assert.That(results, Is.Not.Empty);
            Assert.That(results.First().Name, Is.EqualTo("London"));
            Assert.That(results.First().Country, Is.EqualTo("GB"));
        }

        [Test]
        public void WorksWithLatinExtraCharacters()
        {
            // arrange
            string searchText = "Mün";

            // act
            var results = _placeRepository.Find(searchText, 10).ToList();

            // assert
            Assert.That(results, Is.Not.Empty);
            Assert.That(results.First().Name, Is.EqualTo("Munich"));
            Assert.That(results.First().Country, Is.EqualTo("DE"));
        }

        [Test]
        public void WorksWithCyrillic()
        {
            // arrange
            string searchText = "Моск";

            // act
            var results = _placeRepository.Find(searchText, 10).ToList();

            // assert
            Assert.That(results, Is.Not.Empty);
            Assert.That(results.First().Name, Is.EqualTo("Moscow"));
            Assert.That(results.First().Country, Is.EqualTo("RU"));
        }

        [Test]
        public void SelectsNoMoreThanMaximum()
        {
            // arrange
            string searchText = "San";
            int maxResults = 5;

            // act
            var results = _placeRepository.Find(searchText, maxResults).ToList();

            // assert
            Assume.That(results, Is.Not.Empty);
            Assert.That(results.Count, Is.LessThanOrEqualTo(maxResults));
        }

        [Test]
        public void ReturnsEmptyResultIfNothingWasFound()
        {
            // arrange
            string searchText = "spkt";

            // act
            var results = _placeRepository.Find(searchText, 10).ToList();
        
            // assert
            Assert.That(results, Is.Empty);
        }

        [Test]
        public void PerformanceIsAcceptable()
        {
            // arrange
            string searchText = "San";
            var stopwatch = new Stopwatch();

            // act
            stopwatch.Start();
            _placeRepository.Find(searchText, 10).ToList();
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            // assert
            Assert.That(elapsedMs, Is.LessThanOrEqualTo(500));
        }
    }
}
