using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GraphicsAteknea
{
    /// <summary>
    /// Interaction logic for ListOfUsersControl.xaml
    /// </summary>
    public partial class ListOfUsersControl : UserControl
    {
        #region Variables_declaration

        private List<UserTemplateContainerClass> _usersList;

        public List<UserTemplateContainerClass> UsersDataList {get{ return _usersList; }}

        //event from Send Button
        public static readonly RoutedEvent SendToDdEv = EventManager.RegisterRoutedEvent("SendToDdEv", 
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ListOfUsersControl));

        //event from Refresh Button
        public static readonly RoutedEvent GetFromDdEv = EventManager.RegisterRoutedEvent("SendToDd", 
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ListOfUsersControl));

        //eventhandler from Send Button
        public event RoutedEventHandler SendToDd
        {
            add { AddHandler(SendToDdEv, value); }
            remove { RemoveHandler(SendToDdEv, value); }
        }

        //eventhandler from Refresh Button
        public event RoutedEventHandler GetFromDd
        {
            add { AddHandler(GetFromDdEv, value); }
            remove { RemoveHandler(GetFromDdEv, value); }
        }

        #endregion

        #region PublicAccessors_constructor_and_init

        /// <summary>
        /// Class constructor
        /// </summary>
        public ListOfUsersControl()
        {
            InitializeComponent();
            _usersList = new List<UserTemplateContainerClass>();
        }

        /// <summary>
        /// PublicAccessor to add items, notice that it doesn't have any problem tu update the graphics from every thread running on the application,
        /// It can be done by delegating the INotifyChange to the main aplication dispatcher on an asyncronous way.
        /// </summary>
        /// <param name="usersInitLst"></param>
        public void AddSingleUser(string name, string lastName, string mail, bool enable)
        {
            _usersList.Add(new UserTemplateContainerClass(name, lastName, mail, enable));
            var container = _usersList[_usersList.Count - 1];
            UsersList.Items.Add(new UserTemplate());
            // Then I apply my custom binding logic for that 'Page' (because this control acts like an intermediate page)
            BindControl(container.ViewBinding, (UserTemplate)UsersList.Items[UsersList.Items.Count - 1]);
        }
        public void DeleteAllUsers()
        {
            UsersList.Items.Clear();
            _usersList.Clear();
        }
        #endregion

        #region LocalEvents_to_update_ChildControls_and_its_ViewModels

        /// <summary>
        /// Add user to listbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            //Here I'm adding a new control and its asociated container to the parent control
            //It could be more encapsulated on a method but there are few lines
            _usersList.Add(new UserTemplateContainerClass("", "", "", false));
            var container = _usersList[_usersList.Count - 1];
            UsersList.Items.Add(new UserTemplate());
            // Then I apply my custom binding logic for that 'Page' (because this control acts like an intermediate page)
            BindControl(container.ViewBinding, (UserTemplate)UsersList.Items[UsersList.Items.Count - 1]);
        }

        /// <summary>
        /// Delete selected users from listbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // A little bit of linq to get the selected indexes on the list
            var selectedItemIndexes = (from object o in UsersList.SelectedItems select UsersList.Items.IndexOf(o)).ToList();

            // And here I remove the control an its associated container
            foreach(var item in selectedItemIndexes)
            {
                _usersList.RemoveAt(item);
                UsersList.Items.RemoveAt(item);
            }

        }
        #endregion

        #region RoutedEvents_to_parent_for_graphics_decoupling

        /// <summary>
        /// this click event is raised to the MainWin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(SendToDdEv, this));
        }
        /// <summary>
        /// this click event is raised to the MainWin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(GetFromDdEv, this));
        }

        #endregion

        #region PageBindingLogic_for_child_UserControl

        /// <summary>
        /// It is on e of the KEY methods for the architecture, it is a dynamic connector between a container object on the ParentControl
        /// and all the desired bindable properties from the child control. In this case as we want to actualize variables through the view 
        /// in a real-time scenario the binding mode should be OneWayToSource
        /// </summary>
        /// <param name="model"></param>
        /// <param name="view"></param>
        private void BindControl(UserTemplateViewModel model, UserTemplate view)
        {
            Binding b = new Binding
            {
                Source = model,
                Path = new PropertyPath("Name"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                IsAsync = true,
                Mode = BindingMode.OneWayToSource
            };
            BindingOperations.SetBinding(view.NameTxt, TextBox.TextProperty, b);

            b = new Binding
            {
                Source = model,
                Path = new PropertyPath("LastName"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                IsAsync = true,
                Mode = BindingMode.OneWayToSource
            };
            BindingOperations.SetBinding(view.LstNameTxt, TextBox.TextProperty, b);

            b = new Binding
            {
                Source = model,
                Path = new PropertyPath("Mail"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                IsAsync = true,
                Mode = BindingMode.OneWayToSource
            };
            BindingOperations.SetBinding(view.MailTxt,
                TextBox.TextProperty, b);

            b = new Binding
            {
                Source = model,
                Path = new PropertyPath("EnableStat"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                IsAsync = true,
                Mode = BindingMode.OneWayToSource
            };
            BindingOperations.SetBinding(view.EnabledBox, CheckBox.IsCheckedProperty, b);
        }
        #endregion

    }
}
