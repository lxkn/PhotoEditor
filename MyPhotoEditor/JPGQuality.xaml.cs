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
    /// Logika interakcji dla klasy JPGQuality.xaml
    /// </summary>
    public partial class JPGQuality : Window
    {
        public JPGQuality()
        {
            InitializeComponent();
        }
        private string value;

        public  string Value
        {
            get { return value; }
            set { value = value; }
        }


        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            Value = tbJPEG.Text;
            this.Hide();
        }
    }
}
