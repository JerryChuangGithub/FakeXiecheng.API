using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.Helpers;
using FakeXiecheng.API.Models;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FakeXiecheng.API.Controllers
{
    [ApiController]
    [Route("api/ShoppingCart")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly ITouristRouteRepository _touristRepository;

        private readonly IMapper _mapper;

        private readonly IHttpClientFactory _httpClientFactory;

        public ShoppingCartController(
            IHttpContextAccessor httpContextAccessor,
            ITouristRouteRepository touristRepository,
            IMapper mapper,
            IHttpClientFactory httpClientFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _touristRepository = touristRepository;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
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

        [HttpDelete("items/({itemIds})")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> DeleteShoppingCartItemsAsync(
            [ModelBinder(typeof(ArrayModelBinder))] [FromRoute]
            IEnumerable<int> itemIds)
        {
            var lineItems = (await _touristRepository.GetShoppingCartItemsByIds(itemIds)).ToArray();
            if (lineItems.Length < 1)
                return NotFound("找不到購物車商品項目");

            _touristRepository.DeleteShoppingCartItems(lineItems);
            await _touristRepository.SaveAsync();

            return NoContent();
        }

        [HttpPost("Checkout")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> Checkout()
        {
            var userId = _httpContextAccessor
                .HttpContext
                .User
                .FindFirst(ClaimTypes.NameIdentifier)
                .Value;

            var shoppingCart = await _touristRepository
                .GetShoppingCartByUserIdAsync(userId);

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                State = OrderStateEnum.Pending,
                OrderItems = shoppingCart.ShoppingCartItem,
                CreateDateUTC = DateTime.UtcNow
            };

            shoppingCart.ShoppingCartItem = null;

            _touristRepository.AddOrder(order);
            await _touristRepository.SaveAsync();

            return Ok(_mapper.Map<OrderDto>(order));
        }

        [HttpPost("{orderId}/PlaceOrder")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> PlaceOrder([FromRoute] Guid orderId)
        {
            var userId = _httpContextAccessor
                .HttpContext
                .User
                .FindFirst(ClaimTypes.NameIdentifier)
                .Value;

            var order = await _touristRepository.GetOrderById(orderId);
            order.PaymentProcessing();
            await this._touristRepository.SaveAsync();

            var httpClient = _httpClientFactory.CreateClient();
            const string url = @"http://localhost:5000/api/FakeVanderPaymentProcess?orderNumber={0}&returnFault={1}";
            var response = await httpClient.PostAsync(string.Format(url, order.Id, false), null);

            var isApproved = false;
            var transactionMetadata = string.Empty;

            if (response.IsSuccessStatusCode)
            {
                transactionMetadata = await response.Content.ReadAsStringAsync();
                var jsonObject = (JObject) JsonConvert.DeserializeObject(transactionMetadata);
                isApproved = jsonObject["approved"].Value<bool>();
            }

            if (isApproved)
            {
                order.PaymentApprove();
            }
            else
            {
                order.PaymentReject();
            }

            order.TransactionMetadata = transactionMetadata;
            await _touristRepository.SaveAsync();

            return Ok();
        }
    }
}