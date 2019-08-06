using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueueingSystem.Models;

namespace QueueingSystem.DataAccess
{
    public class DataAccess
    {
        public string ConnectionString { get; private set; }
        private SqlConnection connection;
        public DataAccess(string connectionString)
        {
            ConnectionString = connectionString;
            //initialize connection string to database
            connection = new SqlConnection(ConnectionString);

            //test for connectivity to database
            try
            {
                connection.Open();
                connection.Close();
            }
            catch (InvalidOperationException exc)
            {
                Console.WriteLine("An error occurred while initializing the connection.\n" +
                    "InvalidOperationException Raised: " + exc.Message);
            }

            catch (SqlException exc)
            {
                Console.WriteLine("The connection failed to initialize.\n" +
                    "SqlException Raised: " + exc.Message);
            }
        }

        public void SetConnectionString(string connectionString)
        {
            //close current connection if it is open
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
                connection.Dispose();
            }
            else
            {
                connection.Dispose();
            }

            ConnectionString = connectionString;
            //initialize connection string to database
            connection = new SqlConnection(ConnectionString);

            //test for connectivity to database
            try
            {
                connection.Open();
                connection.Close();
            }
            catch (InvalidOperationException exc)
            {
                Console.WriteLine("An error occurred while initializing the connection.\n" +
                    "InvalidOperationException Raised: " + exc.Message);
            }

            catch (SqlException exc)
            {
                Console.WriteLine("The connection failed to initialize.\n" +
                    "SqlException Raised: " + exc.Message);
            }
        }

        public bool AddAdmin(Admin newAdmin)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "AddAdmin";
            bool isSuccess = false;

            newAdmin = NormalizeAccountNullValuesToDBNull(newAdmin) as Admin;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("FirstName", SqlDbType.VarChar, 50).Value = newAdmin.GetFirstName();

                //handle if middle name is null
                if (newAdmin.GetMiddleName() == null)
                {
                    cmd.Parameters.Add("MiddleName", SqlDbType.VarChar, 50).Value = DBNull.Value;
                }
                else
                {
                    cmd.Parameters.Add("MiddleName", SqlDbType.VarChar, 50).Value = newAdmin.GetMiddleName();
                }

                cmd.Parameters.Add("LastName", SqlDbType.VarChar, 50).Value = newAdmin.GetLastName();
                cmd.Parameters.Add("Email", SqlDbType.VarChar, 50).Value = newAdmin.GetEmail();
                cmd.Parameters.Add("Password", SqlDbType.VarChar, 128).Value = newAdmin.GetPassword();

                //handle if contact number is null
                if (newAdmin.GetContactNumber() == null)
                {
                    cmd.Parameters.Add("ContactNumber", SqlDbType.NChar, 11).Value = DBNull.Value;
                }
                else
                {
                    cmd.Parameters.Add("ContactNumber", SqlDbType.NChar, 11).Value = newAdmin.GetContactNumber();
                }

                //start of query
                try
                {
                    isSuccess = cmd.ExecuteNonQuery() > 0;
                }
                catch(SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return isSuccess;
        }

        public bool AddGuest()
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "AddGuest";
            bool isSuccess = false;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                //start of query
                try
                {
                    isSuccess = cmd.ExecuteNonQuery() > 0;;
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return isSuccess;
        }

        public bool AddLane(Lane newLane)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "AddLane";
            bool isSuccess = false;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("LaneNumber", SqlDbType.Int).Value = newLane.LaneNumber;
                cmd.Parameters.Add("LaneName", SqlDbType.VarChar, 50).Value = newLane.LaneName;
                cmd.Parameters.Add("Capacity", SqlDbType.Int).Value = newLane.Capacity;

                //start of query
                try
                {
                    isSuccess = cmd.ExecuteNonQuery() > 0;
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return isSuccess;
        }

        public bool AddLaneQueue(
            LaneQueue newLaneQueue
            )
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "AddLaneQueue";
            bool isSuccess = false;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("QueueAttendantID", SqlDbType.NChar, 12).Value = newLaneQueue.Attendant.QueueAttendantID;
                cmd.Parameters.Add("LaneID", SqlDbType.Int).Value = newLaneQueue.QueueLane.LaneID;
                cmd.Parameters.Add("Tolerance", SqlDbType.Time, 7).Value = newLaneQueue.Tolerance;

                //start of query
                try
                {
                    isSuccess = cmd.ExecuteNonQuery() > 0;
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return isSuccess;
        }

        public bool AddQueueAttendant(QueueAttendant newQueueAttendant)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "AddQueueAttendant";
            bool isSuccess = false;

            newQueueAttendant = NormalizeAccountNullValuesToDBNull(newQueueAttendant) as QueueAttendant;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("FirstName", SqlDbType.VarChar, 50).Value = newQueueAttendant.GetFirstName();

                //handle null middle names
                if (newQueueAttendant.GetMiddleName() == null)
                {
                    cmd.Parameters.Add("MiddleName", SqlDbType.VarChar, 50).Value = DBNull.Value;
                }
                else
                {
                    cmd.Parameters.Add("MiddleName", SqlDbType.VarChar, 50).Value = newQueueAttendant.GetMiddleName();
                }

                cmd.Parameters.Add("LastName", SqlDbType.VarChar, 50).Value = newQueueAttendant.GetLastName();
                cmd.Parameters.Add("Email", SqlDbType.VarChar, 50).Value = newQueueAttendant.GetEmail();
                cmd.Parameters.Add("Password", SqlDbType.VarChar, 128).Value = newQueueAttendant.GetPassword();

                //handle null contact numbers
                if (newQueueAttendant.GetContactNumber() == null)
                {
                    cmd.Parameters.Add("ContactNumber", SqlDbType.NChar, 11).Value = DBNull.Value;
                }
                else
                {
                    cmd.Parameters.Add("ContactNumber", SqlDbType.NChar, 11).Value = newQueueAttendant.GetContactNumber();
                }

                //check if lane is unset
                if (newQueueAttendant.DesignatedLane.LaneID == 0 &&
                    newQueueAttendant.DesignatedLane.LaneNumber == 0
                    )
                {
                    cmd.Parameters.Add("LaneID", SqlDbType.Int).Value = DBNull.Value;
                }
                else
                {
                    cmd.Parameters.Add("LaneID", SqlDbType.Int).Value = newQueueAttendant.DesignatedLane.LaneID;
                }

                //start of query
                try
                {
                    isSuccess = cmd.ExecuteNonQuery() > 0;
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return isSuccess;
        }

        public bool AddQueueTicket(
            QueueTicket newQueueTicket, 
            Dictionary<QueueStatus, int> queueStatusMapper
            )
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "AddQueueTicket";
            bool isSuccess = false;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("LaneID", SqlDbType.Int).Value = newQueueTicket.QueueLane.LaneID;
                if (newQueueTicket.owner is User user)
                {
                    cmd.Parameters.Add("AccountNumber", SqlDbType.BigInt).Value = user.AccountNumber;
                }
                else if (newQueueTicket.owner is Guest guest)
                {
                    cmd.Parameters.Add("AccountNumber", SqlDbType.BigInt).Value = guest.AccountNumber;
                }
                else
                {
                    //must exit, unexpected account type
                    throw new ArgumentException("Invalid or illegal specified parameter for account, it is not an user or guest account."
                        , "queueAttendantAccount");
                }
                cmd.Parameters.Add("PriorityNumber", SqlDbType.SmallInt).Value = newQueueTicket.PriorityNumber;
                cmd.Parameters.Add("QueueDate", SqlDbType.Date).Value = newQueueTicket.QueueDateTime;
                cmd.Parameters.Add("QueueTime", SqlDbType.Time, 7).Value = newQueueTicket.QueueDateTime.TimeOfDay;
                cmd.Parameters.Add("QueueStatusID", SqlDbType.Int).Value = queueStatusMapper[newQueueTicket.Status];

