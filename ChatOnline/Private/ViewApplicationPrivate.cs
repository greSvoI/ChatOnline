using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChatOnline.PrivateChat
{
    public class ViewApplicationPrivate : INotifyPropertyChanged
    {
        ObservableCollection<string> listMessage;
        public  ObservableCollection<string> ListMmessage { get => listMessage; set { listMessage = value; OnPropertyChanged(""); } }

        public ViewApplicationPrivate()
        {
            listMessage = new ObservableCollection<string>();
        }





        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop= "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
         
    }
}
