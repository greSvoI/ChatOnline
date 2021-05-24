using ChatOnline.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ServerChat
{
    public class ClientUser
    {
        TcpClient client;
        TcpClient sharing;
        public NetworkStream networkStream;
        public NetworkStream networkSharing;
        Server server;
        User user = new User();
        public  User User { get => user; set { user = value; } }
       
        public ClientUser(TcpClient message,TcpClient sharing,Server server)
        {
            this.client = message;
            this.sharing = sharing;
            this.server = server;
            User.ID = client.Client.RemoteEndPoint.ToString();
        }
        internal void ProcessFileSharing()
        {
            try
            {
                networkSharing = sharing.GetStream();
                byte[] data = new byte[256];
                while (true)
                {
                    InfoFile info = new InfoFile();
                    networkSharing.Read(data, 0, data.Length);
                    info.Desserialize(data);
                    if (info.FileLenght == 0)
                        SendFile(info);
                    else 
                        GetFile(info);
                    if(!string.IsNullOrEmpty(info.PrivateName))
                    {
                        server.PrivateMassage(User, info.PrivateName);
                    }
                    networkSharing.Flush();
                    
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message+ "ProcessFileSharing");

            }

        }
        internal void ProcessMessage()
        {
            try
            {
                networkStream = client.GetStream();
                byte[] data;
                while (true)
                {

                    data = new byte[64];
                    networkStream.Read(data, 0, data.Length);
                    User.Name = Encoding.Unicode.GetString(data).Trim('\0'); //Получаем Имя

                    if (!server.clients.Any(x => x.User.Name == User.Name))
                    {
                        networkStream.Flush();
                        data[0] = 1;
                        networkStream.Write(data, 0, data.Length);
                        break;
                    }
                    else
                    {
                        data[0] = 0;
                        networkStream.Write(data, 0, data.Length);
                        networkStream.Flush();
                    }
                }
                server.AddConnection(this);//Отправляем список активных
                while (true)
                {
                    try
                    {
                        data = new byte[1024];
                        data = GetMsg();
                        if (data == null) Close();

                        User.Desserialize(data);
                       if(!string.IsNullOrEmpty(User.NamePrivate))
                       {
                            server.PrivateMassage(User,User.NamePrivate);
                       }
                       else
                       server.BroadCastUser(User);
                    }
                    catch (Exception ex)
                    {
                        break;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "ProcessMessage");
            }
        }
        private void GetFile(InfoFile info)
        {
            try
            {
                server.Dispatcher.Invoke(() => {
                    server.ListUser.Add("Получено : " + info.FileName);
                });
                int len = (int)info.FileLenght;
                FileStream fs = new FileStream(info.FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, len);
                byte[] data;
                int size;
                do
                {
                    data = new byte[1000000];
                    size = networkSharing.Read(data, 0, data.Length);
                    fs.Write(data, 0, size);
                    if (fs.Length == len) break;
                } while (size > 0);
                server.Dispatcher.Invoke(() => {
                    server.ListUser.Add("Загружено : " + info.FileName);
                });
                fs.Close();
                User.Message = info.FileName;
                server.BroadCastUser(User);
            }

            catch (Exception ex)
            {

                MessageBox.Show(ex.Message+"GetFile");
            }
        }
        private void SendFile(InfoFile info)
        {
            try
            {
                FileStream fs = new FileStream(info.FileName, FileMode.Open, FileAccess.Read);
                info.FileLenght = fs.Length;
                byte[] data = info.Serialize();
                networkSharing.Write(data, 0, data.Length);


                data = new byte[fs.Length];
                fs.Read(data, 0, (int)fs.Length);
                fs.Close();
                networkSharing.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message + "SendFile");
            }
        }
        private byte[] GetMsg()
        {
            byte[] data = new byte[1024];
            try
            {
                do
                {
                    if (networkStream.Read(data, 0, data.Length) == 0)
                        return null;

                } while (networkStream.DataAvailable);

                return data;
            }
            catch (Exception)
            {

                return null;
            }
            
        }
        internal void Close()
        {
            server.RemoveConnection(User.ID);
            networkStream?.Close();
            networkSharing?.Close();
            client?.Close();
            sharing?.Close();
            
        }


    }
}
