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
    }
}
