using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueueingSystem.Models;
using QS_BusinessLogic = QueueingSystem.BusinessLogic;
using QueueingSystem.DataAccess;
using System.Configuration;
using System.Collections.Generic;

namespace BusinessLogicInterface_Tests
{
    [TestClass]
    public class QueueingSystemTests
    {
        private string connectionString;
        private QS_BusinessLogic.QueueingSystem queueSystem;
        private QS_BusinessLogic.Login loginSystem;

        [TestInitialize]
        public void Init()
        {
            connectionString = ConfigurationManager.ConnectionStrings["QueueingSystemDB"].ConnectionString;
            queueSystem = new QS_BusinessLogic.QueueingSystem(connectionString);
            loginSystem = new QS_BusinessLogic.Login(connectionString);
        }

        [TestCleanup]
        public void Cleanup()
        {
            var dal = new DataAccess(connectionString);
            dal.ResetDatabase();
        }

        [TestMethod]
        public void AddQueueLane_Test()
        {
            Lane newLane = new Lane();
            newLane.LaneName = "Customer Service 1";
            newLane.Capacity = 15;

            Assert.IsTrue(
                queueSystem.AddNewQueueLane(
                    newLane
                    )
                );
            Assert.IsTrue(
                queueSystem.GetInactiveLaneCount() == 1
                );
        }

        [TestMethod]
        public void AddLaneQueue_Test()
        {
            //LANE INSTANCE START
            Lane newLane = new Lane();
            newLane.LaneName = "Customer Service 1";
            newLane.Capacity = 15;

            Assert.IsTrue(
                queueSystem.AddNewQueueLane(
                    newLane
                    )
                );

            newLane = queueSystem.GetQueueLane(newLane.LaneNumber);

            Assert.IsNotNull(newLane);

            //LANE INSTANCE END
            //QUEUEATTENDANT INSTANCE START
            var testUser1 = new QueueAttendant();

            testUser1.SetFullName("A", "AA", "AAA");
            testUser1.SetEmail("A@email.com");
            testUser1.SetContactNumber("12345678901");
            testUser1.SetPassword("AAA");
            testUser1.DesignatedLane = null; //leave it unset

            Assert.IsTrue(
                loginSystem.RegisterAsQueueAttendant(testUser1)
                );
            testUser1 = loginSystem.LoginAsQueueAttendant(
                testUser1.GetEmail(),
                testUser1.GetPassword()
                );

            Assert.IsNotNull(
                testUser1
                );
            //QUEUEATTENDANT INSTANCE END

            //LANE QUEUE INSTANCE START
            //consider max. 12 hours for priority numbers
            var tolerance = TimeSpan.FromHours(12);

            Assert.IsTrue(
                queueSystem.SetLaneActive(
                newLane.LaneNumber,
                testUser1,
                tolerance
                )
                );

            Assert.IsTrue(
                queueSystem.GetActiveLaneCount() == 1
                );
            //LANE QUEUE INSTANCE END
        }

        [TestMethod]
        public void AddMultipleLaneQueues_Test()
        {
            //LANE INSTANCE START
            Lane newLane = new Lane();
            newLane.LaneName = "Customer Service";
            newLane.Capacity = 15;

            //  add three lanes
            for(int i =0; i < 3; i++)
            Assert.IsTrue(
                queueSystem.AddNewQueueLane(
                    newLane
                    )
                );

            Assert.IsTrue(
                queueSystem.GetLaneCount() == 3
                );

            Assert.IsTrue(
                queueSystem.GetInactiveLaneCount() == 3
                );

            var laneList = new List<Lane>();

            var newLane1 = queueSystem.GetQueueLane(1);
            var newLane2 = queueSystem.GetQueueLane(2);
            var newLane3 = queueSystem.GetQueueLane(3);

            Assert.IsNotNull(newLane1);
            Assert.IsNotNull(newLane2);
            Assert.IsNotNull(newLane3);

            laneList.Add(newLane1);
            laneList.Add(newLane2);
            laneList.Add(newLane3);

            //LANE INSTANCE END
            //QUEUEATTENDANT INSTANCE START
            var testUserList = new List<QueueAttendant>();
            char letter = 'A';

            for (int i = 0; i < 3; i++)
            {
                var testUser1 = new QueueAttendant();

                testUser1.SetFullName(
                    letter.ToString(), 
                    new string(letter, i + 1), 
                    new string(letter, i + 2));
                testUser1.SetEmail(letter + "@email.com");
                testUser1.SetContactNumber("12345678901");
                testUser1.SetPassword(new string(letter, 3));
                testUser1.DesignatedLane = null; //leave it unset

                testUserList.Add(testUser1);
                letter++;
            }
            

            //add three queue attendants
            for (int i = 0; i < testUserList.Count; i++)
            {
                Assert.IsTrue(
                loginSystem.RegisterAsQueueAttendant(testUserList[i])
                );

                testUserList[i] = loginSystem.LoginAsQueueAttendant(
                testUserList[i].GetEmail(),
                testUserList[i].GetPassword()
                );

                Assert.IsNotNull(
                    testUserList[i]
                    );
            }
            //QUEUEATTENDANT INSTANCE END

            //LANE QUEUE INSTANCE START
            var lqList = new List<LaneQueue>();

            for (int i = 0; i < 3; i++)
            {
                //consider max. 12 hours for priority numbers
                var tolerance = TimeSpan.FromHours(12);

                Assert.IsTrue(
                    queueSystem.SetLaneActive(
                    laneList[i].LaneNumber,
                    testUserList[i],
                    tolerance
                    )
                    );
            }

            Assert.IsTrue(
                queueSystem.GetActiveLaneCount() == 3
                );
            
            //LANE QUEUE INSTANCE END
        }

