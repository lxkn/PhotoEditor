using System;
using System.Drawing;

namespace MyPhotoEditor
{
    public class PointTransformation
    {
        private static int[,] _originalChannelValues;
        private static int[,] _realChannelValues;
        public static void InitTransform(DirectBitmap bitmap)
        {
            _originalChannelValues = new int[3, bitmap.Width * bitmap.Height];
            _realChannelValues = new int[3, bitmap.Width * bitmap.Height];
            for (int i = 0; i < bitmap.Height * bitmap.Width; i++)
            {
                var color = Color.FromArgb(bitmap.Bits[i]);
                _originalChannelValues[0, i] = color.R;
                _originalChannelValues[1, i] = color.G;
                _originalChannelValues[2, i] = color.B;

                _realChannelValues[0, i] = color.R;
                _realChannelValues[1, i] = color.G;
                _realChannelValues[2, i] = color.B;

            }
        }
        public static void ConvertToGray(DirectBitmap bitmap, GrayConversionMode mode)
        {
            for (int i = 0; i < bitmap.Height * bitmap.Width; i++)
            {
                var color = Color.FromArgb(bitmap.Bits[i]);
                int R = color.R;
                int G = color.G;
                int B = color.B;

                if (mode == GrayConversionMode.Average)
                {
                    int avg = (R + G + B) / 3;
                    R = avg;
                    G = avg;
                    B = avg;
                }
                else
                {
                    int grayScale = (int)((R * 0.3) + (G * 0.59) + (B * 0.11));
                    R = grayScale;
                    G = grayScale;
                    B = grayScale;
                }
                bitmap.Bits[i] = Color.FromArgb(R, G, B).ToArgb();
            }
        }
        public static void ModifyBrightness(DirectBitmap bitmap, int value)
        {
            for (int i = 0; i < bitmap.Height * bitmap.Width; i++)
            {
                var color = Color.FromArgb(bitmap.Bits[i]);
                int R = color.R;
                int G = color.G;
                int B = color.B;

                ChangeChannelBrightness(ref R, ref _originalChannelValues[0, i], value);
                ChangeChannelBrightness(ref G, ref _originalChannelValues[1, i], value);
                ChangeChannelBrightness(ref B, ref _originalChannelValues[2, i], value);

                _realChannelValues[0, i] = R;
                _realChannelValues[1, i] = G;
                _realChannelValues[2, i] = B;

                bitmap.Bits[i] = Color.FromArgb(R, G, B).ToArgb();
            }

        }
        private static void MultiplyChannelByValue(ref int channel, ref int realChannelValue, double value)
        {
            if (realChannelValue * value >= 0 && realChannelValue * value <= 255)
            {
                channel = Convert.ToInt32(realChannelValue * value);
            }
            else if (realChannelValue * value < 0) channel = 0;
            else if (realChannelValue * value > 255) channel = 255;
        }
        private static void AddValueToChannel(ref int channel, ref int realChannelValue, int value)
        {
            if (channel + value >= 0 && channel + value <= 255)
            {
                channel = channel + value;
            }
            else if (channel + value < 0) channel = 0;
            else if (channel + value > 255) channel = 255;
        }
        private static void ChangeChannelBrightness(ref int channel, ref int originalChannelValue, int value)
        {
            if (originalChannelValue + value >= 0 && originalChannelValue + value <= 255)
            {
                channel = originalChannelValue + value;
            }
            else if (originalChannelValue + value < 0) channel = 0;
            else if (originalChannelValue + value > 255) channel = 255;
        }
    }
}
