using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MyPhotoEditor
{
    public class DirectBitmap : IDisposable
    {
        public Bitmap Bitmap { get; private set; }
        public bool Disposed { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }

        protected GCHandle BitsHandle { get; private set; }

        public int[] Bits { get; private set; }
        public DirectBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Bits = new Int32[width * height];
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppArgb, BitsHandle.AddrOfPinnedObject());
        }
        public DirectBitmap(int[] bits, int width, int height)
        {
            Width = width;
            Height = height;
            Bits = new int[bits.Length];
            int idx = 0;
            foreach (int i in bits)
            {
                Bits[idx++] = i;
            }
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppArgb, BitsHandle.AddrOfPinnedObject());
        }
        public DirectBitmap(Bitmap bitmap)
        {
            Width = bitmap.Width;
            Height = bitmap.Height;
            Bits = new int[Width * Height];
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    var pixel = bitmap.GetPixel(j, i);
                    int R = pixel.R;
                    int G = pixel.G;
                    int B = pixel.B;
                    Bits[i * Width + j] = Color.FromArgb(R, G, B).ToArgb();
                }
            }
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(Width, Height, Width * 4, PixelFormat.Format32bppArgb, BitsHandle.AddrOfPinnedObject());
        }

        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            Bitmap.Dispose();
            BitsHandle.Free();
        }
    }
}
