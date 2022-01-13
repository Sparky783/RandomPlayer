using RandomFile.ViewModels;
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
    /// Logique d'interaction pour MediaPlayerWindow.xaml
    /// </summary>
    public partial class MediaPlayerWindow : Window
    {
        public MediaPlayerWindow()
        {
            InitializeComponent();
        }

        private MediaPlayerWindowViewModel ViewModel
        {
            get { return (MediaPlayerWindowViewModel)Resources["ViewModel"]; }
        }
    }
}
