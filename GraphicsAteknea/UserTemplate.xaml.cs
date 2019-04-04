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
using System.ComponentModel;
using System.Windows.Threading;
using System.Reflection;
using System.IO;

namespace GraphicsAteknea
{
    /// <summary>
    /// Lógica de interacción para UserTemplate.xaml
    /// </summary>
    public partial class UserTemplate : UserControl
    {
        public UserTemplate()
        {
            InitializeComponent();
        }
    }
    /// <summary>
    /// This is the view model and the class who notify property changes on UI controls
    /// </summary>
    public class UserTemplateViewModel : INotifyPropertyChanged
    {
        private string _name;
        private string _lastName;
        private string _mail;
        private bool _enableStat;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyChange(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyChange("Name");
            }
        }
        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                NotifyChange("LastName");
            }
        }
        public string Mail
        {
            get { return _mail; }
            set
            {
                _mail = value;
                NotifyChange("Mail");
            }
        }
        public bool EnableStat
        {
            get { return _enableStat; }
            set
            {
                _enableStat = value;
                NotifyChange("EnableStat");
            }
        }
    }
    /// <summary>
    /// This is a container clas who invokes the ViewModel once an event have to be notified to the GUI
    /// </summary>
    public class UserTemplateContainerClass
    {
        private string _name;
        private string _lastName;
        private string _mail;
        private bool _enableStat;
        private ImageResourceManager _resManager;

        public UserTemplateViewModel ViewBinding { get; set; }

        public UserTemplateContainerClass(string name, string lstname, string mail, bool enab)
        {
            ViewBinding = new UserTemplateViewModel();
            _resManager = new ImageResourceManager();
            Name = name;
            LastName = lstname;
            Mail = mail;
            EnableStat = enab;
        }
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() =>
                {
                    ViewBinding.Name = value;
                }));
            }
        }
        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() =>
                {
                    ViewBinding.LastName = value;
                }));
            }
        }
        public string Mail
        {
            get { return _mail; }
            set
            {
                _mail = value;
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() =>
                {
                    ViewBinding.Mail = value;
                }));
            }
        }
        public bool EnableStat
        {
            get { return _enableStat; }
            set
            {
                _enableStat = value;
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() =>
                {
                    ViewBinding.EnableStat = value;
                }));
            }
        }
    }
}
