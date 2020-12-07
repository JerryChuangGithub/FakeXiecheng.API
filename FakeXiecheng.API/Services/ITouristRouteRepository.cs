using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeXiecheng.API.Models;

namespace FakeXiecheng.API.Services
{
    public interface ITouristRouteRepository
    {
        Task<IEnumerable<TouristRoute>> GetTouristRoutesAsync(string keyword, string ratingOperator, int? ratingValue);

        Task<IEnumerable<TouristRoute>> GetTouristRoutesByIdsAsync(IEnumerable<Guid> touristRouteIds);

        void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes);

        Task<TouristRoute> GetTouristRouteAsync(Guid touristRouteId);

        Task<bool> TouristRouteExistsAsync(Guid touristRouteId);

        Task<IEnumerable<TouristRoutePicture>> GetPictureByTouristRouteIdAsync(Guid touristRouteId);

        Task<TouristRoutePicture> GetPictureAsync(int pictureId);

        void AddTouristRoute(TouristRoute touristRoute);

        void AddPicture(Guid touristRouteId, TouristRoutePicture picture);

        void DeleteTouristRoute(TouristRoute touristRoute);

        void DeleteTouristRoutePicture(TouristRoutePicture picture);

        Task<ShoppingCart> GetShoppingCartByUserIdAsync(string userId);

        void CreateShoppingCart(ShoppingCart shoppingCart);

        void AddShoppingCartItem(LineItem lineItem);

        Task<LineItem> GetShoppingCartItemByIdAsync(int itemId);

        void DeleteShoppingCartItem(LineItem lineItem);

        Task<bool> SaveAsync();
    }
}