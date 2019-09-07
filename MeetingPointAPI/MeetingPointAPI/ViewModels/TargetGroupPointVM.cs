using System;
using System.Collections.Generic;

namespace MeetingPointAPI.ViewModels
{
    public class TargetGroupPointVM
    {
        public Guid GroupUid { get; set; }
        public IEnumerable<string> Category { get; set; }
        public DateTime? Time { get; set; }
    }
}
