using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueueingSystem.Models;
using QueueingSystem.DataAccess;
using System.Collections.Generic;

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

            testUser1 = dal.GetUserWithEmail(
                testUser1.GetEmail()
                );

            Assert.IsNotNull(
                testUser1
                );

            testUser1.SetFullName("B", "BB", "BBB");
            testUser1.SetEmail("B@email.com");
            testUser1.SetContactNumber("ABCDE123456");
            testUser1.SetPassword("BBB");

            string email = "B@email.com";

            Assert.IsTrue(
                dal.EditAccountWithAccountNumber(testUser1)
                );

            testUser1 = dal.GetUserWithEmail(
                testUser1.GetEmail()
                );

            Assert.IsNotNull(
                testUser1
                );

            Assert.AreEqual(
                email,
                testUser1.GetEmail()
                );
        }
    }
}
