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
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

namespace NAR.ImageProcessing.Segmentation
{
    /// <summary>
    /// Hough line transformation.
    /// </summary>
    ///
    /// <remarks><para>The class implements Hough line transformation, which allows to detect
    /// straight lines in an image. Lines, which are found by the class, are provided in
    /// <a href="http://en.wikipedia.org/wiki/Polar_coordinate_system">polar coordinates system</a> -
    /// lines' distances from image's center and lines' slopes are provided.
    /// The pole of polar coordinates system is put into processing image's center and the polar
    /// axis is directed to the right from the pole. Lines' slope is measured in degrees and
    /// is actually represented by angle between polar axis and line's radius (normal going
    /// from pole to the line), which is measured in counter-clockwise direction.
    /// </para>
    /// 
    /// <para><note>Found lines may have negative <see cref="HoughLine.Radius">radius</see>.
    /// This means, that the line resides in lower part of the polar coordinates system
    /// and its <see cref="HoughLine.Theta"/> value should be increased by 180 degrees and
    /// radius should be made positive.
    /// </note></para>
    /// 
    /// <para>The class accepts binary images for processing, which are represented by 8 bpp grayscale images.
    /// All black pixels (0 pixel's value) are treated as background, but pixels with different value are
    /// treated as lines' pixels.</para>
    /// 
    /// <para>See also documentation to <see cref="HoughLine"/> class for additional information
    /// about Hough Lines.</para>
    /// 
    /// </remarks>
    /// 
    /// <seealso cref="HoughLine"/>
    /// 
    public class HoughLineTransformation
    {
        #region Variables
        // Hough transformation quality settings
        private int _stepsPerDegree;
        private int _houghHeight;
        private double _thetaStep;

        // precalculated Sine and Cosine values
        private double[] _sinMap;
        private double[] _cosMap;
        // Hough map
        private short[,] _houghMap;
        private short _maxMapIntensity = 0;

        private int _localPeakRadius = 4;
        private short _minLineIntensity = 10;
        private ArrayList _lines = new ArrayList();

          
        #endregion

        #region Properties
        /// <summary>
        /// Steps per degree.
        /// </summary>
        /// 
        /// <remarks><para>The value defines quality of Hough line transformation and its ability to detect
        /// lines' slope precisely.</para>
        /// 
        /// <para>Default value is set to <b>1</b>. Minimum value is <b>1</b>. Maximum value is <b>10</b>.</para></remarks>
        /// 
        public int StepsPerDegree
        {
            get { return _stepsPerDegree; }
            set
            {
                _stepsPerDegree = Math.Max(1, Math.Min(10, value));
                _houghHeight = 180 * _stepsPerDegree;
                _thetaStep = Math.PI / _houghHeight;

                // precalculate Sine and Cosine values
                _sinMap = new double[_houghHeight];
                _cosMap = new double[_houghHeight];

                for (int i = 0; i < _houghHeight; i++)
                {
                    _sinMap[i] = Math.Sin(i * _thetaStep);
                    _cosMap[i] = Math.Cos(i * _thetaStep);
                }
            }
        }

