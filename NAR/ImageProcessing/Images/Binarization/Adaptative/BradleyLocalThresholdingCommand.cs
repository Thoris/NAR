using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ImageProcessing.Images.Binarization.Adaptative
{
    public class BradleyLocalThresholdingCommand : IAdaptativeBinarization, ICommand
    {
        #region Variables
        private int _windowSize = 41;
        private float _pixelBrightnessDifferenceLimit = 0.15f;
        #endregion

        #region Properties
        /// <summary>
        /// Window size to calculate average value of pixels for.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies window size around processing pixel, which determines number of
        /// neighbor pixels to use for calculating their average brightness.</para>
        /// 
        /// <para>Default value is set to <b>41</b>.</para>
        /// 
        /// <para><note>The value should be odd.</note></para>
        /// </remarks>
        /// 
        public int WindowSize
        {
            get { return _windowSize; }
            set { _windowSize = Math.Max(3, value | 1); }
        }
        #endregion

        #region Constructors/Destructors
        public BradleyLocalThresholdingCommand(int windowSize, float pixelBrightnessDifferenceLimit)
        {
            _windowSize = windowSize;
            _pixelBrightnessDifferenceLimit = pixelBrightnessDifferenceLimit;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Calculate mean value of pixels in the specified rectangle without checking it's coordinates.
        /// </summary>
        /// 
        /// <param name="x1">X coordinate of left-top rectangle's corner.</param>
        /// <param name="y1">Y coordinate of left-top rectangle's corner.</param>
        /// <param name="x2">X coordinate of right-bottom rectangle's corner.</param>
        /// <param name="y2">Y coordinate of right-bottom rectangle's corner.</param>
        /// 
        /// <returns>Returns mean value of pixels in the specified rectangle.</returns>
        /// 
        /// <remarks>Both specified points are included into the calculation rectangle.</remarks>
        /// 
        private float GetRectangleMeanUnsafe(uint[,] integralImage, int x1, int y1, int x2, int y2)
        {
            x2++;
            y2++;

            
            // return sum divided by actual rectangles size
            return (float)((double)(integralImage[y2, x2] + integralImage[y1, x1] - integralImage[y2, x1] - integralImage[y1, x2]) /
                (double)((x2 - x1) * (y2 - y1)));
        }
        private uint[,] FromBitmap(Model.IImage image)
        {
            int width = image.Image.Width;
            int height = image.Image.Height;

            uint[,] integralImage = new uint[height + 1, width + 1];
            int size = width * height * 3;

            //Creating the size of bytes similar to the image considering 3 bytes of each pixel (RGB)
            byte[] result = new byte[size];

            Array.Copy(image.Bytes, result, size);

            int pos = 0;

            // for each line
            for (int y = 1; y <= height; y++)
            {
                uint rowSum = 0;

                // for each pixel
                for (int x = 1; x <= width; x++, pos+= 3)
                {
                    rowSum += result[pos];

                    integralImage[y, x] = rowSum + integralImage[y - 1, x];
                }
                //src += offset;
            }

            return integralImage;

        }

        #endregion


        #region ICommand members
        public Model.IImage Execute(Model.IImage image)
        {

    

            int width = image.Image.Width;
            int height = image.Image.Height;
            int widthM1 = width - 1;
            int heightM1 = height - 1;

            int stride = width * 3;

            int offset = stride - width;
            int radius = _windowSize / 2;

            float avgBrightnessPart = 1.0f - _pixelBrightnessDifferenceLimit;
            int size = width * height * 3;
            
            //Creating the size of bytes similar to the image considering 3 bytes of each pixel (RGB)
            byte[] result = new byte[size];

            Array.Copy(image.Bytes, result, size);

            int pos = 0;

            //byte* ptr = (byte*)image.ImageData.ToPointer();

            uint[,] integralImage = FromBitmap(image);
            


            for (int y = 0; y < height; y++)
            {
                // rectangle's Y coordinates
                int y1 = y - radius;
                int y2 = y + radius;

                if (y1 < 0)
                    y1 = 0;
                if (y2 > heightM1)
                    y2 = heightM1;

                for (int x = 0; x < width; x++, pos+=3)
                {
                    // rectangle's X coordinates
                    int x1 = x - radius;
                    int x2 = x + radius;

                    if (x1 < 0)
                        x1 = 0;
                    if (x2 > widthM1)
                        x2 = widthM1;

                    result[pos] = (byte)((result[pos] < (int)(GetRectangleMeanUnsafe(integralImage, x1, y1, x2, y2) * avgBrightnessPart)) ? 0 : 255);
                    result[pos + 1] = result[pos];
                    result[pos + 2] = result[pos];
                }

                //ptr += offset;
            }

            return new Model.ImageBitmap(width, height, result);
        }
        #endregion
    }
}
