using System;
using System.Collections.Generic;
using System.Linq;
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
        public IActionResult GetPictureListForTouristRoute(Guid touristRouteId)
        {
            if (_touristRouteRepository.TouristRouteExists(touristRouteId) == false)
            {
                return NotFound("旅遊路線不存在");
            }

            var picturesFromRepo = _touristRouteRepository.GetPictureByTouristRouteId(touristRouteId).ToList();
            if (picturesFromRepo.Count < 1)
            {
                return NotFound("照片不存在");
            }
            
            return Ok(_mapper.Map<IEnumerable<TouristRoutePictureDto>>(picturesFromRepo));
        }

        [HttpGet("{pictureId}", Name = "GetPicture")]
        public IActionResult GetPicture(Guid touristRouteId, int pictureId)
        {
            if (_touristRouteRepository.TouristRouteExists(touristRouteId) == false)
                return NotFound("旅遊路線不存在");

            var pictureFromRepo = _touristRouteRepository.GetPicture(pictureId);
            if (pictureFromRepo == null)
                return NotFound("相片不存在");

            return Ok(_mapper.Map<TouristRoutePictureDto>(pictureFromRepo));
        }

        [HttpPost]
        public IActionResult CreatePicture(
            [FromRoute]Guid touristRouteId,
            [FromBody]TouristRoutePictureCreationDto touristRoutePictureCreationDto)
        {
            if (_touristRouteRepository.TouristRouteExists(touristRouteId) == false)
                return NotFound("旅遊路線不存在");

            var pictureModel = _mapper.Map<TouristRoutePicture>(touristRoutePictureCreationDto);
            pictureModel.TouristRouteId = touristRouteId;

            _touristRouteRepository.AddPicture(pictureModel);
            _touristRouteRepository.Save();

            return CreatedAtRoute(
                "GetPicture",
                new { touristRouteId = pictureModel.TouristRouteId, pictureId = pictureModel.Id },
                _mapper.Map<TouristRoutePictureDto>(pictureModel));
        }
    }
}