using System;
using System.Collections.Generic;
using System.Linq;
using QueueingSystem.Models;

namespace QueueingSystem.Models
{
    public class LaneQueue
    {
        private const int LANE_DEFAULT_CAPACITY = 10;
        private readonly TimeSpan LANE_DEFAULT_TOLERANCE;

        public int LaneQueueID { get; set; }
        public List<QueueTicket> QueueList { get; private set; }
        public QueueAttendant Attendant { get; private set; }
        public Lane QueueLane { get; private set; }

        /// <summary>
        /// maximum amount of time to consider priority numbers for queues
        /// </summary>
        public TimeSpan Tolerance { get; set; }

        public LaneQueue()
        {
            QueueList = new List<QueueTicket>();
            Attendant = new QueueAttendant();
            QueueLane = new Lane();
            QueueLane.Capacity = LANE_DEFAULT_CAPACITY;
            Tolerance = LANE_DEFAULT_TOLERANCE;
        }

        /// <summary>
        /// Sets the queue list, queue tickets must be of the same lane
        /// </summary>
        /// <param name="queueList"></param>
        /// <returns></returns>
        public bool SetQueueList(List<QueueTicket> queueList)
        {
            //we must assume that the Lane has been already set here, otherwise it won't set
            //prevent overriding the list here if the one given is empty

            if(queueList.Count != 0 && 
                queueList.Count <= QueueLane.Capacity &&
                !queueList.Any( item => item.QueueLane.LaneID != QueueLane.LaneID)
                )
            {
                //list must be sorted by priority number and latest queue date time
                //and following the tolerance rule

                QueueList = new List<QueueTicket>(queueList);
                QueueList.Sort(CompareToWithTolerance); //uses a modified comparator for queuing logic
                //reverse the list since it is in ascending order
                QueueList.Reverse();

                Attendant.DesignatedLane = QueueLane;
                return true;
            }
            else if (queueList.Count != 0 &&
                queueList.Count > QueueLane.Capacity &&
                !queueList.Any(item => item.QueueLane.LaneID != QueueLane.LaneID))
            {
                //trim queue list to fit intended capacity of this lane queue
                for (int i = 0; i < queueList.Count; i++)
                {
                    queueList.RemoveAt(QueueLane.Capacity + i);
                }

                //list must be sorted by priority number and latest queue date time
                //and following the tolerance rule
                QueueList = new List<QueueTicket>(queueList);
                QueueList.Sort(CompareToWithTolerance); //uses a modified comparator for queuing logic

                //reverse the list since it is in ascending order
                QueueList.Reverse();

                Attendant.DesignatedLane = QueueLane;
                return true;
            }
            else
            {
                //cases: empty list given, there are some tickets that do not match to this lane
                return false;
            }
            
        }

        /// <summary>
        /// Enqueues a queue ticket to the queue list with priority considerations
        /// </summary>
        /// <param name="newTicket"></param>
        public void EnqueueTicket(QueueTicket newTicket)
        {
            if(QueueList.Count < QueueLane.Capacity &&
                newTicket.QueueLane.LaneID == newTicket.QueueLane.LaneID)
            {
                int indexToInsert = QueueList.FindIndex(
                    item => item.HasHigherPriority(
                        newTicket.PriorityNumber,
                        newTicket.QueueDateTime,
                        Tolerance
                        ));

                if (indexToInsert != -1)
                {
                    QueueList.Insert(indexToInsert, newTicket);
                }
                else
                {
                    //index at end of list
                    QueueList.Add(newTicket);
                }
                    

            }
        }

        /// <summary>
        /// Gets and removes the ticket at the front of the queue, returns null if empty
        /// </summary>
        /// <returns></returns>
        public QueueTicket DequeueTicket()
        {
            if (QueueList.Count != 0)
            {
                var ticketToReturn = QueueList[0];
                QueueList.RemoveAt(0);
                return ticketToReturn;
            }
            else
            {
                return null;
            }
            
        }

        /// <summary>
        /// Gets only the ticket at the front of the queue, returns null if empty
        /// </summary>
        /// <returns></returns>
        public QueueTicket PeekTicket()
        {
            if (QueueList.Count != 0)
            {
                return QueueList[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Resets the queue list
        /// </summary>
        public void EmptyQueue()
        {
            QueueList = new List<QueueTicket>();
        }

        public void SetAttendant(QueueAttendant attendant)
        {
            attendant.DesignatedLane = QueueLane;
            Attendant = attendant;
        }

        public void SetQueueLane(Lane queueLane)
        {
            QueueLane = queueLane;
            Attendant.DesignatedLane = QueueLane;
            foreach (var item in QueueList)
            {
                item.QueueLane = QueueLane;
            }
            
        }

        public int GetQueueCapacity()
        {
            return QueueLane.Capacity;
        }

        public void SetQueueCapacity(int capacity)
        {
            if (QueueList.Count > capacity)
            {
                //trim queue list to fit intended capacity of this lane queue
                for (int i = 0; i < QueueList.Count; i++)
                {
                    QueueList.RemoveAt(QueueLane.Capacity + i);
                }
            }

            QueueLane.Capacity = capacity;
        }

        private int CompareToWithTolerance(QueueTicket item, QueueTicket nextItem)
        {
            //HasHigherPriority comparison is in the perspective of the parameter object

            //next item has higher priority
            if (item.HasHigherPriority(
                nextItem.PriorityNumber,
                nextItem.QueueDateTime,
                this.Tolerance
                ))
            {
                return -1;
            }
            //current item has higher priority
            else
            {
                return 1;
            }
        }
    }
}
