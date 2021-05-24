using ChatOnline;
using ChatOnline.Model;
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
        public ViewApplicationPrivate viewPrivate;
        
        public PrivatChat()
        {
            InitializeComponent();
            ImageBrush imBrush = new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri("2.jpg", UriKind.Relative)),
                Stretch = Stretch.Fill
            };
            this.Background = imBrush;
            DataContext = viewPrivate = new ViewApplicationPrivate();
        }
    }
}
