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
using System.Windows.Media;
using System.Windows.Threading;

namespace ChatOnline
{
    class ViewApplication : INotifyPropertyChanged
    {
        static Random random = new Random();
        string name;
        string selectName;
        public string UserName { get => name; set { name = value; OnPropertyChanged(""); } }
        public string SelectUser { set { selectName = value; OnPropertyChanged(""); } }
        private User user;
        public User User { get => user; set { user = value; } }

        User temp = new User();
        public Dispatcher MyDispatcher { get; set; }
       // public Brush MyBrush { get => User.Brush; set { User.Brush = value; OnPropertyChanged(""); } }

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
            user = new User();
            User.Brush = new SolidColorBrush(Colors.LightCoral);
            msgBox = new ObservableCollection<User>();
            userBox = new ObservableCollection<string>();
            client = new TcpClient();
            this.MyDispatcher = Dispatcher.CurrentDispatcher;
            //Connect();
        }
        public ICommand Private => new DelegateCommand(() => PrivateTo());
        public ICommand ConnectTo => new DelegateCommand(()=>Connect());
        private void Connect()
        {
            if (!string.IsNullOrEmpty(UserName))
                User.Name = UserName;
            else return;

            if (!client.Connected)
            {
                client.Connect("127.0.0.1", 8000);
                stream = client.GetStream();
            }


            //byte[] data = new byte[64];
            //data = Encoding.Unicode.GetBytes(User.Name);
            //stream.Write(data, 0, data.Length);//Отправляем имя

            Thread receiveThread = new Thread(new ThreadStart(ReceiveMsg));
            receiveThread.Start();

        }

        private void PrivateTo()
        {
            if (string.IsNullOrEmpty(selectName)) return;

        }
        private void ReceiveMsg()
        {
            try
            {

                byte[] data;
                while (true)
                {
                    data = new byte[64];
                    data = Encoding.Unicode.GetBytes(User.Name);
                    stream.Write(data, 0, data.Length);//Отправляем имя
                    data = new byte[64];
                    stream.Read(data, 0, data.Length);
                    if (data[0] == 0)
                    {
                        stream.Flush();
                        MessageBox.Show("Логин занят!");
                        return;
                    }
                    else 
                        break;
                }

                data = new byte[1024];

                stream.Read(data, 0, data.Length);//Активных клиентов

                string temps = Encoding.Unicode.GetString(data, 0, data.Length);
                string []client = temps.Split(',');

                

                this.User.ID = client[0];

                for (int i = 1; i < client.Length; i++)
                {
                    client[i].Trim('\0');
                    if(!client[i].StartsWith("*"))
                        MyDispatcher.Invoke(() =>
                        {
                            UserBox.Add(client[i]);
                        });
                }




                while (true)
                {
                    data = new byte[1024];
                    int bytes = 0;//Пока не удалять будет нужен
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        temp.Desserialize(data);

                    } while (stream.DataAvailable);

                   

                    if (!UserBox.Any(x=>x == temp.Name))//Другой вариант temp.Connect==true ? но пока так
                    {
                        MyDispatcher.Invoke(() =>
                        {
                            UserBox.Add(temp.Name);
                        });
                        temp.ConnectClient = false;
                    }

                    if(temp.DisconnectClient)
                    {
                        MyDispatcher.Invoke(() =>
                        {
                            //temp.Message = "Bye";
                            MsgBox.Add(temp);
                            UserBox.Remove(temp.Name);
                           
                        });
                    }

                    if (temp.ID != User.ID && !string.IsNullOrEmpty(temp.Message))
                    {
                      MyDispatcher.Invoke(() =>
                      {
                          temp.Brush = new SolidColorBrush(Colors.LightBlue);
                          MsgBox.Add(temp);
                      });
                    }
                    else if(temp.ID == User.ID && !string.IsNullOrEmpty(temp.Message))
                    {
                        MyDispatcher.Invoke(() =>
                        {
                            User.Message = temp.Message;
                            MsgBox.Add(User);
                        });
                    }
                   
                }
                
            }
            catch (Exception e)
            {
                Close();
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
        public void Close()
        {
            stream?.Close();
            client?.Close();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        
    }
    
}
