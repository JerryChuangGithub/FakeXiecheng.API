using System;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<TouristRoute> GetTouristRoutes(
            string keyword,
            string ratingOperator,
            int ratingValue)
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

            return query;
        }

        public TouristRoute GetTouristRoute(Guid touristRouteId)
        {
            return _context.TouristRoutes.Include(x => x.TouristRoutePictures).FirstOrDefault(x => x.Id == touristRouteId);
        }

        public bool TouristRouteExists(Guid touristRouteId)
        {
            return _context.TouristRoutes.Any(x => x.Id == touristRouteId);
        }

        public IEnumerable<TouristRoutePicture> GetPictureByTouristRouteId(Guid touristRouteId)
        {
            return _context.TouristRoutePictures.Where(x => x.TouristRouteId == touristRouteId);
        }

        public TouristRoutePicture GetPicture(int pictureId)
        {
            return _context.TouristRoutePictures.FirstOrDefault(x => x.Id == pictureId);
        }
    }
}