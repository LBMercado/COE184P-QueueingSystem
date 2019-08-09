using System;
using System.Collections.Generic;
using System.Text;
using QueueingSystem.Models;

namespace QueueingSystem.BusinessLogic
{
    interface ILogin
    {
        bool IsCorrectLogin(string email, string password);

        Guest LoginAsGuest(int guestNumber);

        User LoginAsUser(string email, string password);

        Admin LoginAsAdmin(string email, string password);

        QueueAttendant LoginAsQueueAttendant(string email, string password);

        int GenerateGuestNumber();

        bool RegisterAsUser(User newUser);

        bool RegisterAsAdmin(Admin newAdmin);

        bool RegisterAsQueueAttendant(QueueAttendant newQueueAttendant);

        bool ResetPassword(object account, string newPassword);

        bool IsUserAccount(long accountNumber);

        bool IsGuestAccount(long accountNumber);

        bool IsAdminAccount(long accountNumber);

        bool IsQueueAttendantAccount(long accountNumber);
    }
}