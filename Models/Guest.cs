using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace QueueingSystem.Models
{
    [DataContract]
    [KnownType(typeof(Account))]
    public class Guest : Account
    {
        [DataMember]
        public int GuestNumber { get; private set; }

        public Guest()
        {
            FirstName = "";
            MiddleName = "";
            LastName = "Guest";
        }

        public override void SetFullName(string firstName, string middleName, string lastName)
        {
            throw new NotImplementedException("Guest name cannot be overwritten.");
        }

        public override string GetFullName()
        {
            return LastName;
        }

        public override void SetContactNumber(string contactNumber)
        {
            throw new NotImplementedException("Guests cannot have a contact number. Instantiate a 'User' object instead");
        }

        public override string GetContactNumber()
        {
            throw new NotImplementedException("Guests cannot have a contact number. Instantiate a 'User' object instead");
        }

        public override void SetEmail(string email)
        {
            throw new NotImplementedException("Guests cannot have an email. Instantiate a 'User' object instead");
        }

        public override string GetEmail()
        {
            throw new NotImplementedException("Guests cannot have an email. Instantiate a 'User' object instead");
        }

        public override void SetPassword(string password)
        {
            throw new NotImplementedException("Guests cannot have a password. Instantiate a 'User' object instead");
        }

        public override string GetPassword()
        {
            throw new NotImplementedException("Guests cannot have an password. Instantiate a 'User' object instead");
        }

        public void SetGuestNumber(int guestNumber)
        {
            GuestNumber = guestNumber;
            FirstName = "";
            MiddleName = "";
            LastName = "Guest" + GuestNumber;
        }
    }
}
