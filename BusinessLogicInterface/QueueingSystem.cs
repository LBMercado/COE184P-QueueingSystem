using System;
using System.Collections.Generic;
using System.Text;
using QueueingSystem.Models;
using QueueingSystem.DataAccess;

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

        private void InitializeLaneQueues()
        {
            laneQueues = new Dictionary<int, LaneQueue>();
            foreach (var lane in dataAccessLogic.GetLanes())
            {
                var laneQueue = dataAccessLogic.GetLaneQueueWithLaneID(lane.LaneID);

                //get waiting status queue tickets only
                laneQueue.SetQueueList(
                    dataAccessLogic.GetQueueTicketsWithLaneIDAndStatus(
                        lane.LaneID,
                        QueueStatus.WAITING,
                        queueStatusMapper
                        )
                    );

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

        public bool AddNewQueueLane(string laneName)
        {
            throw new NotImplementedException();
        }

        public bool DeleteQueueLane(int queueLaneNumber)
        {
            throw new NotImplementedException();
        }

        public QueueTicket DequeueLane(int queueLaneNumber)
        {
            throw new NotImplementedException();
        }

        public bool EditQueueLane(Lane editedQueueLane)
        {
            throw new NotImplementedException();
        }

        public bool EnqueueLane(int queueLaneNumber, QueueTicket newQueueTicket)
        {
            throw new NotImplementedException();
        }

        public List<Lane> GetAllQueueLanes()
        {
            throw new NotImplementedException();
        }

        public object GetCurrentlyServingInLane(int queueLaneNumber)
        {
            throw new NotImplementedException();
        }

        public int GetLaneCount()
        {
            throw new NotImplementedException();
        }

        public int GetLatestGuessNumber()
        {
            throw new NotImplementedException();
        }

        public object[] GetListOfQueuedInLane(int queueLaneNumber)
        {
            throw new NotImplementedException();
        }

        public object GetNextToBeServedInLane(int queueLaneNumber)
        {
            throw new NotImplementedException();
        }

        public QueueAttendant GetQueueAttendantOfLane(int queueLaneNumber)
        {
            throw new NotImplementedException();
        }

        public Lane GetQueueLane(int queueLaneNumber)
        {
            throw new NotImplementedException();
        }

        public Lane GetQueueLane(string queueLaneName)
        {
            throw new NotImplementedException();
        }

        public Lane GetQueueLaneOfTicket(int queueTicketNumber)
        {
            throw new NotImplementedException();
        }

        public bool IsEmptyQueueAtLane(string queueLaneNumber)
        {
            throw new NotImplementedException();
        }

        public bool IsFullQueueAtLane(string queueLaneNumber)
        {
            throw new NotImplementedException();
        }

        public bool IsGuest(object user)
        {
            throw new NotImplementedException();
        }

        public void SetLaneQueueCapacity(string queueLaneNumber, int queueCapacity)
        {
            throw new NotImplementedException();
        }

        public void SetLaneQueueTolerance(TimeSpan timeTolerance)
        {
            throw new NotImplementedException();
        }

        public bool SetQueueAttendantOfLane(int queueLaneNumber, QueueAttendant queueAttendant)
        {
            throw new NotImplementedException();
        }
    }
}
