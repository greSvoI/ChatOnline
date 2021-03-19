using ChatOnline;
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
using System.Windows.Shapes;

namespace ChatOnline
{
    /// <summary>
    /// Логика взаимодействия для PrivatChat.xaml
    /// </summary>
    public partial class PrivatChat : Window
    {
        public ViewApplicationPrivate view;
        public PrivatChat()
        {
            InitializeComponent();
            DataContext = view = new ViewApplicationPrivate();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                view.msg = textboxMsg.Text;
                view.ListMmessage.Add(textboxMsg.Text);
                textboxMsg.Clear();
                view.Send();
                 
            }
        }
    }
}
