using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FakeXiecheng.API.Controllers
{
    [ApiController]
    [Route("api/ShoppingCart")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly ITouristRouteRepository _touristRepository;
        
        private IMapper _mapper;

        public ShoppingCartController(
            IHttpContextAccessor httpContextAccessor,
            ITouristRouteRepository touristRepository,
            IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _touristRepository = touristRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetShoppingCart()
        {
            var userId = _httpContextAccessor
                .HttpContext
                .User
                .FindFirst(ClaimTypes.NameIdentifier)
                .Value;

            var shoppingCart = await _touristRepository.GetShoppingCartByUserId(userId);
            
            return Ok(_mapper.Map<ShoppingCartDto>(shoppingCart));
        }
    }
}