using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.Models;
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
        public async Task<IActionResult> GetShoppingCartAsync()
        {
            var userId = _httpContextAccessor
                .HttpContext
                .User
                .FindFirst(ClaimTypes.NameIdentifier)
                .Value;

            var shoppingCart = await _touristRepository.GetShoppingCartByUserIdAsync(userId);

            return Ok(_mapper.Map<ShoppingCartDto>(shoppingCart));
        }

        [HttpPost("items")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> AddShoppingCartItemAsync(
            [FromBody] AddShoppingCartItemDto addShoppingCartItemDto)
        {
            var userId = _httpContextAccessor
                .HttpContext
                .User
                .FindFirst(ClaimTypes.NameIdentifier)
                .Value;

            var shoppingCart = await _touristRepository
                .GetShoppingCartByUserIdAsync(userId);

            var touristRoute = await _touristRepository
                .GetTouristRouteAsync(addShoppingCartItemDto.TouristRouteId);
            if (touristRoute == null)
            {
                return NotFound("旅遊路線不存在");
            }

            var lineItem = new LineItem
            {
                TouristRouteId = touristRoute.Id,
                ShoppingCartId = shoppingCart.Id,
                OriginalPrice = touristRoute.OriginalPrice,
                DiscountPrice = touristRoute.DiscountPrice
            };

            _touristRepository.AddShoppingCartItem(lineItem);
            await _touristRepository.SaveAsync();

            return Ok(_mapper.Map<ShoppingCartDto>(shoppingCart));
        }

        [HttpDelete("items/{itemId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> DeleteShoppingCartItemAsync([FromRoute] int itemId)
        {
            var lineItem = await _touristRepository.GetShoppingCartItemByIdAsync(itemId);
            if (lineItem == null)
                return NotFound("找不到購物車商品項目");

            _touristRepository.DeleteShoppingCartItem(lineItem);
            await _touristRepository.SaveAsync();

            return NoContent();
        }
    }
}