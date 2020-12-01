using System;
using System.Collections.Generic;
using FakeXiecheng.API.Models;

namespace FakeXiecheng.API.Services
{
    public interface ITouristRouteRepository
    {
        IEnumerable<TouristRoute> GetTouristRoutes(string keyword, string ratingOperator, int? ratingValue);

        IEnumerable<TouristRoute> GetTouristRoutesByIds(IEnumerable<Guid> touristRouteIds);

        void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes);

        TouristRoute GetTouristRoute(Guid touristRouteId);

        bool TouristRouteExists(Guid touristRouteId);

        IEnumerable<TouristRoutePicture> GetPictureByTouristRouteId(Guid touristRouteId);

        TouristRoutePicture GetPicture(int pictureId);

        void AddTouristRoute(TouristRoute touristRoute);

        void AddPicture(Guid touristRouteId, TouristRoutePicture picture);

        void DeleteTouristRoute(TouristRoute touristRoute);

        void DeleteTouristRoutePicture(TouristRoutePicture picture);

        bool Save();
    }
}