using System;
using System.Collections.Generic;
using System.Text;
using QueueingSystem.Models;

namespace QueueingSystem.BusinessLogic
{
    interface IAccountManagement
    {
        User GetUserAccount(long accountNumber);

        User GetUserAccount(string userID);

        User GetUserAccount(string email, string password);

        Guest GetGuestAccount(long accountNumber);

        Guest GetGuestAccount(int guestNumber);

        Admin GetAdminAccount(long accountNumber);

        Admin GetAdminAccount(string adminID);

        Admin GetAdminAccount(string email, string password);

        List<QueueAttendant> GetQueueAttendants();

        QueueAttendant GetQueueAttendant(long accountNumber);

        QueueAttendant GetQueueAttendant(string attendantID);

        QueueAttendant GetQueueAttendant(string email, string password);

        bool UpdateUserAccount(User user);

        bool UpdateAdminAccount(Admin admin);

        bool UpdateQueueAttendant(QueueAttendant attendant);

        List<QueueTicket> GetUserTickets(long accountNumber);

        List<QueueTicket> GetUserTickets(string userID);

        List<QueueTicket> GetGuestTickets(long accountNumber);

        List<QueueTicket> GetGuestTickets(int guestNumber);
    }
}