        [TestMethod]
        public void QueueingSystem_QueueFunctionality_Test()
        {
            //LANE INSTANCE START
            Lane newLane = new Lane();
            newLane.LaneName = "Customer Service 1";
            newLane.Capacity = 15;

            Assert.IsTrue(
                queueSystem.AddNewQueueLane(
                    newLane
                    )
                );

            newLane = queueSystem.GetQueueLane(newLane.LaneNumber);

            Assert.IsNotNull(newLane);

            //LANE INSTANCE END
            //QUEUEATTENDANT INSTANCE START
            var testUser1 = new QueueAttendant();

            testUser1.SetFullName("A", "AA", "AAA");
            testUser1.SetEmail("A@email.com");
            testUser1.SetContactNumber("12345678901");
            testUser1.SetPassword("AAA");
            testUser1.DesignatedLane = null; //leave it unset

            Assert.IsTrue(
                loginSystem.RegisterAsQueueAttendant(testUser1)
                );
            testUser1 = loginSystem.LoginAsQueueAttendant(
                testUser1.GetEmail(),
                testUser1.GetPassword()
                );

            Assert.IsNotNull(
                testUser1
                );
            //QUEUEATTENDANT INSTANCE END

            //LANE QUEUE INSTANCE START
            //consider max. 12 hours for priority numbers
            var tolerance = TimeSpan.FromHours(12);

            Assert.IsTrue(
                queueSystem.SetLaneActive(
                newLane.LaneNumber,
                testUser1,
                tolerance
                )
                );

            Assert.IsTrue(
                queueSystem.GetActiveLaneCount() == 1
                );
            //LANE QUEUE INSTANCE END

            //USER INSTANCE START
            var testQueuer = new User();

            testQueuer.SetFullName("B", "BB", "BBB");
            testQueuer.SetEmail("B@email.com");
            testQueuer.SetContactNumber("12345678901");
            testQueuer.SetPassword("BBB");

            Assert.IsTrue(
                loginSystem.RegisterAsUser(testQueuer)
                );
            testQueuer = loginSystem.LoginAsUser(
                testQueuer.GetEmail(),
                testQueuer.GetPassword()
                );

            Assert.IsNotNull(
                testQueuer
                );

            var testQueuer2 = new User();

            testQueuer2.SetFullName("C", "CC", "CCC");
            testQueuer2.SetEmail("C@email.com");
            testQueuer2.SetContactNumber("12345678901");
            testQueuer2.SetPassword("CCC");

            Assert.IsTrue(
                loginSystem.RegisterAsUser(testQueuer2)
                );
            testQueuer2 = loginSystem.LoginAsUser(
                testQueuer2.GetEmail(),
                testQueuer2.GetPassword()
                );

            Assert.IsNotNull(
                testQueuer2
                );
            //USER INSTANCE END
            //QUEUE TICKET INSTANCE START

            var qt = new QueueTicket();
            qt.owner = testQueuer;
            qt.PriorityNumber = 0;
            qt.QueueLane = newLane;
            qt.Status = QueueStatus.WAITING;

            Assert.IsTrue(
                queueSystem.EnqueueLane(
                    qt
                    )
                );

            var qt2 = new QueueTicket();
            qt2.owner = testQueuer2;
            qt2.PriorityNumber = 1;
            qt2.QueueLane = newLane;
            qt2.Status = QueueStatus.WAITING;

            Assert.IsTrue(
                queueSystem.EnqueueLane(
                    qt2
                    )
                );

            Assert.AreEqual(
                2,
                queueSystem.GetQueueCountAtLane(qt.QueueLane.LaneNumber));

            var dequeuedTicket = queueSystem.DequeueLane(1);
            var dequeuedUser = dequeuedTicket.owner as User;

            //priorities working
            Assert.AreEqual(
                testQueuer2.UserID,
                dequeuedUser.UserID
                );

            //QUEUE TICKET INSTANCE END
        }

