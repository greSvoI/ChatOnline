using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChatOnline
{
    public class ViewApplicationPrivate : INotifyPropertyChanged
    {
        public string Name;
        public string privateName;
        public string msg;
        public User user;
        public NetworkStream stream;

        ObservableCollection<string> listMessage;
        public  ObservableCollection<string> ListMmessage { get => listMessage; set { listMessage = value; OnPropertyChanged(""); } }
        public ViewApplicationPrivate()
        {
            user = new User();
            listMessage = new ObservableCollection<string>();
        }
        public void Send()
        {
            user.Name = Name;
            user.NamePrivate = privateName;
            user.Message = msg;
            user.ConnectPrivate = true;
            byte[] data = user.Serialize();
            stream.Write(data, 0, data.Length);
        }




        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop= "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
         
    }
}
