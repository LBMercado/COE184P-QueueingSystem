using System;
using System.Collections.Generic;
using System.Text;
using QueueingSystem.Models;
using QueueingSystem.DataAccess;
using System.Linq;

namespace QueueingSystem.BusinessLogic
{
    public class QueueingSystem : IQueueingSystem
    {
        private string connectionString;
        private DataAccess.DataAccess dataAccessLogic;
        private Dictionary<int, LaneQueue> laneQueues;
        private Dictionary<QueueStatus, int> queueStatusMapper;

        public QueueingSystem(string connectionString)
        {
            this.connectionString = connectionString;

            dataAccessLogic = new DataAccess.DataAccess(this.connectionString);

            queueStatusMapper = dataAccessLogic.GetQueueStatuses();
            InitializeLaneQueues();
        }

        /// <summary>
        /// Resets the application initial values, use sparingly
        /// </summary>
        private void InitializeLaneQueues()
        {
            laneQueues = new Dictionary<int, LaneQueue>();
            foreach (var lane in dataAccessLogic.GetLanes())
            {
                var laneQueue = dataAccessLogic.GetLaneQueueWithLaneID(lane.LaneID);

                //consider lanes which have no lane queues
                if (laneQueue == null)
                {
                    //map the lane number to the lane queue as null
                    laneQueues.Add(
                    lane.LaneNumber,
                    null
                    );

                    continue;
                }

                //get queue tickets for the lane
                //  waiting tickets
                var queueTicketList = dataAccessLogic.GetQueueTicketsWithLaneIDAndStatus(
                        lane.LaneID,
                        QueueStatus.WAITING,
                        queueStatusMapper
                        );
                //  ongoing tickets
                queueTicketList.AddRange(
                    dataAccessLogic.GetQueueTicketsWithLaneIDAndStatus(
                        lane.LaneID,
                        QueueStatus.ONGOING,
                        queueStatusMapper
                        )
                    );

                laneQueue.SetQueueList(queueTicketList);

                //finally, map the lane number to the lane queue instance
                laneQueues.Add(
                    lane.LaneNumber,
                    laneQueue
                    );
            }
        }

        public void SetConnectionString(string connectionString)
        {
            this.connectionString = connectionString;

            dataAccessLogic = new DataAccess.DataAccess(this.connectionString);
        }

        /// <summary>
        /// Gets all lanes
        /// </summary>
        /// <returns></returns>
        public List<Lane> GetAllQueueLanes()
        {
            return dataAccessLogic.GetLanes();
        }

        /// <summary>
        /// Gets the lanes with no lane queues
        /// </summary>
        /// <returns></returns>
        public List<Lane> GetInactiveQueueLanes()
        {
            var laneList = dataAccessLogic.GetLanes();
            var inactiveLaneList = new List<Lane>();

            //inactive if it has no lane queues
            foreach(var lane in laneList)
            {
                if (dataAccessLogic.GetLaneQueueWithLaneID(lane.LaneID) == null)
                {
                    inactiveLaneList.Add(lane);
                }
            }

            return inactiveLaneList;
        }

        /// <summary>
        /// Gets the lanes which have lane queues
        /// </summary>
        /// <returns></returns>
        public List<Lane> GetActiveQueueLanes()
        {
            var laneList = dataAccessLogic.GetLanes();
            var activeLaneList = new List<Lane>();

            //active if it has lane queues
            foreach (var lane in laneList)
            {
                if (dataAccessLogic.GetLaneQueueWithLaneID(lane.LaneID) != null)
                {
                    activeLaneList.Add(lane);
                }
            }

            return activeLaneList;
        }

        /// <summary>
        /// Gets the lane associated with the lane number
        /// </summary>
        /// <param name="queueLaneNumber"></param>
        /// <returns></returns>
        public Lane GetQueueLane(int queueLaneNumber)
        {
            return dataAccessLogic.GetLaneWithLaneNumber(queueLaneNumber);
        }

