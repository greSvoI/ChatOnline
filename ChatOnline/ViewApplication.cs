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
        static Random random = new Random();

        public User user = new User();
        User temp = new User();
        public Dispatcher MyDispatcher { get; set; }

        private ObservableCollection<User> msgBox;
        private ObservableCollection<string> userBox;
        public ObservableCollection<User> MsgBox
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


        public static TcpClient client;
        public static NetworkStream stream;

        public ViewApplication()
        {
            user.Name = random.Next(100).ToString();
            msgBox = new ObservableCollection<User>();
            userBox = new ObservableCollection<string>();
            client = new TcpClient();
            this.MyDispatcher = Dispatcher.CurrentDispatcher;
            Connect();
        }
        private void Connect()
        {
            client.Connect("127.0.0.1",8000);
            stream = client.GetStream();

            byte[] data = Encoding.Unicode.GetBytes(user.Name);
            stream.Write(data, 0, data.Length);
            UserBox.Add(user.Name);


            Thread receiveThread = new Thread(new ThreadStart(ReceiveMsg));
            receiveThread.Start();

        }

        private void ReceiveMsg()
        {
            try
            {
                byte[] id = new byte[64];
                stream.Read(id,0,id.Length);
                user.ID = Encoding.Unicode.GetString(id, 0, id.Length).Trim('\0');


                while (true)
                {
                    byte[] data = new byte[1024];
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        temp.Desserialize(data);

                    } while (stream.DataAvailable);




                    if (temp.SendMessage)
                    {
                      MyDispatcher.Invoke(() =>
                      {
                            MsgBox.Add(temp);
                      });
                    }
                   
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
            if (!string.IsNullOrEmpty(user.Message))
            {
                byte[] data = user.Serialize();
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
