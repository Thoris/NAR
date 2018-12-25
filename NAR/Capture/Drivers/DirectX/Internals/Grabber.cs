using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;

namespace NAR.Capture.Drivers.DirectX.Internals
{
    //
    // Video grabber
    //
    public class Grabber : ISampleGrabberCB
    {
        #region Variables

        private int width, height;
        public IMediaSample _mediaSample;
        public long len;

        #endregion

        #region Properties

        // Width property
        public int Width
        {
            get { return width; }
            set { width = value; }
        }
        // Height property
        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        #endregion

        #region Constructors/Destructors

        // Constructor
        public Grabber()
        {

        }

        #endregion

        #region ISampleGrabberCB members

        // Callback to receive samples
        public int SampleCB(double sampleTime, IMediaSample sample)
        {

            _mediaSample = sample;


            

            //long pStart, pEnd;

            //sample.GetMediaTime(out pStart, out pEnd);



            //int hr;
            //IntPtr buffer;
            //AMMediaType mediaType;
            //VideoInfoHeader videoInfo;
            //int frameWidth;
            //int frameHeight;
            //int stride;
            //int bufferLength;



            //len = sample.GetSize();
            //hr = sample.GetPointer(out buffer);

            
            

            //hr = sample.GetMediaType(out mediaType);
            

            //bufferLength = sample.GetSize();

            //try
            //{
            //    videoInfo = new VideoInfoHeader();
            //    Marshal.PtrToStructure(mediaType.FormatPtr, videoInfo);

            //    frameWidth = videoInfo.BmiHeader.Width;
            //    frameHeight = videoInfo.BmiHeader.Height;
            //    stride = frameWidth * (videoInfo.BmiHeader.BitCount / 8);

            //    //CopyMemory(imageBuffer, buffer, bufferLength);

            //    //Bitmap bitmapOfFrame = new Bitmap(frameWidth, frameHeight, stride, PixelFormat.Format24bppRgb, buffer);
            //    //bitmapOfFrame.Save("C:\\Users\\...\\...\\...\\....jpg");


            //}
            //catch (Exception ex)
            //{

            //}

            return 0;

        }

        // Callback method that receives a pointer to the sample buffer
        public int BufferCB(double sampleTime, IntPtr buffer, int bufferLen)
        {
        
            // create new image
                System.Drawing.Bitmap image = new Bitmap(width, height, PixelFormat.Format24bppRgb);

                // lock bitmap data
                BitmapData imageData = image.LockBits(
                    new Rectangle(0, 0, width, height),
                    ImageLockMode.ReadWrite,
                    PixelFormat.Format24bppRgb);

                // copy image data
                int srcStride = imageData.Stride;
                int dstStride = imageData.Stride;

                unsafe
                {
                    byte* dst = (byte*)imageData.Scan0.ToPointer() + dstStride * (height - 1);
                    byte* src = (byte*)buffer.ToPointer();

                    for (int y = 0; y < height; y++)
                    {
                        Win32.memcpy(dst, src, srcStride);
                        dst -= dstStride;
                        src += srcStride;
                    }
                }

                // unlock bitmap data
                image.UnlockBits(imageData);

                //// notify parent
                //if (snapshotMode)
                //{
                //    parent.OnSnapshotFrame(image);
                //}
                //else
                //{
                //    parent.OnNewFrame(image);
                //}

                // release the image
                image.Dispose();





                //this.bufferedSize = BufferLen;

                //int stride = this.SnapShotWidth * 3;

                //Marshal.Copy(pBuffer, this.savedArray, 0, BufferLen);

                //GCHandle handle = GCHandle.Alloc(this.savedArray, GCHandleType.Pinned);
                //int scan0 = (int)handle.AddrOfPinnedObject();
                //scan0 += (this.SnapShotHeight - 1) * stride;
                //Bitmap b = new Bitmap(this.SnapShotWidth, this.SnapShotHeight, -stride,
                //    System.Drawing.Imaging.PixelFormat.Format24bppRgb, (IntPtr)scan0);
                //handle.Free();
                //SetBitmap = b;
                //return 0;
            return 0;
        }

        #endregion
    }
}
