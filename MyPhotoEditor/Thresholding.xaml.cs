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

namespace MyPhotoEditor
{
    /// <summary>
    /// Logika interakcji dla klasy Thresholding.xaml
    /// </summary>
    public partial class Thresholding : Window
    {
        public Thresholding()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)this.Owner).doBinarization((int)slider_binary.Value);
            this.Close();
        }
    }
}
