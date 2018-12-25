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

namespace NAR.ImageProcessing.Corner
{

    /// <summary>
    /// Moravec corners detector.
    /// </summary>
    /// 
    /// <remarks><para>The class implements Moravec corners detector. For information about algorithm's
    /// details its <a href="http://www.cim.mcgill.ca/~dparks/CornerDetector/mainMoravec.htm">description</a>
    /// should be studied.</para>
    /// 
    /// <para><note>Due to limitations of Moravec corners detector (anisotropic response, etc.) its usage is limited
    /// to certain cases only.</note></para>
    /// 
    /// <para>The class processes only grayscale 8 bpp and color 24/32 bpp images.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create corner detector's instance
    /// MoravecCornersDetector mcd = new MoravecCornersDetector( );
    /// // process image searching for corners
    /// List&lt;IntPoint&gt; corners = scd.ProcessImage( image );
    /// // process points
    /// foreach ( IntPoint corner in corners )
    /// {
    ///     // ... 
    /// }
    /// </code>
    /// </remarks>
    /// 
    /// <seealso cref="SusanCornersDetector"/>
    /// 
    public class MoravecCornersCommand : CornerBase, ICommand, ICornerDetector
    {
        #region Variables

        private static int[] xDelta = new int[8] { -1, 0, 1, 1, 1, 0, -1, -1 };
        private static int[] yDelta = new int[8] { -1, -1, -1, 0, 1, 1, 1, 0 };

        // window size
        private int windowSize = 3;
        // threshold which is used to filter interest points
        private int threshold = 500;
        #endregion

        #region Properties

        /// <summary>
        /// Window size used to determine if point is interesting, [3, 15].
        /// </summary>
        /// 
        /// <remarks><para>The value specifies window size, which is used for initial searching of
        /// corners candidates and then for searching local maximums.</para>
        /// 
        /// <para>Default value is set to <b>3</b>.</para>
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">Setting value is not odd.</exception>
        /// 
        public int WindowSize
        {
            get { return windowSize; }
            set
            {
                // check if value is odd
                if ((value & 1) == 0)
                    throw new ArgumentException("The value shoule be odd.");

                windowSize = Math.Max(3, Math.Min(15, value));
            }
        }

        /// <summary>
        /// Threshold value, which is used to filter out uninteresting points.
        /// </summary>
        /// 
        /// <remarks><para>The value is used to filter uninteresting points - points which have value below
        /// specified threshold value are treated as not corners candidates. Increasing this value decreases
        /// the amount of detected point.</para>
        /// 
        /// <para>Default value is set to <b>500</b>.</para>
        /// </remarks>
        /// 
        public int Threshold
        {
            get { return threshold; }
            set { threshold = value; }
        }

        #endregion

