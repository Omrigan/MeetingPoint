using System.Collections.Generic;

namespace MeetingPointAPI.Models
{
    public class HereExploreResponseResult
    {
        public HereExploreResult Results { get; set; }
    }

    public class HereExploreResult
    {
        public IEnumerable<HereExploreItem> Items { get; set; }
    }

    public class HereExploreItem
    {
        public double[] Position { get; set; }
        public int? Distance { get; set; }
        public string Title { get; set; }
        public double? AverageRating { get; set; }
        public HereExploreCategory Category { get; set; }
        public string Icon { get; set; }
        public string Vicinity { get; set; }
        public string Type { get; set; }
        public string Href { get; set; }
        public string Id { get; set; }

        public Coordinate GetCoordinate() => new Coordinate
        {
            Latitude = Position[0],
            Longitude = Position[1]
        };
    }

    public class HereExploreCategory
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Href { get; set; }
        public string Type { get; set; }
    }
}
