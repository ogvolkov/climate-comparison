using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ClimateComparison.Models;
using ClimateComparison.Data;

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

        // GET api/places?search
        [HttpGet]
        public IEnumerable<Place> Get(string search)
        {
            return _placeRepository.Find(search, 15);
        }
    }
}
