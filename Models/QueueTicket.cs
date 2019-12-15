using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace QueueingSystem.Models
{
    [DataContract]
    public enum QueueStatus
    { 
        [EnumMember]ONGOING, 
        [EnumMember]WAITING,
        [EnumMember]FINISHED
    }

    [DataContract]
    public class QueueTicket
    {
        [DataMember]
        public string QueueID { get; set; }

        [DataMember]
        public int QueueNumber { get; set; }

        [DataMember]
        public Lane QueueLane { get; set; }

        [DataMember]
        public short PriorityNumber { get; set; }

        [IgnoreDataMember]
        public object owner;

        [DataMember]
        public DateTime QueueDateTime { get; set; }

        [DataMember]
        public QueueStatus Status { get; set; }

        public QueueTicket()
        {
            QueueLane = new Lane();
            PriorityNumber = 0; //default priority
            QueueDateTime = DateTime.Now;
            Status = QueueStatus.WAITING;
        }

        public QueueTicket(QueueTicket otherQT)
        {
            QueueID = otherQT.QueueID;
            QueueNumber = otherQT.QueueNumber;
            QueueLane = new Lane(otherQT.QueueLane);
            PriorityNumber = otherQT.PriorityNumber;
            this.owner = otherQT.owner;
            QueueDateTime = otherQT.QueueDateTime;
            Status = otherQT.Status;
        }

        public void SetOwner(User owner)
        {
            this.owner = owner;
        }

        public void SetOwner(Guest owner)
        {
            this.owner = owner;
        }

        public object GetOwner()
        {
            return owner;
        }

        /// <summary>
        /// Compare a given priority if it is higher than in this instance, higher values indicate higher priority.
        /// </summary>
        /// <param name="priorityNumber"></param>
        /// <returns></returns>
        public bool HasHigherPriority(short priorityNumber)
        {
            return priorityNumber > PriorityNumber;
        }

        /// <summary>
        /// Compare given date time if it is earlier than in this instance, then it has higher priority
        /// </summary>
        /// <param name="queueDateTime"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public bool HasHigherPriority(DateTime queueDateTime)
        {
            TimeSpan diffDuration = queueDateTime.Subtract(QueueDateTime);
            
            //negative value means given date time is earlier
            return diffDuration.CompareTo(TimeSpan.Zero) < 0;
        }

        /// <summary>
        /// Compare a given priority if it is higher than in this instance, higher values indicate higher priority.
        /// DateTime is considered here to consider the time aspect of the queues.
        /// Given DateTime must be within tolerance, else it automatically has higher priority if it is earlier
        /// Default tolerance is 24 hours.
        /// </summary>
        /// <param name="priorityNumber"></param>
        /// <param name="queueDateTime"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public bool HasHigherPriority(short priorityNumber, DateTime queueDateTime, TimeSpan? tolerance)
        {
            if (tolerance == null || tolerance.GetValueOrDefault() == TimeSpan.Zero)
            {
                //default tolerance of a day
                tolerance = TimeSpan.FromHours(24);
            }
            TimeSpan diffDuration = queueDateTime.Subtract(QueueDateTime);

            //negative value means given date time is earlier
            bool isEarlierDateTime = diffDuration.CompareTo(TimeSpan.Zero) < 0;

            // difference between the compared date times must be within tolerance, 
            return (
                diffDuration.Duration() <= tolerance
                    // given priorityNumber must be higher than in this instance to have more priority
                    ?(priorityNumber > this.PriorityNumber
                        ? true
                        // else, consider if their priorities are equal
                        : priorityNumber == this.PriorityNumber
                            // then, the earlier one has priority
                            ? isEarlierDateTime
                                // if less, then it automatically has less priority
                                : false)
                    // else the earlier date has automatically higher priority
                    : isEarlierDateTime
                    );
                
        }
    }
}