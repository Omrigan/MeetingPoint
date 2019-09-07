namespace MeetingPointAPI.Models
{
    public class HereRouteResponse
    {
        public HereRouteResponseWrap Response { get; set; }
    }

    public class HereRouteResponseWrap
    {
        public HereRouteInfo[] Route { get; set; }
    }

    public class HereRouteInfo
    {
        public HereLegInfo[] Leg { get; set; }
        public HerePublicTransportLine[] PublicTransportLine { get; set; }
    }

    public class HerePublicTransportLine
    {
        public string LineName { get; set; }
    }

    public class HereLegInfo
    {
        public int Length { get; set; }
        public int TravelTime { get; set; }
        public HereManeuverInfo[] Maneuver { get; set; }
    }

    public class HereManeuverInfo
    {
        public Coordinate Position { get; set; }
        public int Length { get; set; }
        public int TravelTime { get; set; }
        public int FirstPoint { get; set; }
        public int LastPoint { get; set; }
        public string Id { get; set; }
        public string Action { get; set; }
        public string StopName { get; set; }
    }
}
