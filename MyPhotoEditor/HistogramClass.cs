using AForge.Imaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;


namespace MyPhotoEditor
{
    public class HistogramClass
    {
        System.Drawing.Bitmap bmp;
        public PointCollection luminanceHistogramPoints;
        public PointCollection redColorHistogramPoints;
        public PointCollection greenColorHistogramPoints;
        public PointCollection blueColorHistogramPoints;

        public HistogramClass()
        {

        }
        public HistogramClass(Bitmap imageFilePath)
        {
            bmp = new System.Drawing.Bitmap(imageFilePath);
            // Luminance
            ImageStatisticsHSL hslStatistics = new ImageStatisticsHSL(bmp);
            this.luminanceHistogramPoints = ConvertToPointCollection(hslStatistics.Luminance.Values);
            // RGB
            ImageStatistics rgbStatistics = new ImageStatistics(bmp);
            this.redColorHistogramPoints = ConvertToPointCollection(rgbStatistics.Red.Values);
            this.greenColorHistogramPoints = ConvertToPointCollection(rgbStatistics.Green.Values);
            this.blueColorHistogramPoints = ConvertToPointCollection(rgbStatistics.Blue.Values);
        }

        private PointCollection ConvertToPointCollection(int[] values)
        {

            values = SmoothHistogram(values);

            int max = values.Max();

            PointCollection points = new PointCollection();
            // first point (lower-left corner)
            points.Add(new System.Windows.Point(0, max));
            // middle points
            for (int i = 0; i < values.Length; i++)
            {
                points.Add(new System.Windows.Point(i, max - values[i]));
            }
            // last point (lower-right corner)
            points.Add(new System.Windows.Point(values.Length - 1, max));

            return points;
        }

        private int[] SmoothHistogram(int[] originalValues)
        {
            int[] smoothedValues = new int[originalValues.Length];

            double[] mask = new double[] { 0.25, 0.5, 0.25 };

            for (int bin = 1; bin < originalValues.Length - 1; bin++)
            {
                double smoothedValue = 0;
                for (int i = 0; i < mask.Length; i++)
                {
                    smoothedValue += originalValues[bin - 1 + i] * mask[i];
                }
                smoothedValues[bin] = (int)smoothedValue;
            }

            return smoothedValues;
        }
    }
}
