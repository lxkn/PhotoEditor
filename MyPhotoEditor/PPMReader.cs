using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MyPhotoEditor
{
    public class PPMReader
    {
       
        public bool P6type { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int maxNumber { get; set; }

        public Bitmap ReadFile(string filename, string filenametosave, int color = -1, double scaleValue = 0)
        {
            Bitmap bitmap;
            FileStream stream = new FileStream(filename, FileMode.Open);

            PPMReader header = new PPMReader();

            BinaryReader reader = new BinaryReader(stream);

            try
            {
                string checkType;
                ReadValue(reader, out checkType);
                if ("P3" == checkType) header.P6type = false;
                else if (checkType == "P6") header.P6type = true;
                else { throw new Exception("Plik niepoprawny"); }
                header.width = ReadValue(reader);
                header.height = ReadValue(reader);
                header.maxNumber = ReadValue(reader);
                bool scale = false;
                if (header.maxNumber > 255) scale = true;
                int divider = header.maxNumber / 255;

                int stride = 3 * header.width;
                byte[] image = new byte[header.width * header.height * 3];

                if (!header.P6type)
                {
                    int charsLeft = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
                    int index = 0;
                    string value = "";
                    char[] data = reader.ReadChars(charsLeft);
                    int i = 0;
                    while (Char.IsWhiteSpace(data[i])) i++;
                    for (; i < data.Length; i++)
                        if (data[i] == '#')
                        {
                            while (data[i] != '\n') i++;
                        }
                        else if (Char.IsWhiteSpace(data[i]) && !String.IsNullOrWhiteSpace(value))
                        {
                            var temp = int.Parse(value);
                            if (scale)
                            {
                                temp = temp / divider;
                            }
                            image[index] = (byte)temp;
                            value = "";
                            index++;
                        }
                        else
                        {
                            value += data[i];
                        }
                }
                else
                {
                    int bytesLeft = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
                    image = reader.ReadBytes(bytesLeft);
                }
                byte[] tempData = new byte[image.Length];
                for (int i = 0; i < image.Length; i++)
                {
                    tempData[i] = image[image.Length - 1 - i];
                }
                image = tempData;
                var pixelFormat = System.Drawing.Imaging.PixelFormat.Format24bppRgb;

                if (stride % 4 != 0)
                {
                    bitmap = new Bitmap(header.width, header.height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    Color pixelColor = new Color();
                    for (int x = 0; x < header.width; x++)
                    {
                        for (int y = 0; y < header.height; y++)
                        {
                            int i = x + y * header.width;
                            i = 3 * i;
                            switch (color)
                            {
                                case -1:
                                    pixelColor = Color.FromArgb(image[i + 2], image[i + 1], image[i]);
                                    break;
                                case 0:
                                    pixelColor = Color.FromArgb((int)(scaleValue * image[i + 2]), image[i + 1], image[i]);
                                    break;
                                case 1:
                                    pixelColor = Color.FromArgb(image[i + 2], (int)(scaleValue * image[i + 1]), image[i]);
                                    break;
                                case 2:
                                    pixelColor = Color.FromArgb(image[i + 2], image[i + 1], (int)(scaleValue * image[i]));
                                    break;
                            }
                            bitmap.SetPixel(x, y, pixelColor);
                        }
                    }
                }
                else
                {
                    IntPtr pImageData = Marshal.AllocHGlobal(image.Length);
                    Marshal.Copy(image, 0, pImageData, image.Length);
                    bitmap = new Bitmap(header.width, header.height, stride, pixelFormat, pImageData);
                }
                bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                bitmap.Save(filenametosave);
                stream.Close();
                return bitmap;
            }
            catch (Exception ex)
            {
                stream.Close();
                Console.WriteLine(ex);
            }
            stream.Close();
            return null;
        }

        private Bitmap CreateBitmap(PPMReader header, byte[] image)
        {
            return null;
        }

        private int ReadValue(BinaryReader reader)
        {
            string temp;
            int value = ReadValue(reader, out temp);
            if (value == -1) { return ReadValue(reader); }
            return value;
        }

        private int ReadValue(BinaryReader reader, out string stringValue)
        {
            stringValue = "";
            char temp = (char)reader.PeekChar();

            if (Char.IsWhiteSpace(temp))
            {
                while (Char.IsWhiteSpace(temp = (char)reader.PeekChar())) reader.ReadChar();
            }
            if (temp == '#')
            {
                while (temp != '\n')
                {
                    byte[] bytes = reader.ReadBytes(1);
                    var test = System.Text.Encoding.UTF8.GetString(bytes).ToCharArray();
                    if (test[0] == '\n') temp = '\n';

                }
            }
            if (Char.IsWhiteSpace(temp))
            {
                while (Char.IsWhiteSpace(temp = (char)reader.PeekChar())) reader.ReadChar();
            }
            while (!Char.IsWhiteSpace((char)reader.PeekChar()))
            {
                stringValue += reader.ReadChar().ToString();
            }
            int intResult;
            if (int.TryParse(stringValue, out intResult))
            {
                return intResult;
            }
            return -1;
        }
    }

}

