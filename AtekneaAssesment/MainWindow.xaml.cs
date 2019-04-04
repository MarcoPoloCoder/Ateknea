using System.Windows;
using GraphicsAteknea;
using System;
using DatabaseOperations;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AtekneaAssesment
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DatabaseConnector _dbManager;
        private List<DbUser> _dbUserList;

        // I haven't got enough time to perform this actions with a TaskScheduler
        private Task _dbTasks;
        private bool _activeDbTask = false;

        /// <summary>
        /// Class constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            listUsersControl.AddHandler(ListOfUsersControl.SendToDdEv, new RoutedEventHandler(RefreshDbTable));
            listUsersControl.AddHandler(ListOfUsersControl.GetFromDdEv, new RoutedEventHandler(RefreshUsersList));
            dbControl.AddHandler(DataBaseConnectionControl.ConnectDdEv, new RoutedEventHandler(ConnectToDb));
            _dbUserList = new List<DbUser>();
        }

        /// <summary>
        /// Connect to DB routed event from child control DatabaseConnectionControl
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectToDb(object sender, RoutedEventArgs e)
        {
            try
            {
                _dbTasks = new Task(() => {
                    _activeDbTask = true;
                    _dbManager = new DatabaseConnector(dbControl.PortText, dbControl.UserText, dbControl.PasswordText);
                    _dbManager.ConnectSincronously();
                    _activeDbTask = false;
                });
                _dbTasks.Start();
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Failed connecting to database due to: \n\r" + Ex.Message, "ERROR INFO");
            }
        }
        /// <summary>
        /// Get UsersTable row routed event from child control ListOfUsersControl
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshDbTable(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check that the introduced data is valid and remaps it for the DB, I know it could be done in a more elegant way but I had no time in two afternoons
                if (!ValidateData(listUsersControl.UsersDataList)) throw (new Exception("There are some users with incorrect parameters..."));
                // Check the database connection
                if (_dbManager == null||_dbManager.ConnectionStatus == false) throw (new Exception("There is no DB connection"));

                if (_activeDbTask) throw (new Exception("There is an active task with the database."));

                _dbTasks = new Task(() =>
                {
                    _activeDbTask = true;
                    _dbManager.DeleteUsersTable();
                    _dbManager.CreateUsersTable();
                    _dbManager.AddRowsUsersTable(_dbUserList);
                    _activeDbTask = false;
                });
                _dbTasks.Start();
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Failed updating database due to: \n\r" + Ex.Message, "ERROR INFO", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Method to validate Data Input requirements and remap view to DbUser class
        /// </summary>
        /// <param name="Users"></param>
        /// <returns></returns>
        private bool ValidateData(List<UserTemplateContainerClass> Users)
        {
            _dbUserList = new List<DbUser>();
            Regex Alphanumeric = new Regex(@"^[a-zA-Z0-9\s,]*$");
            List<int> evaluations = new List<int>();

            //Those are all the string validations when trying to upload a user (optimized with Linq)
            evaluations.Add(Users.IndexOf(Users.Find(x => x.Name.Length > 20)));
            evaluations.Add(Users.IndexOf(Users.Find(x => !Alphanumeric.IsMatch(x.Name))));
            evaluations.Add(Users.IndexOf(Users.Find(x => x.LastName.Length > 40)));
            evaluations.Add(Users.IndexOf(Users.Find(x => x.LastName.StartsWith(" "))));
            evaluations.Add(Users.IndexOf(Users.Find(x => x.LastName.EndsWith(" "))));
            evaluations.Add(Users.IndexOf(Users.Find(x => !Alphanumeric.IsMatch(x.LastName))));
            evaluations.Add(Users.IndexOf(Users.Find(x => x.Mail.Length > 20)));
            evaluations.Add(Users.IndexOf(Users.Find(x => x.Mail.Contains("@"))));

            // Then we remap the values on the GUI to our private variable passed to the DB
            foreach(var user in Users)
            {
                _dbUserList.Add(new DbUser() { Name = user.Name, LastName = user.LastName, Mail = user.Mail, Enabled = user.EnableStat ? "1" : "0" });
            }
            // If any of our string evaluations has an index different to -1, it means an error ocurred introducing data
            return !evaluations.Exists(x=>x!=-1);
        }

        /// <summary>
        ///  Update UsersTable with view info routed event from child control ListOfUsersControl
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshUsersList(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_dbManager == null || _dbManager.ConnectionStatus == false) throw (new Exception("There is no DB connection"));
                if (_activeDbTask) throw (new Exception("There is an active task with the database."));
                List<DbUser> usrLst= new List<DbUser>();
                _dbTasks = new Task(() =>
                {
                    _activeDbTask = true;
                    usrLst = _dbManager.ReadUsersTable();
                    listUsersControl.DeleteAllUsers();
                    _activeDbTask = false;
                });
                _dbTasks.Start();

                _dbTasks.Wait();

                foreach(var user in usrLst)
                {
                    listUsersControl.AddSingleUser(user.Name, user.LastName, user.Mail, user.Enabled.Contains("1"));
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Failed updating database due to: \n\r" + Ex.Message, "ERROR INFO", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
