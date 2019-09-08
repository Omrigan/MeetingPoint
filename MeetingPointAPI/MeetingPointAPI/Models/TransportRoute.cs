using System;
using System.Collections.Generic;
using System.Globalization;

namespace MeetingPointAPI.Models
{
    public class TransportRoute
    {
        public string Transport { get; set; }
        public List<RoutePoint> Points { get; set; }
    }

    public class RoutePoint
    {
        public Coordinate Coordinate { get; set; }
        public string Description { get; set; }
        public string Time { get; set; }

        public TimeSpan GetTime() => DateTime.ParseExact(Time, "HH:mm:ss", CultureInfo.InvariantCulture).TimeOfDay;
    }
}
