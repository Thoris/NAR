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
using System.Collections;

namespace NAR.ImageProcessing.Segmentation
{
    /// <summary>
    /// Hough circle transformation.
    /// </summary>
    ///
    /// <remarks><para>The class implements Hough circle transformation, which allows to detect
    /// circles of specified radius in an image.</para>
    /// 
    /// <para>The class accepts binary images for processing, which are represented by 8 bpp grayscale images.
    /// All black pixels (0 pixel's value) are treated as background, but pixels with different value are
    /// treated as circles' pixels.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// HoughCircleTransformation circleTransform = new HoughCircleTransformation( 35 );
    /// // apply Hough circle transform
    /// circleTransform.ProcessImage( sourceImage );
    /// Bitmap houghCirlceImage = circleTransform.ToBitmap( );
    /// // get circles using relative intensity
    /// HoughCircle[] circles = circleTransform.GetCirclesByRelativeIntensity( 0.5 );
    /// 
    /// foreach ( HoughCircle circle in circles )
    /// {
    ///     // ...
    /// }
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample8.jpg" width="400" height="300" />
    /// <para><b>Hough circle transformation image:</b></para>
    /// <img src="img/imaging/hough_circles.jpg" width="400" height="300" />
    /// </remarks>
    /// 
    /// <seealso cref="HoughLineTransformation"/>
    /// 
    public class HoughCircleTransformation
    {
        #region Variables
        // circle radius to detect
        private int _radiusToDetect;
        
        // Hough map
        private short[,] _houghMap;
        private short _maxMapIntensity = 0;

        // Hough map's width and height
        private int _width;
        private int _height;

        private int _localPeakRadius = 4;
        private short _minCircleIntensity = 10;
        private ArrayList _circles = new ArrayList();
        #endregion

        #region Properties

        /// <summary>
        /// Minimum circle's intensity in Hough map to recognize a circle.
        /// </summary>
        ///
        /// <remarks><para>The value sets minimum intensity level for a circle. If a value in Hough
        /// map has lower intensity, then it is not treated as a circle.</para>
        /// 
        /// <para>Default value is set to <b>10</b>.</para></remarks>
        ///
        public short MinCircleIntensity
        {
            get { return _minCircleIntensity; }
            set { _minCircleIntensity = value; }
        }

        /// <summary>
        /// Radius for searching local peak value.
        /// </summary>
        /// 
        /// <remarks><para>The value determines radius around a map's value, which is analyzed to determine
        /// if the map's value is a local maximum in specified area.</para>
        /// 
        /// <para>Default value is set to <b>4</b>. Minimum value is <b>1</b>. Maximum value is <b>10</b>.</para></remarks>
        /// 
        public int LocalPeakRadius
        {
            get { return _localPeakRadius; }
            set { _localPeakRadius = Math.Max(1, Math.Min(10, value)); }
        }

        /// <summary>
        /// Maximum found intensity in Hough map.
        /// </summary>
        /// 
        /// <remarks><para>The property provides maximum found circle's intensity.</para></remarks>
        /// 
        public short MaxIntensity
        {
            get { return _maxMapIntensity; }
        }

