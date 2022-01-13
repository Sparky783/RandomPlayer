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

namespace RandomFile.Views
{
    /// <summary>
    /// Logique d'interaction pour DetailsWindow.xaml
    /// </summary>
    public partial class DetailsWindow : Window
    {
        public DetailsWindow()
        {
            InitializeComponent();
        }

        public void SetFile()
        {
            /*Video video = new Video(videoPath, false);
            Size size = video.DefaultSize;

            Console.WriteLine("Width: " + size.Width);
            Console.WriteLine("Height: " + size.Height);*/
        }
    }
}
