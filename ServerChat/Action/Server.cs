using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ServerChat
{
    public class Server : INotifyPropertyChanged
    {
        readonly string ip = "127.0.0.1";
        IPEndPoint GetfileIP;
        TcpListener tcpListener;
        TcpListener tcpListenerSharing;
        public Dispatcher Dispatcher { get; set; }

        public List<ClientUser> clients = new List<ClientUser>();

        ObservableCollection<string> message = new ObservableCollection<string>();

        public Server()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
        }
        public ObservableCollection<string> ListUser 
         { 
            get => message; 
            set
            { 
                message = value;
                OnPropertyChanged("");
            }
        }
        protected internal void AddConnection(ClientUser user)
        {
            clients.Add(user);
            string client = user.User.ID+",";
            byte[] data = new byte[1024];
           
            

            for (int i = 0; i < clients.Count; i++)
            {
                client += clients[i].User.Name + ",";
            }

            data = Encoding.Unicode.GetBytes(client+="*");
            user.networkStream.Write(data, 0, data.Length);
           
            Dispatcher.Invoke(new Action(() => { ListUser.Add("Connect : " + user.User.Name +" "+ DateTime.Now); }));
            user.User.ConnectClient = true;
            BroadCastUser(user.User);
        }
        protected internal void RemoveConnection(string id)
        {
            ClientUser user = clients.FirstOrDefault(x => x.User.ID == id);
           
            if (user != null)
            {
                user.User.DisconnectClient = true;
                Dispatcher.Invoke(new Action(() => { ListUser.Add("Disconnect : " + user.User.Name + " " + DateTime.Now); }));
            }
            BroadCastUser(user.User);
            clients.Remove(user);
        }
        protected internal void Listen()
        {
            try
            {
               
                tcpListener = new TcpListener(IPAddress.Parse(ip), 8000);
                tcpListener.Start();
                tcpListenerSharing = new TcpListener(IPAddress.Parse(ip), 8005);
                tcpListenerSharing.Start();
                while (true)
                {

                    TcpClient message = tcpListener.AcceptTcpClient();
                    TcpClient sharing = tcpListenerSharing.AcceptTcpClient();
                    ClientUser clientUser = new ClientUser(message, sharing, this);
                    Thread clientMessage = new Thread(new ThreadStart(clientUser.ProcessMessage));
                    clientMessage.Start();
                    Thread clientSharing = new Thread(new ThreadStart(clientUser.ProcessFileSharing));
                    clientSharing.Start();
                   
                }

            }
            catch (Exception e)
            {
                Disconnect();
                
            }
        }
        protected internal void SendFile(object client)
        {
            try
            {
                TcpClient tcp = (TcpClient)client;
                NetworkStream stream = null;
                User user = new User();
                stream = tcp.GetStream();

                byte[] data = null;
                data = new byte[256];
                stream.Read(data, 0, data.Length);
                user.Desserialize(data);

                FileStream fs = new FileStream(user.FileName, FileMode.Open, FileAccess.Read);
                while (true)
                {
                    



                }  
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        protected internal void PrivateMassage(User user,string name)
        {
            byte[] data = user.Serialize();
            foreach(ClientUser item in clients)
            {
                if (item.User.Name == name)
                    item.networkStream.Write(data, 0, data.Length);
            }
        }
        protected internal void BroadCastMsg(string msg)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                ListUser.Add(msg);
            }));
            byte[] data = Encoding.Unicode.GetBytes(msg);
            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].networkStream.Write(data,0,data.Length);
            }
        }
        protected internal void BroadCastUser(User user)
        {
            byte[] data = user.Serialize();
            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].networkStream.Write(data, 0, data.Length);
            }
        }
        protected internal void Disconnect()
        {
            tcpListener.Stop();
            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

    }
}
