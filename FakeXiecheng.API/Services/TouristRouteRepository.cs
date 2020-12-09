using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeXiecheng.API.Database;
using FakeXiecheng.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FakeXiecheng.API.Services
{
    public class TouristRouteRepository : ITouristRouteRepository
    {
        private readonly AppDbContext _context;

        public TouristRouteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TouristRoute>> GetTouristRoutesAsync(
            string keyword,
            string ratingOperator,
            int? ratingValue)
        {
            IQueryable<TouristRoute> query = _context
                .TouristRoutes
                .Include(x => x.TouristRoutePictures);

            if (string.IsNullOrWhiteSpace(keyword) == false)
            {
                keyword = keyword.Trim();
                query = query.Where(x => x.Title.Contains(keyword));
            }

            if (ratingValue >= 0)
            {
                query = ratingOperator switch
                {
                    "largerThan" => query.Where(r => r.Rating >= ratingValue),
                    "lessThan" => query.Where(r => r.Rating <= ratingValue),
                    _ => query.Where(r => r.Rating == ratingValue)
                };
            }

            return await query.ToArrayAsync();
        }

        public async Task<IEnumerable<TouristRoute>> GetTouristRoutesByIdsAsync(IEnumerable<Guid> touristRouteIds)
        {
            return await _context
                .TouristRoutes
                .Where(x => touristRouteIds.Contains(x.Id))
                .ToArrayAsync();
        }

        public void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes)
        {
            _context.TouristRoutes.RemoveRange(touristRoutes);
        }

        public async Task<TouristRoute> GetTouristRouteAsync(Guid touristRouteId)
        {
            return await _context
                .TouristRoutes
                .Include(x => x.TouristRoutePictures)
                .FirstOrDefaultAsync(x => x.Id == touristRouteId);
        }

        public async Task<bool> TouristRouteExistsAsync(Guid touristRouteId)
        {
            return await _context.TouristRoutes.AnyAsync(x => x.Id == touristRouteId);
        }

        public async Task<IEnumerable<TouristRoutePicture>> GetPictureByTouristRouteIdAsync(Guid touristRouteId)
        {
            return await _context
                .TouristRoutePictures
                .Where(x => x.TouristRouteId == touristRouteId)
                .ToArrayAsync();
        }

        public async Task<TouristRoutePicture> GetPictureAsync(int pictureId)
        {
            return await _context
                .TouristRoutePictures
                .FirstOrDefaultAsync(x => x.Id == pictureId);
        }

        public void AddTouristRoute(TouristRoute touristRoute)
        {
            if (touristRoute == null)
                throw new ArgumentNullException(nameof(touristRoute));

            _context.TouristRoutes.Add(touristRoute);
        }

        public void AddPicture(Guid touristRouteId, TouristRoutePicture picture)
        {
            if (picture == null)
                throw new ArgumentNullException(nameof(picture));

            if (touristRouteId == null)
                throw new ArgumentNullException(nameof(touristRouteId));

            picture.TouristRouteId = touristRouteId;
            _context.TouristRoutePictures.Add(picture);
        }

        public void DeleteTouristRoute(TouristRoute touristRoute)
        {
            _context.TouristRoutes.Remove(touristRoute);
        }

        public void DeleteTouristRoutePicture(TouristRoutePicture picture)
        {
            _context.TouristRoutePictures.Remove(picture);
        }

        public async Task<ShoppingCart> GetShoppingCartByUserIdAsync(string userId)
        {
            return await _context.ShoppingCarts
                .Include(s => s.User)
                .Include(s => s.ShoppingCartItem)
                .ThenInclude(li => li.TouristRoute)
                .Where(s => s.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public void CreateShoppingCart(ShoppingCart shoppingCart)
        {
             _context.ShoppingCarts.Add(shoppingCart);
        }

        public void AddShoppingCartItem(LineItem lineItem)
        {
            _context.LineItemss.Add(lineItem);
        }

        public async Task<LineItem> GetShoppingCartItemByIdAsync(int itemId)
        {
            return await _context.LineItemss.Where(li => li.Id == itemId).FirstOrDefaultAsync();
        }

        public void DeleteShoppingCartItem(LineItem lineItem)
        {
            _context.LineItemss.Remove(lineItem);
        }

        public async Task<IEnumerable<LineItem>> GetShoppingCartItemsByIds(IEnumerable<int> itemIds)
        {
            return await _context.LineItemss.Where(li => itemIds.Contains(li.Id)).ToArrayAsync();
        }

        public void DeleteShoppingCartItems(IEnumerable<LineItem> items)
        {
            _context.LineItemss.RemoveRange(items);
        }

        public void AddOrder(Order order)
        {
            _context.Orders.Add(order);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserId(string userId)
        {
            return await _context.Orders.Where(o => o.UserId == userId).ToArrayAsync();
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }
    }
}