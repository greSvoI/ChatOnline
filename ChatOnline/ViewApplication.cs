using ChatOnline.Command;
using System;
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
using System.Windows.Threading;

namespace ChatOnline
{
    class ViewApplication : INotifyPropertyChanged
    {
        static string Name = "User";
        string msg;
        public Dispatcher Dispatcher { get; set; }

        private ObservableCollection<string> msgBox;
        private ObservableCollection<string> userBox;
        public ObservableCollection<string> MsgBox
        { 
            get => msgBox; 
            set
            {
                msgBox = value; OnPropertyChanged("");
            }
        }
        public ObservableCollection<string> UserBox
        {
            get => userBox;
            set
            {
                userBox = value; OnPropertyChanged("");
            }
        }


        static TcpClient client;
        static NetworkStream stream;

        public ViewApplication()
        {
            msgBox = new ObservableCollection<string>();
            client = new TcpClient();
            this.Dispatcher = Dispatcher.CurrentDispatcher;
            Connect();
        }
        private void Connect()
        {
            client.Connect("127.0.0.1",8000);
            stream = client.GetStream();

            byte[] data = Encoding.Unicode.GetBytes(Name);
            stream.Write(data, 0, data.Length);

            Thread receiveThread = new Thread(new ThreadStart(ReceiveMsg));
            receiveThread.Start();

        }

        private void ReceiveMsg()
        {
            try
            {
                while (true)
                {
                    byte[] data = new byte[64];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));

                    } while (stream.DataAvailable);

                    Dispatcher.CurrentDispatcher.Invoke(() => { MsgBox.Add(builder.ToString()); });
                   
                }
                
            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message + "ReceiveMsg()");
            }
        }

        public ICommand Send => new DelegateCommand(() => SendMsg());
        public void SendMsg()
        {
            if (msg != "")
            {
                byte[] data = Encoding.Unicode.GetBytes(msg);
                stream.Write(data, 0, data.Length);
            }

        }



        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
