using System.Collections.Generic;
using System.Linq;

namespace MeetingPointAPI.Models
{
    public class TargetRoute
    {
        public int TravelTime { get; set; }
        public IEnumerable<TransportRoute> Routes { get; set; }

        public RoutePoint GetStartPoint() => Routes.FirstOrDefault().Points.FirstOrDefault();
        public RoutePoint GetLastPoint() => Routes.LastOrDefault().Points.LastOrDefault();
    }
}
