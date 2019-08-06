using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueueingSystem.Models;
using QueueingSystem.DataAccess;
using System.Collections.Generic;

namespace DataAccessInterface_Tests
{
    [TestClass]
    public class UnitTest_DataAccess_AddTests
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
        public void AddUserTest_ExpectedInputs_AllSupplied()
        {
            var testUser1 = new User();

            testUser1.SetFullName("A", "AA", "AAA");
            testUser1.SetEmail("A@email.com");
            testUser1.SetContactNumber("12345678901");
            testUser1.SetPassword("AAA");

            Assert.IsTrue(
                dal.AddUser(testUser1)
                );
        }

        [TestMethod]
        public void AddUserTest_ExpectedInputs_OptionalInputsUnsupplied()
        {
            var testUser1 = new User();

            testUser1.SetFullName("A","", "AAA");
            testUser1.SetEmail("A@email.com");
            testUser1.SetContactNumber("");
            testUser1.SetPassword("AAA");

            Assert.IsTrue(
                dal.AddUser(testUser1)
                );
        }

        [TestMethod]
        public void AddUserTest_ExpectedInputs_RequiredInputsUnsupplied()
        {
            var testUser1 = new User();

            testUser1.SetFullName("A", "AA", null);
            testUser1.SetEmail(null);
            testUser1.SetContactNumber("12345678901");
            testUser1.SetPassword("AAA");

            Assert.IsFalse(
                dal.AddUser(testUser1)
                );
        }

        [TestMethod]
        public void AddGuestTest()
        {
            Assert.IsTrue(
                dal.AddGuest()
                );
        }

        [TestMethod]
        public void AddGuessTest_ThenGetGuessWithGuessNumber()
        {
            Assert.IsTrue(
                dal.AddGuest()
                );
            Guest guest = dal.GetGuestWithAccountNumber(
                dal.GetGuestNumberSeed());
            Assert.IsNotNull(guest);
        }

        [TestMethod]
        public void AddAdminTest_ExpectedInputs_AllSupplied()
        {
            var testUser1 = new Admin();

            testUser1.SetFullName("A", "AA", "AAA");
            testUser1.SetEmail("A@email.com");
            testUser1.SetContactNumber("12345678901");
            testUser1.SetPassword("AAA");

            Assert.IsTrue(
                dal.AddAdmin(testUser1)
                );
        }

        [TestMethod]
        public void AddAdminTest_ExpectedInputs_OptionalInputsUnsupplied()
        {
            var testUser1 = new Admin();

            testUser1.SetFullName("A", "", "AAA");
            testUser1.SetEmail("A@email.com");
            testUser1.SetContactNumber("");
            testUser1.SetPassword("AAA");

            Assert.IsTrue(
                dal.AddAdmin(testUser1)
                );
        }

        [TestMethod]
        public void AddAdminTest_ExpectedInputs_RequiredInputsUnsupplied()
        {
            var testUser1 = new Admin();

            testUser1.SetFullName("A", "AA", null);
            testUser1.SetEmail(null);
            testUser1.SetContactNumber("12345678901");
            testUser1.SetPassword("AAA");

            Assert.IsFalse(
                dal.AddAdmin(testUser1)
                );
        }

        [TestMethod]
        public void AddQueueAttendantTest_ExpectedInputs_AllSupplied()
        {
            var testLane1 = new Lane();
            testLane1.LaneNumber = 1;
            testLane1.LaneName = "Test Lane 1";
            testLane1.Capacity = 10;

            Assert.IsTrue(
                dal.AddLane(testLane1)
                );

            testLane1 = dal.GetLaneWithLaneNumber(1);

            Assert.IsNotNull(
                testLane1
                );

            var testUser1 = new QueueAttendant();

            testUser1.SetFullName("A", "AA", "AAA");
            testUser1.SetEmail("A@email.com");
            testUser1.SetContactNumber("12345678901");
            testUser1.SetPassword("AAA");
            testUser1.DesignatedLane.LaneID = testLane1.LaneID;

            Assert.IsTrue(
                dal.AddQueueAttendant(testUser1)
                );
        }

