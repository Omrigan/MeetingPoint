using MeetingPointAPI.Helpers;
using MeetingPointAPI.Models;
using MeetingPointAPI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingPointAPI.Services
{
    public class HereService : IHereService
    {
        private readonly string appId = "nCSzEMs5Mt4xNwpSu67q";
        private readonly string appCode = "BKcZWZqhrhY2sMaIlmKh6Q";

        private static NumberFormatInfo NFI = new NumberFormatInfo { NumberDecimalSeparator = "." };

        public async Task<HereExploreResponseResult> GetPlaces(Coordinate coordinate, IEnumerable<string> categories, int radius)
        {
            var url = new StringBuilder()
                .Append("https://places.demo.api.here.com/places/v1/discover/explore")
                .Append($"?app_id={appId}")
                .Append($"&app_code={appCode}")
                .Append(categories == null || !categories.Any() ? string.Empty : $"&cat={string.Join(",", categories)}")
                .Append($"&in={coordinate.Latitude.ToString(NFI)},{coordinate.Longitude.ToString(NFI)};r={radius.ToString()}")
                .Append("&Accept-Language=ru-RU")
                .ToString();

            return await HttpHepler.GetResult<HereExploreResponseResult>(url);
        }

        public async Task<HereRouteResponse> GetRoutes(DateTime time, Coordinate from, Coordinate to, string mode = "publicTransport")
        {
            var url = new StringBuilder()
                .Append("https://route.api.here.com/routing/7.2/calculateroute.json")
                .Append($"?app_id={appId}")
                .Append($"&app_code={appCode}")
                .Append($"&language=ru-ru")
                .Append($"&mode=fastest;{mode}")
                .Append($"&maneuverattributes=po,ti,pt,ac,di,fj,ix")
                .Append($"&routeattributes=sh,gr")
                .Append($"&waypoint0=geo!stopOver!{from.Latitude.ToString(NFI)},{from.Longitude.ToString(NFI)}")
                .Append($"&waypoint1=geo!stopOver!{to.Latitude.ToString(NFI)},{to.Longitude.ToString(NFI)}")
                .Append($"&departure={time.ToString("yyyy-MM-ddTHH:mm:ss")}")
                .ToString();

            return await HttpHepler.GetResult<HereRouteResponse>(url);
        }
    }
}
