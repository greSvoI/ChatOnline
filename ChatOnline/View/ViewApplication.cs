using ChatOnline.Command;
using System;
using ChatOnline;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ChatOnline.Model;
using System.Windows.Controls;

namespace ChatOnline
{
    public class ViewApplication : INotifyPropertyChanged
    {
        string name;
        string selectName;
       
        
        private ObservableCollection<Object> msgBox;
        private ObservableCollection<string> userBox;
        protected internal Client client;
        protected internal List<PrivatChat> PrivatChats = new List<PrivatChat>();
        protected internal Dispatcher MyDispatcher { get; set; }
        public string UserName { get => name; set { name = value; OnPropertyChanged(""); } }
        public string SelectUser { get => selectName; set { selectName = value; OnPropertyChanged(""); } }
        private User user = new User();
        public User GetUser { get => user; set { user = value;OnPropertyChanged(""); } }
        public ObservableCollection<Object> MsgBox { get => msgBox; set { msgBox = value; OnPropertyChanged(""); } }
        public ObservableCollection<string> UserBox { get => userBox; set { userBox = value; OnPropertyChanged(""); } }

        public ViewApplication(TextBox box)
        {
            
            client = new Client(this,box);
            msgBox = new ObservableCollection<Object>();
            userBox = new ObservableCollection<string>();
            this.MyDispatcher = Dispatcher.CurrentDispatcher;
        }
        public ICommand Send => new DelegateCommand(() => client.SendMsg());
        public ICommand PrivateMsg => new DelegateCommand(() => client.PrivateTo());
        public ICommand ConnectTo => new DelegateCommand(()=>client.Connect());
        public ICommand SendToAll => new DelegateCommand(()=> client.SendFile(false));
        public ICommand SendOne => new DelegateCommand(()=>client.SendFile(true));
        
        public ICommand ClickLoad => new DelegateCommand(()=>client.LoadFile());

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        
    }
    
}
