using System;
using System.Collections.Generic;
using System.Text;

namespace QueueingSystem.Models
{
    public class Lane
    {
        public int LaneID { get; set; }

        public string LaneName { get; set; }

        public int LaneNumber { get; set; }

        public int Capacity { get; set; }

        public Lane() { }

        public Lane(Lane otherLane)
        {
            LaneID = otherLane.LaneID;
            LaneName = otherLane.LaneName;
            LaneNumber = otherLane.LaneNumber;
            Capacity = otherLane.Capacity;
        }
    }
}
