﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace QueueingSystem.Models
{
    [DataContract]
    [KnownType(typeof(Account))]
    public class User : Account
    {
        public User() { }

        public User(User user)
            : base(
                user.AccountNumber,
                user.Email,
                user.Password,
                user.ContactNumber,
                user.FirstName,
                user.MiddleName,
                user.LastName
                  )
        {
            UserID = user.UserID;
        }

        [DataMember]
        public string UserID { get; set; }

        public override void SetFullName(string firstName, string middleName, string lastName)
        {
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
        }

        public override string GetFullName()
        {
            return LastName + ", " + FirstName + ", " + MiddleName;
        }

        public string GetFirstName()
        {
            return FirstName;
        }

        public string GetMiddleName()
        {
            return MiddleName;
        }

        public string GetLastName()
        {
            return LastName;
        }

        public override void SetEmail(string email)
        {
            Email = email;
        }

        public override string GetEmail()
        {
            return Email;
        }

        public override void SetPassword(string password)
        {
            Password = password;
        }

        public override string GetPassword()
        {
            return Password;
        }

        public override void SetContactNumber(string contactNumber)
        {
            ContactNumber = contactNumber;
        }

        public override string GetContactNumber()
        {
            return ContactNumber;
        }
    }
}