        /// <summary>
        /// Found circles count.
        /// </summary>
        /// 
        /// <remarks><para>The property provides total number of found circles, which intensity is higher (or equal to),
        /// than the requested <see cref="MinCircleIntensity">minimum intensity</see>.</para></remarks>
        /// 
        public int CirclesCount
        {
            get { return _circles.Count; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HoughCircleTransformation"/> class.
        /// </summary>
        /// 
        /// <param name="radiusToDetect">Circles' radius to detect.</param>
        /// 
        public HoughCircleTransformation(int radiusToDetect)
        {
            this._radiusToDetect = radiusToDetect;
        }


        #endregion

        /// <summary>
        /// Process an image building Hough map.
        /// </summary>
        /// 
        /// <param name="image">Source unmanaged image to process.</param>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        /// 
        //public void ProcessImage(UnmanagedImage image)
        //{
        //    if (image.PixelFormat != PixelFormat.Format8bppIndexed)
        //    {
        //        throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
        //    }

        //    // get source image size
        //    width = image.Width;
        //    height = image.Height;

        //    int srcOffset = image.Stride - width;

        //    // allocate Hough map of the same size like image
        //    houghMap = new short[height, width];

        //    // do the job
        //    unsafe
        //    {
        //        byte* src = (byte*)image.ImageData.ToPointer();

        //        // for each row
        //        for (int y = 0; y < height; y++)
        //        {
        //            // for each pixel
        //            for (int x = 0; x < width; x++, src++)
        //            {
        //                if (*src != 0)
        //                {
        //                    DrawHoughCircle(x, y);
        //                }
        //            }
        //            src += srcOffset;
        //        }
        //    }

        //    // find max value in Hough map
        //    maxMapIntensity = 0;
        //    for (int i = 0; i < height; i++)
        //    {
        //        for (int j = 0; j < width; j++)
        //        {
        //            if (houghMap[i, j] > maxMapIntensity)
        //            {
        //                maxMapIntensity = houghMap[i, j];
        //            }
        //        }
        //    }

        //    CollectCircles();
        //}

        /// <summary>
        /// Ñonvert Hough map to bitmap. 
        /// </summary>
        /// 
        /// <returns>Returns 8 bppp grayscale bitmap, which shows Hough map.</returns>
        /// 
        /// <exception cref="ApplicationException">Hough transformation was not yet done by calling
        /// ProcessImage() method.</exception>
        /// 
        //public Bitmap ToBitmap()
        //{
        //    // check if Hough transformation was made already
        //    if (houghMap == null)
        //    {
        //        throw new ApplicationException("Hough transformation was not done yet.");
        //    }

        //    int width = houghMap.GetLength(1);
        //    int height = houghMap.GetLength(0);

        //    // create new image
        //    Bitmap image = AForge.Imaging.Image.CreateGrayscaleImage(width, height);

        //    // lock destination bitmap data
        //    BitmapData imageData = image.LockBits(
        //        new Rectangle(0, 0, width, height),
        //        ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

        //    int offset = imageData.Stride - width;
        //    float scale = 255.0f / maxMapIntensity;

        //    // do the job
        //    unsafe
        //    {
        //        byte* dst = (byte*)imageData.Scan0.ToPointer();

        //        for (int y = 0; y < height; y++)
        //        {
        //            for (int x = 0; x < width; x++, dst++)
        //            {
        //                *dst = (byte)System.Math.Min(255, (int)(scale * houghMap[y, x]));
        //            }
        //            dst += offset;
        //        }
        //    }

        //    // unlock destination images
        //    image.UnlockBits(imageData);

        //    return image;
        //}

        #region Methods
        public Model.IImage CreateGraph()
        {

            int width = _houghMap.GetLength(1);
            int height = _houghMap.GetLength(0);
            int bytesPerPixel = 3;

            byte[] array = new byte[width * height * bytesPerPixel];

            float scale = 255.0f / _maxMapIntensity;

            int pos = 0;
            for (int line = 0; line < height; line++)
            {
                for (int column = 0; column < width; column++)
                {
                    byte value = (byte)System.Math.Min(255, (int)(scale * _houghMap[line, column]));
                    array[pos] = value;
                    array[pos + 1] = value;
                    array[pos + 2] = value;
                    pos += 3;


                }
            }

            return new Model.ImageBitmap(width, height, array);
        }

        public Model.IImage Execute(Model.IImage image)
        {
            // get source image size
            _width = image.Image.Width;
            _height = image.Image.Height;
            //int srcOffset = image.Stride - width;

            // allocate Hough map of the same size like image
            _houghMap = new short[_height, _width];

            int bytesPerPixel = 3;
            int pos = 0;

            int size = _width * _height * bytesPerPixel;
            byte[] result = new byte[size];
            Array.Copy(image.Bytes, result, size);


            byte src = result[pos];

            // for each row
            for (int y = 0; y < _height; y++)
            {
                // for each pixel
                for (int x = 0; x < _width; x++)
                {
                    src = result[pos];

                    if (src != 0)
                    {
                        DrawHoughCircle(x, y);
                    }

                    pos += bytesPerPixel;
                    
                }
                
            }


            // find max value in Hough map
            _maxMapIntensity = 0;
            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    if (_houghMap[i, j] > _maxMapIntensity)
                    {
                        _maxMapIntensity = _houghMap[i, j];
                    }
                }
            }

            CollectCircles();


            return new Model.ImageBitmap(_width, _height, result);
        }
        
        /// <summary>
        /// Get specified amount of circles with highest intensity.
        /// </summary>
        /// 
        /// <param name="count">Amount of circles to get.</param>
        /// 
        /// <returns>Returns arrary of most intesive circles. If there are no circles detected,
        /// the returned array has zero length.</returns>
        /// 
        public HoughCircle[] GetMostIntensiveCircles(int count)
        {
            // lines count
            int n = Math.Min(count, _circles.Count);

            // result array
            HoughCircle[] dst = new HoughCircle[n];
            _circles.CopyTo(0, dst, 0, n);

            return dst;
        }