        #region Constructors/Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MoravecCornersDetector"/> class.
        /// </summary>
        /// 
        /// <param name="threshold">Threshold value, which is used to filter out uninteresting points.</param>
        /// <param name="windowSize">Window size used to determine if point is interesting.</param>
        /// 
        public MoravecCornersCommand(int threshold, int windowSize, bool showPoints)
            : base (showPoints)
        {
            this.Threshold = threshold;
            this.WindowSize = windowSize;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Process image looking for corners.
        /// </summary>
        /// 
        /// <param name="image">Unmanaged source image to process.</param>
        /// 
        /// <returns>Returns array of found corners (X-Y coordinates).</returns>
        ///
        /// <exception cref="UnsupportedImageFormatException">The source image has incorrect pixel format.</exception>
        /// 
        public List<Point> ProcessImage(Model.IImage image)
        {
            //// check image format
            //if (
            //    (image.PixelFormat != PixelFormat.Format8bppIndexed) &&
            //    (image.PixelFormat != PixelFormat.Format24bppRgb) &&
            //    (image.PixelFormat != PixelFormat.Format32bppRgb) &&
            //    (image.PixelFormat != PixelFormat.Format32bppArgb)
            //    )
            //{
            //    throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
            //}

            // get source image size
            int width = image.Image.Width;
            int height = image.Image.Height;
            //int stride = image.Image.Stride;
            int pixelSize = Bitmap.GetPixelFormatSize(image.Image.PixelFormat) / 8;
            // window radius
            int windowRadius = windowSize / 2;

            // offset
            //int offset = stride - windowSize * pixelSize;

            // create moravec cornerness map
            int[,] moravecMap = new int[height, width];



                //byte* ptr = (byte*)image.ImageData.ToPointer();

                //// for each row
                //for (int y = windowRadius, maxY = height - windowRadius; y < maxY; y++)
                //{
                //    // for each pixel
                //    for (int x = windowRadius, maxX = width - windowRadius; x < maxX; x++)
                //    {
                //        int minSum = int.MaxValue;

                //        // go through 8 possible shifting directions
                //        for (int k = 0; k < 8; k++)
                //        {
                //            // calculate center of shifted window
                //            int sy = y + yDelta[k];
                //            int sx = x + xDelta[k];

                //            // check if shifted window is within the image
                //            if (
                //                (sy < windowRadius) || (sy >= maxY) ||
                //                (sx < windowRadius) || (sx >= maxX)
                //            )
                //            {
                //                // skip this shifted window
                //                continue;
                //            }

                //            int sum = 0;

                //            byte* ptr1 = ptr + (y - windowRadius) * stride + (x - windowRadius) * pixelSize;
                //            byte* ptr2 = ptr + (sy - windowRadius) * stride + (sx - windowRadius) * pixelSize;

                //            // for each windows' rows
                //            for (int i = 0; i < windowSize; i++)
                //            {
                //                // for each windows' pixels
                //                for (int j = 0, maxJ = windowSize * pixelSize; j < maxJ; j++, ptr1++, ptr2++)
                //                {
                //                    int dif = *ptr1 - *ptr2;
                //                    sum += dif * dif;
                //                }
                //                ptr1 += offset;
                //                ptr2 += offset;
                //            }

                //            // check if the sum is mimimal
                //            if (sum < minSum)
                //            {
                //                minSum = sum;
                //            }
                //        }

                //        // threshold the minimum sum
                //        if (minSum < threshold)
                //        {
                //            minSum = 0;
                //        }

                //        moravecMap[y, x] = minSum;
                //    }
                //}
            






            // collect interesting points - only those points, which are local maximums
            List<Point> cornersList = new List<Point>();

            // for each row
            for (int y = windowRadius, maxY = height - windowRadius; y < maxY; y++)
            {
                // for each pixel
                for (int x = windowRadius, maxX = width - windowRadius; x < maxX; x++)
                {
                    int currentValue = moravecMap[y, x];

                    // for each windows' rows
                    for (int i = -windowRadius; (currentValue != 0) && (i <= windowRadius); i++)
                    {
                        // for each windows' pixels
                        for (int j = -windowRadius; j <= windowRadius; j++)
                        {
                            if (moravecMap[y + i, x + j] > currentValue)
                            {
                                currentValue = 0;
                                break;
                            }
                        }
                    }

                    // check if this point is really interesting
                    if (currentValue != 0)
                    {
                        cornersList.Add(new Point(x, y));
                    }
                }
            }

            return cornersList;
        }

        public Model.IImage Execute(Model.IImage image)
        {


            // get source image size
            int width = image.Image.Width;
            int height = image.Image.Height;

            
            //int pixelSize = Bitmap.GetPixelFormatSize(image.Image.PixelFormat) / 8;
            int pixelSize = 3;
            
            int stride = width * pixelSize;


            // window radius
            int windowRadius = windowSize / 2;

            // offset
            int offset = stride - windowSize * pixelSize;

            // create moravec cornerness map
            int[,] moravecMap = new int[height, width];


            int pos = 0;


            //byte* ptr = (byte*)image.ImageData.ToPointer();

            // for each row
            for (int y = windowRadius, maxY = height - windowRadius; y < maxY; y++)
            {
                // for each pixel
                for (int x = windowRadius, maxX = width - windowRadius; x < maxX; x++)
                {
                    int minSum = int.MaxValue;

                    // go through 8 possible shifting directions
                    for (int k = 0; k < 8; k++)
                    {
                        // calculate center of shifted window
                        int sy = y + yDelta[k];
                        int sx = x + xDelta[k];

                        // check if shifted window is within the image
                        if (
                            (sy < windowRadius) || (sy >= maxY) ||
                            (sx < windowRadius) || (sx >= maxX)
                        )
                        {
                            // skip this shifted window
                            continue;
                        }

                        int sum = 0;

                        int posPtr1 = pos + (y - windowRadius) * stride + (x - windowRadius) * pixelSize;
                        int posPtr2 = pos + (sy - windowRadius) * stride + (sx - windowRadius) * pixelSize;

                        //int ptr1 = image.Bytes[posPtr1];
                        //int ptr2 = image.Bytes[posPtr2];

                        //byte* ptr1 = ptr + (y - windowRadius) * stride + (x - windowRadius) * pixelSize;
                        //byte* ptr2 = ptr + (sy - windowRadius) * stride + (sx - windowRadius) * pixelSize;

                        // for each windows' rows
                        for (int i = 0; i < windowSize; i++)
                        {
                            // for each windows' pixels
                            for (int j = 0, maxJ = windowSize * pixelSize; j < maxJ; j++, posPtr1++, posPtr2++)
                            {
                                int dif = image.Bytes[posPtr1] - image.Bytes[posPtr2];
                                sum += dif * dif;
                            }
                            posPtr1 += offset;
                            posPtr2 += offset;
                        }

                        // check if the sum is mimimal
                        if (sum < minSum)
                        {
                            minSum = sum;
                        }
                    }

                    // threshold the minimum sum
                    if (minSum < threshold)
                    {
                        minSum = 0;
                    }

                    moravecMap[y, x] = minSum;
                }
            }







            // collect interesting points - only those points, which are local maximums
            _cornersList = new List<Point>();

            // for each row
            for (int y = windowRadius, maxY = height - windowRadius; y < maxY; y++)
            {
                // for each pixel
                for (int x = windowRadius, maxX = width - windowRadius; x < maxX; x++)
                {
                    int currentValue = moravecMap[y, x];

                    // for each windows' rows
                    for (int i = -windowRadius; (currentValue != 0) && (i <= windowRadius); i++)
                    {
                        // for each windows' pixels
                        for (int j = -windowRadius; j <= windowRadius; j++)
                        {
                            if (moravecMap[y + i, x + j] > currentValue)
                            {
                                currentValue = 0;
                                break;
                            }
                        }
                    }

                    // check if this point is really interesting
                    if (currentValue != 0)
                    {
                        _cornersList.Add(new Point(x, y));
                    }
                }
            }


            if (_showPoints)
                return base.ShowPoints(image);
            else
                return image;
        }
        #endregion
    }
}
