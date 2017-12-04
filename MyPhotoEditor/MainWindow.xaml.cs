using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
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
using Xceed.Wpf.Toolkit;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using Image = System.Drawing.Image;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace MyPhotoEditor
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point p;
        private Point q;
        private Rect s;
        private int _maskSize = 3;
        private Rectangle rectangle;
        private Ellipse ellipse;
        private Line line;
        private DirectBitmap _directBitmap;
        private IntViewModel[,] _mask;
        private List<Line> lines;
        private List<Ellipse> ellipses;
        private List<Rectangle> rectList;
        private double x;
        private double y;
        private Color color;
        private int mode;
        private int switcher;
        private List<Rect> pListAlfa;
        Bitmap bitmap;
        int state;
        private bool buttonRClicked, buttonEClicked, buttonLClicked, buttonCClicked;
        Point[] points = new Point[10];
        Point[] points2 = new Point[30];
        Line[] lineArray = new Line[12];
        List<Point> changeAlfaList;
        int nextPoint = 0;
        int nextPoint2 = 0;
        int stateButton = 0;
        int h = 0, v = 0;
        int changeLX, changeLY, changeL;
        private List<Rectangle> pointList;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ImageSource DisplayedImage
        {
            get { return new BitmapImage(new Uri(@"C:\Users\Seba\Documents\Visual Studio 2017\Projects\MyPhotoEditor\MyPhotoEditor\b.jpg")); }
        }

        public int MaskSize
        {
            get
            {
                return _maskSize;
            }
            set
            {
                if(value%2 == 1 && value>=3)
                {
                    _maskSize = value;
                    OnPropertyChanged("MaskSize");
                }
            }
        }
        

        public MainWindow()
        {
            GenerateMaskView();
            InitializeComponent();
            grid_Change.Visibility = Visibility.Collapsed;
            grid_CMYK.Visibility = Visibility.Collapsed;
            Grid_2d.Visibility = Visibility.Collapsed;
            rectList = new List<Rectangle>();
            ellipses = new List<Ellipse>();
            lines = new List<Line>();
            color = new Color();
            pointList = new List<Rectangle>();
            pListAlfa = new List<Rect>();
            changeAlfaList = new List<Point>();
            ClrPckerBackground.SelectedColor = Color.FromArgb(150, 000, 000, 000);
            inkCanvas1.Background = new SolidColorBrush(Color.FromRgb(246, 246, 246));
            tbR.IsReadOnly = true;
            tbG.IsReadOnly = true;
            tbB.IsReadOnly = true;
            switcher = 2;
            mode = 1;
            Error.Foreground = Brushes.Green;
            Error.Text = "Zmieniasz CMK -> RGB";
            //background.ImageSource = new BitmapImage(new Uri("pack://application:,,,/MyPhotoEditor;/b.jpg"));
            name.DataContext = this;
        }
        
        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas1.Children.Clear();
            //this.inkCanvas1.Strokes.Clear();
            //this.RectangleViewModel.Fill = Brushes.Blue;
            //this.RectangleViewModel.Width = 100;
            //this.RectangleViewModel.Height = 100;
        }
        
        private void buttonChangeSizeIn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbChangeHeight.Text) || string.IsNullOrWhiteSpace(tbChangeWidth.Text))
                return;
            switch (state)
            {
                case 1:
                    rectList[rectList.Count() - 1].Height = Double.Parse(tbChangeHeight.Text);
                    rectList[rectList.Count() - 1].Width = Double.Parse(tbChangeWidth.Text);
                    break;
                case 2:
                    ellipses[ellipses.Count() - 1].Height = Double.Parse(tbChangeHeight.Text);
                    ellipses[ellipses.Count() - 1].Width = Double.Parse(tbChangeWidth.Text);
                    break;
                case 3:
                    lines[lines.Count() - 1].Height = Double.Parse(tbChangeHeight.Text);
                    lines[lines.Count() - 1].Width = Double.Parse(tbChangeWidth.Text);
                    break;
            }
        
            //    RectangleViewModel.Height = Double.Parse(tbChangeHeight.Text);
            //    RectangleViewModel.Width = Double.Parse(tbChangeWidth.Text);
            //    RectangleViewModel.Opacity = 1;


        }

        private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
        }

        private void inkCanvas1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(rectangle != null)
                rectList.Add(rectangle);
            if(ellipse != null)
                ellipses.Add(ellipse);
            if(line != null)
                lines.Add(line);
            Mouse.Capture(null);
            rectangle = null;
        }

        private void inkCanvas1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(inkCanvas1);
            p = Mouse.GetPosition(inkCanvas1);
            switch (state)
            {
                case 1:
                    rectangle = new Rectangle();
                    rectangle.Stroke = new SolidColorBrush(ClrPckerBackground.SelectedColor.Value);
                    inkCanvas1.Children.Add(rectangle);
                    break;
                case 2:
                    ellipse = new Ellipse();
                    ellipse.Stroke = new SolidColorBrush(ClrPckerBackground.SelectedColor.Value);
                    inkCanvas1.Children.Add(ellipse);
                    break;
                case 3:
                    line = new Line();
                    line.Stroke = new SolidColorBrush(ClrPckerBackground.SelectedColor.Value);
                    line.X1 = p.X;
                    line.Y1 = p.Y;
                    inkCanvas1.Children.Add(line);
                    break;
                case 4:
                    try
                    {
                        points[nextPoint] = Mouse.GetPosition(inkCanvas1);
                        Mouse.Capture(null);
                        SolidColorBrush color = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                        Rectangle mark = new Rectangle
                        {
                            Stroke = color,
                            StrokeThickness = 5,
                            Height = 3,
                            Width = 3

                        };
                        mark.SetValue(Canvas.LeftProperty, points[nextPoint].X);
                        mark.SetValue(Canvas.TopProperty, points[nextPoint].Y);
                        inkCanvas1.Children.Add(mark);
                        nextPoint++;
                    }
                    catch(Exception esa)
                    {
                        Console.Write(esa.Message.ToString());
                        nextPoint = 0;
                        inkCanvas1.Children.Clear();
                    }


                    break;
                case 5:
                    try
                    {
                        points2[nextPoint2] = Mouse.GetPosition(inkCanvas1);
                        Mouse.Capture(null);
                        SolidColorBrush c = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                        Rectangle mark2 = new Rectangle
                        {
                            Stroke = c,
                            StrokeThickness = 5,
                            Height = 3,
                            Width = 3

                        };
                        if (stateButton == 0)
                        {
                            mark2.SetValue(Canvas.LeftProperty, points2[nextPoint2].X);
                            mark2.SetValue(Canvas.TopProperty, points2[nextPoint2].Y);
                            pointList.Add(mark2);
                            inkCanvas1.Children.Add(mark2);
                            break;
                        }
                        else
                        {
                            pointList[pointList.Count - 1].SetValue(Canvas.LeftProperty, points2[nextPoint].X + h);
                            pointList[pointList.Count - 1].SetValue(Canvas.TopProperty, points2[nextPoint].Y + v);
                            Rectangle next = new Rectangle
                            {
                                Stroke = c,
                                StrokeThickness = 3,
                                Height = 3,
                                Width = 3
                            };
                            next = pointList[pointList.Count - 1];
                            inkCanvas1.Children.Add(next);
                            pointList.Add(next);
                        }
                    }
                    catch(Exception es)
                    {
                        //nextPoint = 0;
                    }
                    break;
                case 6:
                    try
                    {
                        points2[nextPoint2] = Mouse.GetPosition(inkCanvas1);
                        Mouse.Capture(null);
                        SolidColorBrush c = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                        Rectangle mark6 = new Rectangle
                        {
                            Stroke = c,
                            StrokeThickness = 5,
                            Height = 3,
                            Width = 3

                        };
                            mark6.SetValue(Canvas.LeftProperty, points2[nextPoint2].X);
                            mark6.SetValue(Canvas.TopProperty, points2[nextPoint2].Y);
                            changeAlfaList.Add(points2[nextPoint2]);
                            inkCanvas1.Children.Add(mark6);
                            pointList.Add(mark6);
                    }
                    catch (Exception es)
                    {
                        //nextPoint = 0;
                    }
                    break;
            }
        }

        PolyLineSegment GetBezierApproximation(Point[] controlPoints, int outputSegmentCount)
        {
            Point[] points = new Point[outputSegmentCount + 1];
            for (int i = 0; i <= outputSegmentCount; i++)
            {
                double t = (double)i / outputSegmentCount;
                points[i] = GetBezierPoint(t, controlPoints, 0, controlPoints.Length);
            }
            return new PolyLineSegment(points, true);
        }

        Point GetBezierPoint(double t, Point[] controlPoints, int index, int count)
        {
            if (count == 1)
                return controlPoints[index];
            var P0 = GetBezierPoint(t, controlPoints, index, count - 1);
            var P1 = GetBezierPoint(t, controlPoints, index + 1, count - 1);
            return new Point((1 - t) * P0.X + t * P1.X, (1 - t) * P0.Y + t * P1.Y);
        }

        private void Bezier(object sender, RoutedEventArgs e)
        {
            var b = GetBezierApproximation(points, 256);
            PathFigure pf = new PathFigure(b.Points[0], new[] { b }, false);
            PathFigureCollection pfc = new PathFigureCollection();
            pfc.Add(pf);
            var pge = new PathGeometry();
            pge.Figures = pfc;
            System.Windows.Shapes.Path p = new System.Windows.Shapes.Path();
            p.Data = pge;
            p.Stroke = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            inkCanvas1.Children.Add(p);
            nextPoint = 0;
        }

        private void inkCanvas1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released || rectangle == null && ellipse == null && line == null)
                return;

            q = e.GetPosition(inkCanvas1);
            x = Math.Min(q.X, p.X);
            y = Math.Min(q.Y, p.Y);
            double width = Math.Max(q.X, p.X) - x;
            double height = Math.Max(q.Y, p.Y) - y;
            switch (state)
            {
                case 1:
                    rectangle.Width = width;
                    rectangle.Height = height;
                    Canvas.SetLeft(rectangle, x);
                    Canvas.SetTop(rectangle, y);
                    break;
                case 2:
                    q = e.GetPosition(inkCanvas1);
                    x = Math.Min(q.X, p.X);
                    y = Math.Min(q.Y, p.Y);
                    ellipse.Width = width;
                    ellipse.Height = height;
                    Canvas.SetLeft(ellipse, x);
                    Canvas.SetTop(ellipse, y);
                    break;
                case 3:
                    q = e.GetPosition(inkCanvas1);
                    line.X2 = q.X;
                    line.Y2 = q.Y;
                    break;
                case 4:
                    q = e.GetPosition(inkCanvas1);
                    
                    break;
                case 6:
                    q = e.GetPosition(inkCanvas1);
                    break;

            }

        }
        

        private void inkCanvas1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void ButtonLine_OnClick(object sender, RoutedEventArgs e)
        {
            ButtonEllipse.Background = Brushes.Transparent;
            ButtonRectangle.Background = Brushes.Transparent;
            state = 3;
            grid_Change.Visibility = Visibility.Visible;
            Grid_LoadingFiles.Visibility = Visibility.Collapsed;
            Grid_2d.Visibility = Visibility.Collapsed;
            ButtonLine.Background=Brushes.CornflowerBlue;
            grid_CMYK.Visibility = Visibility.Collapsed;
        }

        private void ButtonEllipse_OnClick(object sender, RoutedEventArgs e)
        {
            ButtonRectangle.Background = Brushes.Transparent;
            ButtonLine.Background = Brushes.Transparent;
            state = 2;
            grid_Change.Visibility = Visibility.Visible;
            Grid_LoadingFiles.Visibility = Visibility.Collapsed;
            Grid_2d.Visibility = Visibility.Collapsed;
            ButtonEllipse.Background = Brushes.CornflowerBlue;
            grid_CMYK.Visibility = Visibility.Collapsed;
        }

        private void ButtonRectangle_OnClick(object sender, RoutedEventArgs e)
        {

            ButtonEllipse.Background = Brushes.Transparent;
            ButtonLine.Background = Brushes.Transparent;
            state = 1;
            grid_Change.Visibility = Visibility.Visible;
            Grid_LoadingFiles.Visibility = Visibility.Collapsed;
            Grid_2d.Visibility = Visibility.Collapsed;
            ButtonRectangle.Background = Brushes.CornflowerBlue;
            grid_CMYK.Visibility = Visibility.Collapsed;
        }
        
        public static System.Drawing.Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            var destRect = new System.Drawing.Rectangle(0, 0, width, height);
            var destImage = new System.Drawing.Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = System.Drawing.Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            grid_Change.Visibility = Visibility.Collapsed;
            grid_CMYK.Visibility = Visibility.Collapsed;
            Grid_LoadingFiles.Visibility = Visibility.Visible;
            Grid_2d.Visibility = Visibility.Collapsed;
            string filename = "";
            PPMReader ppm = new PPMReader();
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".ppm";
            dlg.Filter = "PPM Files (*.ppm)|*.ppm|JPEG Files (*.jpeg)|*.jpeg";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                // Open document 
                filename = dlg.FileName;
                bitmap = ppm.ReadFile(filename, "test123.bmp");
                if (bitmap.Height < 100 && bitmap.Width < 100)
                    bitmap = ResizeImage(bitmap, bitmap.Width * 100, bitmap.Height * 100);
                else
                    bitmap = ResizeImage(bitmap, bitmap.Width, bitmap.Height);
                using (MemoryStream memory = new MemoryStream())
                {
                    bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                    memory.Position = 0;
                    BitmapImage bitmapimage = new BitmapImage();
                    bitmapimage.BeginInit();
                    bitmapimage.StreamSource = memory;
                    bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapimage.EndInit();
                    _directBitmap = new DirectBitmap(bitmap);
                    ImageSource imageSource = bitmapimage;
                    ImageView.Source = imageSource;
                }


            }
        }

        private void MenuItem_OnClickSave(object sender, RoutedEventArgs e)
        {
            if (ImageView.Source != null)
            {
                ImageCodecInfo jpg = GetEncoder(ImageFormat.Jpeg);


                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                JPGQuality jpgWindow = new JPGQuality();
                jpgWindow.Show();
                long quality = (long) Convert.ToDouble(jpgWindow.Value);
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, quality);
                myEncoderParameters.Param[0] = myEncoderParameter;
                bitmap.Save(@"zapisany.jpg", jpg, myEncoderParameters);

            }
        }
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private void CheckTextBox()
        {
            if(string.IsNullOrWhiteSpace(fileR.Text))
            {
                fileR.Text = "0";
            }
            if(string.IsNullOrWhiteSpace(fileG.Text))
            {
                fileG.Text = "0";
            }
            if(string.IsNullOrWhiteSpace(fileB.Text))
            {
                fileB.Text = "0";
            }
            

        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            CheckTextBox();
            int valr,valg,valb,sumr,sumg,sumb;
            valr = Convert.ToInt32(fileR.Text);
            valg = Convert.ToInt32(fileG.Text);
            valb = Convert.ToInt32(fileB.Text);
            if(valr >= 0&& valr <= 255 && valg >= 0 && valg <= 255 && valb >= 0 && valb <= 255)
            {
                if(bitmap!=null)
                {
                    for (int i = 0; i < bitmap.Width; i++)
                    {
                        for (int j = 0; j < bitmap.Height; j++)
                        {
                            var color = bitmap.GetPixel(i, j);
                            var r = color.R;
                            var g = color.G;
                            var b = color.B;
                            sumr = valr + r;
                            sumg = valg + g;
                            sumb = valb + b;
                            if(sumr>255)
                            {
                                sumr = 255;
                            }
                            if(sumg>255)
                            {
                                sumg = 255;
                            }
                            if(sumb>255)
                            {
                                sumb = 255;
                            }
                            color = System.Drawing.Color.FromArgb(sumr, sumg, sumb);
                            bitmap.SetPixel(i, j, color);
                        }
                    }

                    Reload();
                }
            }
            else
            {
                return;
            }
        }

        private void ButtonMinus_Click(object sender, RoutedEventArgs e)
        {
            CheckTextBox();
            int valr, valg, valb, sumr, sumg, sumb;
            valr = Convert.ToInt32(fileR.Text);
            valg = Convert.ToInt32(fileG.Text);
            valb = Convert.ToInt32(fileB.Text);
            if (valr >= 0 && valr <= 255 && valg >= 0 && valg <= 255 && valb >= 0 && valb <= 255)
            {
                if (bitmap != null)
                {
                    for (int i = 0; i < bitmap.Width; i++)
                    {
                        for (int j = 0; j < bitmap.Height; j++)
                        {
                            var color = bitmap.GetPixel(i, j);
                            var r = color.R;
                            var g = color.G;
                            var b = color.B;
                            sumr = r - valr;
                            sumg = g - valg;
                            sumb = b - valb;
                            if (sumr < 0)
                            {
                                sumr = 0;
                            }
                            if (sumg < 0)
                            {
                                sumg = 0;
                            }
                            if (sumb < 0)
                            {
                                sumb = 0;
                            }
                            if (valr == 0)
                            {
                                sumr = r;
                            }
                            if (valb == 0)
                            {
                                sumb = b;
                            }
                            if (valg == 0)
                            {
                                sumg = g;
                            }

                            color = System.Drawing.Color.FromArgb(sumr, sumg, sumb);
                            bitmap.SetPixel(i, j, color);
                        }
                    }

                    Reload();
                }
            }
            else
            {
                return;
            }

        }

        private void ButtonMultiply_Click(object sender, RoutedEventArgs e)
        {
            CheckTextBox();
            int valr, valg, valb, sumr, sumg, sumb;
            valr = Convert.ToInt32(fileR.Text);
            valg = Convert.ToInt32(fileG.Text);
            valb = Convert.ToInt32(fileB.Text);
            if (valr >= 0 && valr <= 255 && valg >= 0 && valg <= 255 && valb >= 0 && valb <= 255)
            {
                if (bitmap != null)
                {
                    for (int i = 0; i < bitmap.Width; i++)
                    {
                        for (int j = 0; j < bitmap.Height; j++)
                        {
                            var color = bitmap.GetPixel(i, j);
                            var r = color.R;
                            var g = color.G;
                            var b = color.B;
                            sumr = valr * r;
                            sumg = valg * g;
                            sumb = valb * b;
                            if (sumr > 255)
                            {
                                sumr = 255;
                            }
                            if (sumg > 255)
                            {
                                sumg = 255;
                            }
                            if (sumb > 255)
                            {
                                sumb = 255;
                            }
                            if(valr==0)
                            {
                                sumr = r;
                            }
                            if(valb == 0)
                            {
                                sumb = b;
                            }
                            if(valg == 0)
                            {
                                sumg = g;
                            }

                            color = System.Drawing.Color.FromArgb(sumr, sumg, sumb);
                            bitmap.SetPixel(i, j, color);
                        }
                    }
                    
                    Reload();
                }
            }
            
            else
            {
                return;
            }
        }


        private void ButtonSplit_Click(object sender, RoutedEventArgs e)
        {
            CheckTextBox();
            int valr, valg, valb, sumr, sumg, sumb;
            valr = Convert.ToInt32(fileR.Text);
            valg = Convert.ToInt32(fileG.Text);
            valb = Convert.ToInt32(fileB.Text);
            if (valr >= 0 && valr <= 255 && valg >= 0 && valg <= 255 && valb >= 0 && valb <= 255)
            {
                if (bitmap != null)
                {
                    for (int i = 0; i < bitmap.Width; i++)
                    {
                        for (int j = 0; j < bitmap.Height; j++)
                        {
                            var color = bitmap.GetPixel(i, j);
                            var r = color.R;
                            var g = color.G;
                            var b = color.B;
                            sumr = r;
                            sumg = g;
                            sumb = b;

                            if (valr !=0)
                                sumr = r / valr;
                            if (valg != 0)
                                sumg = g / valg;

                            if (valb != 0)
                                sumb = b / valb;
                            if (sumr > 255)
                            {
                                sumr = 255;
                            }
                            if (sumg > 255)
                            {
                                sumg = 255;
                            }
                            if (sumb > 255)
                            {
                                sumb = 255;
                            }
                            if(valr == 0 )
                            {
                                sumr = r;
                            }
                            if(valb == 0)
                            {
                                sumb = b;
                            }
                            if(valg == 0)
                            {
                                sumg = g;
                            }
                            
                            color = System.Drawing.Color.FromArgb(sumr, sumg, sumb);
                            bitmap.SetPixel(i, j, color);
                        }
                    }

                    Reload();
                }
            }
            else
            {
                return;
            }
        }

        private void ButtonBrightness_Click(object sender, RoutedEventArgs e)
        {
            CheckTextBox();
            int value,valr,valb,valg, sumr, sumg, sumb;
            //valr = Convert.ToInt32(fileR.Text);
            //valg = Convert.ToInt32(fileG.Text);
            //valb = Convert.ToInt32(fileB.Text);
            if (Check.IsChecked == true)
            {
                value = Convert.ToInt32(Brightness.Value);
                valr = (value * 255) / 100;
                valg = valr;
                valb = valr;
            }
            else
            {
                value = Convert.ToInt32(Brightness.Value);
                valr = -((value * 255) / 100);
                valg = valr;
                valb = valr;
            }
            
                if (bitmap != null)
                {
                    for (int i = 0; i < bitmap.Width; i++)
                    {
                        for (int j = 0; j < bitmap.Height; j++)
                        {
                            var color = bitmap.GetPixel(i, j);
                            var r = color.R;
                            var g = color.G;
                            var b = color.B;
                            sumr = valr + r;
                            sumg = valg + g;
                            sumb = valb + b;
                            if (sumr > 255)
                            {
                                sumr = 255;
                            }
                            if (sumr < 0) sumr = 0;
                            if (sumg > 255)
                            {
                                sumg = 255;
                            }
                            if (sumg < 0) sumg = 0;
                            if (sumb > 255)
                            {
                                sumb = 255;
                            }
                            if (sumb < 0) sumb = 0;
                            color = System.Drawing.Color.FromArgb(sumr, sumg, sumb);
                            bitmap.SetPixel(i, j, color);
                        }
                    }

                    Reload();
                }
            else
            {
                return;
            }
        }

        private void ButtonGray_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int value;
                if (bitmap != null)
                {
                    for (int i = 0; i < bitmap.Width; i++)
                    {
                        for (int j = 0; j < bitmap.Height; j++)
                        {
                            var color = bitmap.GetPixel(i, j);
                            int tempvalue;
                            var r = color.R;
                            var g = color.G;
                            var b = color.B;

                            var intensity = (r + g + b) / 3;
                            color = System.Drawing.Color.FromArgb((int)(intensity), (int)(intensity), (int)(intensity));

                            bitmap.SetPixel(i, j, color);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void Reload()
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                ImageSource imageSource = bitmapimage;
                ImageView.Source = imageSource;
            }
        }

        private void buttonHistogram_Click(object sender, RoutedEventArgs e)
        {
            HistogramClass histogram = new HistogramClass(bitmap);
            Histogram hisWindow = new Histogram(histogram);
            hisWindow.Show();
        }

        private void buttonBinar_Click(object sender, RoutedEventArgs e)
        {
            Thresholding win3 = new Thresholding();
            win3.Owner = this;
            win3.Show();
        }

        public void doBinarization(int value)
        {
            try
            {
                // Variable for image brightness
                double avgBright = 0;
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        // Get the brightness of this pixel
                        avgBright += bitmap.GetPixel(x, y).GetBrightness();
                    }
                }

                // Get the average brightness and limit it's min / max
                avgBright = avgBright / (bitmap.Width * bitmap.Height);
                avgBright = avgBright < .3 ? .3 : avgBright;
                avgBright = avgBright > .7 ? .7 : avgBright;

                for (int x = 0; x < bitmap.Width; x++)
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        if (bitmap.GetPixel(x, y).GetBrightness() > avgBright) bitmap.SetPixel(x, y, System.Drawing.Color.White);
                        else bitmap.SetPixel(x, y, System.Drawing.Color.Black);
                    }
                }
                Reload();
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                //MessageBox.Show(e.Message);
            };
        }

        private void buttonBezier_Click(object sender, RoutedEventArgs e)
        {
            state = 4;
        }

        private void buttonDoBezier_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas1.Children.Clear();
            Bezier(sender, e);
        }

        private void button2d_Click(object sender, RoutedEventArgs e)
        {
            state = 5;
            Grid_2d.Visibility = Visibility.Visible;
            grid_Change.Visibility = Visibility.Collapsed;
            grid_CMYK.Visibility = Visibility.Collapsed;
            Grid_LoadingFiles.Visibility = Visibility.Collapsed;
        }

        private void buttonKropka_Click(object sender, RoutedEventArgs e)
        {
            state = 6;
        }

        //private void MenuItem_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        //{

        //}

        private void Dilation_Click(object sender, RoutedEventArgs e)
        {
            var bitmap = MorphologicalOperators.Dilation(_directBitmap, GetIntMask());
            ImageSource imageSource = BitmapConverter.GetBitmapSource(bitmap.Bitmap);
            ImageView.Source = imageSource;
        }

        private void Erosion_Click(object sender, RoutedEventArgs e)
        {
            var bitmap = MorphologicalOperators.Erosion(_directBitmap, GetIntMask());
            ImageSource imageSource = BitmapConverter.GetBitmapSource(bitmap.Bitmap);
            ImageView.Source = imageSource;
        }

        private void Opening_Click(object sender, RoutedEventArgs e)
        {

            var bitmap = MorphologicalOperators.Opening(_directBitmap, GetIntMask());
            ImageSource imageSource = BitmapConverter.GetBitmapSource(bitmap.Bitmap);
            ImageView.Source = imageSource;
        }

        private void Closing_Click(object sender, RoutedEventArgs e)
        {
            var bitmap = MorphologicalOperators.Closing(_directBitmap, GetIntMask());
            ImageSource imageSource = BitmapConverter.GetBitmapSource(bitmap.Bitmap);
            ImageView.Source = imageSource;
        }

        private void Thinning_Click(object sender, RoutedEventArgs e)
        {
            var bitmap = MorphologicalOperators.Thinning(_directBitmap, GetIntMask());
            ImageSource imageSource = BitmapConverter.GetBitmapSource(bitmap.Bitmap);
            ImageView.Source = imageSource;
        }

        private void Thickening_Click(object sender, RoutedEventArgs e)
        {
            var bitmap = MorphologicalOperators.Thickening(_directBitmap, GetIntMask());
            ImageSource imageSource = BitmapConverter.GetBitmapSource(bitmap.Bitmap);
            ImageView.Source = imageSource;
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            stateButton = 0;
        }
        

        private void buttonChangeAlfa_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbPointX.Text) && string.IsNullOrWhiteSpace(tbPointY.Text) && string.IsNullOrWhiteSpace(tbAlfa.Text))
                return;

            changeLX = Convert.ToInt32(tbPointX.Text);
            changeLY = Convert.ToInt32(tbPointY.Text);
            changeL = Convert.ToInt32(tbAlfa.Text);
            if(state == 6)
            {
                var q = pointList[pointList.Count() - 1];
                changeLX = Convert.ToInt32(Canvas.GetLeft(q));
                changeLY = Convert.ToInt32(Canvas.GetTop(q));
            }
            double xp,yp;
            xp = changeLX + (x - changeLX)*Math.Cos(changeL)-(y-changeLY)*Math.Sin(changeL);
            yp = changeLY + (x - changeLX) * Math.Sin(changeL) - (y - changeLY) * Math.Cos(changeL);
            rectList[rectList.Count() - 1].SetValue(Canvas.LeftProperty, xp);
            rectList[rectList.Count() - 1].SetValue(Canvas.TopProperty, yp);
        }

        private void buttonAccept_Click(object sender, RoutedEventArgs e)
        {
            if(!(string.IsNullOrWhiteSpace(tbH.Text) ) && !(string.IsNullOrWhiteSpace(tbV.Text)))
            {
                stateButton = 1;
                h = Convert.ToInt32(tbH.Text);
                v = Convert.ToInt32(tbV.Text);
                //pointList[pointList.Count - 1].SetValue(Canvas.LeftProperty, points2[nextPoint].X + h);
                //pointList[pointList.Count - 1].SetValue(Canvas.TopProperty, points2[nextPoint].Y + v);
                //inkCanvas1.Children.Add(pointList[pointList.Count - 1]);
            }
        }

        private void ButtonChangeCMYK_OnClick(object sender, RoutedEventArgs e)
        {
            grid_Change.Visibility = Visibility.Collapsed;
            Grid_LoadingFiles.Visibility = Visibility.Collapsed;
            Grid_2d.Visibility = Visibility.Collapsed;
            double k, c, m, y;
            int r, g, b;
           
            if (mode==1)
            {
                if (string.IsNullOrWhiteSpace(tbK.Text) && (string.IsNullOrWhiteSpace(tbC.Text)) &&
                        (string.IsNullOrWhiteSpace(tbM.Text)) && (string.IsNullOrWhiteSpace(tbY.Text)))
                        return;
                if (tbC.Text.Contains(".") || tbM.Text.Contains(".") || tbY.Text.Contains(".") ||
                    tbK.Text.Contains("."))
                {
                    Error.Foreground=Brushes.Red;
                    Error.Text = "PO PRZECINKU!!!";
                    return;
                }

                c = Convert.ToDouble(tbC.Text);
                m = Convert.ToDouble(tbM.Text);
                y = Convert.ToDouble(tbY.Text);
                k = Convert.ToDouble(tbK.Text);

                //r = 1 - Math.Min(1, c * (1 - k) + k);
                //g = 1 - Math.Min(1, m * (1 - k) + k);
                //b = 1 - Math.Min(1, y * (1 - k) + k);
                r = Convert.ToInt32(255 * (1 - c) * (1 - k));
                g = Convert.ToInt32(255 * (1 - m) * (1 - k));
                b = Convert.ToInt32(255 * (1 - y) * (1 - k));
                tbR.Text = r.ToString();
                tbG.Text = g.ToString();
                tbB.Text = b.ToString();
                CanvasColor.Background = new SolidColorBrush(Color.FromRgb(Convert.ToByte(tbR.Text), Convert.ToByte(tbG.Text), Convert.ToByte(tbB.Text)));
            }
            if (mode == 2)
            {
                if ((string.IsNullOrWhiteSpace(tbR.Text)) && (string.IsNullOrWhiteSpace(tbG.Text)) &&
                    (string.IsNullOrWhiteSpace(tbB.Text)))
                    return;

                double rp, gp, bp;
                rp = (Convert.ToDouble(tbR.Text) / 255);
                gp = (Convert.ToDouble(tbG.Text) / 255);
                bp = (Convert.ToDouble(tbB.Text) / 255);
                k = 1 - Math.Max(rp, Math.Max(gp, bp));
                c = Convert.ToDouble((1 - rp - k) / (1 - k));
                m = Convert.ToDouble((1 - gp - k) / (1 - k));
                y = Convert.ToDouble((1 - bp - k) / (1 - k));
                tbC.Text = Math.Round(c, 5).ToString();
                tbM.Text = Math.Round(m, 5).ToString();
                tbY.Text = Math.Round(y, 5).ToString();
                tbK.Text = Math.Round(k, 5).ToString();
                CanvasColor.Background = new SolidColorBrush(Color.FromRgb(Convert.ToByte(tbR.Text), Convert.ToByte(tbG.Text), Convert.ToByte(tbB.Text)));
            }

           
        }

        private void ButtonRGB_OnClick(object sender, RoutedEventArgs e)
        {
            Error.Foreground = Brushes.Green;
            if (switcher == 1)
            {
                tbR.IsReadOnly = true;
                tbG.IsReadOnly = true;
                tbB.IsReadOnly = true;
                tbC.IsReadOnly = false;
                tbM.IsReadOnly = false;
                tbY.IsReadOnly = false;
                tbK.IsReadOnly = false;
                mode = 1;
                switcher = 2;
                Error.Text = "Zmieniasz CMK -> RGB";
                return;
            }
            if (switcher == 2)
            {
                tbR.IsReadOnly = false;
                tbG.IsReadOnly = false;
                tbB.IsReadOnly = false;
                tbC.IsReadOnly = true;
                tbM.IsReadOnly = true;
                tbY.IsReadOnly = true;
                tbK.IsReadOnly = true;
                mode = 2;
                switcher = 1;
                Error.Text = "Zmieniasz RGB->CMK";
                return;
            }
        }

        private void ButtonCMYK_OnClick(object sender, RoutedEventArgs e)
        {
            Grid_LoadingFiles.Visibility = Visibility.Collapsed;
            grid_CMYK.Visibility = Visibility.Visible;
            Grid_2d.Visibility = Visibility.Collapsed;
        }

        private void GenerateMaskView()
        {
            _mask = new IntViewModel[_maskSize, _maskSize];
            for (int i = 0; i < _maskSize; i++)
            {
                for (int j = 0; j < _maskSize; j++)
                {
                    _mask[i, j] = new IntViewModel(1);
                    if (i == _maskSize / 2 && j == _maskSize / 2)
                    {
                        _mask[i, j].Value = 2;
                    }
                    var textBox = new TextBox();
                    var binding = new Binding();
                    binding.Path = new PropertyPath("Value");
                    binding.Mode = BindingMode.TwoWay;
                    binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    binding.Source = _mask[i, j];
                    Grid.SetRow(textBox, i);
                    Grid.SetColumn(textBox, j);
                }
            }
        }

        private int[,] GetIntMask()
        {
            int[,] mask = new int[_maskSize, _maskSize];
            for (int i = 0; i < _maskSize; i++)
            {
                for (int j = 0; j < _maskSize; j++)
                {
                    mask[i, j] = _mask[i, j].Value;
                }
            }
            return mask;
        }
    }

}
