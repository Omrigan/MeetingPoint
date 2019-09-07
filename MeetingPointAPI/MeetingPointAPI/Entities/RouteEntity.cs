using System;

namespace MeetingPointAPI.Entities
{
    public class RouteEntity
    {
        public int Id { get; set; }
        public Guid GroupUid { get; set; }
        public int LocatioId { get; set; }
        public string MemberRoutes { get; set; }
    }
}
