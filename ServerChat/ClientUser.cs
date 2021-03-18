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
                User.Name = Encoding.Unicode.GetString(data);
                server.AddConnection(this);


                byte[] id = Encoding.Unicode.GetBytes(user.ID);
                networkStream.Write(id, 0, id.Length);

                while (true)
                {
                    try
                    {
                        user.Desserialize(GetMsg());
                        server.BroadCastUser(User);
                    }
                    catch (Exception ex)
                    {
                        server.RemoveConnection(user.ID);
                        Close();
                        break;
                    }


                }







            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message + "ClientUser Process");
            }
        }
        private byte[] GetMsg()
        {
            byte[] data = new byte[1024];
           
            do
            {
                networkStream.Read(data, 0, data.Length);

            } while (networkStream.DataAvailable);

            return data;
        }
        internal void Close()
        {
            if (networkStream != null)
                networkStream.Close();
            if (client != null)
                client.Close();
        }


    }
}
