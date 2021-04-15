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

        private ObservableCollection<User> msgBox;
        private ObservableCollection<string> userBox;
        protected internal Client client;
        protected internal List<PrivatChat> PrivatChats = new List<PrivatChat>();
        protected internal Dispatcher MyDispatcher { get; set; }
       
        public string UserName { get => name; set { name = value; OnPropertyChanged(""); } }
        public string SelectUser { get => selectName; set { selectName = value; OnPropertyChanged(""); } }
        public ObservableCollection<User> MsgBox { get => msgBox; set { msgBox = value; OnPropertyChanged(""); } }
        public ObservableCollection<string> UserBox { get => userBox; set { userBox = value; OnPropertyChanged(""); } }
        public ViewApplication(TextBox box)
        {
            
            client = new Client(this,box);
            msgBox = new ObservableCollection<User>();
            userBox = new ObservableCollection<string>();
            this.MyDispatcher = Dispatcher.CurrentDispatcher;
        }
        public ICommand Send => new DelegateCommand(() => client.SendMsg());
        public ICommand Accept => new DelegateCommand(() => client.AcceptFile());
        public ICommand PrivateMsg => new DelegateCommand(() => client.PrivateTo());
        public ICommand ConnectTo => new DelegateCommand(()=>client.Connect());
        public ICommand SendToAll => new DelegateCommand(()=> client.SendFileEveryone());
        public ICommand SendOne => new DelegateCommand(()=>client.SendToOne());

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        
    }
    
}
