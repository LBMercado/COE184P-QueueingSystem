using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace QueueingSystem.Models
{
    [DataContract]
    public class Lane
    {
        [DataMember]
        public int LaneID { get; set; }

        [DataMember]
        public string LaneName { get; set; }

        [DataMember]
        public int LaneNumber { get; set; }

        [DataMember]
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
