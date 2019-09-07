using MeetingPointAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MeetingPointAPI.Helpers
{
    public static class CoordinateHelper
    {
        public static Coordinate GetTargetCoordinate(IEnumerable<Coordinate> points)
        {
            var sumPoint = points.Aggregate((p1, p2) => new Coordinate
            {
                Longitude = p1.Longitude + p2.Longitude,
                Latitude = p1.Latitude + p2.Latitude
            });

            var countPoints = points.Count();

            return new Coordinate
            {
                Longitude = sumPoint.Longitude / countPoints,
                Latitude = sumPoint.Latitude / countPoints
            };
        }

        public static double GetMaxDistance(Coordinate center, IEnumerable<Coordinate> points)
        {
            var maxDistance = double.MinValue;
            foreach(var point in points)
            {
                var distance = Math.Pow(point.Latitude - center.Latitude, 2) + Math.Pow(point.Longitude + center.Longitude, 2);
                if (distance > maxDistance)
                    maxDistance = distance;
            }

            return maxDistance == double.MinValue ? 1000 : Math.Sqrt(maxDistance) * 111;
        }
    }
}
