using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ServerChat
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class ServerWindow : Window
    {
        static Server server;
        static Thread listenThread;
        static Thread listenLoadThread;
       
        public ServerWindow()
        {
            InitializeComponent();
            DataContext = server = new Server();
            listenThread = new Thread(new ThreadStart(server.Listen));
            listenThread.Start();

            //listenLoadThread = new Thread(new ThreadStart(server.ListenLoad));
            //listenLoadThread.Start();


            Closing += (s, e) => { server.Disconnect(); };
        }
    }
}
