using MeetingPointAPI.Entities;
using MeetingPointAPI.Models;

namespace MeetingPointAPI.Helpers
{
    public static class ModelConverter
    {
        public static Place ToPlace(LocationEntity location)
        {
            return new Place
            {
                Title = location.Title,
                Category = location.Category,
                Coordinate = new Coordinate
                {
                    Longitude = location.Longitude,
                    Latitude = location.Latitude
                },
                Distance = location.Distance,
                Href = location.Href,
                Icon = location.Icon,
                Type = location.Type,
                Vicinity = location.Vicinity
            };
        }
    }
}
