using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ChatOnline.Model
{
     public class Client
    {
        static TcpClient tcpclient;
        static NetworkStream stream;
        readonly string ip = "127.0.0.1";
        readonly int port = 8000;
        ViewApplication view;
        public User User { get; set; }

        public Client(ViewApplication view)
        {
            this.view = view;
            this.User = new User();
            this.User.Brush = new SolidColorBrush(Colors.LightCoral);
            tcpclient = new TcpClient();
        }
        protected internal void Connect()
        {
            if (!string.IsNullOrEmpty(view.UserName))
                User.Name = view.UserName;
            else return;

            if (!tcpclient.Connected)
            {
                tcpclient.Connect(ip, port);
                stream = tcpclient.GetStream();
            }
            Thread receiveThread = new Thread(new ThreadStart(ReceiveMsg));
            receiveThread.Start();
        }
        protected internal void PrivateTo()
        {
            if (string.IsNullOrEmpty(view.SelectUser))
                return;

            User.NamePrivate = view.SelectUser;
            User.ConnectPrivate = true;

            byte[] data = User.Serialize();
            stream.Write(data, 0, data.Length);
            User.NamePrivate = "";
            User.ConnectPrivate = false;
        }
        protected internal void SendFileEveryone()
        {

        }
        protected internal void SendToOne()
        {

        }
        protected internal void AcceptFile()
        {

        }
        public void SendMsg()
        {
            if (!string.IsNullOrEmpty(User.Message))
            {
                byte[] data = User.Serialize();
                stream.Write(data, 0, data.Length);
            }

        }
        private void ReceiveMsg()
        {
            try
            {
                User temp = new User();
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
                string[] client = temps.Split(',');

                this.User.ID = client[0];

                for (int i = 1; i < client.Length; i++)
                {
                    client[i].Trim('\0');
                    if (!client[i].StartsWith("*"))
                        view.MyDispatcher.Invoke(() =>
                        {
                            view.UserBox.Add(client[i]);
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



                    if (!view.UserBox.Any(x => x == temp.Name))//Другой вариант temp.Connect==true ? но пока так
                    {
                        view.MyDispatcher.Invoke(() =>
                        {
                            view.UserBox.Add(temp.Name);
                        });

                    }

                    if (temp.DisconnectClient)
                    {
                        view.MyDispatcher.Invoke(() =>
                        {
                            //temp.Message = "Bye";
                            view.MsgBox.Add(temp);
                            view.UserBox.Remove(temp.Name);

                        });
                    }

                    if (temp.ConnectPrivate)
                    {
                        if (view.PrivatChats.Any(x => x.view.privateName == temp.Name))
                            view.MyDispatcher.Invoke(() =>
                            {
                                foreach (var item in view.PrivatChats)

                                    if (item.view.privateName == temp.Name)
                                        item.view.ListMmessage.Add(temp.Message);
                            });


                        else
                        {
                            view.MyDispatcher.Invoke(() =>
                            {
                                PrivatChat chat = new PrivatChat();
                                chat.view.Name = view.UserName;
                                chat.view.privateName = temp.Name;
                                chat.view.user = temp;
                                chat.view.stream = stream;
                                chat.Title = "Владелец" + view.UserName + " => " + temp.Name;

                                chat.Show();
                                view.PrivatChats.Add(chat);
                                if (string.IsNullOrEmpty(temp.Message))
                                    chat.view.ListMmessage.Add(temp.Message);
                            });


                        }


                    }

                    if (temp.ID != User.ID && !string.IsNullOrEmpty(temp.Message) && !temp.ConnectPrivate)
                    {
                        view.MyDispatcher.Invoke(() =>
                        {
                            temp.Brush = new SolidColorBrush(Colors.LightBlue);
                            view.MsgBox.Add(temp);
                        });
                    }
                    if (temp.ID == User.ID && !string.IsNullOrEmpty(temp.Message) && !temp.ConnectPrivate)
                    {
                        view.MyDispatcher.Invoke(() =>
                        {
                            User.Message = temp.Message;
                            view.MsgBox.Add(User);
                        });
                    }

                }

            }
            catch (Exception e)
            {
                Close();
            }
        }
        public void Close()
        {
            stream?.Close();
            tcpclient?.Close();
        }

    }
}
