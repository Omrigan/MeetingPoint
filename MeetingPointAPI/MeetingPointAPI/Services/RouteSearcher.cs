using MeetingPointAPI.Models;
using MeetingPointAPI.Repositories;
using MeetingPointAPI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingPointAPI.Services
{
    public class RouteSearcher : IRouteSearcher
    {
        private readonly IHereService _hereService;
        private readonly DBRepository _dbRepository;

        public RouteSearcher(IHereService hereService, DBRepository dbRepository)
        {
            _hereService = hereService;
            _dbRepository = dbRepository;
        }

        public async Task<List<TargetRoute>> GetRoutes(Coordinate from, Coordinate to, DateTime time) => await GetHereRoutes(from, to, time);

        private async Task<List<TargetRoute>> GetHereRoutes(Coordinate from, Coordinate to, DateTime time, bool allowPedestrian = false)
        {
            var result = await _hereService.GetRoutes(time, from, to);
            if (result == null && allowPedestrian)
                result = await _hereService.GetRoutes(time, from, to, "pedestrian");

            if (result == null)
                return null;

            return result.Response.Route.Select(r =>
            {
                var maneuvers = r.Leg.FirstOrDefault().Maneuver;
                var transports = r.PublicTransportLine;

                var routes = new List<TransportRoute>();
                int i = 0;
                int travelTime = 0;
                var route = new TransportRoute() { Transport = "Пешком", Points = new List<RoutePoint>() };
                foreach (var maneuver in maneuvers)
                {
                    var p = new RoutePoint
                    {
                        Description = maneuver.StopName,
                        Time = time.AddSeconds(travelTime).ToString("HH:mm:ss"),
                        Coordinate = new Coordinate
                        {
                            Latitude = maneuver.Position.Latitude,
                            Longitude = maneuver.Position.Longitude
                        }
                    };
                    route.Points.Add(p);
                    travelTime += maneuver.TravelTime;

                    if (maneuver.Action == "enter")
                    {
                        if (route.Points.Count() > 1)
                            routes.Add(route);

                        route = new TransportRoute() { Transport = $"Автобус {transports[i].LineName}", Points = new List<RoutePoint>() { p } };
                        i++;
                    }

                    if (maneuver.Action == "leave")
                    {
                        routes.Add(route);
                        route = new TransportRoute() { Transport = $"Пешком", Points = new List<RoutePoint>() };
                    }
                }
                routes.Add(route);

                return new TargetRoute
                {
                    Routes = routes,
                    TravelTime = r.Leg.FirstOrDefault().TravelTime
                };
            })
            .ToList();
        }
    }
}
