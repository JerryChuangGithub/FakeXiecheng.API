using System;
using System.Linq;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace FakeXiecheng.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TouristRoutesController : Controller
    {
        private readonly ITouristRouteRepository _touristRouteRepository;

        public TouristRoutesController(ITouristRouteRepository touristRouteRepository)
        {
            _touristRouteRepository = touristRouteRepository;
        }

        [HttpGet]
        public IActionResult GetTouristRoutes()
        {
            var touristRoutesFromRepo = _touristRouteRepository.GetTouristRoutes();

            if (touristRoutesFromRepo == null || touristRoutesFromRepo.Any() == false)
            {
                return NotFound("沒有旅遊路線");
            }

            return Ok(touristRoutesFromRepo);
        }

        [HttpGet("{touristRouteId:Guid}")]
        public IActionResult GetTouristRouteById(Guid touristRouteId)
        {
            var touristRouteFromRepo = _touristRouteRepository.GetTouristRoute(touristRouteId);

            if (touristRouteFromRepo == null)
            {
                return NotFound($"{touristRouteId} 旅遊路線: 找不到");
            }

            return Ok(touristRouteFromRepo);
        }
    }
}