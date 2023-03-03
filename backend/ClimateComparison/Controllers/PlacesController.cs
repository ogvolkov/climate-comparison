using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ClimateComparison.DataAccess.DTO;
using ClimateComparison.DataAccess.Repositories;

namespace ClimateComparison.Controllers
{
    [Route("api/[controller]")]
    public class PlacesController : Controller
    {
        private readonly PlaceRepository _placeRepository;

        public PlacesController(PlaceRepository citiesRepository)
        {
            _placeRepository = citiesRepository ?? throw new System.ArgumentNullException(nameof(citiesRepository));
        }

        [HttpGet]
        public async Task<IReadOnlyCollection<Place>> Get(string search)
        {
            return await _placeRepository.Find(search, 15);
        }

        [HttpGet("{placeId}")]
        public async Task<Place> Details(int placeId)
        {
            return await _placeRepository.Get(placeId);
        }
    }
}
