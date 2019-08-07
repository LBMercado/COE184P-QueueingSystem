using System;
using System.Collections.Generic;
using System.Text;
using QueueingSystem.Models;

namespace QueueingSystem.BusinessLogic
{
    public class Login : ILogin
    {
        private string connectionString;
        private DataAccess.DataAccess dataAccessLogic;

        public Login(string connectionString)
        {
            this.connectionString = connectionString;

            dataAccessLogic = new DataAccess.DataAccess(this.connectionString);
        }

        public void SetConnectionString(string connectionString)
        {
            this.connectionString = connectionString;

            dataAccessLogic = new DataAccess.DataAccess(this.connectionString);
        }
        public bool IsCorrectLogin(string email, string password)
        {
            var hashedPassword = Hasher.HashSHA512(password).Digest;

            var accountNumber = dataAccessLogic.GetAccountNumberWithEmailPassword(email, hashedPassword);

            //a value of -1 indicates incorrect or non-existent login credentials
            return accountNumber != -1;

        }

        public Admin LoginAsAdmin(string email, string password)
        {
            var hashedPassword = Hasher.HashSHA512(password).Digest;
            var accountNumber = dataAccessLogic.GetAccountNumberWithEmailPassword(email, hashedPassword);

            return dataAccessLogic.GetAdminWithAccountNumber(accountNumber);
        }

        public Guest LoginAsGuest(int guestNumber)
        {
            return dataAccessLogic.GetGuestWithGuestNumber(guestNumber);
        }

        public QueueAttendant LoginAsQueueAttendant(string email, string password)
        {
            var hashedPassword = Hasher.HashSHA512(password).Digest;
            var accountNumber = dataAccessLogic.GetAccountNumberWithEmailPassword(email, hashedPassword);

            return dataAccessLogic.GetQueueAttendantWithAccountNumber(accountNumber);
        }

        public User LoginAsUser(string email, string password)
        {
            var hashedPassword = Hasher.HashSHA512(password).Digest;
            var accountNumber = dataAccessLogic.GetAccountNumberWithEmailPassword(email, hashedPassword);

            return dataAccessLogic.GetUserWithAccountNumber(accountNumber);
        }

        /// <summary>
        /// Disregards admin id and account number
        /// </summary>
        /// <param name="newAdmin"></param>
        /// <returns></returns>
        public bool RegisterAsAdmin(Admin newAdmin)
        {
            var hashedPassword = Hasher.HashSHA512(newAdmin.GetPassword()).Digest;
            newAdmin.SetPassword(hashedPassword);

            return dataAccessLogic.AddAdmin(newAdmin);
        }

        public int GenerateGuestNumber()
        {
            bool isSuccess = dataAccessLogic.AddGuest();
            if (isSuccess)
            {
                return dataAccessLogic.GetGuestNumberSeed();
            }
            else
            {
                throw new Exception("Failed to generate guest number.");
            }
        }

        /// <summary>
        /// Disregards attendant id and account number
        /// </summary>
        /// <param name="newQueueAttendant"></param>
        /// <returns></returns>
        public bool RegisterAsQueueAttendant(QueueAttendant newQueueAttendant)
        {
            var hashedPassword = Hasher.HashSHA512(newQueueAttendant.GetPassword()).Digest;
            newQueueAttendant.SetPassword(hashedPassword);

            return dataAccessLogic.AddQueueAttendant(newQueueAttendant);
        }

        /// <summary>
        /// Disregards user id and account number
        /// </summary>
        /// <param name="newUser"></param>
        /// <returns></returns>
        public bool RegisterAsUser(User newUser)
        {
            var hashedPassword = Hasher.HashSHA512(newUser.GetPassword()).Digest;
            newUser.SetPassword(hashedPassword);

            return dataAccessLogic.AddUser(newUser);
        }

        /// <summary>
        /// Resets the password of the account
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool ResetPassword(object account, string newPassword)
        {
            var hashedPassword = Hasher.HashSHA512(newPassword).Digest;
            if (account is User user)
            {
                user.SetPassword(hashedPassword);

                return dataAccessLogic.EditAccountWithAccountNumber(
                    user
                    );
            }
            else if (account is Admin admin)
            {
                admin.SetPassword(hashedPassword);

                return dataAccessLogic.EditAccountWithAccountNumber(
                    admin
                    );
            }
            else if (account is QueueAttendant attendant)
            {
                attendant.SetPassword(hashedPassword);

                return dataAccessLogic.EditAccountWithAccountNumber(
                    attendant
                    );
            }
            else if (account is Guest guest)
            {
                //illegal
                throw new ArgumentException("Guest accounts do not have passwords.");
            }
            else
            {
                //unexpected account object, or wrong type
                throw new ArgumentException("Account parameter is not an account.", "account");
            }
        }
    }
}
