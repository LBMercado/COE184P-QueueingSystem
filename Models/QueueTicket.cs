using System;
using System.Collections.Generic;
using System.Text;

namespace QueueingSystem.Models
{
    public enum QueueStatus{ ONGOING, WAITING, FINISHED}

    public class QueueTicket
    {
        public string QueueID { get; set; }

        public int QueueNumber { get; set; }

        public Lane QueueLane { get; set; }

        public short PriorityNumber { get; set; }

        public object owner;

        public DateTime QueueDateTime { get; set; }

        public QueueStatus Status { get; set; }

        public QueueTicket()
        {
            QueueLane = new Lane();
            PriorityNumber = 0; //default priority
            QueueDateTime = DateTime.Now;
            Status = QueueStatus.WAITING;
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
        public bool HasHigherPriority(short priorityNumber, DateTime queueDateTime, TimeSpan tolerance = default)
        {
            if (tolerance == default)
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
