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

            return query;
        }

        public IEnumerable<TouristRoute> GetTouristRoutesByIds(IEnumerable<Guid> touristRouteIds)
        {
            return _context.TouristRoutes.Where(x => touristRouteIds.Contains(x.Id));
        }

        public void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes)
        {
            _context.TouristRoutes.RemoveRange(touristRoutes);
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

        public bool Save()
        {
            return _context.SaveChanges() >= 0;
        }
    }
}