        [TestMethod]
        public void AddQueueAttendantTest_ExpectedInputs_OptionalInputsUnsupplied()
        {
            var testLane1 = new Lane();
            testLane1.LaneNumber = 1;
            testLane1.LaneName = "Test Lane 1";
            testLane1.Capacity = 10;

            Assert.IsTrue(
                dal.AddLane(testLane1)
                );

            testLane1 = dal.GetLaneWithLaneNumber(1);

            Assert.IsNotNull(
                testLane1
                );

            var testUser1 = new QueueAttendant();

            testUser1.SetFullName("A", "", "AAA");
            testUser1.SetEmail("A@email.com");
            testUser1.SetContactNumber("");
            testUser1.SetPassword("AAA");
            //lane id is unsupplied by default

            Assert.IsTrue(
                dal.AddQueueAttendant(testUser1)
                );
        }

        [TestMethod]
        public void AddQueueAttendantTest_ExpectedInputs_RequiredInputsUnsupplied()
        {
            var testUser1 = new QueueAttendant();

            testUser1.SetFullName("A", "AA", null);
            testUser1.SetEmail(null);
            testUser1.SetContactNumber("12345678901");
            testUser1.SetPassword("AAA");
            //lane id is unsupplied by default

            Assert.IsFalse(
                dal.AddQueueAttendant(testUser1)
                );
        }

        [TestMethod]
        public void AddQueueAttendantTest_ExpectedInputs_RequiredInputsUnsupplied_InvalidLaneID()
        {
            var testUser1 = new QueueAttendant();

            testUser1.SetFullName("A", "AA", null);
            testUser1.SetEmail(null);
            testUser1.SetContactNumber("12345678901");
            testUser1.SetPassword("AAA");
            testUser1.DesignatedLane.LaneID = -999; //laneid not available in database

            Assert.IsFalse(
                dal.AddQueueAttendant(testUser1)
                );
        }

        [TestMethod]
        public void AddQueueTicketTest()
        {
            var testLane1 = new Lane();
            testLane1.LaneNumber = 1;
            testLane1.LaneName = "Test Lane 1";
            testLane1.Capacity = 10;

            Assert.IsTrue(
                dal.AddLane(testLane1)
                );

            testLane1 = dal.GetLaneWithLaneNumber(1);

            Assert.IsNotNull(
                testLane1
                );

            var testUser1 = new User();

            testUser1.SetFullName("B", "BB", "BBB");
            testUser1.SetEmail("B@email.com");
            testUser1.SetContactNumber("12345678901");
            testUser1.SetPassword("BBB");

            Assert.IsTrue(
                dal.AddUser(testUser1)
                );

            testUser1 = dal.GetUserWithEmail(testUser1.GetEmail());

            Assert.IsNotNull(
                testUser1
                );

            var testAttendant1 = new QueueAttendant();

            testAttendant1.SetFullName("A", "AA", "AAA");
            testAttendant1.SetEmail("A@email.com");
            testAttendant1.SetContactNumber("12345678901");
            testAttendant1.SetPassword("AAA");
            testAttendant1.DesignatedLane.LaneID = testLane1.LaneID;

            Assert.IsTrue(
                dal.AddQueueAttendant(testAttendant1)
                );

            testAttendant1 = dal.GetQueueAttendantWithEmail("A@email.com");

            Assert.IsNotNull(
                testAttendant1
                );

            var testUser1Ticket = new QueueTicket();
            testUser1Ticket.owner = testUser1;
            testUser1Ticket.QueueLane = testLane1;
            testUser1Ticket.Status = QueueStatus.WAITING;

            Assert.IsTrue(
                dal.AddQueueTicket(
                    testUser1Ticket,
                    queueStatusMapper
                    )
                );
        }

