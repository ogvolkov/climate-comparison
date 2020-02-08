using System.Threading.Tasks;
using ClimateComparison.DataAccess.DTO;
using ClimateComparison.DataAccess.Repositories;
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
        public async Task<Temperature> Temperature(int placeId)
        {
            return await _climateRepository.GetTemperature(placeId);
        }

        [HttpGet("precipitation/{placeId}")]
        public async Task<Precipitation> Precipitation(int placeId)
        {
            return await _climateRepository.GetPrecipitation(placeId);
        }
    }
}
