using MeetingPointAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeetingPointAPI.Services.Interfaces
{
    public interface IHereService
    {
        Task<HereExploreResponseResult> GetPlaces(Coordinate coordinate, IEnumerable<string> categories, int radius);
        Task<HereRouteResponse> GetRoutes(DateTime time, Coordinate from, Coordinate to, string mode = "publicTransport");
    }
}