        [TestMethod]
        public void AddLaneQueueTest()
        {
            var testLane1 = new Lane();
            testLane1.LaneNumber = 1;
            testLane1.LaneName = "Test Lane 1";
            testLane1.Capacity = 10;

            Assert.IsTrue(
                dal.AddLane(testLane1)
                );

            testLane1 = dal.GetLaneWithLaneNumber(1);

            Assert.IsNotNull(
                testLane1
                );

            var testUser1 = new User();

            testUser1.SetFullName("B", "BB", "BBB");
            testUser1.SetEmail("B@email.com");
            testUser1.SetContactNumber("12345678901");
            testUser1.SetPassword("BBB");

            Assert.IsTrue(
                dal.AddUser(testUser1)
                );

            testUser1 = dal.GetUserWithEmail(testUser1.GetEmail());

            Assert.IsNotNull(
                testUser1
                );

            var testAttendant1 = new QueueAttendant();

            testAttendant1.SetFullName("A", "AA", "AAA");
            testAttendant1.SetEmail("A@email.com");
            testAttendant1.SetContactNumber("12345678901");
            testAttendant1.SetPassword("AAA");
            testAttendant1.DesignatedLane.LaneID = testLane1.LaneID;

            Assert.IsTrue(
                dal.AddQueueAttendant(testAttendant1)
                );

            testAttendant1 = dal.GetQueueAttendantWithEmail("A@email.com");

            Assert.IsNotNull(
                testAttendant1
                );

            var testUser1Ticket = new QueueTicket();
            testUser1Ticket.owner = testUser1;
            testUser1Ticket.QueueLane = testLane1;
            testUser1Ticket.Status = QueueStatus.WAITING;

            Assert.IsTrue(
                dal.AddQueueTicket(
                    testUser1Ticket,
                    queueStatusMapper
                    )
                );

            //Lane Queue Start

            var testLaneQueue1 = new LaneQueue();

            testLaneQueue1.SetQueueLane(testLane1);
            testLaneQueue1.SetAttendant(testAttendant1);

            Assert.IsTrue(
                dal.AddLaneQueue(testLaneQueue1)
                );

            testLaneQueue1 = dal.GetLaneQueueWithLaneID(testLane1.LaneID);

            Assert.IsNotNull(
                testLaneQueue1
                );

            testLaneQueue1.SetQueueList(dal.GetQueueTicketsWithLaneIDAndStatus(
                testLane1.LaneID, QueueStatus.WAITING, queueStatusMapper
                ));

            Assert.IsFalse(
                testLaneQueue1.QueueList.Count == 0
                );

            //Lane Queue End
        }

