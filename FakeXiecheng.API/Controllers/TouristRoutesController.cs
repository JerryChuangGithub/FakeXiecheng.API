using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.Helpers;
using FakeXiecheng.API.Models;
using FakeXiecheng.API.ResourceParameters;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.JsonPatch;
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
        public async Task<IActionResult> GetTouristRoutesAsync([FromQuery]TouristRouteResourceParameters parameters)
        {
            var touristRoutesFromRepo = await _touristRouteRepository
                .GetTouristRoutesAsync(parameters.Keyword, parameters.RatingOperator, parameters.RatingValue);

            if (touristRoutesFromRepo == null || touristRoutesFromRepo.Any() == false)
            {
                return NotFound("沒有旅遊路線");
            }

            var touristRoutesDto = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRoutesFromRepo);

            return Ok(touristRoutesDto);
        }

        [HttpGet("{touristRouteId:Guid}", Name = "GetTouristRouteById")]
        [HttpHead("{touristRouteId:Guid}")]
        public async Task<IActionResult> GetTouristRouteByIdAsync(Guid touristRouteId)
        {
            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);

            if (touristRouteFromRepo == null)
            {
                return NotFound($"{touristRouteId} 旅遊路線: 找不到");
            }

            var touristRouteDto = _mapper.Map<TouristRouteDto>(touristRouteFromRepo);

            return Ok(touristRouteDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTouristRouteAsync([FromBody]TouristRouteForCreationDto touristRouteForCreationDto)
        {
            var touristRouteModel = _mapper.Map<TouristRoute>(touristRouteForCreationDto);
            _touristRouteRepository.AddTouristRoute(touristRouteModel);
            await _touristRouteRepository.SaveAsync();
            var touristRouteToReturn = _mapper.Map<TouristRouteDto>(touristRouteModel);
            return CreatedAtRoute(
                "GetTouristRouteById",
                new { touristRouteId = touristRouteToReturn.Id },
                touristRouteToReturn);
        }

        [HttpPut("{touristRouteId:Guid}")]
        public async Task<IActionResult> UpdateTouristRouteAsync(
            [FromRoute]Guid touristRouteId,
            [FromBody]TouristRouteForUpdateDto touristRouteForUpdateDto)
        {
            if (await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId) == false)
                return NotFound("旅遊路線不存在");

            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            _mapper.Map(touristRouteForUpdateDto, touristRouteFromRepo);

            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }

        [HttpPatch("{touristRouteId}")]
        public async Task<IActionResult> PartiallyUpdateTouristRouteAsync(
            [FromRoute]Guid touristRouteId,
            [FromBody]JsonPatchDocument<TouristRouteForUpdateDto> patchDocument)
        {
            if (await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId) == false)
                return NotFound("旅遊路線不存在");

            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            var touristRouteToPatch = _mapper.Map<TouristRouteForUpdateDto>(touristRouteFromRepo);
            patchDocument.ApplyTo(touristRouteToPatch, ModelState);

            if (TryValidateModel(touristRouteToPatch) == false)
                return ValidationProblem(ModelState);

            _mapper.Map(touristRouteToPatch, touristRouteFromRepo);
            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }

        [HttpDelete("{touristRouteId}")]
        public async Task<IActionResult> DeleteTouristRouteAsync(
            [FromRoute]Guid touristRouteId)
        {
            if (await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId) == false)
                return NotFound("旅遊路線不存在");

            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            _touristRouteRepository.DeleteTouristRoute(touristRouteFromRepo);
            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }

        [HttpDelete("({Ids})")]
        public async Task<IActionResult> DeleteByIdsAsync(
            [ModelBinder(typeof(ArrayModelBinder))][FromRoute]IEnumerable<Guid> ids)
        {
            var touristRouteIds = ids as Guid[] ?? ids.ToArray();
            if (touristRouteIds.Length < 1)
                return BadRequest();

            var touristRoutesFromRepo = await _touristRouteRepository.GetTouristRoutesByIdsAsync(touristRouteIds);
            _touristRouteRepository.DeleteTouristRoutes(touristRoutesFromRepo);
            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }
    }
}