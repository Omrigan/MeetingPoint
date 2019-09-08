using System.Collections.Generic;
using System.Linq;

namespace MeetingPointAPI.Models
{
    public class GroupRoutes
    {
        public List<MemberRoute> MemberRoutes { get; set; }
        public double SumTime { get; set; }
        public string Title { get; set; }
    }

    public class MemberRoute
    {
        public Coordinate MemberLocation { get; set; }
        public string MemberId { get; set; }
        public List<TargetRoute> Route { get; set; }
    }

    public class TargetRoute
    {
        public int TravelTime { get; set; }
        public IEnumerable<TransportRoute> Routes { get; set; }

        public RoutePoint GetStartPoint() => Routes.FirstOrDefault().Points.FirstOrDefault();
        public RoutePoint GetLastPoint() => Routes.LastOrDefault().Points.LastOrDefault();
    }
}
