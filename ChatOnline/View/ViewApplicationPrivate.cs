using ChatOnline.Command;
using ChatOnline.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatOnline
{
    public class ViewApplicationPrivate : INotifyPropertyChanged
    {
        
        public string Name;
        public string privateName;
        public string msg;
        public User user;
        public NetworkStream stream;
        public NetworkStream streamSharing;
        private string message;
        public string selectMessage { get => message; set { message = value; OnPropertyChanged(""); } }
        public string Msg { get => msg; set { msg = value; OnPropertyChanged(""); } }

        ObservableCollection<string> listMessage;
        public  ObservableCollection<string> ListMmessage { get => listMessage; set { listMessage = value; OnPropertyChanged(""); } }
        public ViewApplicationPrivate()
        {
            user = new User();
            listMessage = new ObservableCollection<string>();
        }
        public ICommand Load => new DelegateCommand(()=>LoadFile());
        public ICommand SendMsg => new DelegateCommand(() => Send());
        private void Send()
        {
            user.Name = Name;
            user.NamePrivate = privateName;
            user.Message = msg;
            user.ConnectPrivate = true;
            byte[] data = user.Serialize();
            stream.Write(data, 0, data.Length);
            msg = "";
        }
        private void LoadFile()
        {
            InfoFile info = new InfoFile();
            info.FileName = selectMessage;
            if (string.IsNullOrEmpty(info.FileName))
                return;
            byte[] data = info.Serialize();
            streamSharing.Write(data, 0, data.Length);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop= "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
         
    }
}
