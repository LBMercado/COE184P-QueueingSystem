using System;
using System.Collections.Generic;
using System.Text;
using QueueingSystem.Models;

namespace QueueingSystem.BusinessLogic
{
    public interface IQueueingSystem
    {
        List<Lane> GetAllQueueLanes();

        List<Lane> GetInactiveQueueLanes();

        List<Lane> GetActiveQueueLanes();

        Lane GetQueueLane(int queueLaneNumber);

        Lane GetQueueLaneOfTicket(string queueTicketID);

        int GetLaneCount();

        int GetActiveLaneCount();

        int GetInactiveLaneCount();

        bool IsFullQueueAtLane(int queueLaneNumber);

        bool IsEmptyQueueAtLane(int queueLaneNumber);

        int GetQueueCountAtLane(int queueLaneNumber);

        int GetQueueCapacityOfLane(int queueLaneNumber);

        void SetLaneQueueCapacity(int queueLaneNumber, int queueCapacity);

        void SetLaneQueueTolerance(int queueLaneNumber, TimeSpan timeTolerance);

        bool SetQueueAttendantOfLane(int queueLaneNumber, QueueAttendant queueAttendant);

        QueueAttendant GetQueueAttendantOfLane(int queueLaneNumber);

        bool IsGuest(object account);

        bool IsUser(object account);

        object GetFrontQueuedInLane(int queueLaneNumber);

        List<object> GetListOfQueuedInLane(int queueLaneNumber);

        object GetLastQueuedInLane(int queueLaneNumber);

        bool EnqueueLane(QueueTicket newQueueTicket);

        QueueTicket DequeueLane(int queueLaneNumber);

        QueueTicket PeekLane(int queueLaneNumber);

        bool AddNewQueueLane(Lane newQueueLane);

        bool EditQueueLane(Lane editedQueueLane);

        bool DeleteQueueLane(int queueLaneNumber);

        List<QueueStatus> GetQueueStatuses();
    }
}
