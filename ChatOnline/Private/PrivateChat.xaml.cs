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

namespace ChatOnline.PrivateChat
{
    /// <summary>
    /// Логика взаимодействия для PrivateChat.xaml
    /// </summary>
    public partial class PrivateChat : Window
    {
        public ViewApplicationPrivate view;
        public PrivateChat()
        {
            InitializeComponent();
            DataContext = view = new ViewApplicationPrivate();
        }
    }
}
