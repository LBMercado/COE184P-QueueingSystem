using System;
using System.Collections.Generic;
using System.Text;

namespace QueueingSystem.Models
{
    public class QueueAttendant : Account
    {
        public QueueAttendant() { }
        public QueueAttendant(QueueAttendant queueAttendant)
                : base(
                    queueAttendant.AccountNumber,
                    queueAttendant.Email,
                    queueAttendant.Password,
                    queueAttendant.ContactNumber,
                    queueAttendant.FirstName,
                    queueAttendant.MiddleName,
                    queueAttendant.LastName
                      )
        {
            QueueAttendantID = queueAttendant.QueueAttendantID;

            if (queueAttendant.DesignatedLane == null)
            {
                DesignatedLane = new Lane();
            }
            else
            {
                DesignatedLane = new Lane(queueAttendant.DesignatedLane);
            }
        }
        public string QueueAttendantID { get; set; }
        public Lane DesignatedLane { get; set; }
        public override void SetFullName(string firstName, string middleName, string lastName)
        {
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;

            DesignatedLane = new Lane();
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
