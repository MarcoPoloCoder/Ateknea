using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GraphicsAteknea
{
    /// <summary>
    /// Lógica de interacción para DataBaseConnectionControl.xaml
    /// </summary>
    public partial class DataBaseConnectionControl : UserControl
    {
        public string UserText { get { return UserName.Text;} }
        public string PasswordText { get { return Password.Text; } }
        public string PortText { get { return Password.Text; } }

        //event from Refresh Button
        public static readonly RoutedEvent ConnectDdEv = EventManager.RegisterRoutedEvent("ConnectDb",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ListOfUsersControl));

        //eventhandler from Send Button
        public event RoutedEventHandler ConnectDb
        {
            add { AddHandler(ConnectDdEv, value); }
            remove { RemoveHandler(ConnectDdEv, value); }
        }

        public DataBaseConnectionControl()
        {
            InitializeComponent();
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ConnectDdEv, this));
        }
    }
}
