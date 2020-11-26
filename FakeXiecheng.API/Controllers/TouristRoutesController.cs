using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AutoMapper;
using FakeXiecheng.API.Dtos;
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
        public IActionResult GetTouristRoutes(
            [FromQuery]string keyword,
            string rating)
        {
            var regex = new Regex(@"([A-Za-z0-9\-]+)(\d+)");
            var ratingOperator = string.Empty;
            var ratingValue = -1;
            var match = regex.Match(rating);
            if (match.Success)
            {
                ratingOperator = match.Groups[1].Value;
                ratingValue = int.Parse(match.Groups[2].Value);
            }
            
            var touristRoutesFromRepo = _touristRouteRepository.GetTouristRoutes(keyword);

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
    }
}