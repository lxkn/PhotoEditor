using System;
using System.Collections.Generic;
using System.Drawing;
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
    /// Logika interakcji dla klasy Histogram.xaml
    /// </summary>
    public partial class Histogram : Window
    {
        public Histogram()
        {

        }

        public Histogram(HistogramClass bitmap)
        {
            InitializeComponent();
            pol_lum.Points = bitmap.luminanceHistogramPoints;
            pol_blue.Points = bitmap.blueColorHistogramPoints;
            pol_red.Points = bitmap.redColorHistogramPoints;
            pol_green.Points = bitmap.greenColorHistogramPoints;
        }
    }
}
