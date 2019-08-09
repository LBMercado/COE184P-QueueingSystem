using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueueingSystem.Models;
using QueueingSystem.DataAccess;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DataAccessInterface_Tests
{
    [TestClass]
    public class UnitTest_DataAccess_EditTests
    {
        private string connectionString;
        private DataAccess dal;
        private Dictionary<QueueStatus, int> queueStatusMapper;

        [TestInitialize]
        public void Initialize()
        {
            connectionString = ConfigurationManager.ConnectionStrings["QueueingSystemDB"].ConnectionString;
            dal = new DataAccess(connectionString);
            queueStatusMapper = dal.GetQueueStatuses();
        }

        [TestCleanup]
        public void Clean_up()
        {
            dal.ResetDatabase();
        }

        [TestMethod]
        public void EditUser_ExpectedInputs_AllSupplied()
        {
            var testUser1 = new User();

            testUser1.SetFullName("A", "AA", "AAA");
            testUser1.SetEmail("A@email.com");
            testUser1.SetContactNumber("12345678901");
            testUser1.SetPassword("AAA");

            Assert.IsTrue(
                dal.AddUser(testUser1)
                );

            var retUser1 = dal.GetUserWithEmail(
                testUser1.GetEmail()
                );
            var editedUser1 = new User(retUser1);

            Assert.IsNotNull(
                retUser1
                );

            editedUser1.SetFullName("B", "BB", "BBB");
            editedUser1.SetEmail("B@email.com");
            editedUser1.SetContactNumber("ABCDE123456");
            editedUser1.SetPassword("BBB");

            Assert.IsTrue(
                dal.EditAccountWithAccountNumber(editedUser1)
                );

            retUser1 = dal.GetUserWithEmail(
                editedUser1.GetEmail()
                );

            Assert.IsNotNull(
                retUser1
                );

            //make sure all details are edited
            Assert.AreEqual(
                editedUser1.GetEmail(),
                retUser1.GetEmail()
                );

            Assert.AreEqual(
                editedUser1.GetFullName(),
                retUser1.GetFullName()
                );

            Assert.AreEqual(
                editedUser1.GetContactNumber(),
                retUser1.GetContactNumber()
                );

            //make sure passwords are not equal anymore
            Assert.AreNotEqual(
                editedUser1.GetPassword(),
                testUser1.GetPassword()
                );
        }
    }
}
