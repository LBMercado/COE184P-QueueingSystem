using System;
using System.Collections.Generic;
using System.Text;
using QueueingSystem.Models;

namespace QueueingSystem.BusinessLogic
{
    public interface IQueueingSystem
    {
        List<Lane> GetAllQueueLanes();

        Lane GetQueueLane(int queueLaneNumber);

        Lane GetQueueLane(string queueLaneName);

        Lane GetQueueLaneOfTicket(int queueTicketNumber);

        int GetLaneCount();

        bool IsFullQueueAtLane(string queueLaneNumber);

        bool IsEmptyQueueAtLane(string queueLaneNumber);

        void SetLaneQueueCapacity(string queueLaneNumber, int queueCapacity);

        void SetLaneQueueTolerance(TimeSpan timeTolerance);

        bool SetQueueAttendantOfLane(int queueLaneNumber, QueueAttendant queueAttendant);

        QueueAttendant GetQueueAttendantOfLane(int queueLaneNumber);

        bool IsGuest(object user);

        object GetCurrentlyServingInLane(int queueLaneNumber);

        object GetNextToBeServedInLane(int queueLaneNumber);

        object[] GetListOfQueuedInLane(int queueLaneNumber);

        bool EnqueueLane(int queueLaneNumber, QueueTicket newQueueTicket);

        QueueTicket DequeueLane(int queueLaneNumber);

        bool AddNewQueueLane(string laneName);

        bool EditQueueLane(Lane editedQueueLane);

        bool DeleteQueueLane(int queueLaneNumber);

        int GetLatestGuessNumber();
    }
}
