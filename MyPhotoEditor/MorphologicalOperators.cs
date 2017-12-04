using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPhotoEditor
{
    public class MorphologicalOperators
    {
        public static DirectBitmap Dilation(DirectBitmap bitmap, int[,] mask)
        {
            DirectBitmap newBitmap = new DirectBitmap(bitmap.Bits, bitmap.Width, bitmap.Height);

            PointTransformation.ConvertToGray(newBitmap, GrayConversionMode.Colorimetric);

            var maskSize = (int)Math.Sqrt(mask.Length);
            for (int i = 0; i < bitmap.Height; i += 1)
            {
                for (int j = 0; j < bitmap.Width; j += 1)
                {
                    bool neighbourFound = false;
                    int k = 0;
                    int pointI = 0, pointJ = 0;
                    for (int z = maskSize / 2; z >= -1 * maskSize / 2; z--)
                    {
                        int l = 0;
                        for (int x = maskSize / 2; x >= -1 * maskSize / 2; x--)
                        {
                            var t = i - z;
                            var c = j - x;

                            t = Normalize(t, bitmap.Height);
                            c = Normalize(c, bitmap.Width);

                            var color = Color.FromArgb(bitmap.Bits[t * bitmap.Width + c]);

                            if (mask[k, l] == 1 && GetColor(color.R) == 255)
                            {
                                neighbourFound = true;
                            }
                            if (mask[k, l] == 2)
                            {
                                pointI = t;
                                pointJ = c;
                            }
                            l++;
                        }
                        k++;
                    }

                    if (neighbourFound)
                    {
                        newBitmap.Bits[pointI * bitmap.Width + pointJ] = Color.FromArgb(255, 255, 255).ToArgb();
                    }
                    else
                    {
                        newBitmap.Bits[pointI * bitmap.Width + pointJ] = Color.FromArgb(0, 0, 0).ToArgb();
                    }

                }
            }

            return newBitmap;
        }
        public static DirectBitmap Erosion(DirectBitmap bitmap, int[,] mask)
        {
            DirectBitmap newBitmap = new DirectBitmap(bitmap.Bits, bitmap.Width, bitmap.Height);

            PointTransformation.ConvertToGray(newBitmap, GrayConversionMode.Colorimetric);

            var maskSize = (int)Math.Sqrt(mask.Length);
            for (int i = 0; i < bitmap.Height; i += 1)
            {
                for (int j = 0; j < bitmap.Width; j += 1)
                {
                    bool neighbourFound = false;
                    int k = 0;
                    int pointI = 0, pointJ = 0;
                    for (int z = maskSize / 2; z >= -1 * maskSize / 2; z--)
                    {
                        int l = 0;
                        for (int x = maskSize / 2; x >= -1 * maskSize / 2; x--)
                        {
                            var t = i - z;
                            var c = j - x;

                            t = Normalize(t, bitmap.Height);
                            c = Normalize(c, bitmap.Width);

                            var color = Color.FromArgb(bitmap.Bits[t * bitmap.Width + c]);

                            if (mask[k, l] == 1 && GetColor(color.R) == 0)
                            {
                                neighbourFound = true;
                            }
                            if (mask[k, l] == 2)
                            {
                                pointI = t;
                                pointJ = c;
                            }
                            l++;
                        }
                        k++;
                    }

                    if (neighbourFound)
                    {
                        newBitmap.Bits[pointI * bitmap.Width + pointJ] = Color.FromArgb(0, 0, 0).ToArgb();
                    }
                    else
                    {
                        newBitmap.Bits[pointI * bitmap.Width + pointJ] = Color.FromArgb(255, 255, 255).ToArgb();
                    }

                }
            }

            return newBitmap;
        }
        public static DirectBitmap Opening(DirectBitmap bitmap, int[,] mask)
        {
            var erodedBitmap = Erosion(bitmap, mask);
            return Dilation(erodedBitmap, mask);
        }
        public static DirectBitmap Closing(DirectBitmap bitmap, int[,] mask)
        {
            var dilatedBitmap = Dilation(bitmap, mask);
            return Erosion(dilatedBitmap, mask);
        }
        public static DirectBitmap Thinning(DirectBitmap bitmap, int[,] mask)
        {
            DirectBitmap newBitmap = new DirectBitmap(bitmap.Bits, bitmap.Width, bitmap.Height);

            PointTransformation.ConvertToGray(newBitmap, GrayConversionMode.Colorimetric);

            var maskSize = (int)Math.Sqrt(mask.Length);
            for (int i = 0; i < bitmap.Height; i += 1)
            {
                for (int j = 0; j < bitmap.Width; j += 1)
                {
                    bool neighbourFound = false;
                    int k = 0;
                    int pointI = 0, pointJ = 0;
                    for (int z = maskSize / 2; z >= -1 * maskSize / 2; z--)
                    {
                        int l = 0;
                        for (int x = maskSize / 2; x >= -1 * maskSize / 2; x--)
                        {
                            var t = i - z;
                            var c = j - x;

                            t = Normalize(t, bitmap.Height);
                            c = Normalize(c, bitmap.Width);

                            var color = Color.FromArgb(bitmap.Bits[t * bitmap.Width + c]);

                            if (mask[k, l] == 1 && GetColor(color.R) == 0)
                            {
                                neighbourFound = true;
                            }
                            if (mask[k, l] == 2)
                            {
                                pointI = t;
                                pointJ = c;
                            }
                            l++;
                        }
                        k++;
                    }

                    if (neighbourFound)
                    {
                        newBitmap.Bits[pointI * bitmap.Width + pointJ] = Color.FromArgb(0, 0, 0).ToArgb();
                    }

                }
            }
            for (int i = 0; i < bitmap.Height; i += 1)
            {
                for (int j = 0; j < bitmap.Width; j += 1)
                {
                    var color = Color.FromArgb(bitmap.Bits[i * bitmap.Width + j]);
                    var newColor = Color.FromArgb(newBitmap.Bits[i * bitmap.Width + j]);
                    if (color.R > 125)
                    {
                        if (newColor.R == 255)
                        {
                            newBitmap.Bits[i * bitmap.Width + j] = Color.FromArgb(0, 0, 0).ToArgb();
                        }
                        else
                        {
                            newBitmap.Bits[i * bitmap.Width + j] = Color.FromArgb(255, 255, 255).ToArgb();
                        }
                    }
                    else
                    {
                        newBitmap.Bits[i * bitmap.Width + j] = Color.FromArgb(0, 0, 0).ToArgb();
                    }
                }
            }
            return newBitmap;
        }
        public static DirectBitmap Thickening(DirectBitmap bitmap, int[,] mask)
        {
            DirectBitmap newBitmap = new DirectBitmap(bitmap.Bits, bitmap.Width, bitmap.Height);

            PointTransformation.ConvertToGray(newBitmap, GrayConversionMode.Colorimetric);

            var maskSize = (int)Math.Sqrt(mask.Length);
            for (int i = 0; i < bitmap.Height; i += 1)
            {
                for (int j = 0; j < bitmap.Width; j += 1)
                {
                    bool neighbourFound = false;
                    int k = 0;
                    int pointI = 0, pointJ = 0;
                    for (int z = maskSize / 2; z >= -1 * maskSize / 2; z--)
                    {
                        int l = 0;
                        for (int x = maskSize / 2; x >= -1 * maskSize / 2; x--)
                        {
                            var t = i - z;
                            var c = j - x;

                            t = Normalize(t, bitmap.Height);
                            c = Normalize(c, bitmap.Width);

                            var color = Color.FromArgb(bitmap.Bits[t * bitmap.Width + c]);

                            if (mask[k, l] == 1 && GetColor(color.R) == 255)
                            {
                                neighbourFound = true;
                            }
                            if (mask[k, l] == 2)
                            {
                                pointI = t;
                                pointJ = c;
                            }
                            l++;
                        }
                        k++;
                    }

                    if (neighbourFound)
                    {
                        newBitmap.Bits[pointI * bitmap.Width + pointJ] = Color.FromArgb(255, 255, 255).ToArgb();
                    }

                }
            }
            for (int i = 0; i < bitmap.Height; i += 1)
            {
                for (int j = 0; j < bitmap.Width; j += 1)
                {
                    var color = Color.FromArgb(bitmap.Bits[i * bitmap.Width + j]);
                    var newColor = Color.FromArgb(newBitmap.Bits[i * bitmap.Width + j]);
                    if (color.R > 125)
                    {
                        newBitmap.Bits[i * bitmap.Width + j] = Color.FromArgb(255, 255, 255).ToArgb();
                    }
                }
            }
            return newBitmap;
        }
        private static int Normalize(int value, int maxValue)
        {
            if (value >= maxValue) return maxValue - 1;
            if (value < 0) return 0;
            return value;
        }
        private static int GetColor(int value)
        {
            if (value > 125) return 255;
            return 0;
        }
    }
}
