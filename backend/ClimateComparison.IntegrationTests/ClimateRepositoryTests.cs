using System;
using System.Diagnostics;
using System.Linq;
using ClimateComparison.DataAccess.Infra;
using ClimateComparison.DataAccess.Repositories;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace ClimateComparison.IntegrationTests
{
    [TestFixture]
    public class ClimateRepositoryTests
    {
        private ClimateRepository _climateRepository;

        [SetUp]
        public void SetUp()
        {
            var builder = new ConfigurationBuilder();
            builder.AddEnvironmentVariables("CLIMATE_COMPARISON_");
            var configuration = builder.Build();

            var connectionProvider = new CloudTableClientProvider(configuration);

            _climateRepository = new ClimateRepository(connectionProvider);
        }

        [Test]
        public void RetrievesAverageHighs()
        {
            // arrange
            int placeId = 105989;

            // act
            var temperature = _climateRepository.GetTemperature(placeId);

            // assert
            Assert.That(temperature.MonthlyAverageHighs, Is.Not.Null);
            Assert.That(temperature.MonthlyAverageHighs, Is.Not.Null);
            Assert.That(temperature.MonthlyAverageHighs.Length, Is.EqualTo(12));

            Assert.That(temperature.MonthlyAverageHighs.All(t => t >= 0 && t < 25), Is.True);

            Console.WriteLine(string.Join(' ', temperature.MonthlyAverageHighs));
        }

        [Test]
        public void AverageHighsTakeReasonableTime()
        {
            // arrange
            int placeId = 66728;
            var stopwatch = new Stopwatch();

            // act
            stopwatch.Start();
            _climateRepository.GetTemperature(placeId);
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            // assert
            Assert.That(elapsedMs, Is.LessThanOrEqualTo(500));
        }

        [Test]
        public void RetrievesPrecipitation()
        {
            // arrange
            int placeId = 105989;

            // act
            var precipitation = _climateRepository.GetPrecipitation(placeId);

            // assert
            Assert.That(precipitation.MonthlyAverages, Is.Not.Null);
            Assert.That(precipitation.MonthlyAverages, Is.Not.Null);
            Assert.That(precipitation.MonthlyAverages.Length, Is.EqualTo(12));

            Assert.That(precipitation.MonthlyAverages.All(t => t >= 0 && t < 400), Is.True);

            Console.WriteLine(string.Join(' ', precipitation.MonthlyAverages));
        }

        [Test]
        public void PrecipitationTakesReasonableTime()
        {
            // arrange
            int placeId = 66728;
            var stopwatch = new Stopwatch();

            // act
            stopwatch.Start();
            _climateRepository.GetPrecipitation(placeId);
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            // assert
            Assert.That(elapsedMs, Is.LessThanOrEqualTo(500));
        }
    }
}