        [TestMethod]
        public void AddLaneQueueTest_QueueMechanismTest1()
        {
            var testLane1 = new Lane();
            testLane1.LaneNumber = 1;
            testLane1.LaneName = "Test Lane 1";
            testLane1.Capacity = 10;

            Assert.IsTrue(
                dal.AddLane(testLane1)
                );

            testLane1 = dal.GetLaneWithLaneNumber(1);

            Assert.IsNotNull(
                testLane1
                );

            var testUserList = new List<User>();

            char letter = 'B';
            for(int i = 0; i < 10; i++)
            {
                var testUser1 = new User();
                letter++;

                testUser1.SetFullName(
                    new string(letter, i),
                    new string(letter, i + 1),
                    new string(letter, i + 2));
                testUser1.SetEmail(letter + "@email.com");
                testUser1.SetContactNumber("12345678901");
                testUser1.SetPassword("password");

                testUserList.Add(testUser1);
            }

            for(int i = 0; i < testUserList.Count; i++)
            {
                Assert.IsTrue(
                dal.AddUser(testUserList[i])
                );

                testUserList[i] = dal.GetUserWithEmail(testUserList[i].GetEmail());

                Assert.IsNotNull(
                    testUserList[i]
                    );
            }

            var testAttendant1 = new QueueAttendant();

            testAttendant1.SetFullName("A", "AA", "AAA");
            testAttendant1.SetEmail("A@email.com");
            testAttendant1.SetContactNumber("12345678901");
            testAttendant1.SetPassword("AAA");
            testAttendant1.DesignatedLane.LaneID = testLane1.LaneID;

            Assert.IsTrue(
                dal.AddQueueAttendant(testAttendant1)
                );

            testAttendant1 = dal.GetQueueAttendantWithEmail("A@email.com");

            Assert.IsNotNull(
                testAttendant1
                );

            var testUserTicketList = new List<QueueTicket>();
            for(int i = 0; i < testUserList.Count; i++)
            {
                var testUser1Ticket = new QueueTicket();
                testUser1Ticket.owner = testUserList[i];
                testUser1Ticket.QueueLane = testLane1;
                testUser1Ticket.Status = QueueStatus.WAITING;
                if (i == 9)
                    testUser1Ticket.PriorityNumber = 1;

                Assert.IsTrue(
                    dal.AddQueueTicket(
                        testUser1Ticket,
                        queueStatusMapper
                        )
                    );
            }

            //Lane Queue Start

            var testLaneQueue1 = new LaneQueue();

            testLaneQueue1.SetQueueLane(testLane1);
            testLaneQueue1.SetAttendant(testAttendant1);

            Assert.IsTrue(
                dal.AddLaneQueue(testLaneQueue1)
                );

            testLaneQueue1 = dal.GetLaneQueueWithLaneID(testLane1.LaneID);

            Assert.IsNotNull(
                testLaneQueue1
                );

            testLaneQueue1.SetQueueList(dal.GetQueueTicketsWithLaneIDAndStatus(
                testLane1.LaneID, QueueStatus.WAITING, queueStatusMapper
                ));

            Assert.IsFalse(
                testLaneQueue1.QueueList.Count == 0
                );

            //verify that the priority mechanic is working
            Assert.AreEqual(
                testUserList[9].GetLastName(),
                (testLaneQueue1.DequeueTicket().owner as User).GetLastName()
                );

            //Lane Queue End
        }

