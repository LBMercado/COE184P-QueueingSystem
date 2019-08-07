using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueueingSystem.Models;
using QueueingSystem.BusinessLogic;
using QueueingSystem.DataAccess;
using System.Configuration;

namespace BusinessLogicInterface_Tests
{
    [TestClass]
    public class LoginTests
    {
        private string connectionString;
        private Login login;

        [TestInitialize]
        public void Init()
        {
            connectionString = ConfigurationManager.ConnectionStrings["QueueingSystemDB"].ConnectionString;
            login = new Login(connectionString);
        }

        [TestCleanup]
        public void Cleanup()
        {
            var dal = new DataAccess(connectionString);
            dal.ResetDatabase();
        }

        [TestMethod]
        public void RegisterUser_Expected_Test()
        {
            var testUser1 = new User();

            testUser1.SetFullName("A", "AA", "AAA");
            testUser1.SetEmail("A@email.com");
            testUser1.SetContactNumber("12345678901");
            testUser1.SetPassword("AAA");

            Assert.IsTrue(
                login.RegisterAsUser(testUser1)
                ); 
        }

        [TestMethod]
        public void LoginAsUser_Expected_Test()
        {
            var testUser1 = new User();

            testUser1.SetFullName("A", "AA", "AAA");
            testUser1.SetEmail("A@email.com");
            testUser1.SetContactNumber("12345678901");
            testUser1.SetPassword("AAA");

            Assert.IsTrue(
                login.RegisterAsUser(testUser1)
                );

            Assert.IsNotNull(
                login.LoginAsUser(testUser1.GetEmail(),
                    testUser1.GetPassword())
                );
        }
    }
}
