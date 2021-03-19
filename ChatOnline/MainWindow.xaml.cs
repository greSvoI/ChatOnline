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
            view.Close();
        }

        private void textBoxSend_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                view.User.Message = textBoxSend.Text;
                view.User.SendMessage = true;
                view.Send.Execute(null);
                textBoxSend.Clear();
            }
        }

        private void MenuItem_Click_Private(object sender, RoutedEventArgs e)
        {
            view.Private.Execute(null);//Как привязать?
        }

        
    }
}