                //start of query
                try
                {
                    isSuccess = cmd.ExecuteNonQuery() > 0;
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return isSuccess;
        }

        public bool AddUser(User newUser)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "AddUser";
            bool isSuccess = false;

            newUser = NormalizeAccountNullValuesToDBNull(newUser) as User;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("FirstName", SqlDbType.VarChar, 50).Value = newUser.GetFirstName();

                //handle if middle name is null
                if(newUser.GetMiddleName() == null)
                {
                    cmd.Parameters.Add("MiddleName", SqlDbType.VarChar, 50).Value = DBNull.Value;
                }
                else
                {
                    cmd.Parameters.Add("MiddleName", SqlDbType.VarChar, 50).Value = newUser.GetMiddleName();
                }

                cmd.Parameters.Add("LastName", SqlDbType.VarChar, 50).Value = newUser.GetLastName();
                cmd.Parameters.Add("Email", SqlDbType.VarChar, 50).Value = newUser.GetEmail();
                cmd.Parameters.Add("Password", SqlDbType.VarChar, 128).Value = newUser.GetPassword();

                //handle if contact number is null
                if (newUser.GetContactNumber() == null)
                {
                    cmd.Parameters.Add("ContactNumber", SqlDbType.NChar, 11).Value = DBNull.Value;
                }
                else
                {
                    cmd.Parameters.Add("ContactNumber", SqlDbType.NChar, 11).Value = newUser.GetContactNumber();
                }

                //start of query
                try
                {
                    isSuccess = cmd.ExecuteNonQuery() > 0;
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return isSuccess;
        }

        public Dictionary<QueueStatus, int> GetQueueStatuses()
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "GetQueueStatuses";
            var queueStatusMapper = new Dictionary<QueueStatus, int>();

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                //start of query
                try
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int queueStatusID = reader.GetInt32(reader.GetOrdinal("QueueStatusID"));
                            string queueStatusState = reader["QueueStatusState"].ToString();

