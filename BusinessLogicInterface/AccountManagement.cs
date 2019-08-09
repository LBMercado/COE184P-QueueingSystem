using System;
using System.Collections.Generic;
using System.Text;
using QueueingSystem.Models;
using QueueingSystem.DataAccess;

namespace QueueingSystem.BusinessLogic
{
    public class AccountManagement : IAccountManagement
    {
        private string connectionString;
        private DataAccess.DataAccess dal;

        public AccountManagement(string connectionString)
        {
            this.connectionString = connectionString;
            dal = new DataAccess.DataAccess(this.connectionString);
        }

        public void SetConnectionString(string connectionString)
        {
            this.connectionString = connectionString;
            dal = new DataAccess.DataAccess(this.connectionString);
        }

        public Admin GetAdminAccount(long accountNumber)
        {
            return dal.GetAdminWithAccountNumber(accountNumber);
        }

        public Admin GetAdminAccount(string adminID)
        {
            return dal.GetAdminWithAdminID(adminID);
        }

        public Admin GetAdminAccount(string email, string password)
        {
            long accountNumber = dal.GetAccountNumberWithEmailPassword(email, password);

            if (accountNumber == -1)
            {
                return null;
            }

            return dal.GetAdminWithAccountNumber(accountNumber);
        }

        public Guest GetGuestAccount(long accountNumber)
        {
            return dal.GetGuestWithAccountNumber(accountNumber);
        }

        public Guest GetGuestAccount(int guestNumber)
        {
            return dal.GetGuestWithGuestNumber(guestNumber);
        }

        public List<QueueTicket> GetGuestTickets(long accountNumber)
        {
            return dal.GetQueueTicketsOf(accountNumber);
        }

        public List<QueueTicket> GetGuestTickets(int guestNumber)
        {
            Guest guest = dal.GetGuestWithGuestNumber(guestNumber);

            if (guest == null)
            {
                return new List<QueueTicket>();
            }

            return dal.GetQueueTicketsOf(guest.AccountNumber);
        }

        public QueueAttendant GetQueueAttendant(long accountNumber)
        {
            return dal.GetQueueAttendantWithAccountNumber(accountNumber);
        }

        public QueueAttendant GetQueueAttendant(string attendantID)
        {
            return dal.GetQueueAttendantWithID(attendantID);
        }

        public QueueAttendant GetQueueAttendant(string email, string password)
        {
            var accountNumber = dal.GetAccountNumberWithEmailPassword(email, password);

            if (accountNumber == -1)
            {
                return null;
            }

            return dal.GetQueueAttendantWithAccountNumber(accountNumber);
        }

        public User GetUserAccount(long accountNumber)
        {
            return dal.GetUserWithAccountNumber(accountNumber);
        }

        public User GetUserAccount(string userID)
        {
            return dal.GetUserWithUserID(userID);
        }

        public User GetUserAccount(string email, string password)
        {
            var accountNumber = dal.GetAccountNumberWithEmailPassword(email, password);

            if(accountNumber == -1)
            {
                return null;
            }

            return dal.GetUserWithAccountNumber(accountNumber);
        }

        public List<QueueTicket> GetUserTickets(long accountNumber)
        {
            return dal.GetQueueTicketsOf(accountNumber);
        }

        public List<QueueTicket> GetUserTickets(string userID)
        {
            var user = dal.GetUserWithUserID(userID);

            if(user == null)
            {
                return new List<QueueTicket>();
            }

            return dal.GetQueueTicketsOf(user.AccountNumber);
        }

        public bool UpdateAdminAccount(Admin admin)
        {
            if (admin == null ||
                admin.AdminID == string.Empty ||
                admin.AdminID == null ||
                admin.AccountNumber > 0)
            {
                return false;
            }

            return dal.EditAccountWithAccountNumber(admin.AccountNumber);
        }

        public bool UpdateQueueAttendant(QueueAttendant attendant)
        {
            if (attendant == null ||
                attendant.QueueAttendantID == string.Empty ||
                attendant.QueueAttendantID == null ||
                attendant.AccountNumber > 0)
            {
                return false;
            }

            return dal.EditAccountWithAccountNumber(attendant.AccountNumber);
        }

        public bool UpdateUserAccount(User user)
        {
            if (user == null ||
                user.UserID == string.Empty ||
                user.UserID == null ||
                user.AccountNumber > 0)
            {
                return false;
            }

            return dal.EditAccountWithAccountNumber(user.AccountNumber);
        }
    }
}
