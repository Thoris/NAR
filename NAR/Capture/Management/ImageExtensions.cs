/******************************************************************************
* Copyright (c) 2012, TAP Consulting Group
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither TAP Consulting Group nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY TAP Consulting Group ''AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL TAP Consulting Group BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace NAR.Capture.Management
{
    public sealed class ImageExtensions
    {
        #region Constructors/Destructors

        private ImageExtensions()
        {

        }

        #endregion

        #region Methods
        public  Bitmap BitmapTo1Bpp(Bitmap img)
        {

            int w = img.Width;

            int h = img.Height;

            Bitmap bmp = new Bitmap(w, h, PixelFormat.Format1bppIndexed);

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);

            for (int y = 0; y < h; y++)
            {

                byte[] scan = new byte[(w + 7) / 8];

                for (int x = 0; x < w; x++)
                {

                    Color c = img.GetPixel(x, y);

                    if (c.GetBrightness() >= 0.5) scan[x / 8] |= (byte)(0x80 >> (x % 8));

                }

                Marshal.Copy(scan, 0, (IntPtr)((int)data.Scan0 + data.Stride * y), scan.Length);

            }

            bmp.UnlockBits(data);

            return bmp;

        }
        public void GetRGB(Bitmap image, int startX, int startY, int w, int h, int[] rgbArray, int offset, int scansize)
        {
            const int PixelWidth = 3;
            const PixelFormat PixelFormat = PixelFormat.Format24bppRgb;

            // En garde!
            if (image == null) throw new ArgumentNullException("image");
            if (rgbArray == null) throw new ArgumentNullException("rgbArray");
            if (startX < 0 || startX + w > image.Width) throw new ArgumentOutOfRangeException("startX");
            if (startY < 0 || startY + h > image.Height) throw new ArgumentOutOfRangeException("startY");
            if (w < 0 || w > scansize || w > image.Width) throw new ArgumentOutOfRangeException("w");
            if (h < 0 || (rgbArray.Length < offset + h * scansize) || h > image.Height) throw new ArgumentOutOfRangeException("h");

            BitmapData data = image.LockBits(new Rectangle(startX, startY, w, h), System.Drawing.Imaging.ImageLockMode.ReadOnly, PixelFormat);
            try
            {
                byte[] pixelData = new Byte[data.Stride];
                for (int scanline = 0; scanline < data.Height; scanline++)
                {
                    Marshal.Copy(data.Scan0 + (scanline * data.Stride), pixelData, 0, data.Stride);
                    for (int pixeloffset = 0; pixeloffset < data.Width; pixeloffset++)
                    {
                        // PixelFormat.Format32bppRgb means the data is stored
                        // in memory as BGR. We want RGB, so we must do some 
                        // bit-shuffling.
                        rgbArray[offset + (scanline * scansize) + pixeloffset] =
                            (pixelData[pixeloffset * PixelWidth + 2] << 16) +   // R 
                            (pixelData[pixeloffset * PixelWidth + 1] << 8) +    // G
                            pixelData[pixeloffset * PixelWidth];                // B
                    }
                }
            }
            finally
            {
                image.UnlockBits(data);
            }
        }

        private int[] getRGB(Bitmap bmp, int line)
        {
            var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            try
            {
                var ptr = (IntPtr)((long)data.Scan0 + data.Stride * (bmp.Height - line - 1));
                var ret = new int[bmp.Width];
                System.Runtime.InteropServices.Marshal.Copy(ptr, ret, 0, ret.Length * 4);
                return ret;
            }
            finally
            {
                bmp.UnlockBits(data);
            }
        }

        /// <summary>
        /// Copies a bitmap into a 1bpp/8bpp bitmap of the same dimensions, fast
        /// </summary>
        /// <param name="b">original bitmap</param>
        /// <param name="bpp">1 or 8, target bpp</param>
        /// <returns>a 1bpp copy of the bitmap</returns>
        static System.Drawing.Bitmap CopyToBpp(System.Drawing.Bitmap b, int bpp)
        {
            if (bpp != 1 && bpp != 8) throw new System.ArgumentException("1 or 8", "bpp");

            // Plan: built into Windows GDI is the ability to convert
            // bitmaps from one format to another. Most of the time, this
            // job is actually done by the graphics hardware accelerator card
            // and so is extremely fast. The rest of the time, the job is done by
            // very fast native code.
            // We will call into this GDI functionality from C#. Our plan:
            // (1) Convert our Bitmap into a GDI hbitmap (ie. copy unmanaged->managed)
            // (2) Create a GDI monochrome hbitmap
            // (3) Use GDI "BitBlt" function to copy from hbitmap into monochrome (as above)
            // (4) Convert the monochrone hbitmap into a Bitmap (ie. copy unmanaged->managed)

            int w = b.Width, h = b.Height;
            IntPtr hbm = b.GetHbitmap(); // this is step (1)
            //
            // Step (2): create the monochrome bitmap.
            // "BITMAPINFO" is an interop-struct which we define below.
            // In GDI terms, it's a BITMAPHEADERINFO followed by an array of two RGBQUADs
            Capture.Drivers.WebCamWinAPI.API.BitmapInfoHeader bmi = new Capture.Drivers.WebCamWinAPI.API.BitmapInfoHeader();
            bmi.biSize = 40;  // the size of the BITMAPHEADERINFO struct
            bmi.biWidth = w;
            bmi.biHeight = h;
            bmi.biPlanes = 1; // "planes" are confusing. We always use just 1. Read MSDN for more info.
            bmi.biBitCount = (short)bpp; // ie. 1bpp or 8bpp
            bmi.biCompression = 0; // ie. the pixels in our RGBQUAD table are stored as RGBs, not palette indexes
            bmi.biSizeImage = (int)(((w + 7) & 0xFFFFFFF8) * h / 8);
            bmi.biXPelsPerMeter = 1000000; // not really important
            bmi.biYPelsPerMeter = 1000000; // not really important
            // Now for the colour table.
            int ncols = (int)1 << bpp; // 2 colours for 1bpp; 256 colours for 8bpp
            bmi.biClrUsed = ncols;
            bmi.biClrImportant = ncols;
            
            bmi.cols = new uint[256]; // The structure always has fixed size 256, even if we end up using fewer colours
            if (bpp == 1) { bmi.cols[0] = Capture.Drivers.WebCamWinAPI.API.Function.MAKERGB(0, 0, 0); bmi.cols[1] = Capture.Drivers.WebCamWinAPI.API.Function.MAKERGB(255, 255, 255); }
            else { for (int i = 0; i < ncols; i++) bmi.cols[i] = Capture.Drivers.WebCamWinAPI.API.Function.MAKERGB(i, i, i); }
            // For 8bpp we've created an palette with just greyscale colours.
            // You can set up any palette you want here. Here are some possibilities:
            // greyscale: for (int i=0; i<256; i++) bmi.cols[i]=MAKERGB(i,i,i);
            // rainbow: bmi.biClrUsed=216; bmi.biClrImportant=216; int[] colv=new int[6]{0,51,102,153,204,255};
            //          for (int i=0; i<216; i++) bmi.cols[i]=MAKERGB(colv[i/36],colv[(i/6)%6],colv[i%6]);
            // optimal: a difficult topic: http://en.wikipedia.org/wiki/Color_quantization
            // 
            // Now create the indexed bitmap "hbm0"
            IntPtr bits0; // not used for our purposes. It returns a pointer to the raw bits that make up the bitmap.
            IntPtr hbm0 = Capture.Drivers.WebCamWinAPI.API.Function.CreateDIBSection(IntPtr.Zero, ref bmi, 0, out bits0, IntPtr.Zero, 0);
            //
            // Step (3): use GDI's BitBlt function to copy from original hbitmap into monocrhome bitmap
            // GDI programming is kind of confusing... nb. The GDI equivalent of "Graphics" is called a "DC".
            IntPtr sdc = Capture.Drivers.WebCamWinAPI.API.Function.GetDC(IntPtr.Zero);       // First we obtain the DC for the screen
            // Next, create a DC for the original hbitmap
            IntPtr hdc = Capture.Drivers.WebCamWinAPI.API.Function.CreateCompatibleDC(sdc); Capture.Drivers.WebCamWinAPI.API.Function.SelectObject(hdc, hbm);
            // and create a DC for the monochrome hbitmap
            IntPtr hdc0 = Capture.Drivers.WebCamWinAPI.API.Function.CreateCompatibleDC(sdc); Capture.Drivers.WebCamWinAPI.API.Function.SelectObject(hdc0, hbm0);
            // Now we can do the BitBlt:
            Capture.Drivers.WebCamWinAPI.API.Function.BitBlt(hdc0, 0, 0, w, h, hdc, 0, 0, Capture.Drivers.WebCamWinAPI.API.Function.SRCCOPY);
            // Step (4): convert this monochrome hbitmap back into a Bitmap:
            System.Drawing.Bitmap b0 = System.Drawing.Bitmap.FromHbitmap(hbm0);
            //
            // Finally some cleanup.
            Capture.Drivers.WebCamWinAPI.API.Function.DeleteDC(hdc);
            Capture.Drivers.WebCamWinAPI.API.Function.DeleteDC(hdc0);
            Capture.Drivers.WebCamWinAPI.API.Function.ReleaseDC(IntPtr.Zero, sdc);
            Capture.Drivers.WebCamWinAPI.API.Function.DeleteObject(hbm);
            Capture.Drivers.WebCamWinAPI.API.Function.DeleteObject(hbm0);
            //
            return b0;
        }

        

        public byte[] BitmapToByteArray(Image image, ImageFormat format)
        {
            //// Lock source bitmap in memory
            //BitmapData sourceData = ((Bitmap)image).LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            //// Copy image data to binary array
            //int imageSize = sourceData.Stride * sourceData.Height;
            //byte[] sourceBuffer = new byte[imageSize];
            //Marshal.Copy(sourceData.Scan0, sourceBuffer, 0, imageSize);

            //// Unlock source bitmap
            //((Bitmap)image).UnlockBits(sourceData);

            return null; // GetBytes((Bitmap)image);


            //int[] res = getRGB((Bitmap)image, 1);
            //if (res != null)
            //{
            //}

            ////Desabilitando para trabalhar com o bitmap
            //BitmapData bmData = image.LockBits(new Rectangle(0, 0, _bmpBitmap.Width, _bmpBitmap.Height),
            //    ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            //int total = image.Height * image.Width * 3;
            ////Criando o vetor da imagem
            //byte[] abyImage = new byte[total];

            ////Copiando a imagem no vetor
            //System.Runtime.InteropServices.Marshal.Copy(image.ha, abyImage, 0, total);

            ////Habilitando para trabalhar novamente com o bitmaop
            //_bmpBitmap.UnlockBits(bmData);

            //ImageConverter converter = new ImageConverter();


            //ImageConverter converter = new ImageConverter();
            //return (byte[])converter.ConvertTo(image, typeof(byte[]));



            //Bitmap img = new Bitmap(image); // 32-bit
            //Bitmap img8bit = new Bitmap(image.Width, image.Height, PixelFormat.Format8bppIndexed); // 8-bit

            // copy img to img8bit -- HOW?
            //img8bit.Save(imgNewPath, ImageFormat.Png);


            //System.Drawing.Bitmap b0 = CopyToBpp((Bitmap)image, 8);


            //using (MemoryStream ms = new MemoryStream())
            //{
            //    //BitmapTo1Bpp((Bitmap)image).Save(ms, format);

            //    image.Save(ms, format);
            //    return ms.ToArray();
            //}
        }
        public Bitmap ByteArrayToBitmap(byte[] byteArray)
        {

            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                Bitmap img = (Bitmap)Image.FromStream(ms);
                return img;
            }

        }
        //private unsafe byte[] BmpToBytes_Unsafe(Bitmap bmp)
        //{
        //    BitmapData bData = bmp.LockBits(new Rectangle(new Point(), bmp.Size),
        //        ImageLockMode.ReadOnly,
        //        PixelFormat.Format24bppRgb);
        //    // number of bytes in the bitmap
        //    int byteCount = bData.Stride * bmp.Height;
        //    byte[] bmpBytes = new byte[byteCount];

        //    // Copy the locked bytes from memory
        //    Marshal.Copy(bData.Scan0, bmpBytes, 0, byteCount);

        //    // don't forget to unlock the bitmap!!
        //    bmp.UnlockBits(bData);

        //    return bmpBytes;
        //}
        //private unsafe Bitmap BytesToBmp(byte[] bmpBytes, Size imageSize)
        //{
        //    Bitmap bmp = new Bitmap(imageSize.Width, imageSize.Height);

        //    BitmapData bData = bmp.LockBits(new Rectangle(new Point(), bmp.Size),
        //        ImageLockMode.WriteOnly,
        //        PixelFormat.Format24bppRgb);

        //    // Copy the bytes to the bitmap object
        //    Marshal.Copy(bmpBytes, 0, bData.Scan0, bmpBytes.Length);
        //    bmp.UnlockBits(bData);
        //    return bmp;
        //}






        /// <summary>
        /// Converts the byte array to bitmap
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="bytes">The byte array which contains data from image.</param>
        /// <returns>Bitmap converted</returns>
        public static Bitmap ConvertByteArrayToBitmap(int width, int height, byte[] bytes)
        {

            //Creating the new bitmap to be populed from byte array
            Bitmap image = new Bitmap(width, height);

            //Starting the image attributes locking that
            BitmapData BitMap1Data = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            //Getting the address from the first pixel
            IntPtr dataPtr = BitMap1Data.Scan0; //This assumes you've created a IntPtr dataPtr somewhere else. 

            //Copying the byte array
            Marshal.Copy(bytes, 0, dataPtr, width * height * 3);

            //Unlocking the image
            image.UnlockBits(BitMap1Data);

            return image;

        }
        /// <summary>
        /// Converts the image to byte array
        /// </summary>
        /// <param name="image">The image to be extracted.</param>
        /// <returns>array of bytes extracted from the image</returns>
        public static byte[] ConvertBitmapToByteArray(Bitmap image)
        {
            int width = image.Width;
            int height = image.Height;
            int sizeLen = width * height * 3;


            //Disabling the original image to work with bitmap
            BitmapData bmData = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            //creating the vetor to populate the image 
            byte[] abyImage = new byte[sizeLen];

            //Copying the image to the vector
            System.Runtime.InteropServices.Marshal.Copy(bmData.Scan0, abyImage, 0, sizeLen);

            //Enabling the original image
            image.UnlockBits(bmData);

            return abyImage;
        }
        #endregion
    }
}