                            queueStatusMapper.Add(
                                (QueueStatus)Enum.Parse(typeof(QueueStatus), queueStatusState),
                                queueStatusID
                                );
                        }
                    }
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return queueStatusMapper;
        }

        public bool DeleteLaneQueue(int laneQueueID)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "DeleteLaneQueue";
            bool isSuccess = false;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("LaneQueueID", SqlDbType.Int).Value = laneQueueID;

                //start of query
                try
                {
                    isSuccess = cmd.ExecuteNonQuery() > 0;
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return isSuccess;
        }

        public bool DeleteLaneWithLaneID(int laneID)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "DeleteLaneWithLaneID";
            bool isSuccess = false;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("LaneID", SqlDbType.Int).Value = laneID;

                //start of query
                try
                {
                    isSuccess = cmd.ExecuteNonQuery() > 0;
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return isSuccess;
        }

        public bool DeleteLaneWithLaneNumber(int laneNumber)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "DeleteLaneWithLaneNumber";
            bool isSuccess = false;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("LaneNumber", SqlDbType.Int).Value = laneNumber;

                //start of query
                try
                {
                    isSuccess = cmd.ExecuteNonQuery() > 0;
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return isSuccess;
        }

        public bool DeleteQueueTicketWithQueueNumber(int queueNumber)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "DeleteQueueTicketWithQueueNumber";
            bool isSuccess = false;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("QueueNumber", SqlDbType.Int).Value = queueNumber;

                //start of query
                try
                {
                    isSuccess = cmd.ExecuteNonQuery() > 0;
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return isSuccess;
        }

        public bool DeleteQueueTicketWithQueueTicketID(string queueTicketID)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "DeleteQueueTicketWithQueueTicketID";
            bool isSuccess = false;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("QueueTicketID", SqlDbType.NChar, 16).Value = queueTicketID;

                //start of query
                try
                {
                    isSuccess = cmd.ExecuteNonQuery() > 0;
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return isSuccess;
        }

        public bool EditAccountWithAccountNumber(object editedAccount)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            //determine type of account
            if (editedAccount is User userAccount)
            {
                string procName = "EditAccountWithAccountNumber";
                bool isSuccess = false;

                userAccount = NormalizeAccountNullValuesToDBNull(userAccount) as User;

                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = procName;

                    cmd.Parameters.Add("AccountNumber", SqlDbType.BigInt).Value = userAccount.AccountNumber;
                    cmd.Parameters.Add("FirstName", SqlDbType.VarChar, 50).Value = userAccount.GetFirstName();

                    //handle null middle names
                    if (userAccount.GetMiddleName() == null)
                    {
                        cmd.Parameters.Add("MiddleName", SqlDbType.VarChar, 50).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("MiddleName", SqlDbType.VarChar, 50).Value = userAccount.GetMiddleName();
                    }

                    cmd.Parameters.Add("LastName", SqlDbType.VarChar, 50).Value = userAccount.GetLastName();
                    cmd.Parameters.Add("Email", SqlDbType.VarChar, 50).Value = userAccount.GetEmail();
                    cmd.Parameters.Add("Password", SqlDbType.VarChar, 128).Value = userAccount.GetPassword();

                    //handle null contact numbers
                    if (userAccount.GetContactNumber() == null)
                    {
                        cmd.Parameters.Add("ContactNumber", SqlDbType.NChar, 11).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("ContactNumber", SqlDbType.NChar, 11).Value = userAccount.GetContactNumber();
                    }

                    //start of query
                    try
                    {
                        isSuccess = cmd.ExecuteNonQuery() > 0;
                    }
                    catch (SqlException exc)
                    {
                        Console.WriteLine("SQL Exception encountered: " + exc.Message);
                    }
                    //end of query
                    connection.Close();
                }

                return isSuccess;
            }
            else if (editedAccount is Guest guestAccount)
            {
                //guest accounts cannot be edited
                throw new ArgumentException("Guest accounts cannot be edited.", "editedAccount");
            }
            else if (editedAccount is Admin adminAccount)
            {
                string procName = "EditAccountWithAccountNumber";
                bool isSuccess = false;

                adminAccount = NormalizeAccountNullValuesToDBNull(adminAccount) as Admin;

                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = procName;

                    cmd.Parameters.Add("AccountNumber", SqlDbType.BigInt).Value = adminAccount.AccountNumber;
                    cmd.Parameters.Add("FirstName", SqlDbType.VarChar, 50).Value = adminAccount.GetFirstName();
                    cmd.Parameters.Add("MiddleName", SqlDbType.VarChar, 50).Value = adminAccount.GetMiddleName();
                    cmd.Parameters.Add("LastName", SqlDbType.VarChar, 50).Value = adminAccount.GetLastName();
                    cmd.Parameters.Add("Email", SqlDbType.VarChar, 50).Value = adminAccount.GetEmail();
                    cmd.Parameters.Add("Password", SqlDbType.VarChar, 128).Value = adminAccount.GetPassword();
                    cmd.Parameters.Add("ContactNumber", SqlDbType.NChar, 11).Value = adminAccount.GetContactNumber();

                    //start of query
                    try
                    {
                        isSuccess = cmd.ExecuteNonQuery() > 1;
                    }
                    catch (SqlException exc)
                    {
                        Console.WriteLine("SQL Exception encountered: " + exc.Message);
                    }
                    //end of query
                    connection.Close();
                }

                return isSuccess;
            }
            else if (editedAccount is QueueAttendant queueAttendantAccount)
            {
                string procName = "EditAccountWithAccountNumber";
                bool isSuccess = false;

                queueAttendantAccount = NormalizeAccountNullValuesToDBNull(queueAttendantAccount) as QueueAttendant;

                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = procName;

                    cmd.Parameters.Add("AccountNumber", SqlDbType.BigInt).Value = queueAttendantAccount.AccountNumber;
                    cmd.Parameters.Add("FirstName", SqlDbType.VarChar, 50).Value = queueAttendantAccount.GetFirstName();
                    cmd.Parameters.Add("MiddleName", SqlDbType.VarChar, 50).Value = queueAttendantAccount.GetMiddleName();
                    cmd.Parameters.Add("LastName", SqlDbType.VarChar, 50).Value = queueAttendantAccount.GetLastName();
                    cmd.Parameters.Add("Email", SqlDbType.VarChar, 50).Value = queueAttendantAccount.GetEmail();
                    cmd.Parameters.Add("Password", SqlDbType.VarChar, 128).Value = queueAttendantAccount.GetPassword();
                    cmd.Parameters.Add("ContactNumber", SqlDbType.NChar, 11).Value = queueAttendantAccount.GetContactNumber();

                    //lane id is not edited here, expected parameter edits account attributes only, not exclusive specific attributes

                    //start of query
                    try
                    {
                        isSuccess = cmd.ExecuteNonQuery() > 1;
                    }
                    catch (SqlException exc)
                    {
                        Console.WriteLine("SQL Exception encountered: " + exc.Message);
                    }
                    //end of query
                    connection.Close();
                }

                return isSuccess;
            }
            else
            {
                //must exit, unexpected parameter
                throw new ArgumentException("Invalid specified parameter for account, it is not an account.", "queueAttendantAccount");
            }
        }

        public bool EditDesignatedLaneOfQueueAttendant(string queueAttendantID, int laneID)
        {
            string procName = "EditDesignatedLaneOfQueueAttendant";
            bool isSuccess = false;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("QueueAttendantID", SqlDbType.NChar, 12).Value = queueAttendantID;
                cmd.Parameters.Add("LaneID", SqlDbType.Int).Value = laneID;

                //start of query
                try
                {
                    isSuccess = cmd.ExecuteNonQuery() > 0;
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return isSuccess;
        }

        public bool EditLaneQueue(
            LaneQueue editedLaneQueue
            )
        {
            string procName = "EditLaneQueue";
            bool isSuccess = false;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("LaneQueueID", SqlDbType.Int).Value = editedLaneQueue.LaneQueueID;
                cmd.Parameters.Add("QueueAttendantID", SqlDbType.NChar, 12).Value = editedLaneQueue.Attendant.QueueAttendantID;
                cmd.Parameters.Add("LaneID", SqlDbType.Int).Value = editedLaneQueue.QueueLane.LaneID;
                cmd.Parameters.Add("Tolerance", SqlDbType.Time, 7).Value = editedLaneQueue.Tolerance;

                //start of query
                try
                {
                    isSuccess = cmd.ExecuteNonQuery() > 0;
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return isSuccess;
        }

        public bool EditLaneWithLaneID(Lane editedLane)
        {
            string procName = "EditLaneWithLaneID";
            bool isSuccess = false;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("LaneName", SqlDbType.VarChar, 50).Value = editedLane.LaneName;
                cmd.Parameters.Add("LaneNumber", SqlDbType.Int).Value = editedLane.LaneNumber;
                cmd.Parameters.Add("LaneID", SqlDbType.Int).Value = editedLane.LaneID;
                cmd.Parameters.Add("Capacity", SqlDbType.Int).Value = editedLane.Capacity;

                //start of query
                try
                {
                    isSuccess = cmd.ExecuteNonQuery() > 0;
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return isSuccess;
        }

        public bool EditLaneWithLaneNumber(Lane editedLane)
        {
            string procName = "EditLaneWithLaneNumber";
            bool isSuccess = false;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("LaneName", SqlDbType.VarChar, 50).Value = editedLane.LaneName;
                cmd.Parameters.Add("LaneNumber", SqlDbType.Int).Value = editedLane.LaneNumber;
                cmd.Parameters.Add("Capacity", SqlDbType.Int).Value = editedLane.Capacity;

                //start of query
                try
                {
                    isSuccess = cmd.ExecuteNonQuery() > 0;
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return isSuccess;
        }

        public bool EditQueueTicketStatusWithTicketID(
            int queueTicketID,
            QueueStatus queueStatus,
            Dictionary<QueueStatus, int> queueStatusMapper
            )
        {
            string procName = "EditQueueTicketStatusWithTicketID";
            bool isSuccess = false;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("QueueTicketID", SqlDbType.NChar, 16).Value = queueTicketID;
                cmd.Parameters.Add("QueueStatusID", SqlDbType.Int).Value = queueStatusMapper[queueStatus];

                //start of query
                try
                {
                    isSuccess = cmd.ExecuteNonQuery() > 0;
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return isSuccess;
        }

        public bool EditQueueTicketStatusWithTicketNumber(
            int queueTicketNumber,
            QueueStatus queueStatus,
            Dictionary<QueueStatus, int> queueStatusMapper
            )
        {
            string procName = "EditQueueTicketStatusWithTicketNumber";
            bool isSuccess = false;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("QueueTicketNumber", SqlDbType.Int).Value = queueTicketNumber;
                cmd.Parameters.Add("QueueStatusID", SqlDbType.Int).Value = queueStatusMapper[queueStatus];

                //start of query
                try
                {
                    isSuccess = cmd.ExecuteNonQuery() > 0;
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return isSuccess;
        }

        public bool EditQueueTicketWithQueueNumber(
            QueueTicket editedQueueTicket,
            Dictionary<QueueStatus, int> queueStatusMapper
            )
        {
            string procName = "EditQueueTicketWithQueueNumber";
            bool isSuccess = false;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("QueueNumber", SqlDbType.Int).Value = editedQueueTicket.QueueNumber;
                cmd.Parameters.Add("LaneID", SqlDbType.Int).Value = editedQueueTicket.QueueLane.LaneID;
                if (editedQueueTicket.owner is User user)
                {
                    cmd.Parameters.Add("AccountNumber", SqlDbType.BigInt).Value = user.AccountNumber;
                }
                else if (editedQueueTicket.owner is Guest guest)
                {
                    cmd.Parameters.Add("AccountNumber", SqlDbType.BigInt).Value = guest.AccountNumber;
                }
                else
                {
                    //must exit, unexpected account type
                    throw new ArgumentException("Invalid or illegal specified parameter for account, it is not an user or guest account."
                        , "queueAttendantAccount");
                }
                cmd.Parameters.Add("PriorityNumber", SqlDbType.SmallInt).Value = editedQueueTicket.PriorityNumber;
                cmd.Parameters.Add("QueueDate", SqlDbType.Date).Value = editedQueueTicket.QueueDateTime;
                cmd.Parameters.Add("QueueTime", SqlDbType.Time, 7).Value = editedQueueTicket.QueueDateTime.TimeOfDay;
                cmd.Parameters.Add("QueueStatusID", SqlDbType.Int).Value = queueStatusMapper[editedQueueTicket.Status];

                //start of query
                try
                {
                    isSuccess = cmd.ExecuteNonQuery() > 0;
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return isSuccess;
        }

        public bool EditQueueTicketWithQueueTicketID(
            QueueTicket editedQueueTicket,
            Dictionary<QueueStatus, int> queueStatusMapper
            )
        {
            string procName = "EditQueueTicketWithQueueTicketID";
            bool isSuccess = false;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("QueueTicketID", SqlDbType.NChar, 16).Value = editedQueueTicket.QueueID;
                cmd.Parameters.Add("QueueNumber", SqlDbType.Int).Value = editedQueueTicket.QueueNumber;
                cmd.Parameters.Add("LaneID", SqlDbType.Int).Value = editedQueueTicket.QueueLane.LaneID;
                if (editedQueueTicket.owner is User user)
                {
                    cmd.Parameters.Add("AccountNumber", SqlDbType.BigInt).Value = user.AccountNumber;
                }
                else if (editedQueueTicket.owner is Guest guest)
                {
                    cmd.Parameters.Add("AccountNumber", SqlDbType.BigInt).Value = guest.AccountNumber;
                }
                else
                {
                    //must exit, unexpected account type
                    throw new ArgumentException("Invalid or illegal specified parameter for account, it is not an user or guest account."
                        , "queueAttendantAccount");
                }
                cmd.Parameters.Add("PriorityNumber", SqlDbType.SmallInt).Value = editedQueueTicket.PriorityNumber;
                cmd.Parameters.Add("QueueDate", SqlDbType.Date).Value = editedQueueTicket.QueueDateTime;
                cmd.Parameters.Add("QueueTime", SqlDbType.Time, 7).Value = editedQueueTicket.QueueDateTime.TimeOfDay;
                cmd.Parameters.Add("QueueStatusID", SqlDbType.Int).Value = queueStatusMapper[editedQueueTicket.Status];

                //start of query
                try
                {
                    isSuccess = cmd.ExecuteNonQuery() > 0;
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return isSuccess;
        }

        public Admin GetAdminWithAccountNumber(long accountNumber)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "GetAdminWithAccountNumber";
            Admin retAdmin = null;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("AccountNumber", SqlDbType.BigInt).Value = accountNumber;

                //start of query
                try
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            retAdmin = new Admin();

                            retAdmin.AccountNumber = reader.GetInt64(reader.GetOrdinal("AccountNumber"));
                            retAdmin.AdminID = reader["AdminID"].ToString();
                            retAdmin.SetFullName(
                                reader["FirstName"].ToString(),
                                SafeGetString(reader, reader.GetOrdinal("MiddleName")),
                                reader["LastName"].ToString()
                                );
                            retAdmin.SetEmail(reader["Email"].ToString());
                            retAdmin.SetPassword(reader["Password"].ToString());
                            retAdmin.SetContactNumber(SafeGetString(reader, reader.GetOrdinal("ContactNumber")));
                        }
                    }
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return retAdmin;
        }

        public Admin GetAdminWithAdminID(string adminID)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "GetAdminWithAdminID";
            Admin retAdmin = null;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("AdminID", SqlDbType.NChar, 12).Value = adminID;

                //start of query
                try
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            retAdmin = new Admin();

                            retAdmin.AccountNumber = reader.GetInt64(reader.GetOrdinal("AccountNumber"));
                            retAdmin.AdminID = reader["AdminID"].ToString();
                            retAdmin.SetFullName(
                                reader["FirstName"].ToString(),
                                SafeGetString(reader, reader.GetOrdinal("MiddleName")),
                                reader["LastName"].ToString()
                                );
                            retAdmin.SetEmail(reader["Email"].ToString());
                            retAdmin.SetPassword(reader["Password"].ToString());
                            retAdmin.SetContactNumber(SafeGetString(reader, reader.GetOrdinal("ContactNumber")));
                        }
                    }
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return retAdmin;
        }

        public Admin GetAdminWithEmail(string adminEmail)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "GetAdminWithEmail";
            Admin retAdmin = null;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("Email", SqlDbType.VarChar, 50).Value = adminEmail;

                //start of query
                try
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            retAdmin = new Admin();

                            retAdmin.AccountNumber = reader.GetInt64(reader.GetOrdinal("AccountNumber"));
                            retAdmin.AdminID = reader["AdminID"].ToString();
                            retAdmin.SetFullName(
                                reader["FirstName"].ToString(),
                                SafeGetString(reader, reader.GetOrdinal("MiddleName")),
                                reader["LastName"].ToString()
                                );
                            retAdmin.SetEmail(reader["Email"].ToString());
                            retAdmin.SetPassword(reader["Password"].ToString());
                            retAdmin.SetContactNumber(SafeGetString(reader, reader.GetOrdinal("ContactNumber")));
                        }
                    }
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return retAdmin;
        }

        public Guest GetGuestWithGuestNumber(int guestNumber)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "GetGuestWithGuestNumber";
            Guest retGuest = null;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("GuestNumber", SqlDbType.Int).Value = guestNumber;

                //start of query
                try
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            retGuest = new Guest();

                            retGuest.AccountNumber = reader.GetInt64(reader.GetOrdinal("AccountNumber"));
                            retGuest.SetGuestNumber(guestNumber);
                        }
                    }
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return retGuest;
        }

        public Guest GetGuestWithAccountNumber(long accountNumber)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "GetGuestWithAccountNumber";
            Guest retGuest = null;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("AccountNumber", SqlDbType.BigInt).Value = accountNumber;

                //start of query
                try
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            retGuest = new Guest();

                            retGuest.AccountNumber = reader.GetInt64(reader.GetOrdinal("AccountNumber"));
                            retGuest.SetGuestNumber(
                                reader.GetInt32(reader.GetOrdinal("GuestNumber"))
                                );
                        }
                    }
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return retGuest;
        }

        public LaneQueue GetLaneQueueWithLaneID(int laneID)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "GetLaneQueueWithLaneID";
            LaneQueue retLaneQueue = null;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("LaneID", SqlDbType.Int).Value = laneID;

                //start of query
                try
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            retLaneQueue = new LaneQueue();

                            retLaneQueue.LaneQueueID = reader.GetInt32(reader.GetOrdinal("LaneQueueID"));
                            retLaneQueue.Tolerance = reader.GetTimeSpan(reader.GetOrdinal("Tolerance"));

                            retLaneQueue.Attendant.QueueAttendantID = reader["QueueAttendantID"].ToString();
                            retLaneQueue.Attendant.SetFullName(
                                reader["FirstName"].ToString(),
                                reader["MiddleName"].ToString(),
                                reader["LastName"].ToString()
                                );
                            retLaneQueue.Attendant.SetEmail(
                                reader["Email"].ToString()
                                );
                            retLaneQueue.Attendant.SetPassword(
                                reader["Password"].ToString()
                                );
                            retLaneQueue.Attendant.SetContactNumber(
                                reader["ContactNumber"].ToString()
                                );

                            retLaneQueue.QueueLane.LaneID = reader.GetInt32(reader.GetOrdinal("LaneID"));
                            retLaneQueue.QueueLane.LaneNumber = reader.GetInt32(reader.GetOrdinal("LaneNumber"));
                            retLaneQueue.QueueLane.LaneName = reader["LaneName"].ToString();
                            retLaneQueue.SetQueueCapacity(
                                reader.GetInt32(reader.GetOrdinal("Capacity"))
                                );
                        }
                    }
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return retLaneQueue;
        }

        public LaneQueue GetLaneQueueWithQueueAttendantID(string queueAttendantID)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "GetLaneQueueWithQueueAttendantID";
            LaneQueue retLaneQueue = null;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("QueueAttendantID", SqlDbType.NChar, 12).Value = queueAttendantID;

                //start of query
                try
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            retLaneQueue = new LaneQueue();

                            retLaneQueue.LaneQueueID = reader.GetInt32(reader.GetOrdinal("LaneQueueID"));
                            retLaneQueue.Tolerance = reader.GetTimeSpan(reader.GetOrdinal("Tolerance"));

                            retLaneQueue.Attendant.QueueAttendantID = reader["QueueAttendantID"].ToString();
                            retLaneQueue.Attendant.SetFullName(
                                reader["FirstName"].ToString(),
                                reader["MiddleName"].ToString(),
                                reader["LastName"].ToString()
                                );
                            retLaneQueue.Attendant.SetEmail(
                                reader["Email"].ToString()
                                );
                            retLaneQueue.Attendant.SetPassword(
                                reader["Password"].ToString()
                                );
                            retLaneQueue.Attendant.SetContactNumber(
                                reader["ContactNumber"].ToString()
                                );

                            retLaneQueue.QueueLane.LaneID = reader.GetInt32(reader.GetOrdinal("LaneID"));
                            retLaneQueue.QueueLane.LaneNumber = reader.GetInt32(reader.GetOrdinal("LaneNumber"));
                            retLaneQueue.QueueLane.LaneName = reader["LaneName"].ToString();
                            retLaneQueue.SetQueueCapacity(
                                reader.GetInt32(reader.GetOrdinal("Capacity"))
                                );
                        }
                    }
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return retLaneQueue;
        }

        public List<Lane> GetLanes()
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "GetLaneWithLaneID";
            List<Lane> retLaneList = new List<Lane>();

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                //start of query
                try
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var retLane = new Lane();

                            retLane.LaneID = reader.GetInt32(reader.GetOrdinal("LaneID"));
                            retLane.LaneNumber = reader.GetInt32(reader.GetOrdinal("LaneNumber"));
                            retLane.LaneName = reader["LaneName"].ToString();
                            retLane.Capacity = reader.GetInt32(reader.GetOrdinal("Capacity"));

                            retLaneList.Add(retLane);
                        }
                    }
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return retLaneList;
        }

        public Lane GetLaneWithLaneID(int laneID)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "GetLaneWithLaneID";
            Lane retLane = null;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("LaneID", SqlDbType.Int).Value = laneID;

                //start of query
                try
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            retLane = new Lane();

                            retLane.LaneID = reader.GetInt32(reader.GetOrdinal("LaneID"));
                            retLane.LaneNumber = reader.GetInt32(reader.GetOrdinal("LaneNumber"));
                            retLane.LaneName = reader["LaneName"].ToString();
                            retLane.Capacity = reader.GetInt32(reader.GetOrdinal("Capacity"));
                        }
                    }
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return retLane;
        }

        public Lane GetLaneWithLaneNumber(int laneNumber)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "GetLaneWithLaneNumber";
            Lane retLane = null;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("LaneNumber", SqlDbType.Int).Value = laneNumber;

                //start of query
                try
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            retLane = new Lane();

                            retLane.LaneID = reader.GetInt32(reader.GetOrdinal("LaneID"));
                            retLane.LaneNumber = reader.GetInt32(reader.GetOrdinal("LaneNumber"));
                            retLane.LaneName = reader["LaneName"].ToString();
                            retLane.Capacity = reader.GetInt32(reader.GetOrdinal("Capacity"));
                        }
                    }
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return retLane;
        }

        public QueueAttendant GetQueueAttendantWithAccountNumber(long accountNumber)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "GetQueueAttendantWithAccountNumber";
            QueueAttendant retQueueAttendant = null;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("AccountNumber", SqlDbType.BigInt).Value = accountNumber;

                //start of query
                try
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            retQueueAttendant = new QueueAttendant();

                            retQueueAttendant.AccountNumber = reader.GetInt64(reader.GetOrdinal("AccountNumber"));
                            retQueueAttendant.SetFullName(
                                reader["FirstName"].ToString(),
                                SafeGetString(reader,reader.GetOrdinal("MiddleName")),
                                reader["LastName"].ToString()
                                );
                            retQueueAttendant.SetEmail(
                                reader["Email"].ToString()
                                );
                            retQueueAttendant.SetPassword(
                                reader["Password"].ToString()
                                );
                            retQueueAttendant.SetContactNumber(
                                SafeGetString(reader, reader.GetOrdinal("ContactNumber"))
                                );
                            retQueueAttendant.QueueAttendantID = reader["QueueAttendantID"].ToString();

                            retQueueAttendant.DesignatedLane.LaneID = SafeGetInt32(reader, reader.GetOrdinal("LaneID"));

                            //lane can be left to null, must check for it
                            if (retQueueAttendant.DesignatedLane.LaneID != 0)
                            {
                                //lane is set
                                retQueueAttendant.DesignatedLane.LaneNumber = reader.GetInt32(reader.GetOrdinal("LaneNumber"));
                                retQueueAttendant.DesignatedLane.LaneName = reader["LaneName"].ToString();
                                retQueueAttendant.DesignatedLane.Capacity = reader.GetInt32(reader.GetOrdinal("Capacity"));
                            }
                            else
                            {
                                //lane is unset, leave to default values
                                retQueueAttendant.DesignatedLane = new Lane();
                            }

                            
                        }
                    }
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return retQueueAttendant;
        }

        public QueueAttendant GetQueueAttendantWithEmail(string email)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "GetQueueAttendantWithEmail";
            QueueAttendant retQueueAttendant = null;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("Email", SqlDbType.VarChar, 50).Value = email;

                //start of query
                try
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            retQueueAttendant = new QueueAttendant();

                            retQueueAttendant.AccountNumber = reader.GetInt64(reader.GetOrdinal("AccountNumber"));
                            retQueueAttendant.SetFullName(
                                reader["FirstName"].ToString(),
                                SafeGetString(reader, reader.GetOrdinal("MiddleName")),
                                reader["LastName"].ToString()
                                );
                            retQueueAttendant.SetEmail(
                                reader["Email"].ToString()
                                );
                            retQueueAttendant.SetPassword(
                                reader["Password"].ToString()
                                );
                            retQueueAttendant.SetContactNumber(
                                SafeGetString(reader, reader.GetOrdinal("ContactNumber"))
                                );
                            retQueueAttendant.QueueAttendantID = reader["QueueAttendantID"].ToString();

                            retQueueAttendant.DesignatedLane.LaneID = SafeGetInt32(reader, reader.GetOrdinal("LaneID"));

                            //lane can be left to null, must check for it
                            if (retQueueAttendant.DesignatedLane.LaneID != 0)
                            {
                                //lane is set
                                retQueueAttendant.DesignatedLane.LaneNumber = reader.GetInt32(reader.GetOrdinal("LaneNumber"));
                                retQueueAttendant.DesignatedLane.LaneName = reader["LaneName"].ToString();
                                retQueueAttendant.DesignatedLane.Capacity = reader.GetInt32(reader.GetOrdinal("Capacity"));
                            }
                            else
                            {
                                //lane is unset, leave to default values
                                retQueueAttendant.DesignatedLane = new Lane();
                            }
                        }
                    }
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return retQueueAttendant;
        }

        public QueueAttendant GetQueueAttendantWithID(string queueAttendantID)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "GetQueueAttendantWithID";
            QueueAttendant retQueueAttendant = null;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("Email", SqlDbType.NChar, 12).Value = queueAttendantID;

                //start of query
                try
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            retQueueAttendant = new QueueAttendant();

                            retQueueAttendant.AccountNumber = reader.GetInt64(reader.GetOrdinal("AccountNumber"));
                            retQueueAttendant.SetFullName(
                                reader["FirstName"].ToString(),
                                SafeGetString(reader, reader.GetOrdinal("MiddleName")),
                                reader["LastName"].ToString()
                                );
                            retQueueAttendant.SetEmail(
                                reader["Email"].ToString()
                                );
                            retQueueAttendant.SetPassword(
                                reader["Password"].ToString()
                                );
                            retQueueAttendant.SetContactNumber(
                                SafeGetString(reader, reader.GetOrdinal("ContactNumber"))
                                );
                            retQueueAttendant.QueueAttendantID = reader["QueueAttendantID"].ToString();

                            retQueueAttendant.DesignatedLane.LaneID = SafeGetInt32(reader, reader.GetOrdinal("LaneID"));

                            //lane can be left to null, must check for it
                            if (retQueueAttendant.DesignatedLane.LaneID != 0)
                            {
                                //lane is set
                                retQueueAttendant.DesignatedLane.LaneNumber = reader.GetInt32(reader.GetOrdinal("LaneNumber"));
                                retQueueAttendant.DesignatedLane.LaneName = reader["LaneName"].ToString();
                                retQueueAttendant.DesignatedLane.Capacity = reader.GetInt32(reader.GetOrdinal("Capacity"));
                            }
                            else
                            {
                                //lane is unset, leave to default values
                                retQueueAttendant.DesignatedLane = new Lane();
                            }
                        }
                    }
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return retQueueAttendant;
        }

        public QueueAttendant GetQueueAttendantWithLaneID(int laneID)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "GetQueueAttendantWithLaneID";
            QueueAttendant retQueueAttendant = null;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("LaneID", SqlDbType.Int).Value = laneID;

                //start of query
                try
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            retQueueAttendant = new QueueAttendant();

                            retQueueAttendant.AccountNumber = reader.GetInt64(reader.GetOrdinal("AccountNumber"));
                            retQueueAttendant.SetFullName(
                                reader["FirstName"].ToString(),
                                SafeGetString(reader, reader.GetOrdinal("MiddleName")),
                                reader["LastName"].ToString()
                                );
                            retQueueAttendant.SetEmail(
                                reader["Email"].ToString()
                                );
                            retQueueAttendant.SetPassword(
                                reader["Password"].ToString()
                                );
                            retQueueAttendant.SetContactNumber(
                                SafeGetString(reader, reader.GetOrdinal("ContactNumber"))
                                );
                            retQueueAttendant.QueueAttendantID = reader["QueueAttendantID"].ToString();

                            retQueueAttendant.DesignatedLane.LaneID = SafeGetInt32(reader, reader.GetOrdinal("LaneID"));

                            //lane can be left to null, must check for it
                            if (retQueueAttendant.DesignatedLane.LaneID != 0)
                            {
                                //lane is set
                                retQueueAttendant.DesignatedLane.LaneNumber = reader.GetInt32(reader.GetOrdinal("LaneNumber"));
                                retQueueAttendant.DesignatedLane.LaneName = reader["LaneName"].ToString();
                                retQueueAttendant.DesignatedLane.Capacity = reader.GetInt32(reader.GetOrdinal("Capacity"));
                            }
                            else
                            {
                                //lane is unset, leave to default values
                                retQueueAttendant.DesignatedLane = new Lane();
                            }
                        }
                    }
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return retQueueAttendant;
        }

        public List<QueueTicket> GetQueueTicketsOf(
            long accountNumber
            )
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "GetQueueTicketsOf";
            List<QueueTicket> retQueueTickets = new List<QueueTicket>();

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("AccountNumber", SqlDbType.BigInt).Value = accountNumber;

                //start of query
                try
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            var queueTicketItem = new QueueTicket();

                            queueTicketItem.QueueID = reader["QueueTicketID"].ToString();
                            queueTicketItem.QueueNumber = reader.GetInt32(reader.GetOrdinal("QueueNumber"));
                            queueTicketItem.PriorityNumber = reader.GetInt16(reader.GetOrdinal("PriorityNumber"));
                            queueTicketItem.QueueDateTime =
                                reader.GetDateTime(reader.GetOrdinal("QueueDate")).Add(
                                    reader.GetTimeSpan(reader.GetOrdinal("QueueTime"))
                                    );
                            queueTicketItem.Status = (QueueStatus)Enum.Parse(
                                typeof(QueueStatus),
                                reader["QueueStatusState"].ToString()
                                );

                            //must determine account type
                            long qTAccountNumber = reader.GetInt64(reader.GetOrdinal("AccountNumber"));
                            object qTOwner = GetUserWithAccountNumber(qTAccountNumber);
                            if (qTOwner == null)
                            {
                                qTOwner = GetGuestWithAccountNumber(qTAccountNumber);
                                if (qTOwner != null)
                                {
                                    var guestOwner = qTOwner as Guest;
                                    queueTicketItem.SetOwner(guestOwner);
                                }
                                else
                                {
                                    // must not continue, something wrong with the database, possibly tampered?
                                    throw new InvalidOperationException("Unexpected account type, it is not a user or guest, or it does not exist");
                                }
                            }
                            else
                            {
                                var userOwner = qTOwner as User;
                                queueTicketItem.SetOwner(userOwner);
                            }

                            queueTicketItem.QueueLane.LaneNumber = reader.GetInt32(reader.GetOrdinal("LaneNumber"));
                            queueTicketItem.QueueLane.LaneID = reader.GetInt32(reader.GetOrdinal("LaneID"));
                            queueTicketItem.QueueLane.LaneName = reader["LaneName"].ToString();
                            queueTicketItem.QueueLane.Capacity = reader.GetInt32(reader.GetOrdinal("Capacity"));

                            retQueueTickets.Add(queueTicketItem);
                        }
                    }
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return retQueueTickets;
        }

        public List<QueueTicket> GetQueueTicketsOfWithStatus(
            long accountNumber,
            QueueStatus status,
            Dictionary<QueueStatus, int> queueStatusMapper
            )
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "GetQueueTicketsOfWithStatus";
            List<QueueTicket> retQueueTickets = new List<QueueTicket>();

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("AccountNumber", SqlDbType.BigInt).Value = accountNumber;
                cmd.Parameters.Add("QueueStatusID", SqlDbType.Int).Value = queueStatusMapper[status];

                //start of query
                try
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            var queueTicketItem = new QueueTicket();

                            queueTicketItem.QueueID = reader["QueueTicketID"].ToString();
                            queueTicketItem.QueueNumber = reader.GetInt32(reader.GetOrdinal("QueueNumber"));
                            queueTicketItem.PriorityNumber = reader.GetInt16(reader.GetOrdinal("PriorityNumber"));
                            queueTicketItem.QueueDateTime =
                                reader.GetDateTime(reader.GetOrdinal("QueueDate")).Add(
                                    reader.GetTimeSpan(reader.GetOrdinal("QueueTime"))
                                    );
                            queueTicketItem.Status = (QueueStatus)Enum.Parse(
                                typeof(QueueStatus),
                                reader["QueueStatusState"].ToString()
                                );

                            //must determine account type
                            long qTAccountNumber = reader.GetInt64(reader.GetOrdinal("AccountNumber"));
                            object qTOwner = GetUserWithAccountNumber(qTAccountNumber);
                            if (qTOwner == null)
                            {
                                qTOwner = GetGuestWithAccountNumber(qTAccountNumber);
                                if (qTOwner != null)
                                {
                                    var guestOwner = qTOwner as Guest;
                                    queueTicketItem.SetOwner(guestOwner);
                                }
                                else
                                {
                                    // must not continue, something wrong with the database, possibly tampered?
                                    throw new InvalidOperationException("Unexpected account type, it is not a user or guest, or it does not exist");
                                }
                            }
                            else
                            {
                                var userOwner = qTOwner as User;
                                queueTicketItem.SetOwner(userOwner);
                            }

                            queueTicketItem.QueueLane.LaneNumber = reader.GetInt32(reader.GetOrdinal("LaneNumber"));
                            queueTicketItem.QueueLane.LaneID = reader.GetInt32(reader.GetOrdinal("LaneID"));
                            queueTicketItem.QueueLane.LaneName = reader["LaneName"].ToString();
                            queueTicketItem.QueueLane.Capacity = reader.GetInt32(reader.GetOrdinal("Capacity"));

                            retQueueTickets.Add(queueTicketItem);
                        }
                    }
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return retQueueTickets;
        }

        public QueueTicket GetQueueTicketWithTicketID(string queueTicketID)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "GetQueueTicketWithTicketID";
            QueueTicket retQueueTicket = new QueueTicket();

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("QueueTicketID", SqlDbType.NChar, 16).Value = queueTicketID;

                //start of query
                try
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        if(reader.Read())
                        {
                            retQueueTicket = new QueueTicket();

                            retQueueTicket.QueueID = reader["QueueTicketID"].ToString();
                            retQueueTicket.QueueNumber = reader.GetInt32(reader.GetOrdinal("QueueNumber"));
                            retQueueTicket.PriorityNumber = reader.GetInt16(reader.GetOrdinal("PriorityNumber"));
                            retQueueTicket.QueueDateTime =
                                reader.GetDateTime(reader.GetOrdinal("QueueDate")).Add(
                                    reader.GetTimeSpan(reader.GetOrdinal("QueueTime"))
                                    );
                            retQueueTicket.Status = (QueueStatus)Enum.Parse(
                                typeof(QueueStatus),
                                reader["QueueStatusState"].ToString()
                                );

                            //must determine account type
                            long qTAccountNumber = reader.GetInt64(reader.GetOrdinal("AccountNumber"));
                            object qTOwner = GetUserWithAccountNumber(qTAccountNumber);
                            if (qTOwner == null)
                            {
                                qTOwner = GetGuestWithAccountNumber(qTAccountNumber);
                                if (qTOwner != null)
                                {
                                    var guestOwner = qTOwner as Guest;
                                    retQueueTicket.SetOwner(guestOwner);
                                }
                                else
                                {
                                    // must not continue, something wrong with the database, possibly tampered?
                                    throw new InvalidOperationException("Unexpected account type, it is not a user or guest, or it does not exist");
                                }
                            }
                            else
                            {
                                var userOwner = qTOwner as User;
                                retQueueTicket.SetOwner(userOwner);
                            }

                            retQueueTicket.QueueLane.LaneNumber = reader.GetInt32(reader.GetOrdinal("LaneNumber"));
                            retQueueTicket.QueueLane.LaneID = reader.GetInt32(reader.GetOrdinal("LaneID"));
                            retQueueTicket.QueueLane.LaneName = reader["LaneName"].ToString();
                            retQueueTicket.QueueLane.Capacity = reader.GetInt32(reader.GetOrdinal("Capacity"));
                        }
                    }
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return retQueueTicket;
        }

        public List<QueueTicket> GetQueueTicketsOfWithStatusAndLaneID(
            long accountNumber,
            int laneID,
            QueueStatus status,
            Dictionary<QueueStatus, int> queueStatusMapper
            )
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "GetQueueTicketsOfWithStatusAndLaneID";
            List<QueueTicket> retQueueTickets = new List<QueueTicket>();

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("AccountNumber", SqlDbType.BigInt).Value = accountNumber;
                cmd.Parameters.Add("QueueStatusID", SqlDbType.Int).Value = queueStatusMapper[status];
                cmd.Parameters.Add("LaneID", SqlDbType.Int).Value = laneID;

                //start of query
                try
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while(reader.Read())
                        {
                            var queueTicketItem = new QueueTicket();

                            queueTicketItem.QueueID = reader["QueueTicketID"].ToString();
                            queueTicketItem.QueueNumber = reader.GetInt32(reader.GetOrdinal("QueueNumber"));
                            queueTicketItem.PriorityNumber = reader.GetInt16(reader.GetOrdinal("PriorityNumber"));
                            queueTicketItem.QueueDateTime =
                                reader.GetDateTime(reader.GetOrdinal("QueueDate")).Add(
                                    reader.GetTimeSpan(reader.GetOrdinal("QueueTime"))
                                    );
                            queueTicketItem.Status = (QueueStatus)Enum.Parse(
                                typeof(QueueStatus),
                                reader["QueueStatusState"].ToString()
                                );

                            //must determine account type
                            long qTAccountNumber = reader.GetInt64(reader.GetOrdinal("AccountNumber"));
                            object qTOwner = GetUserWithAccountNumber(qTAccountNumber);
                            if (qTOwner == null)
                            {
                                qTOwner = GetGuestWithAccountNumber(qTAccountNumber);
                                if (qTOwner != null)
                                {
                                    var guestOwner = qTOwner as Guest;
                                    queueTicketItem.SetOwner(guestOwner);
                                }
                                else
                                {
                                    // must not continue, something wrong with the database, possibly tampered?
                                    throw new InvalidOperationException("Unexpected account type, it is not a user or guest, or it does not exist");
                                }
                            }
                            else
                            {
                                var userOwner = qTOwner as User;
                                queueTicketItem.SetOwner(userOwner);
                            }

                            queueTicketItem.QueueLane.LaneNumber = reader.GetInt32(reader.GetOrdinal("LaneNumber"));
                            queueTicketItem.QueueLane.LaneID = reader.GetInt32(reader.GetOrdinal("LaneID"));
                            queueTicketItem.QueueLane.LaneName = reader["LaneName"].ToString();
                            queueTicketItem.QueueLane.Capacity = reader.GetInt32(reader.GetOrdinal("Capacity"));

                            retQueueTickets.Add(queueTicketItem);
                        }
                    }
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return retQueueTickets;
        }

        public List<QueueTicket> GetQueueTicketsWithLaneIDAndStatus(
            int laneID,
            QueueStatus status,
            Dictionary<QueueStatus, int> queueStatusMapper
            )
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "GetQueueTicketsWithLaneIDAndStatus";
            List<QueueTicket> retQueueTickets = new List<QueueTicket>();

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("QueueStatusID", SqlDbType.Int).Value = queueStatusMapper[status];
                cmd.Parameters.Add("LaneID", SqlDbType.Int).Value = laneID;

                //start of query
                try
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while(reader.Read())
                        {
                            var queueTicketItem = new QueueTicket();

                            queueTicketItem.QueueID = reader["QueueTicketID"].ToString();
                            queueTicketItem.QueueNumber = reader.GetInt32(reader.GetOrdinal("QueueNumber"));
                            queueTicketItem.PriorityNumber = reader.GetInt16(reader.GetOrdinal("PriorityNumber"));
                            queueTicketItem.QueueDateTime =
                                reader.GetDateTime(reader.GetOrdinal("QueueDate")).Add(
                                    reader.GetTimeSpan(reader.GetOrdinal("QueueTime"))
                                    );
                            queueTicketItem.Status = (QueueStatus)Enum.Parse(
                                typeof(QueueStatus),
                                reader["QueueStatusState"].ToString()
                                );

                            //going to do a second read, initialize a new data access object
                            var dataAccess2 = new DataAccess(this.ConnectionString);

                            //must determine account type
                            long qTAccountNumber = reader.GetInt64(reader.GetOrdinal("AccountNumber"));
                            object qTOwner = dataAccess2.GetUserWithAccountNumber(qTAccountNumber);
                            if (qTOwner == null)
                            {
                                qTOwner = dataAccess2.GetGuestWithAccountNumber(qTAccountNumber);
                                if (qTOwner != null)
                                {
                                    var guestOwner = qTOwner as Guest;
                                    queueTicketItem.SetOwner(guestOwner);
                                }
                                else
                                {
                                    // must not continue, something wrong with the database, possibly tampered?
                                    throw new InvalidOperationException("Unexpected account type, it is not a user or guest, or it does not exist");
                                }
                            }
                            else
                            {
                                var userOwner = qTOwner as User;
                                queueTicketItem.SetOwner(userOwner);
                            }

                            queueTicketItem.QueueLane.LaneNumber = reader.GetInt32(reader.GetOrdinal("LaneNumber"));
                            queueTicketItem.QueueLane.LaneID = reader.GetInt32(reader.GetOrdinal("LaneID"));
                            queueTicketItem.QueueLane.LaneName = reader["LaneName"].ToString();
                            queueTicketItem.QueueLane.Capacity = reader.GetInt32(reader.GetOrdinal("Capacity"));

                            retQueueTickets.Add(queueTicketItem);
                        }
                    }
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return retQueueTickets;
        }

        public List<QueueTicket> GetQueueTicketsWithLaneNumberAndStatus(
            int laneNumber,
            QueueStatus status,
            Dictionary<QueueStatus, int> queueStatusMapper
            )
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "GetQueueTicketsWithLaneNumberAndStatus";
            List<QueueTicket> retQueueTickets = new List<QueueTicket>();

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("QueueStatusID", SqlDbType.Int).Value = queueStatusMapper[status];
                cmd.Parameters.Add("LaneNumber", SqlDbType.Int).Value = laneNumber;

                //start of query
                try
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            var queueTicketItem = new QueueTicket();

                            queueTicketItem.QueueID = reader["QueueTicketID"].ToString();
                            queueTicketItem.QueueNumber = reader.GetInt32(reader.GetOrdinal("QueueNumber"));
                            queueTicketItem.PriorityNumber = reader.GetInt16(reader.GetOrdinal("PriorityNumber"));
                            queueTicketItem.QueueDateTime =
                                reader.GetDateTime(reader.GetOrdinal("QueueDate")).Add(
                                    reader.GetTimeSpan(reader.GetOrdinal("QueueTime"))
                                    );
                            queueTicketItem.Status = (QueueStatus)Enum.Parse(
                                typeof(QueueStatus),
                                reader["QueueStatusState"].ToString()
                                );

                            //must determine account type
                            long qTAccountNumber = reader.GetInt64(reader.GetOrdinal("AccountNumber"));
                            object qTOwner = GetUserWithAccountNumber(qTAccountNumber);
                            if (qTOwner == null)
                            {
                                qTOwner = GetGuestWithAccountNumber(qTAccountNumber);
                                if (qTOwner != null)
                                {
                                    var guestOwner = qTOwner as Guest;
                                    queueTicketItem.SetOwner(guestOwner);
                                }
                                else
                                {
                                    // must not continue, something wrong with the database, possibly tampered?
                                    throw new InvalidOperationException("Unexpected account type, it is not a user or guest, or it does not exist");
                                }
                            }
                            else
                            {
                                var userOwner = qTOwner as User;
                                queueTicketItem.SetOwner(userOwner);
                            }

                            queueTicketItem.QueueLane.LaneNumber = reader.GetInt32(reader.GetOrdinal("LaneNumber"));
                            queueTicketItem.QueueLane.LaneID = reader.GetInt32(reader.GetOrdinal("LaneID"));
                            queueTicketItem.QueueLane.LaneName = reader["LaneName"].ToString();
                            queueTicketItem.QueueLane.Capacity = reader.GetInt32(reader.GetOrdinal("Capacity"));

                            retQueueTickets.Add(queueTicketItem);
                        }
                    }
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return retQueueTickets;
        }

        public User GetUserWithAccountNumber(long accountNumber)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "GetUserWithAccountNumber";
            User retUser = null;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("AccountNumber", SqlDbType.BigInt).Value = accountNumber;

                //start of query
                try
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            retUser = new User();

                            retUser.AccountNumber = reader.GetInt64(reader.GetOrdinal("AccountNumber"));
                            retUser.SetFullName(
                                reader["FirstName"].ToString(),
                                SafeGetString(reader, reader.GetOrdinal("MiddleName")),
                                reader["LastName"].ToString()
                                );
                            retUser.SetEmail(
                                reader["Email"].ToString()
                                );
                            retUser.SetPassword(
                                reader["Password"].ToString()
                                );
                            retUser.SetContactNumber(
                                SafeGetString(reader, reader.GetOrdinal("ContactNumber"))
                                );
                            retUser.UserID = reader["UserID"].ToString();
                        }
                    }
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return retUser;
        }

        public User GetUserWithEmail(string email)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "GetUserWithEmail";
            User retUser = null;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("Email", SqlDbType.VarChar, 50).Value = email;

                //start of query
                try
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            retUser = new User();

                            retUser.AccountNumber = reader.GetInt64(reader.GetOrdinal("AccountNumber"));
                            retUser.SetFullName(
                                reader["FirstName"].ToString(),
                                SafeGetString(reader, reader.GetOrdinal("MiddleName")),
                                reader["LastName"].ToString()
                                );
                            retUser.SetEmail(
                                reader["Email"].ToString()
                                );
                            retUser.SetPassword(
                                reader["Password"].ToString()
                                );
                            retUser.SetContactNumber(
                                SafeGetString(reader, reader.GetOrdinal("ContactNumber"))
                                );
                            retUser.UserID = reader["UserID"].ToString();
                        }
                    }
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return retUser;
        }

        public User GetUserWithUserID(string userID)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "GetUserWithUserID";
            User retUser = null;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("UserID", SqlDbType.NChar, 12).Value = userID;

                //start of query
                try
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            retUser = new User();

                            retUser.AccountNumber = reader.GetInt64(reader.GetOrdinal("AccountNumber"));
                            retUser.SetFullName(
                                reader["FirstName"].ToString(),
                                SafeGetString(reader, reader.GetOrdinal("MiddleName")),
                                reader["LastName"].ToString()
                                );
                            retUser.SetEmail(
                                reader["Email"].ToString()
                                );
                            retUser.SetPassword(
                                reader["Password"].ToString()
                                );
                            retUser.SetContactNumber(
                                SafeGetString(reader, reader.GetOrdinal("ContactNumber"))
                                );
                            retUser.UserID = reader["UserID"].ToString();
                        }
                    }
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return retUser;
        }
        public bool LaneNumberExists(int laneNumber)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "LaneNumberExists";
            bool laneNumberExists = false;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                cmd.Parameters.Add("LaneNumber", SqlDbType.Int).Value = laneNumber;

                //start of query
                try
                {
                    //in stored procedure, it returns 1 if found
                    laneNumberExists = (int)cmd.ExecuteScalar() == 1;
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return laneNumberExists;
        }

        /// <summary>
        /// for testing purposes, DO NOT USE OUTSIDE TESTING, resets database to default initial state
        /// </summary>
        public void ResetDatabase()
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "ResetDatabase";

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                //start of query
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }
        }

        /// <summary>
        /// Get the current seed for the guest number
        /// </summary>
        /// <returns></returns>
        public int GetGuestNumberSeed()
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string procName = "GetGuestNumberSeed";
            int guestNumberSeed = 0;

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                //Doesn't matter what the name of the return value is
                var returnParameter = cmd.Parameters.Add("ReturnValue", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;

                //start of query
                try
                {
                    cmd.ExecuteNonQuery();
                    guestNumberSeed = (int)returnParameter.Value;
                }
                catch (SqlException exc)
                {
                    Console.WriteLine("SQL Exception encountered: " + exc.Message);
                }
                //end of query
                connection.Close();
            }

            return guestNumberSeed;
        }

        private static string SafeGetString(SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetString(colIndex);
            }
            else
            {
                return string.Empty;
            }
        }

        private static int SafeGetInt32(SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetInt32(colIndex);
            }
            else
            {
                return -1;
            }
        }

        private static object NormalizeAccountNullValuesToDBNull(object account)
        {
            // only nullable values are contact number and middle name

            if (account is User user)
            {
                if (user.GetContactNumber() == string.Empty)
                {
                    user.SetContactNumber(null);
                }
                if (user.GetMiddleName() == string.Empty)
                {
                    user.SetFullName(
                        user.GetFirstName(),
                        null,
                        user.GetLastName()
                        );
                }
                return user;
            }
            else if (account is Admin admin)
            {
                if (admin.GetContactNumber() == string.Empty)
                {
                    admin.SetContactNumber(null);
                }
                if (admin.GetMiddleName() == string.Empty)
                {
                    admin.SetFullName(
                        admin.GetFirstName(),
                        null,
                        admin.GetLastName()
                        );
                }
                return admin;
            }
            else if (account is QueueAttendant attendant)
            {
                //designated lane can be null
                if (attendant.GetContactNumber() == string.Empty)
                {
                    attendant.SetContactNumber(null);
                }
                if (attendant.GetMiddleName() == string.Empty)
                {
                    attendant.SetFullName(
                        attendant.GetFirstName(),
                        null,
                        attendant.GetLastName()
                        );
                }
                
                return attendant;
            }
            else
            {
                //unexpected, do not proceed
                throw new ArgumentException("Given object instance is not a valid account", "account");
            }
        }
    }
}
