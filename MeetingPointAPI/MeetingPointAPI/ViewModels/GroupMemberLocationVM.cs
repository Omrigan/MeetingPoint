using MeetingPointAPI.Models;
using System;

namespace MeetingPointAPI.ViewModels
{
    public class GroupMemberLocationVM
    {
        public Coordinate Coordinate { get; set; }
        public Guid GroupUid { get; set; }
        public string MemberId { get; set; }
    }
}
