
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChatOnline.Model
{
     public class Client
    {
        static TcpClient tcpclient;
        static TcpClient tcpSharing;

        static NetworkStream streamSharing;
        static NetworkStream stream;

        User temp_user = new User();
        readonly string ip = "127.0.0.1";
        readonly int portsharing =8005;
        readonly int portmsg = 8000;

        string fileName;
        string filePath;

        ViewApplication view;
        public User User { get; set; }
        TextBox box;
        public Client(ViewApplication view,TextBox box)
        {
            this.box = box;
            this.view = view;
            this.User = new User();
            this.User.Brush = new SolidColorBrush(Colors.LightCoral);
            tcpclient = new TcpClient();
            tcpSharing = new TcpClient();
            User.TextDecorations = null;
            
        }
        protected internal void Connect()
        {
            if (!string.IsNullOrEmpty(view.UserName))
                User.Name = view.UserName;
            else return;

            if (!tcpclient.Connected&& !tcpSharing.Connected)
            {
                tcpclient.Connect(ip, portmsg);
                stream = tcpclient.GetStream();
                tcpSharing.Connect(ip, portsharing);
                streamSharing = tcpSharing.GetStream();
            }
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
            Thread receiveThread = new Thread(new ThreadStart(ReceiveMsg));
            receiveThread.Start();
            Thread sharingThread = new Thread(new ThreadStart(ReceiveSharing));
            sharingThread.Start();
        }
        protected internal void ReceiveSharing()
        {
            try
            {
                while (true)
                {
                    byte[] data = new byte[256];
                    InfoFile info = new InfoFile();

                    do
                    {
                        streamSharing.Read(data, 0, data.Length);
                        info.Desserialize(data);
                        int len = (int)info.FileLenght;
                        int size = 0;
                        FileStream fs = new FileStream(info.FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, len);
                        do
                        {
                            data = new byte[1024000];
                            size = streamSharing.Read(data, 0, data.Length);
                            fs.Write(data, 0, size);
                            if (fs.Length == len) break;

                        } while (size > 0);
                        fs.Close();
                        streamSharing.Flush();


                    } while (streamSharing.DataAvailable);
                }
            }
            catch (Exception ex)
            {

                Close();
            }

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

        private void SendFile(InfoFile info)
        {
            try
            {
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                info.FileLenght = fs.Length;

                byte[] data = info.Serialize();
                streamSharing.Write(data, 0, data.Length);
                data = new byte[fs.Length];
                fs.Read(data, 0, (int)fs.Length);
                fs.Close();
                streamSharing.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message + "SendFile");
            }
        }
        protected internal void SendFile(bool flag)
        {
            try
            {

                InfoFile info = new InfoFile();
                OpenFileDialog dialog = new OpenFileDialog();
                if (dialog.ShowDialog() == true)
                {
                    info.FileName = dialog.SafeFileName;
                    filePath = dialog.FileName;
                    if (flag) info.PrivateName = view.SelectUser;
                    SendFile(info);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message + "SendFile");
            }
        }

        //Запрос на загрузку файла
        protected internal void LoadFile()
        {

            InfoFile info = new InfoFile();
            info.FileName = view.GetUser.Message;
            if (string.IsNullOrEmpty(info.FileName))
                return;
            byte[] data = info.Serialize();
            streamSharing.Write(data, 0, data.Length);

        }
        
        public void SendMsg()
        {
            User.Message = box.Text;
            box.Clear();
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
                byte[] data = new byte[256];
                while (true)
                {
                    data = new byte[1024];
                    int bytes = 0;//Пока не удалять будет нужен
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        temp_user.Desserialize(data);

                    } while (stream.DataAvailable);

                    if (!view.UserBox.Any(x => x == temp_user.Name))//Другой вариант temp.Connect==true ? но пока так
                    {
                        view.MyDispatcher.Invoke(() =>
                        {
                            view.UserBox.Add(temp_user.Name);
                        });

                    }

                    if (temp_user.DisconnectClient)
                    {
                        view.MyDispatcher.Invoke(() =>
                        {
                            //temp.Message = "Bye";
                            view.MsgBox.Add(temp_user);
                            view.UserBox.Remove(temp_user.Name);

                        });
                    }

                    if (temp_user.ConnectPrivate)
                    {
                        if (view.PrivatChats.Any(x => x.viewPrivate.privateName == temp_user.Name))
                            view.MyDispatcher.Invoke(() =>
                            {
                                
                                foreach (var item in view.PrivatChats)
                                    
                                    if (item.viewPrivate.privateName == temp_user.Name)
                                        item.viewPrivate.ListMmessage.Add(temp_user.Message);
                            });


                        else
                        {
                            view.MyDispatcher.Invoke(() =>
                            {
                                PrivatChat chat = new PrivatChat();
                                chat.viewPrivate.Name = view.UserName;
                                chat.viewPrivate.privateName = temp_user.Name;
                                chat.viewPrivate.user = temp_user;
                                chat.viewPrivate.stream = stream;
                                chat.viewPrivate.streamSharing = streamSharing;
                                chat.Title = "Владелец" + view.UserName + " => " + temp_user.Name;
                                chat.Show();
                                view.PrivatChats.Add(chat);
                                if (!string.IsNullOrEmpty(temp_user.Message))
                                    chat.viewPrivate.ListMmessage.Add(temp_user.Message);
                            });


                        }


                    }
                    if(temp_user.SendFile)
                    {
                        
                        view.MyDispatcher.Invoke(() =>
                        {
                            User.TextDecorations = TextDecorations.Underline;
                            //temp.Brush = new SolidColorBrush(Colors.LightBlue);
                            view.MsgBox.Add(temp_user);
                            User.TextDecorations = null;
                        });
                    }
                    else if (temp_user.ID != User.ID && !string.IsNullOrEmpty(temp_user.Message) && !temp_user.ConnectPrivate)
                    {
                        view.MyDispatcher.Invoke(() =>
                        {
                            temp_user.Brush = new SolidColorBrush(Colors.LightBlue);
                            view.MsgBox.Add(temp_user);
                        });
                    }
                    else if (temp_user.ID == User.ID && !string.IsNullOrEmpty(temp_user.Message) && !temp_user.ConnectPrivate)
                    {
                        view.MyDispatcher.Invoke(() =>
                        {
                            User.Message = temp_user.Message;
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
            streamSharing?.Close();
            tcpclient?.Close();
            tcpSharing?.Close();
        }

    }
}
