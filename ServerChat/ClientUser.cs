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
            server.AddConnection(this);
        }
        internal void Process()
        {
            try
            {

                networkStream = client.GetStream();
                User.Name = GetMsg();
                string msg = User.Name + " " +DateTime.Now.Hour+ ":"+DateTime.Now.Minute;
                
                server.BroadCastMsg(msg);
                while (true)
                {
                    try
                    {
                        msg = GetMsg();
                        msg = User.Name + " : " + msg;
                        server.BroadCastMsg(msg);
                    }
                    catch (Exception ex)
                    {
                        server.RemoveConnection(user.ID);
                        MessageBox.Show(ex.Message + "Process while()");
                        
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
        private string GetMsg()
        {
            byte[] data = new byte[64];
            int bytes = 0;
            StringBuilder builder = new StringBuilder();
            do
            {
                bytes = networkStream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, data.Length));

            } while (networkStream.DataAvailable);
            return builder.ToString();
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