        [TestMethod]
        public void QueueingSystem_NormalQueueFunctionality_Test()
        {
            //LANE INSTANCE START
            Lane newLane = new Lane();
            newLane.LaneName = "Customer Service 1";
            newLane.Capacity = 15;

            Assert.IsTrue(
                queueSystem.AddNewQueueLane(
                    newLane
                    )
                );

            newLane = queueSystem.GetQueueLane(newLane.LaneNumber);

            Assert.IsNotNull(newLane);

            //LANE INSTANCE END
            //QUEUEATTENDANT INSTANCE START
            var testUser1 = new QueueAttendant();

            testUser1.SetFullName("A", "AA", "AAA");
            testUser1.SetEmail("A@email.com");
            testUser1.SetContactNumber("12345678901");
            testUser1.SetPassword("AAA");
            testUser1.DesignatedLane = null; //leave it unset

            Assert.IsTrue(
                loginSystem.RegisterAsQueueAttendant(testUser1)
                );
            testUser1 = loginSystem.LoginAsQueueAttendant(
                testUser1.GetEmail(),
                testUser1.GetPassword()
                );

            Assert.IsNotNull(
                testUser1
                );
            //QUEUEATTENDANT INSTANCE END

            //LANE QUEUE INSTANCE START
            //consider max. 12 hours for priority numbers
            var tolerance = TimeSpan.FromHours(12);

            Assert.IsTrue(
                queueSystem.SetLaneActive(
                newLane.LaneNumber,
                testUser1,
                tolerance
                )
                );

            Assert.IsTrue(
                queueSystem.GetActiveLaneCount() == 1
                );
            //LANE QUEUE INSTANCE END

            //USER INSTANCE START
            var testQueuer = new User();

            testQueuer.SetFullName("B", "BB", "BBB");
            testQueuer.SetEmail("B@email.com");
            testQueuer.SetContactNumber("12345678901");
            testQueuer.SetPassword("BBB");

            Assert.IsTrue(
                loginSystem.RegisterAsUser(testQueuer)
                );
            testQueuer = loginSystem.LoginAsUser(
                testQueuer.GetEmail(),
                testQueuer.GetPassword()
                );

            Assert.IsNotNull(
                testQueuer
                );

            var testQueuer2 = new User();

            testQueuer2.SetFullName("C", "CC", "CCC");
            testQueuer2.SetEmail("C@email.com");
            testQueuer2.SetContactNumber("12345678901");
            testQueuer2.SetPassword("CCC");

            Assert.IsTrue(
                loginSystem.RegisterAsUser(testQueuer2)
                );
            testQueuer2 = loginSystem.LoginAsUser(
                testQueuer2.GetEmail(),
                testQueuer2.GetPassword()
                );

            Assert.IsNotNull(
                testQueuer2
                );
            //USER INSTANCE END
            //QUEUE TICKET INSTANCE START
            //  normal priorities for queues

            //this should still be at front
            var qt = new QueueTicket();
            qt.owner = testQueuer;
            qt.PriorityNumber = 0; //default priority
            qt.QueueLane = newLane;
            qt.Status = QueueStatus.WAITING;

            Assert.IsTrue(
                queueSystem.EnqueueLane(
                    qt
                    )
                );

            var qt2 = new QueueTicket();
            qt2.owner = testQueuer2;
            qt2.PriorityNumber = 0; //default priority
            qt2.QueueLane = newLane;
            qt2.Status = QueueStatus.WAITING;

            Assert.IsTrue(
                queueSystem.EnqueueLane(
                    qt2
                    )
                );

            //make sure the two tickets are added
            Assert.AreEqual(
                2,
                queueSystem.GetQueueCountAtLane(qt.QueueLane.LaneNumber));

            var dequeuedTicket = queueSystem.DequeueLane(1);
            var dequeuedUser = dequeuedTicket.owner as User;

            //normal queues still work with same priority
            Assert.AreEqual(
                testQueuer.UserID,
                dequeuedUser.UserID
                );

            //QUEUE TICKET INSTANCE END
        }
    }
}
