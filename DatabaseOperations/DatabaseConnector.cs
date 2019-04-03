using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseOperations
{

    public class DatabaseConnector
    {
        #region Variables_Declation
        private string _port;
        private string _userName;
        private string _pswrd;
        private MySqlConnection _connection;
        // Protected query strngs
        protected const string ReadAllUsersQuery = "SELECT * FROM UsersTable;";
        protected const string DeleteTableQuery = "DROP TABLE IF EXISTS UsersTable;";
        protected const string CreateTableQuery = "CREATE TABLE UsersTable(Id INT(11) AUTO_INCREMENT, Name VARCHAR(20) DEFAULT '', LastName VARCHAR(40) DEFAULT '', Mail VARCHAR(20) DEFAULT '', Enabled VARCHAR(20) DEFAULT '', PRIMARY KEY (Id)) ENGINE = InnoDB;";
        protected StringBuilder AddRowsQuery = new StringBuilder("INSERT INTO UsersTable(Name, LastName, Mail, Enabled) VALUES ");

        public bool ConnectionStatus { get; set; }
        #endregion
        #region Constructor_and_PublicAccessors

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="port"></param>
        /// <param name="usr"></param>
        /// <param name="pswrd"></param>
        public DatabaseConnector(string port, string usr, string pswrd)
        {
            _port = port;
            _userName = usr;
            _pswrd = pswrd;
            ConnectionStatus = false;
        }

        /// <summary>
        /// Stabilshing connection method
        /// </summary>
        public void ConnectSincronously()
        {
            try
            {
                string connectionStr = "Server=localhost;Port=" + _port + ";Uid=" + _userName + ";Pwd=" + _pswrd + ";Database=usersateknea";
                _connection = new MySqlConnection(connectionStr);
                _connection.Open();
                if (_connection.Container == null)
                {
                    throw (new Exception("Error while retrieving connection information...\n\r Write a correct USERNAME/PASSWORD and check the server status."));
                }
                ConnectionStatus = true;
                _connection.Close();
            }
            catch (Exception Ex)
            {
                throw (Ex);
            }
        }
        /// <summary>
        /// Delete users Table query
        /// </summary>
        public void DeleteUsersTable()
        {
            try
            {
                _connection.Open();
                var cmd = new MySqlCommand(DeleteTableQuery, _connection);
                var rows = cmd.ExecuteNonQuery();
                _connection.Close();
                if (rows < 1) { throw (new Exception("Error while trying to remove unexistent table...\n\r UsersTable was already removed")); }
            }
            catch (Exception Ex)
            {
                throw (Ex);
            }
        }
        /// <summary>
        /// Create UsersTable query
        /// </summary>
        public void CreateUsersTable()
        {
            try
            {
                _connection.Open();
                var cmd = new MySqlCommand(CreateTableQuery, _connection);
                var rows = cmd.ExecuteNonQuery();
                _connection.Close();
            }
            catch (Exception Ex)
            {
                throw (Ex);
            }
        }
        /// <summary>
        /// Insert data to UsersTable
        /// </summary>
        /// <param name="usrList"></param>
        public void AddRowsUsersTable(List<DbUser> usrList)
        {
            List<string> Rows = new List<string>();
            foreach(var usr in usrList)
            {
                Rows.Add(string.Format("('{0}','{1}','{2}','{3}')", MySqlHelper.EscapeString(usr.Name), MySqlHelper.EscapeString(usr.LastName), MySqlHelper.EscapeString(usr.Mail), MySqlHelper.EscapeString(usr.Enabled)));
            }
            AddRowsQuery.Append(string.Join(",", Rows));
            AddRowsQuery.Append(";");
            _connection.Open();
            using (MySqlCommand myCmd = new MySqlCommand(AddRowsQuery.ToString(), _connection))
            {
                myCmd.CommandType = CommandType.Text;
                myCmd.ExecuteNonQuery();
            }
            _connection.Close();
        }
     /// <summary>
     /// Read all rows on UsersTable
     /// </summary>
     /// <returns></returns>
        public List<DbUser> ReadUsersTable()
        {
            try
            {
                var usrList = new List<DbUser>();
                var cmd = new MySqlCommand(ReadAllUsersQuery, _connection);
                var reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                    throw (new Exception("UsersTable not found on the DB"));
                while (reader.Read())
                {
                    usrList.Add(new DbUser() {Name = reader.GetString("Name"), LastName = reader.GetString("LastName"),
                        Mail = reader.GetString("Mail"), Enabled = reader.GetString("Enabled") });  
                }
                reader.Close();
                return usrList;
            }
            catch (Exception Ex)
            {
                throw (Ex);
            }
            #endregion
        }
    }
    /// <summary>
    /// Class to interact with UsersTable
    /// </summary>
    public class DbUser
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Mail { get; set; }
        public string Enabled { get; set; }
    }
}