        [TestMethod]
        public void AddLaneQueueTest_QueueMechanismTest2()
        {
            var testLane1 = new Lane();
            testLane1.LaneNumber = 1;
            testLane1.LaneName = "Test Lane 1";
            testLane1.Capacity = 10;

            Assert.IsTrue(
                dal.AddLane(testLane1)
                );

            testLane1 = dal.GetLaneWithLaneNumber(1);

            Assert.IsNotNull(
                testLane1
                );

            var testUserList = new List<User>();

            char letter = 'B';
            for (int i = 0; i < 10; i++)
            {
                var testUser1 = new User();
                letter++;

                testUser1.SetFullName(
                    new string(letter, i),
                    new string(letter, i + 1),
                    new string(letter, i + 2));
                testUser1.SetEmail(letter + "@email.com");
                testUser1.SetContactNumber("12345678901");
                testUser1.SetPassword("password");

                testUserList.Add(testUser1);
            }

            for (int i = 0; i < testUserList.Count; i++)
            {
                Assert.IsTrue(
                dal.AddUser(testUserList[i])
                );

                testUserList[i] = dal.GetUserWithEmail(testUserList[i].GetEmail());

                Assert.IsNotNull(
                    testUserList[i]
                    );
            }

            var testAttendant1 = new QueueAttendant();

            testAttendant1.SetFullName("A", "AA", "AAA");
            testAttendant1.SetEmail("A@email.com");
            testAttendant1.SetContactNumber("12345678901");
            testAttendant1.SetPassword("AAA");
            testAttendant1.DesignatedLane.LaneID = testLane1.LaneID;

            Assert.IsTrue(
                dal.AddQueueAttendant(testAttendant1)
                );

            testAttendant1 = dal.GetQueueAttendantWithEmail("A@email.com");

            Assert.IsNotNull(
                testAttendant1
                );

            var testUserTicketList = new List<QueueTicket>();
            for (int i = 0; i < testUserList.Count; i++)
            {
                var testUser1Ticket = new QueueTicket();
                testUser1Ticket.owner = testUserList[i];
                testUser1Ticket.QueueLane = testLane1;
                testUser1Ticket.Status = QueueStatus.WAITING;
                //create a test ticket with high priority within tolerance
                //this should be at the front of the list when queued
                if (i == 7)
                    testUser1Ticket.PriorityNumber = 2;

                //create a test old ticket with high priority outside tolerance,
                //this should be at the front of the list when queued
                if(i == 9)
                {
                    testUser1Ticket.PriorityNumber = 10;
                    testUser1Ticket.QueueDateTime = DateTime.Now.Subtract(TimeSpan.FromDays(2));
                }

                Assert.IsTrue(
                    dal.AddQueueTicket(
                        testUser1Ticket,
                        queueStatusMapper
                        )
                    );
            }

            //Lane Queue Start

            var testLaneQueue1 = new LaneQueue();

            testLaneQueue1.SetQueueLane(testLane1);
            testLaneQueue1.SetAttendant(testAttendant1);
            testLaneQueue1.Tolerance = TimeSpan.FromHours(3);

            Assert.IsTrue(
                dal.AddLaneQueue(testLaneQueue1)
                );

            testLaneQueue1 = dal.GetLaneQueueWithLaneID(testLane1.LaneID);

            Assert.IsNotNull(
                testLaneQueue1
                );

            testLaneQueue1.SetQueueList(dal.GetQueueTicketsWithLaneIDAndStatus(
                testLane1.LaneID, QueueStatus.WAITING, queueStatusMapper
                ));

            Assert.IsFalse(
                testLaneQueue1.QueueList.Count == 0
                );

            //verify that the priority mechanic is working
            //tolerances working (old ticket outside of tolerance automatically has highest priority)
            Assert.AreEqual(
                testUserList[9].GetLastName(),
                (testLaneQueue1.DequeueTicket().owner as User).GetLastName()
                );

            //priorities working, higher priorities go earlier in queues
            Assert.AreEqual(
                testUserList[7].GetLastName(),
                (testLaneQueue1.DequeueTicket().owner as User).GetLastName()
                );

            //Lane Queue End
        }

