using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.Models;
using FakeXiecheng.API.ResourceParameters;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace FakeXiecheng.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TouristRoutesController : Controller
    {
        private readonly IMapper _mapper;

        private readonly ITouristRouteRepository _touristRouteRepository;

        public TouristRoutesController(
            IMapper mapper,
            ITouristRouteRepository touristRouteRepository)
        {
            _mapper = mapper;
            _touristRouteRepository = touristRouteRepository;
        }

        // api/TouristRoutes?keyword=argument
        // if action argument not equal query string, can use [FromQuery(Name = "xxx")]
        [HttpGet]
        [HttpHead]
        public IActionResult GetTouristRoutes([FromQuery]TouristRouteResourceParameters parameters)
        {
            var touristRoutesFromRepo = _touristRouteRepository
                .GetTouristRoutes(parameters.Keyword, parameters.RatingOperator, parameters.RatingValue);

            if (touristRoutesFromRepo == null || touristRoutesFromRepo.Any() == false)
            {
                return NotFound("沒有旅遊路線");
            }

            var touristRoutesDto = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRoutesFromRepo);

            return Ok(touristRoutesDto);
        }

        [HttpGet("{touristRouteId:Guid}")]
        [HttpHead("{touristRouteId:Guid}")]
        public IActionResult GetTouristRouteById(Guid touristRouteId)
        {
            var touristRouteFromRepo = _touristRouteRepository.GetTouristRoute(touristRouteId);

            if (touristRouteFromRepo == null)
            {
                return NotFound($"{touristRouteId} 旅遊路線: 找不到");
            }

            var touristRouteDto = _mapper.Map<TouristRouteDto>(touristRouteFromRepo);

            return Ok(touristRouteDto);
        }

        [HttpPost]
        public IActionResult CreateTouristRoute([FromBody]TouristRouteForCreationDto touristRouteForCreationDto)
        {
            var touristRouteModel = _mapper.Map<TouristRoute>(touristRouteForCreationDto);

            return NoContent();
        }
    }
}