using MeetingPointAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeetingPointAPI.Services.Interfaces
{
    public interface IRouteSearcher
    {
        Task<List<TargetRoute>> GetRoutes(Coordinate from, Coordinate to, DateTime time);
    }
}