        /// <summary>
        /// Get circles with relative intensity higher then specified value.
        /// </summary>
        /// 
        /// <param name="minRelativeIntensity">Minimum relative intesity of circles.</param>
        /// 
        /// <returns>Returns arrary of most intesive circles. If there are no circles detected,
        /// the returned array has zero length.</returns>
        /// 
        public HoughCircle[] GetCirclesByRelativeIntensity(double minRelativeIntensity)
        {
            int count = 0, n = _circles.Count;

            while ((count < n) && (((HoughCircle)_circles[count]).RelativeIntensity >= minRelativeIntensity))
                count++;

            return GetMostIntensiveCircles(count);
        }

        /// <summary>
        /// Collect circles with intesities greater or equal then specified 
        /// </summary>
        private void CollectCircles()
        {
            short intensity;
            bool foundGreater;

            // clean circles collection
            _circles.Clear();

            // for each Y coordinate
            for (int y = 0; y < _height; y++)
            {
                // for each X coordinate
                for (int x = 0; x < _width; x++)
                {
                    // get current value
                    intensity = _houghMap[y, x];

                    if (intensity < _minCircleIntensity)
                        continue;

                    foundGreater = false;

                    // check neighboors
                    for (int ty = y - _localPeakRadius, tyMax = y + _localPeakRadius; ty < tyMax; ty++)
                    {
                        // continue if the coordinate is out of map
                        if (ty < 0)
                            continue;
                        // break if it is not local maximum or coordinate is out of map
                        if ((foundGreater == true) || (ty >= _height))
                            break;

                        for (int tx = x - _localPeakRadius, txMax = x + _localPeakRadius; tx < txMax; tx++)
                        {
                            // continue or break if the coordinate is out of map
                            if (tx < 0)
                                continue;
                            if (tx >= _width)
                                break;

                            // compare the neighboor with current value
                            if (_houghMap[ty, tx] > intensity)
                            {
                                foundGreater = true;
                                break;
                            }
                        }
                    }

                    // was it local maximum ?
                    if (!foundGreater)
                    {
                        // we have local maximum
                        _circles.Add(new HoughCircle(x, y, _radiusToDetect, intensity, (double)intensity / _maxMapIntensity));
                    }
                }
            }

            _circles.Sort();
        }

        // Draw Hough circle:
        // http://www.cs.unc.edu/~mcmillan/comp136/Lecture7/circle.html
        //
        // TODO: more optimizations of circle drawing could be done.
        //
        private void DrawHoughCircle(int xCenter, int yCenter)
        {
            int x = 0;
            int y = _radiusToDetect;
            int p = (5 - _radiusToDetect * 4) / 4;

            SetHoughCirclePoints(xCenter, yCenter, x, y);

            while (x < y)
            {
                x++;
                if (p < 0)
                {
                    p += 2 * x + 1;
                }
                else
                {
                    y--;
                    p += 2 * (x - y) + 1;
                }
                SetHoughCirclePoints(xCenter, yCenter, x, y);
            }
        }

        /// <summary>
        /// Set circle points
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void SetHoughCirclePoints(int cx, int cy, int x, int y)
        {
            if (x == 0)
            {
                SetHoughPoint(cx, cy + y);
                SetHoughPoint(cx, cy - y);
                SetHoughPoint(cx + y, cy);
                SetHoughPoint(cx - y, cy);
            }
            else if (x == y)
            {
                SetHoughPoint(cx + x, cy + y);
                SetHoughPoint(cx - x, cy + y);
                SetHoughPoint(cx + x, cy - y);
                SetHoughPoint(cx - x, cy - y);
            }
            else if (x < y)
            {
                SetHoughPoint(cx + x, cy + y);
                SetHoughPoint(cx - x, cy + y);
                SetHoughPoint(cx + x, cy - y);
                SetHoughPoint(cx - x, cy - y);
                SetHoughPoint(cx + y, cy + x);
                SetHoughPoint(cx - y, cy + x);
                SetHoughPoint(cx + y, cy - x);
                SetHoughPoint(cx - y, cy - x);
            }
        }

        /// <summary>
        /// Set point
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void SetHoughPoint(int x, int y)
        {
            if ((x >= 0) && (y >= 0) && (x < _width) && (y < _height))
            {
                _houghMap[y, x]++;
            }
        }

        #endregion
    }
}
