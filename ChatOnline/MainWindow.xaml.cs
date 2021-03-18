using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace ChatOnline
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        ViewApplication view;
        public MainWindow()
        {

            InitializeComponent();
            DataContext = view = new ViewApplication();
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ViewApplication.client?.Close();
            ViewApplication.stream?.Close();
        }

        private void textBoxSend_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                view.user.Message = textBoxSend.Text;
                view.user.SendMessage = true;
                view.Send.Execute(null);
                textBoxSend.Clear();
            }
        }
    }
}
