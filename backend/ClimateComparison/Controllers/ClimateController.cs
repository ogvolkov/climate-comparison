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

        // GET api/climate/{placeId}
        [HttpGet("{placeId}")]
        public Climate Get(int placeId)
        {
            return _climateRepository.Get(placeId);
        }
    }
}