        /// <summary>
        /// Gets the lane where the queue ticket belongs to given its ticket id
        /// </summary>
        /// <param name="queueTicketID"></param>
        /// <returns></returns>
        public Lane GetQueueLaneOfTicket(string queueTicketID)
        {
            var queueTicket = dataAccessLogic.GetQueueTicketWithTicketID(queueTicketID);

            if (queueTicket != null)
                return queueTicket.QueueLane;
            else
                return null;
        }

        /// <summary>
        /// Gets the total amount of lanes available
        /// </summary>
        /// <returns></returns>
        public int GetLaneCount()
        {
            var laneList = dataAccessLogic.GetLanes();

            return laneList.Count;
        }

        /// <summary>
        /// Gets the amount of lanes with lane queues
        /// </summary>
        /// <returns></returns>
        public int GetActiveLaneCount()
        {
            var laneList = dataAccessLogic.GetLanes();
            var activeLaneList = new List<Lane>();

            //active if it has lane queues
            foreach (var lane in laneList)
            {
                if (dataAccessLogic.GetLaneQueueWithLaneID(lane.LaneID) != null)
                {
                    activeLaneList.Add(lane);
                }
            }

            return activeLaneList.Count;
        }

        /// <summary>
        /// Gets the amount of lanes without lane queues
        /// </summary>
        /// <returns></returns>
        public int GetInactiveLaneCount()
        {
            var laneList = dataAccessLogic.GetLanes();
            var inactiveLaneList = new List<Lane>();

            //inactive if it has no lane queues
            foreach (var lane in laneList)
            {
                if (dataAccessLogic.GetLaneQueueWithLaneID(lane.LaneID) == null)
                {
                    inactiveLaneList.Add(lane);
                }
            }

            return inactiveLaneList.Count;
        }

        public bool IsFullQueueAtLane(int queueLaneNumber)
        {
            if (laneQueues.TryGetValue(queueLaneNumber, out LaneQueue laneQueue))
            {
                if(laneQueue == null)
                {
                    //lane is inactive
                    return false;
                }
                else
                {
                    return laneQueue.GetQueueCapacity()
                        == laneQueue.QueueList.Count;
                }
            }
            else
            {
                //queue lane number is unused
                throw new ArgumentException("queue lane number provided does not point to a lane");
            }
        }

        public bool IsEmptyQueueAtLane(int queueLaneNumber)
        {
            if (laneQueues.TryGetValue(queueLaneNumber, out LaneQueue laneQueue))
            {
                if (laneQueue == null)
                {
                    //lane is inactive
                    return true;
                }
                else
                {
                    return laneQueue.QueueList.Count
                        == 0;
                }
            }
            else
            {
                //queue lane number is unused
                throw new ArgumentException("queue lane number provided does not point to a lane");
            }
        }

        public bool IsLaneNumberUsed(int queueLaneNumber)
        {
            return laneQueues.ContainsKey(queueLaneNumber);
        }

        /// <summary>
        /// Gets the number of queued persons at the specified lane given the lane number.
        /// </summary>
        /// <param name="queueLaneNumber"></param>
        /// <returns></returns>
        public int GetQueueCountAtLane(int queueLaneNumber)
        {
            if (laneQueues.TryGetValue(queueLaneNumber, out LaneQueue laneQueue))
            {
                if (laneQueue == null)
                {
                    //lane is inactive
                    return 0;
                }
                else
                {
                    return laneQueue.QueueList.Count;
                }
            }
            else
            {
                //queue lane number is unused
                throw new ArgumentException("queue lane number provided does not point to a lane");
            }
        }

