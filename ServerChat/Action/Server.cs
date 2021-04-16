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
        TcpListener getfileListener;
        Socket socket;
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
        protected internal void GetFile(User user)
        {
            try
            {
                //Socket socket = getfileListener.AcceptSocket();
                //Dispatcher.Invoke(new Action(() => { ListUser.Add("Send file : " + user.Name + " File name " + user.FileName + " " + DateTime.Now); }));
               
                //int bufferSize = 1024;
                //int filesize = int.Parse(user.Message);
                //byte[] buffer = null;
                //FileStream fs = new FileStream(user.FileName, FileMode.OpenOrCreate);
                //socket.Send(Encoding.Unicode.GetBytes("start"));
                //while (filesize > 0)
                //{
                //    buffer = new byte[bufferSize];
                //    Dispatcher.Invoke(new Action(() => { ListUser.Add("Load"+filesize.ToString()); }));
                //    int size = socket.Receive(buffer, SocketFlags.Partial);

                //    fs.Write(buffer, 0, size);

                //    filesize -= size;
                //}

                //fs.Close();
                //socket.Close();

            }
            catch (Exception ex) 
            {

                MessageBox.Show(ex.Message);
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
                getfileListener = new TcpListener(IPAddress.Parse(ip), 500);
                getfileListener.Start();
                tcpListener = new TcpListener(IPAddress.Parse(ip), 8000);
                tcpListener.Start();

                while (true)
                {

                   TcpClient client = tcpListener.AcceptTcpClient();
                   ClientUser clientUser = new ClientUser(client,this);
                   Thread clientThread = new Thread(new ThreadStart(clientUser.Process));
                   clientThread.Start();
                }

            }
            catch (Exception e)
            {
                Disconnect();
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
