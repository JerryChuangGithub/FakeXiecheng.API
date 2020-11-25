using System;
using System.Collections.Generic;
using System.Linq;
using FakeXiecheng.API.Models;

namespace FakeXiecheng.API.Services
{
    public class MockTouristRouteRepository// : ITouristRouteRepository
    {
        private readonly List<TouristRoute> _routes;

        public MockTouristRouteRepository()
        {
            if (this._routes == null)
            {
                this._routes = InitializeTouristRoutes();
            }
        }

        public IEnumerable<TouristRoute> GetTouristRoutes()
        {
            return this._routes;
        }

        public TouristRoute GetTouristRoute(Guid touristRouteId)
        {
            return this._routes.FirstOrDefault(x => x.Id == touristRouteId);
        }

        private static List<TouristRoute> InitializeTouristRoutes()
        {
            return new List<TouristRoute>
            {
                new TouristRoute
                {
                    Id = Guid.NewGuid(),
                    Title = "太魯閣",
                    Description = "風光明媚太魯閣行",
                    OriginalPrice = 5999,
                    Features = "<p>享受大自然</p>",
                    Fees = "<p>自行前往</p>",
                    Notes = "<p>注意落石</p>"
                },
                new TouristRoute
                {
                    Id = Guid.NewGuid(),
                    Title = "阿里山",
                    Description = "阿里山享受芬多精",
                    OriginalPrice = 5999,
                    Features = "<p>遠離塵囂</p>",
                    Fees = "<p>自行前往</p>",
                    Notes = "<p>注意颱風季節</p>"
                }
            };
        }
    }
}