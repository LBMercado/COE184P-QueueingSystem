using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace QueueingSystem.Models
{
    [DataContract]
    public abstract class Account
    {
        public Account() { }

        public Account(
            long accountNumber,
            string email,
            string password,
            string contactNumber,
            string firstName,
            string middleName,
            string lastName
            )
        {
            AccountNumber = accountNumber;
            Email = email;
            Password = password;
            ContactNumber = contactNumber;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
        }

        [DataMember]
        public long AccountNumber { get; set; }

        [DataMember]
        protected string Email { get; set; }

        [DataMember]
        protected string Password { get; set; }

        [DataMember]
        protected string ContactNumber { get; set; }

        [DataMember]
        protected string FirstName { get; set; }

        [DataMember]
        protected string MiddleName { get; set; }

        [DataMember]
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