        public int GetQueueCapacityOfLane(int queueLaneNumber)
        {
            if (laneQueues.TryGetValue(queueLaneNumber, out LaneQueue laneQueue))
            {
                if (laneQueue == null)
                {
                    //lane is inactive
                    return 0;
                }
                else
                {
                    return laneQueue.GetQueueCapacity();
                }
            }
            else
            {
                //queue lane number is unused
                throw new ArgumentException("queue lane number provided does not point to a lane");
            }
        }

        /// <summary>
        /// Sets the capacity of the lane queue, trims the queue if the current amount does not meet the capacity
        /// </summary>
        /// <param name="queueLaneNumber"></param>
        /// <param name="queueCapacity"></param>
        public void SetLaneQueueCapacity(int queueLaneNumber, int queueCapacity)
        {
            if (laneQueues.TryGetValue(queueLaneNumber, out LaneQueue laneQueue))
            {
                if (laneQueue == null)
                {
                    //lane is inactive
                }
                else
                {
                    laneQueue.SetQueueCapacity(queueCapacity);

                    dataAccessLogic.EditLaneWithLaneID(laneQueue.QueueLane);

                    laneQueues[queueLaneNumber] = laneQueue;
                }
                
            }
        }

        /// <summary>
        /// Sets the maximum amount of time to have priority numbers to take effect
        /// </summary>
        /// <param name="queueLaneNumber"></param>
        /// <param name="timeTolerance"></param>
        public void SetLaneQueueTolerance(int queueLaneNumber, TimeSpan timeTolerance)
        {
            if (laneQueues.TryGetValue(queueLaneNumber, out LaneQueue laneQueue))
            {
                if (laneQueue == null)
                {
                    //lane is inactive
                }
                else
                {
                    laneQueue.Tolerance = timeTolerance;

                    dataAccessLogic.EditLaneQueue(laneQueue);

                    laneQueues[queueLaneNumber] = laneQueue;
                }  
            }
        }

        public bool SetQueueAttendantOfLane(int queueLaneNumber, QueueAttendant queueAttendant)
        {
            if (laneQueues.TryGetValue(queueLaneNumber, out LaneQueue laneQueue))
            {
                if (laneQueue == null)
                {
                    //lane is inactive
                    return false;
                }
                else
                {
                    laneQueue.SetAttendant(queueAttendant);

                    dataAccessLogic.EditDesignatedLaneOfQueueAttendant(queueAttendant.QueueAttendantID,
                        laneQueue.QueueLane.LaneID);

                    laneQueues[queueLaneNumber] = laneQueue;
                    return true;
                }
                
            }
            else
            {
                return false;
            }
        }

