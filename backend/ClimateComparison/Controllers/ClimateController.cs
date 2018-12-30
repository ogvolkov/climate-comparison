using ClimateComparison.Data;
using ClimateComparison.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClimateComparison.Controllers
{
    [Route("api/[controller]")]
    public class ClimateController
    {
        private readonly ClimateRepository _climateRepository;

        public ClimateController(ClimateRepository climateRepository)
        {
            _climateRepository = climateRepository ?? throw new System.ArgumentNullException(nameof(climateRepository));
        }

        [HttpGet("temperature/{placeId}")]
        public Temperature Temperature(int placeId)
        {
            return _climateRepository.GetTemperature(placeId);
        }

        [HttpGet("precipitation/{placeId}")]
        public Precipitation Precipitation(int placeId)
        {
            return _climateRepository.GetPrecipitation(placeId);
        }
    }
}
