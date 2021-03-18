using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ServerChat
{
    public class ClientUser
    {
        TcpClient client;
        public NetworkStream networkStream;
        Server server;
        User user = new User();
        public  User User { get => user; set { user = value; } }
       
        public ClientUser(TcpClient client, Server server)
        {
            this.client = client;
            this.server = server;
            User.ID = client.Client.RemoteEndPoint.ToString();   
        }
        internal void Process()
        {
            try
            {

                networkStream = client.GetStream();
                byte[] data = new byte[64];

                networkStream.Read(data, 0, data.Length);
                User.Name = Encoding.Unicode.GetString(data).Trim('\0'); //Получаем Имя

                server.AddConnection(this);//Отправляем список активных

                while (true)
                {
                    try
                    {
                        data = new byte[1024];
                        data = GetMsg();
                        if (data == null) Close();
                        User.Desserialize(data);
                        server.BroadCastUser(User);
                    }
                    catch (Exception ex)
                    {
                        break;
                    }
                }

            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message + "ClientUser Process");
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
            if (networkStream != null)
                networkStream.Close();
            if (client != null)
                client.Close();
            
        }


    }
}
