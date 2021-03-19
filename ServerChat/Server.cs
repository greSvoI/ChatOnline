﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        TcpListener tcpListener;
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
                tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8000);
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
