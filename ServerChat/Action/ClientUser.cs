﻿using System;
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
                        if(User.SendFile)
                        {
                            GetFile();
                            User.Message = User.FileName;
                            server.BroadCastUser(User);
                            User.SendFile = false;
                        }
                        else if(!string.IsNullOrEmpty(User.NamePrivate))
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
            catch (Exception e)
            {
                //MessageBox.Show(e.Message + "ClientUser Process");
            }
        }
        private void GetFile()
        {
            server.Dispatcher.Invoke(()=> {
                server.ListUser.Add("Получено : "+ User.FileName);
            });
            int len = int.Parse(User.Message);
            FileStream fs = new FileStream(User.FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite,len);
            byte[] data;
            int size;
            do
            {
                data = new byte[9999];
                size = networkStream.Read(data,0,data.Length);
                fs.Write(data, 0, size);
                if (fs.Length == len) break;
            } while (size>0);
            server.Dispatcher.Invoke(() => {
                server.ListUser.Add("Загружено : " + User.FileName);
            });
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
