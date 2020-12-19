using System.Collections.Generic;
using FakeXiecheng.API.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FakeXiecheng.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class RootController : ControllerBase
    {
        [HttpGet(Name = "GetRoot")]
        public IActionResult GetRoot()
        {
            var links = new List<LinkDto>
            {
                new LinkDto(
                    Url.Link("GetRoot", null),
                    "self",
                    "GET"),
                new LinkDto(
                    Url.Link("GetTouristRoutes", null),
                    "get_tourist_routes",
                    "GET"),
                new LinkDto(
                    Url.Link("CreateTouristRoute", null),
                    "create_tourist_route",
                    "POST")
            };

            return Ok(links);
        }
    }
}