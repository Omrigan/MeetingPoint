using MeetingPointAPI.Models;
using System;

namespace MeetingPointAPI.ViewModels
{
    public class GroupMemberLocationVM : GroupMemberVM
    {
        public Coordinate Coordinate { get; set; }
    }

    public class GroupMemberVM : GroupVM
    {
        public string MemberId { get; set; }
    }

    public class GroupVM
    {
        public Guid GroupUid { get; set; }
    }
}
