using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.Models;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace FakeXiecheng.API.Controllers
{
    [Route("api/TouristRoutes/{touristRouteId}/pictures")]
    [ApiController]
    public class TouristRoutePicturesController : ControllerBase
    {
        private readonly IMapper _mapper;

        private readonly ITouristRouteRepository _touristRouteRepository;

        public TouristRoutePicturesController(
            IMapper mapper,
            ITouristRouteRepository touristRouteRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _touristRouteRepository = touristRouteRepository ?? throw new ArgumentNullException(nameof(touristRouteRepository));
        }

        [HttpGet]
        public async Task<IActionResult> GetPictureListForTouristRouteAsync(Guid touristRouteId)
        {
            if (await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId) == false)
            {
                return NotFound("旅遊路線不存在");
            }

            var picturesFromRepo = await _touristRouteRepository.GetPictureByTouristRouteIdAsync(touristRouteId);
            if (picturesFromRepo.Any() == false)
            {
                return NotFound("照片不存在");
            }
            
            return Ok(_mapper.Map<IEnumerable<TouristRoutePictureDto>>(picturesFromRepo));
        }

        [HttpGet("{pictureId}", Name = "GetPicture")]
        public async Task<IActionResult> GetPictureAsync(Guid touristRouteId, int pictureId)
        {
            if (await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId) == false)
                return NotFound("旅遊路線不存在");

            var pictureFromRepo = await _touristRouteRepository.GetPictureAsync(pictureId);
            if (pictureFromRepo == null)
                return NotFound("相片不存在");

            return Ok(_mapper.Map<TouristRoutePictureDto>(pictureFromRepo));
        }

        [HttpPost]
        public async Task<IActionResult> CreatePictureAsync(
            [FromRoute]Guid touristRouteId,
            [FromBody]TouristRoutePictureCreationDto touristRoutePictureCreationDto)
        {
            if (await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId) == false)
                return NotFound("旅遊路線不存在");

            var pictureModel = _mapper.Map<TouristRoutePicture>(touristRoutePictureCreationDto);
            _touristRouteRepository.AddPicture(touristRouteId, pictureModel);
            await _touristRouteRepository.SaveAsync();

            return CreatedAtRoute(
                "GetPicture",
                new { touristRouteId = pictureModel.TouristRouteId, pictureId = pictureModel.Id },
                _mapper.Map<TouristRoutePictureDto>(pictureModel));
        }

        [HttpDelete("{pictureId}")]
        public async Task<IActionResult> DeletePictureAsync(
            [FromRoute]Guid touristRouteId,
            [FromRoute]int pictureId)
        {
            if (await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId) == false)
                return NotFound("旅遊路線不存在");
            
            var pictureFromRepo = await _touristRouteRepository.GetPictureAsync(pictureId);
            if (pictureFromRepo == null)
                return NotFound("相片不存在");

            _touristRouteRepository.DeleteTouristRoutePicture(pictureFromRepo);
            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }
    }
}