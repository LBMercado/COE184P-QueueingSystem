using System;
using System.Collections.Generic;
using System.Text;

namespace QueueingSystem.Models
{
    public abstract class Account
    {
        public long AccountNumber { get; set; }

        protected string Email { get; set; }

        protected string Password { get; set; }

        protected string ContactNumber { get; set; }

        protected string FirstName { get; set; }

        protected string MiddleName { get; set; }

        protected string LastName { get; set; }

        public abstract void SetFullName(string firstName, string middleName, string lastName);

        public abstract string GetFullName();

        public abstract void SetEmail(string email);

        public abstract string GetEmail();

        public abstract void SetPassword(string password);

        public abstract string GetPassword();

        public abstract void SetContactNumber(string contactNumber);

        public abstract string GetContactNumber();
    }
}
