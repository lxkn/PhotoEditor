using System;
using System.Collections.Generic;
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
        private Rectangle rectangle;
        private Ellipse ellipse;
        private Line line;
        private List<Line> lines;
        private List<Ellipse> ellipses;
        private List<Rectangle> rectList;
        private double x;
        private double y;
        private Color color;
        private int mode;
        private int switcher;
        Bitmap bitmap;
        int state;
        private bool buttonRClicked, buttonEClicked, buttonLClicked, buttonCClicked;
        public MainWindow()
        {
            InitializeComponent();
            grid_Change.Visibility = Visibility.Collapsed;
            grid_CMYK.Visibility = Visibility.Collapsed;
            rectList = new List<Rectangle>();
            ellipses = new List<Ellipse>();
            lines = new List<Line>();
            color = new Color();
            ClrPckerBackground.SelectedColor = Color.FromArgb(150, 000, 000, 000);
            inkCanvas1.Background = new SolidColorBrush(Color.FromRgb(246, 246, 246));
            tbR.IsReadOnly = true;
            tbG.IsReadOnly = true;
            tbB.IsReadOnly = true;
            switcher = 2;
            mode = 1;
            Error.Foreground = Brushes.Green;
            Error.Text = "Zmieniasz CMK -> RGB";
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
            }
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
            ButtonLine.Background=Brushes.CornflowerBlue;
            grid_CMYK.Visibility = Visibility.Collapsed;
        }

        private void ButtonEllipse_OnClick(object sender, RoutedEventArgs e)
        {
            ButtonRectangle.Background = Brushes.Transparent;
            ButtonLine.Background = Brushes.Transparent;
            state = 2;
            grid_Change.Visibility = Visibility.Visible;
            ButtonEllipse.Background = Brushes.CornflowerBlue;
            grid_CMYK.Visibility = Visibility.Collapsed;
        }

        private void ButtonRectangle_OnClick(object sender, RoutedEventArgs e)
        {

            ButtonEllipse.Background = Brushes.Transparent;
            ButtonLine.Background = Brushes.Transparent;
            state = 1;
            grid_Change.Visibility = Visibility.Visible;
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

        private void ButtonChangeCMYK_OnClick(object sender, RoutedEventArgs e)
        {
            grid_Change.Visibility = Visibility.Collapsed;
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

            grid_CMYK.Visibility = Visibility.Visible;
        }
    }

}
