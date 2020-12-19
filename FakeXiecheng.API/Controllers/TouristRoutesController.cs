using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.Helpers;
using FakeXiecheng.API.Models;
using FakeXiecheng.API.ResourceParameters;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace FakeXiecheng.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TouristRoutesController : Controller
    {
        private readonly IMapper _mapper;

        private readonly IPropertyMappingService _propertyMappingService;

        private readonly ITouristRouteRepository _touristRouteRepository;

        private readonly IUrlHelper _urlHelper;

        public TouristRoutesController(
            IMapper mapper,
            IPropertyMappingService propertyMappingService,
            ITouristRouteRepository touristRouteRepository,
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor actionContextAccessor)
        {
            _mapper = mapper;
            _propertyMappingService = propertyMappingService;
            _touristRouteRepository = touristRouteRepository;
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
        }

        // api/TouristRoutes?keyword=argument
        // if action argument not equal query string, can use [FromQuery(Name = "xxx")]
        [Produces(
            "application/json",
            "application/vnd.jerry.hateoas+json",
            "application/vnd.jerry.tourist_route.simplify+json",
            "application/vnd.jerry.tourist_route.simplify.hateoas+json")]
        [HttpGet(Name = "GetTouristRoutes")]
        [HttpHead]
        public async Task<IActionResult> GetTouristRoutesAsync(
            [FromQuery] TouristRouteResourceParameters parameters,
            [FromQuery] PaginationResourceParameters paginationParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (MediaTypeHeaderValue.TryParse(mediaType, out var parsedMediaType) == false)
                return BadRequest();

            if (_propertyMappingService.IsMappingExists<TouristRouteDto, TouristRoute>(parameters.OrderBy) == false)
            {
                return BadRequest("排序參數不合法");
            }

            if (_propertyMappingService.IsPropertiesExists<TouristRouteDto>(parameters.Fields) == false)
            {
                return BadRequest("塑形參數不正確");
            }

            var touristRoutesFromRepo = await _touristRouteRepository
                .GetTouristRoutesAsync(
                    parameters.Keyword,
                    parameters.RatingOperator,
                    parameters.RatingValue,
                    paginationParameters.PageSize,
                    paginationParameters.PageNumber,
                    parameters.OrderBy);

            if (touristRoutesFromRepo == null || touristRoutesFromRepo.Any() == false)
            {
                return NotFound("沒有旅遊路線");
            }

            var previousPageLink = touristRoutesFromRepo.HasPrevious
                ? GenerateTouristRouteResourceUrl(parameters, paginationParameters, ResourceUriType.PreviousPage)
                : null;

            var nextPageLink = touristRoutesFromRepo.HasNext
                ? GenerateTouristRouteResourceUrl(parameters, paginationParameters, ResourceUriType.NextPage)
                : null;

            var paginationMetadata = new
            {
                previousPageLink,
                nextPageLink,
                totalCount = touristRoutesFromRepo.TotalCount,
                pageSize = touristRoutesFromRepo.PageSize,
                currentPage = touristRoutesFromRepo.CurrentPage,
                totalPage = touristRoutesFromRepo.TotalPage
            };

            Response.Headers.Add("x-pagination", JsonConvert.SerializeObject(paginationMetadata));

            var isHateoas = parsedMediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            var primaryMediaType = isHateoas
                ? parsedMediaType.SubTypeWithoutSuffix.Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
                : parsedMediaType.SubTypeWithoutSuffix;
            
            // var touristRoutesDto = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRoutesFromRepo);
            // var shapeDataDtoList = touristRoutesDto.ShapeData(parameters.Fields);
            IEnumerable<object> touristRoutesDto;
            IEnumerable<ExpandoObject> shapeDataDtoList;

            if (primaryMediaType == "vnd.jerry.tourist_route.simplify")
            {
                touristRoutesDto = _mapper.Map<IEnumerable<TouristRouteSimplifyDto>>(touristRoutesFromRepo);
                shapeDataDtoList = ((IEnumerable<TouristRouteSimplifyDto>)touristRoutesDto).ShapeData(parameters.Fields);
            }
            else
            {
                touristRoutesDto = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRoutesFromRepo);
                shapeDataDtoList = ((IEnumerable<TouristRouteDto>)touristRoutesDto).ShapeData(parameters.Fields);
            }
            
            if (isHateoas)
            {
                return Ok(new
                {
                    Value = shapeDataDtoList.Select(t =>
                    {
                        var result = t as IDictionary<string, object>;
                        result["Links"] = CreateLinkForTouristRoute((Guid) result["Id"], null);
                        return result;
                    }),
                    Links = CreateLinkForTouristRouteList(parameters, paginationParameters)
                });
            }

            return Ok(shapeDataDtoList);
        }

        [HttpGet("{touristRouteId:Guid}", Name = "GetTouristRouteById")]
        [HttpHead("{touristRouteId:Guid}")]
        public async Task<IActionResult> GetTouristRouteByIdAsync(
            Guid touristRouteId,
            string fields)
        {
            if (_propertyMappingService.IsPropertiesExists<TouristRouteDto>(fields) == false)
            {
                return BadRequest("塑形參數不正確");
            }

            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);

            if (touristRouteFromRepo == null)
            {
                return NotFound($"{touristRouteId} 旅遊路線: 找不到");
            }

            var touristRouteDto = _mapper.Map<TouristRouteDto>(touristRouteFromRepo);

            var linkDtos = CreateLinkForTouristRoute(touristRouteId, fields);
            var result = touristRouteDto.ShapeData(fields) as IDictionary<string, object>;
            result.Add("Links", linkDtos);

            return Ok(result);
        }

        [HttpPost(Name = "CreateTouristRoute")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> CreateTouristRouteAsync(
            [FromBody] TouristRouteForCreationDto touristRouteForCreationDto)
        {
            var touristRouteModel = _mapper.Map<TouristRoute>(touristRouteForCreationDto);
            _touristRouteRepository.AddTouristRoute(touristRouteModel);
            await _touristRouteRepository.SaveAsync();
            var touristRouteToReturn = _mapper.Map<TouristRouteDto>(touristRouteModel);

            var result = touristRouteToReturn.ShapeData(null) as IDictionary<string, object>;
            result["links"] = CreateLinkForTouristRoute(touristRouteToReturn.Id, null);

            return CreatedAtRoute(
                "GetTouristRouteById",
                new { touristRouteId = result["Id"] },
                result);
        }

        [HttpPut("{touristRouteId:Guid}", Name = "UpdateTouristRoute")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> UpdateTouristRouteAsync(
            [FromRoute] Guid touristRouteId,
            [FromBody] TouristRouteForUpdateDto touristRouteForUpdateDto)
        {
            if (await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId) == false)
                return NotFound("旅遊路線不存在");

            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            _mapper.Map(touristRouteForUpdateDto, touristRouteFromRepo);

            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }

        [HttpPatch("{touristRouteId}", Name = "PartiallyUpdateTouristRoute")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> PartiallyUpdateTouristRouteAsync(
            [FromRoute] Guid touristRouteId,
            [FromBody] JsonPatchDocument<TouristRouteForUpdateDto> patchDocument)
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

        [HttpDelete("{touristRouteId}", Name = "DeleteTouristRoute")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> DeleteTouristRouteAsync(
            [FromRoute] Guid touristRouteId)
        {
            if (await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId) == false)
                return NotFound("旅遊路線不存在");

            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            _touristRouteRepository.DeleteTouristRoute(touristRouteFromRepo);
            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }

        [HttpDelete("({Ids})")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> DeleteByIdsAsync(
            [ModelBinder(typeof(ArrayModelBinder))] [FromRoute]
            IEnumerable<Guid> ids)
        {
            var touristRouteIds = ids as Guid[] ?? ids.ToArray();
            if (touristRouteIds.Length < 1)
                return BadRequest();

            var touristRoutesFromRepo = await _touristRouteRepository.GetTouristRoutesByIdsAsync(touristRouteIds);
            _touristRouteRepository.DeleteTouristRoutes(touristRoutesFromRepo);
            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }

        private IEnumerable<LinkDto> CreateLinkForTouristRoute(
            Guid touristRouteId,
            string fields)
        {
            var links = new List<LinkDto>
            {
                new LinkDto(
                    Url.Link("GetTouristRouteById", new { touristRouteId, fields }),
                    "self",
                    "GET"),
                new LinkDto(
                    Url.Link("UpdateTouristRoute", new { touristRouteId }),
                    "update",
                    "PUT"),
                new LinkDto(
                    Url.Link("PartiallyUpdateTouristRoute", new { touristRouteId }),
                    "partially_update",
                    "PATCH"),
                new LinkDto(
                    Url.Link("DeleteTouristRoute", new { touristRouteId }),
                    "delete",
                    "DELETE"),
                new LinkDto(
                    Url.Link("GetPictureListForTouristRoute", new { touristRouteId }),
                    "get_pictures",
                    "GET"),
                new LinkDto(
                    Url.Link("CreateTouristRoutePicture", new { touristRouteId }),
                    "create_picture",
                    "POST")
            };

            return links;
        }

        private IEnumerable<LinkDto> CreateLinkForTouristRouteList(
            TouristRouteResourceParameters parameters,
            PaginationResourceParameters paginationParameters)
        {
            var links = new List<LinkDto>
            {
                new LinkDto(
                    GenerateTouristRouteResourceUrl(
                        parameters, paginationParameters, ResourceUriType.CurrentPage),
                    "self",
                    "GET"),
                new LinkDto(
                    Url.Link("CreateTouristRoute", null),
                    "create_tourist_route",
                    "POST")
            };

            return links;
        }

        private string GenerateTouristRouteResourceUrl(
            TouristRouteResourceParameters parameters,
            PaginationResourceParameters paginationParameters,
            ResourceUriType type)
        {
            return type switch
            {
                ResourceUriType.PreviousPage => _urlHelper.Link(
                    "GetTouristRoutes",
                    new
                    {
                        keyword = parameters.Keyword,
                        rating = parameters.Rating,
                        pageNumber = paginationParameters.PageNumber - 1,
                        pageSize = paginationParameters.PageSize,
                        orderBy = parameters.OrderBy,
                        fields = parameters.Fields
                    }),
                ResourceUriType.NextPage => _urlHelper.Link(
                    "GetTouristRoutes",
                    new
                    {
                        keyword = parameters.Keyword,
                        rating = parameters.Rating,
                        pageNumber = paginationParameters.PageNumber + 1,
                        pageSize = paginationParameters.PageSize,
                        orderBy = parameters.OrderBy,
                        fields = parameters.Fields
                    }),
                ResourceUriType.CurrentPage => _urlHelper.Link(
                    "GetTouristRoutes",
                    new
                    {
                        keyword = parameters.Keyword,
                        rating = parameters.Rating,
                        pageNumber = paginationParameters.PageNumber,
                        pageSize = paginationParameters.PageSize,
                        orderBy = parameters.OrderBy,
                        fields = parameters.Fields
                    }),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }

    internal enum ResourceUriType
    {
        PreviousPage,
        NextPage,
        CurrentPage
    }
}