        public QueueAttendant GetQueueAttendantOfLane(int queueLaneNumber)
        {
            if (laneQueues.TryGetValue(queueLaneNumber, out LaneQueue laneQueue))
            {
                if (laneQueue == null)
                {
                    //lane is inactive
                    return null;
                }
                else
                {
                    return laneQueue.Attendant;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Checks whether the account object is a Guest account
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool IsGuest(object account)
        {
            return account is Guest;
        }

        /// <summary>
        /// Checks whether the account object is a User account
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool IsUser(object account)
        {
            return account is User;
        }

        public object GetFrontQueuedInLane(int queueLaneNumber)
        {
            if (laneQueues.TryGetValue(queueLaneNumber, out LaneQueue laneQueue))
            {
                if (laneQueue == null)
                {
                    //lane is inactive
                    return null;
                }
                else
                {
                    if (laneQueue.QueueList.Count != 0)
                    {
                        return laneQueue.QueueList[0];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                return null;
            }
        }

        public List<object> GetListOfQueuedInLane(int queueLaneNumber)
        {
            var queuedList = new List<object>();
            if (laneQueues.TryGetValue(queueLaneNumber, out LaneQueue laneQueue) 
                && laneQueue != null && laneQueue.QueueList.Count != 0)
            {
                //lane must be active and not empty
                foreach (var queueTicket in laneQueue.QueueList)
                {
                    queuedList.Add(queueTicket.GetOwner());
                }


            }
            return queuedList;
        }

        public object GetLastQueuedInLane(int queueLaneNumber)
        {
            object queued = null;
            if (laneQueues.TryGetValue(queueLaneNumber, out LaneQueue laneQueue) 
                && laneQueue != null && laneQueue.QueueList.Count != 0)
            {
                //get last item in queue
                queued = laneQueue.QueueList[laneQueue.QueueList.Count - 1];
            }
            return queued;
        }

        /// <summary>
        /// Enqueue a new ticket to the queue system
        /// </summary>
        /// <param name="queueLaneNumber"></param>
        /// <param name="newQueueTicket"></param>
        /// <returns></returns>
        public bool EnqueueLane(QueueTicket newQueueTicket)
        {
            if (laneQueues.TryGetValue(newQueueTicket.QueueLane.LaneNumber, out LaneQueue laneQueue))
            {
                if (laneQueue == null)
                {
                    //lane is inactive
                    return false;
                }
                else
                {
                    if (laneQueue.QueueList.Count != laneQueue.GetQueueCapacity())
                    {
                        //reflect changes in database
                        bool isSuccess = dataAccessLogic.AddQueueTicket(
                            newQueueTicket,
                            queueStatusMapper
                            );

                        //check if account is user or guest
                        if (newQueueTicket.owner is User user)
                        {
                            newQueueTicket = dataAccessLogic
                            .GetQueueTicketsOfWithStatusAndLaneID(
                            user.AccountNumber,
                            newQueueTicket.QueueLane.LaneID,
                            newQueueTicket.Status,
                            queueStatusMapper
                            )
                            .DefaultIfEmpty(null)
                            .FirstOrDefault();
                        }
                        else if (newQueueTicket.owner is Guest guest)
                        {
                            newQueueTicket = dataAccessLogic
                            .GetQueueTicketsOfWithStatusAndLaneID(
                            guest.AccountNumber,
                            newQueueTicket.QueueLane.LaneID,
                            newQueueTicket.Status,
                            queueStatusMapper
                            )
                            .DefaultIfEmpty(null)
                            .FirstOrDefault();
                        }
                        else
                        {
                            throw new ArgumentException("Unexpected account type for queue ticket, it is not a guest or user.");
                        }

                        if (!isSuccess ||
                            newQueueTicket == null)
                        {
                            throw new Exception("Failed to add or get new queue ticket.");
                        }

                        laneQueue.EnqueueTicket(newQueueTicket);

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Sets the queue status of the front ticket to ongoing, and removes it from the current queue.
        /// Returns the said queue ticket.
        /// </summary>
        /// <param name="queueLaneNumber"></param>
        /// <returns></returns>
        public QueueTicket DequeueLane(int queueLaneNumber)
        {
            if (laneQueues.TryGetValue(queueLaneNumber, out LaneQueue laneQueue))
            {
                if (laneQueue == null)
                {
                    //lane is inactive
                    return null;
                }
                else
                {
                    if (laneQueue.QueueList.Count != 0)
                    {
                        var retTicket = laneQueue.DequeueTicket();

                        retTicket.Status = QueueStatus.ONGOING;

                        bool isSuccess = dataAccessLogic.EditQueueTicketStatusWithTicketID(retTicket.QueueID,
                            QueueStatus.ONGOING,
                            queueStatusMapper);

                        if (isSuccess)
                        {
                            retTicket = dataAccessLogic.GetQueueTicketWithTicketID(retTicket.QueueID);
                        }
                        else
                        {
                            throw new Exception("Failed to edit the queue status of the dequeued ticket");
                        }

                        return retTicket;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Mark the queue ticket as complete
        /// </summary>
        /// <param name="queueTicketID"></param>
        /// <returns></returns>
        public bool FinishQueueTicket(string queueTicketID)
        {
            //get the associated queue ticket with the queueTicketID
            var queueTicket = dataAccessLogic.GetQueueTicketWithTicketID(queueTicketID);
            //  check first if it exists
            //  make sure it is not already finished
            if (queueTicket != null &&
                queueTicket.Status == QueueStatus.FINISHED)
            {                   
                //mark the queue ticket as finished
                queueTicket.Status = QueueStatus.FINISHED;

                //reflect changes in database
                bool isSuccess = dataAccessLogic.EditQueueTicketStatusWithTicketID(
                    queueTicketID,
                    queueTicket.Status,
                    queueStatusMapper
                    );

                if (isSuccess)
                {

                    //reflect changes in application
                    //  refresh queue information
                    laneQueues[queueTicket.QueueLane.LaneNumber] = dataAccessLogic.GetLaneQueueWithLaneID(
                        queueTicket.QueueLane.LaneID
                        );
                    //  refresh queue list
                    //      ongoing tickets
                    var queueList = dataAccessLogic.GetQueueTicketsWithLaneIDAndStatus(
                            queueTicket.QueueLane.LaneID,
                            QueueStatus.ONGOING,
                            queueStatusMapper
                            );
                    //      waiting tickets
                    queueList.AddRange(
                        dataAccessLogic.GetQueueTicketsWithLaneIDAndStatus(
                            queueTicket.QueueLane.LaneID,
                            QueueStatus.WAITING,
                            queueStatusMapper
                            )
                        );

                    laneQueues[queueTicket.QueueLane.LaneNumber].SetQueueList(queueList);

                    return true;
                }
                else
                {
                    throw new Exception("Failed to set the queue status of the queue ticket to success");
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the front ticket without dequeuing it
        /// </summary>
        /// <param name="queueLaneNumber"></param>
        /// <returns></returns>
        public QueueTicket PeekLane(int queueLaneNumber)
        {
            if (laneQueues.TryGetValue(queueLaneNumber, out LaneQueue laneQueue))
            {
                if (laneQueue == null)
                {
                    //lane is inactive
                    return null;
                }
                else
                {
                    if (laneQueue.QueueList.Count != 0)
                    {
                        return laneQueue.PeekTicket();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                return null;
            }
        }

        public bool AddNewQueueLane(Lane newLane)
        {
            //get highest lane number
            var laneCount = GetLaneCount();

            //set the new lane number
            newLane.LaneNumber = laneCount + 1;

            //reflect changes in database
            bool isSuccess = dataAccessLogic.AddLane(newLane);

            if(isSuccess)
            {
                //reflect changes in application
                //set new inactive lane
                laneQueues.Add(newLane.LaneNumber, null);

                return true;
            }
            else
            {
                //quite possibly a conflicting lane number hard coded into database
                return false;
            }
        }

        /// <summary>
        /// Allows queues to be formed at the specified lane, there must be an attendant available
        /// </summary>
        /// <param name="queueLaneNumber"></param>
        /// <param name="attendant"></param>
        /// <param name="tolerance">Maximum amount of time to consider priority numbers</param>
        /// <returns></returns>
        public bool SetLaneActive(int queueLaneNumber,
            QueueAttendant attendant,
            TimeSpan tolerance)
        {
            //check if the lane queue is existing
            if (laneQueues.TryGetValue(queueLaneNumber, out LaneQueue laneQueue))
            {
                //get the associated lane to the lane number
                var lane = dataAccessLogic.GetLaneWithLaneNumber(
                    queueLaneNumber
                    );

                laneQueue = new LaneQueue();
                laneQueue.SetQueueLane(lane);
                laneQueue.SetAttendant(attendant);
                laneQueue.Tolerance = tolerance;

                bool isSuccess = dataAccessLogic.AddLaneQueue(
                    laneQueue
                    );

                laneQueue = dataAccessLogic.GetLaneQueueWithLaneID(
                    lane.LaneID
                    );

                if (isSuccess &&
                    laneQueue == null)
                {
                    //unexpected error
                    throw new Exception("Failed to get lane queue of lane number: " + 
                        queueLaneNumber);
                }

                if (isSuccess)
                {
                    //reflect changes in application
                    laneQueues[queueLaneNumber] = laneQueue;
                }

                //conditions for failure:
                //  lane queue already exists for this lane number
                //  the attendant object given does not have an id set
                return isSuccess;
            }
            else
            {
                //lane number unused
                return false;
            }
        }

        /// <summary>
        /// Disallows queues in lane, and unassigns the attendant of the lane
        /// </summary>
        /// <param name="queueLaneNumber"></param>
        /// <returns></returns>
        public bool SetLaneInactive(int queueLaneNumber)
        {
            //check if the lane queue is existing
            if (laneQueues.TryGetValue(queueLaneNumber, out LaneQueue laneQueue)
                && laneQueue != null)
            {
                bool isSuccess = dataAccessLogic.DeleteLaneQueue(
                    laneQueue.LaneQueueID
                    );

                if (isSuccess)
                {
                    //reflect changes in application
                    laneQueues[queueLaneNumber] = null;
                }

                //conditions for failure:
                //  lane queue id is not set
                return isSuccess;
            }
            else
            {
                //lane number unused or lane is already inactive
                return false;
            }
        }

        public bool EditQueueLane(Lane editedQueueLane)
        {
            //check if lane number has changed, and if it has conflicts
            var origQueueLane = dataAccessLogic.GetLaneWithLaneNumber(editedQueueLane.LaneNumber);

            if (origQueueLane != null && 
                origQueueLane.LaneID == editedQueueLane.LaneID)
            {
                //lane number has not been modified
                bool isSuccess = dataAccessLogic.EditLaneWithLaneNumber(editedQueueLane);
                if (isSuccess)
                {
                    //reflect changes in application
                    InitializeLaneQueues();
                    return true;
                }
                else
                {
                    throw new Exception("Failed to modify queue lane.");
                }
            }
            else if (origQueueLane != null)
            {
                //lane number has been modified, conflicts exist, must not proceed
                return false;
            }
            else
            {
                //lane number has been modified, conflicts do not exist
                bool isSuccess = dataAccessLogic.EditLaneWithLaneID(editedQueueLane);
                if (isSuccess)
                {
                    //reflect changes in application
                    InitializeLaneQueues();
                    return true;
                }
                else
                {
                    throw new Exception("Failed to modify queue lane.");
                }
            }
        }

        public bool DeleteQueueLane(int queueLaneNumber)
        {
            //make sure that queue lane number exists
            var queueLaneToDelete = dataAccessLogic.GetLaneWithLaneNumber(queueLaneNumber);

            if (queueLaneToDelete != null)
            {
                bool isSuccess = dataAccessLogic.DeleteLaneWithLaneNumber(queueLaneNumber);

                if (isSuccess)
                {
                    //reflect changes in application
                    InitializeLaneQueues();

                    return true;
                }
                else
                {
                    throw new Exception("Failed to delete queue lane.");
                }
            }
            else
            {
                //queue lane number is unused
                return false;
            }
        }

        /// <summary>
        /// Checks whether the specified lane has a lane queue assigned to it
        /// Doesn't distinguish between unused lane numbers and inactive lanes
        /// </summary>
        /// <returns></returns>
        public bool IsLaneActive(int queueLaneNumber)
        {
            //check if the lane queue is existing
            if (laneQueues.TryGetValue(queueLaneNumber, out LaneQueue laneQueue)
                && laneQueue != null)
            {
                //lane number has a lane queue, it is active
                return true;
            }
            else
            {
                //lane number unused or lane is inactive
                return false;
            }
        }

        public List<QueueStatus> GetQueueStatuses()
        {
            return new List<QueueStatus>(queueStatusMapper.Keys);
        }
    }
}