        [TestMethod]
        public void AddLaneQueueTest_QueueMechanismTest3()
        {
            var testLane1 = new Lane();
            testLane1.LaneNumber = 1;
            testLane1.LaneName = "Test Lane 1";
            testLane1.Capacity = 15;

            Assert.IsTrue(
                dal.AddLane(testLane1)
                );

            testLane1 = dal.GetLaneWithLaneNumber(1);

            Assert.IsNotNull(
                testLane1
                );

            var testUserList = new List<User>();

            char letter = 'B';
            for (int i = 0; i < 10; i++)
            {
                var testUser1 = new User();
                letter++;

                testUser1.SetFullName(
                    new string(letter, i),
                    new string(letter, i + 1),
                    new string(letter, i + 2));
                testUser1.SetEmail(letter + "@email.com");
                testUser1.SetContactNumber("12345678901");
                testUser1.SetPassword("password");

                testUserList.Add(testUser1);
            }

            for (int i = 0; i < testUserList.Count; i++)
            {
                Assert.IsTrue(
                dal.AddUser(testUserList[i])
                );

                testUserList[i] = dal.GetUserWithEmail(testUserList[i].GetEmail());

                Assert.IsNotNull(
                    testUserList[i]
                    );
            }

            var testAttendant1 = new QueueAttendant();

            testAttendant1.SetFullName("A", "AA", "AAA");
            testAttendant1.SetEmail("A@email.com");
            testAttendant1.SetContactNumber("12345678901");
            testAttendant1.SetPassword("AAA");
            testAttendant1.DesignatedLane.LaneID = testLane1.LaneID;

            Assert.IsTrue(
                dal.AddQueueAttendant(testAttendant1)
                );

            testAttendant1 = dal.GetQueueAttendantWithEmail("A@email.com");

            Assert.IsNotNull(
                testAttendant1
                );

            var testUserTicketList = new List<QueueTicket>();
            for (int i = 0; i < testUserList.Count; i++)
            {
                var testUser1Ticket = new QueueTicket();
                testUser1Ticket.owner = testUserList[i];
                testUser1Ticket.QueueLane = testLane1;
                testUser1Ticket.Status = QueueStatus.WAITING;
                //create a test ticket with high priority within tolerance
                //this should be at the front of the list when queued
                if (i == 7)
                    testUser1Ticket.PriorityNumber = 2;

                //create a test old ticket with high priority outside tolerance,
                //this should be at the front of the list when queued
                if (i == 9)
                {
                    testUser1Ticket.PriorityNumber = 10;
                    testUser1Ticket.QueueDateTime = DateTime.Now.Subtract(TimeSpan.FromDays(2));
                }

                Assert.IsTrue(
                    dal.AddQueueTicket(
                        testUser1Ticket,
                        queueStatusMapper
                        )
                    );
            }

            //Lane Queue Start

            var testLaneQueue1 = new LaneQueue();

            testLaneQueue1.SetQueueLane(testLane1);
            testLaneQueue1.SetAttendant(testAttendant1);
            testLaneQueue1.Tolerance = TimeSpan.FromHours(3);

            Assert.IsTrue(
                dal.AddLaneQueue(testLaneQueue1)
                );

            testLaneQueue1 = dal.GetLaneQueueWithLaneID(testLane1.LaneID);

            Assert.IsNotNull(
                testLaneQueue1
                );

            testLaneQueue1.SetQueueList(dal.GetQueueTicketsWithLaneIDAndStatus(
                testLane1.LaneID, QueueStatus.WAITING, queueStatusMapper
                ));

            Assert.IsFalse(
                testLaneQueue1.QueueList.Count == 0
                );

            //verify that the priority mechanic is working
            // tolerances working (old ticket outside of tolerance automatically has highest priority)
            Assert.AreEqual(
                testUserList[9].GetLastName(),
                (testLaneQueue1.DequeueTicket().owner as User).GetLastName()
                );

            //priorities working, higher priorities go earlier in queues
            Assert.AreEqual(
                testUserList[7].GetLastName(),
                (testLaneQueue1.DequeueTicket().owner as User).GetLastName()
                );

            //create two new priority tickets within tolerance with the same priority
            var testUser1Ticket1 = new QueueTicket();
            testUser1Ticket1.owner = testUserList[3];
            testUser1Ticket1.QueueLane = testLane1;
            testUser1Ticket1.Status = QueueStatus.WAITING;
            testUser1Ticket1.PriorityNumber = 1;

            var testUser1Ticket2 = new QueueTicket();
            testUser1Ticket2.owner = testUserList[2];
            testUser1Ticket2.QueueLane = testLane1;
            testUser1Ticket2.Status = QueueStatus.WAITING;
            testUser1Ticket2.PriorityNumber = 1;

            testLaneQueue1.EnqueueTicket(testUser1Ticket1);
            testLaneQueue1.EnqueueTicket(testUser1Ticket2);

            Assert.AreEqual(
                testUserList[3].GetLastName(),
                (testLaneQueue1.DequeueTicket().owner as User).GetLastName()
                );

            Assert.AreEqual(
                testUserList[2].GetLastName(),
                (testLaneQueue1.DequeueTicket().owner as User).GetLastName()
                );

            //Lane Queue End
        }
    }
}