        /// <summary>
        /// Minimum <see cref="HoughLine.Intensity">line's intensity</see> in Hough map to recognize a line.
        /// </summary>
        ///
        /// <remarks><para>The value sets minimum intensity level for a line. If a value in Hough
        /// map has lower intensity, then it is not treated as a line.</para>
        /// 
        /// <para>Default value is set to <b>10</b>.</para></remarks>
        ///
        public short MinLineIntensity
        {
            get { return _minLineIntensity; }
            set { _minLineIntensity = value; }
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
        /// Maximum found <see cref="HoughLine.Intensity">intensity</see> in Hough map.
        /// </summary>
        /// 
        /// <remarks><para>The property provides maximum found line's intensity.</para></remarks>
        /// 
        public short MaxIntensity
        {
            get { return _maxMapIntensity; }
        }

        /// <summary>
        /// Found lines count.
        /// </summary>
        /// 
        /// <remarks><para>The property provides total number of found lines, which intensity is higher (or equal to),
        /// than the requested <see cref="MinLineIntensity">minimum intensity</see>.</para></remarks>
        /// 
        public int LinesCount
        {
            get { return _lines.Count; }
        }



        #endregion

        #region Constructors/Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HoughLineTransformation"/> class.
        /// </summary>
        /// 
        public HoughLineTransformation()
        {
            StepsPerDegree = 1;
        }

        #endregion

        #region Methods

        public Model.IImage Execute(Model.IImage image)
        {
            // get source image size
            int width = image.Image.Width;
            int height = image.Image.Height;
            int halfWidth = width / 2;
            int halfHeight = height / 2;

            // make sure the specified rectangle recides with the source image
            //rect.Intersect(new Rectangle(0, 0, width, height));
            
            int startX = -halfWidth ;
            int startY = -halfHeight;
            int stopX = width - halfWidth ;
            int stopY = height - halfHeight ;

            //int offset = image.Stride - rect.Width;
            //int offset = 0;

            // calculate Hough map's width
            int halfHoughWidth = (int)Math.Sqrt(halfWidth * halfWidth + halfHeight * halfHeight);
            int houghWidth = halfHoughWidth * 2;

            _houghMap = new short[_houghHeight, houghWidth];

            //Creating the size of bytes similar to the image considering 3 bytes of each pixel (RGB)
            int size = width * height * 3;
            byte[] result = new byte[size];
            Array.Copy(image.Bytes, result, size);

            int pos = 0;



            // for each row
            for (int y = startY; y < stopY; y++)
            {
                // for each pixel
                for (int x = startX; x < stopX; x++)
                {
                    if (result[pos] != 0)
                    {
                        // for each Theta value
                        for (int theta = 0; theta < _houghHeight; theta++)
                        {
                            int radius = (int)Math.Round(_cosMap[theta] * x - _sinMap[theta] * y) + halfHoughWidth;

                            if ((radius < 0) || (radius >= houghWidth))
                                continue;

                            _houghMap[theta, radius]++;
                        }
                    }

                    pos += 3;
                }

            }


            // find max value in Hough map
            _maxMapIntensity = 0;
            for (int i = 0; i < _houghHeight; i++)
            {
                for (int j = 0; j < houghWidth; j++)
                {
                    if (_houghMap[i, j] > _maxMapIntensity)
                    {
                        _maxMapIntensity = _houghMap[i, j];
                    }
                }
            }

            CollectLines();

            //image.Image.Save("C:\\temp\\test.bmp");


            return image;
        }

        /// <summary>
        /// Get specified amount of lines with highest <see cref="HoughLine.Intensity">intensity</see>.
        /// </summary>
        /// 
        /// <param name="count">Amount of lines to get.</param>
        /// 
        /// <returns>Returns array of most intesive lines. If there are no lines detected,
        /// the returned array has zero length.</returns>
        /// 
        public HoughLine[] GetMostIntensiveLines(int count)
        {
            // lines count
            int n = Math.Min(count, _lines.Count);

            // result array
            HoughLine[] dst = new HoughLine[n];
            _lines.CopyTo(0, dst, 0, n);

            return dst;
        }

        /// <summary>
        /// Get lines with <see cref="HoughLine.RelativeIntensity">relative intensity</see> higher then specified value.
        /// </summary>
        /// 
        /// <param name="minRelativeIntensity">Minimum relative intesity of lines.</param>
        /// 
        /// <returns>Returns array of lines. If there are no lines detected,
        /// the returned array has zero length.</returns>
        /// 
        public HoughLine[] GetLinesByRelativeIntensity(double minRelativeIntensity)
        {
            int count = 0, n = _lines.Count;

            while ((count < n) && (((HoughLine)_lines[count]).RelativeIntensity >= minRelativeIntensity))
                count++;

            return GetMostIntensiveLines(count);
        }
        
        /// <summary>
        /// Collect lines with intesities greater or equal then specified 
        /// </summary>
        private void CollectLines()
        {
            int maxTheta = _houghMap.GetLength(0);
            int maxRadius = _houghMap.GetLength(1);

            short intensity;
            bool foundGreater;

            int halfHoughWidth = maxRadius >> 1;

            // clean lines collection
            _lines.Clear();

            // for each Theta value
            for (int theta = 0; theta < maxTheta; theta++)
            {
                // for each Radius value
                for (int radius = 0; radius < maxRadius; radius++)
                {
                    // get current value
                    intensity = _houghMap[theta, radius];

                    if (intensity < _minLineIntensity)
                        continue;

                    foundGreater = false;

                    // check neighboors
                    for (int tt = theta - _localPeakRadius, ttMax = theta + _localPeakRadius; tt < ttMax; tt++)
                    {
                        // break if it is not local maximum
                        if (foundGreater == true)
                            break;

                        int cycledTheta = tt;
                        int cycledRadius = radius;

                        // check limits
                        if (cycledTheta < 0)
                        {
                            cycledTheta = maxTheta + cycledTheta;
                            cycledRadius = maxRadius - cycledRadius;
                        }
                        if (cycledTheta >= maxTheta)
                        {
                            cycledTheta -= maxTheta;
                            cycledRadius = maxRadius - cycledRadius;
                        }

                        for (int tr = cycledRadius - _localPeakRadius, trMax = cycledRadius + _localPeakRadius; tr < trMax; tr++)
                        {
                            // skip out of map values
                            if (tr < 0)
                                continue;
                            if (tr >= maxRadius)
                                break;

                            // compare the neighboor with current value
                            if (_houghMap[cycledTheta, tr] > intensity)
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
                        _lines.Add(new HoughLine((double)theta / _stepsPerDegree, (short)(radius - halfHoughWidth), intensity, (double)intensity / _maxMapIntensity));
                    }
                }
            }

            _lines.Sort();
        }

        public Model.IImage CreateGraph()
        {
           
            int width = _houghMap.GetLength(1);
            int height = _houghMap.GetLength(0);

            byte[] array = new byte[width * height * 3];

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

            return new Model.ImageBitmap(width, height, array );
        }

        #endregion
    }
}
