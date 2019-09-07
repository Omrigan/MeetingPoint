using MeetingPointAPI.Entities;
using MeetingPointAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeetingPointAPI.Services.Interfaces
{
    public interface IRouteSearcher
    {
        Task<GroupRoutes> GetGroupRoutes(List<MemberLocationEntity> memberLocations, string title, Coordinate to, DateTime time);
        Task<MemberRoute> GetMemberRoutes(string memberId, Coordinate from, Coordinate to, DateTime time);
        Task<List<TargetRoute>> GetHereRoutes(Coordinate from, Coordinate to, DateTime time, bool allowPedestrian = false);
    }
}
