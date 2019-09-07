using System;

namespace MeetingPointAPI.Models
{
    public class RoutesToPlace
    {
        public Guid GroupUid { get; set; }
        public Place Place { get; set; }
        public object MemberRoutes { get; set; }
    }

    public class Place
    {
        public Coordinate Coordinate { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Vicinity { get; set; }
        public string Icon { get; set; }
        public string Href { get; set; }
        public int? Distance { get; set; }
        public string Category { get; set; }
    }
}
