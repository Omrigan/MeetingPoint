using MeetingPointAPI.Models;
using System;

namespace MeetingPointAPI.Entities
{
    public class MemberLocationEntity
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Guid GroupUid { get; set; }
        public string MemberId { get; set; }

        public Coordinate GetCoordinate() => new Coordinate
        {
            Longitude = Longitude,
            Latitude = Latitude
        };
    }
}
