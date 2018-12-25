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
using System.Drawing.Imaging;

namespace NAR.ImageProcessing.Corner
{

    /// <summary>
    /// Susan corners detector.
    /// </summary>
    /// 
    /// <remarks><para>The class implements Susan corners detector, which is described by
    /// S.M. Smith in: <b>S.M. Smith, "SUSAN - a new approach to low level image processing",
    /// Internal Technical Report TR95SMS1, Defense Research Agency, Chobham Lane, Chertsey,
    /// Surrey, UK, 1995</b>.</para>
    /// 
    /// <para><note>Some implementation notes:
    /// <list type="bullet">
    /// <item>Analyzing each pixel and searching for its USAN area, the 7x7 mask is used,
    /// which is comprised of 37 pixels. The mask has circle shape:
    /// <code lang="none">
    ///   xxx
    ///  xxxxx
    /// xxxxxxx
    /// xxxxxxx
    /// xxxxxxx
    ///  xxxxx
    ///   xxx
    /// </code>
    /// </item>
    /// <item>In the case if USAN's center of mass has the same coordinates as nucleus
    /// (central point), the pixel is not a corner.</item>
    /// <item>For noise suppression the 5x5 square window is used.</item></list></note></para>
    /// 
    /// <para>The class processes only grayscale 8 bpp and color 24/32 bpp images.
    /// In the case of color image, it is converted to grayscale internally using
    /// <see cref="GrayscaleBT709"/> filter.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create corners detector's instance
    /// SusanCornersDetector scd = new SusanCornersDetector( );
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
    /// <seealso cref="MoravecCornersDetector"/>
    /// 
    public class SusanCornersCommand : CornerBase, ICommand, ICornerDetector
    {
        #region Variables

        private static int[] rowRadius = new int[7] { 1, 2, 3, 3, 3, 2, 1 };

        /// <summary>
        /// brightness difference threshold 
        /// </summary>
        private int differenceThreshold = 25;
        
        /// <summary>
        /// geometrical threshold 
        /// </summary>
        private int geometricalThreshold = 18;

        #endregion

        #region Properties
        /// <summary>
        /// Brightness difference threshold.
        /// </summary>
        /// 
        /// <remarks><para>The brightness difference threshold controls the amount
        /// of pixels, which become part of USAN area. If difference between central
        /// pixel (nucleus) and surrounding pixel is not higher than difference threshold,
        /// then that pixel becomes part of USAN.</para>
        /// 
        /// <para>Increasing this value decreases the amount of detected corners.</para>
        /// 
        /// <para>Default value is set to <b>25</b>.</para>
        /// </remarks>
        /// 
        public int DifferenceThreshold
        {
            get { return differenceThreshold; }
            set { differenceThreshold = value; }
        }

        /// <summary>
        /// Geometrical threshold.
        /// </summary>
        /// 
        /// <remarks><para>The geometrical threshold sets the maximum number of pixels
        /// in USAN area around corner. If potential corner has USAN with more pixels, than
        /// it is not a corner.</para>
        /// 
        /// <para> Decreasing this value decreases the amount of detected corners - only sharp corners
        /// are detected. Increasing this value increases the amount of detected corners, but
        /// also increases amount of flat corners, which may be not corners at all.</para>
        /// 
        /// <para>Default value is set to <b>18</b>, which is half of maximum amount of pixels in USAN.</para>
        /// </remarks>
        /// 
        public int GeometricalThreshold
        {
            get { return geometricalThreshold; }
            set { geometricalThreshold = value; }
        }
        #endregion

        #region Constructors/Destructors
        ///// <summary>
        ///// Initializes a new instance of the <see cref="SusanCornersDetector"/> class.
        ///// </summary>
        //public SusanCornersDetector()
        //{
        //}

        /// <summary>
        /// Initializes a new instance of the <see cref="SusanCornersDetector"/> class.
        /// </summary>
        /// 
        /// <param name="differenceThreshold">Brightness difference threshold.</param>
        /// <param name="geometricalThreshold">Geometrical threshold.</param>
        /// 
        public SusanCornersCommand(int differenceThreshold, int geometricalThreshold, bool showPoints)
            : base (showPoints)
        {
            this.differenceThreshold = differenceThreshold;
            this.geometricalThreshold = geometricalThreshold;
        }
        #endregion

        #region Methods

        public Model.IImage Execute(Model.IImage image)
        {


            // get source image size
            int width = image.Image.Width;
            int height = image.Image.Height;

            
            int[,] susanMap = new int[height, width];


            int bytesPerPixel = 3;
            int stride = width * bytesPerPixel;
            //int offset = stride - width;

            int pos = stride * 3  + (3 * bytesPerPixel);
            int count = 0;

           // byte src = image.Bytes[pos];
            //byte* src = (byte*)grayImage.ImageData.ToPointer() + stride * 3 + 3;

            // for each row
            for (int y = 3, maxY = height - 3; y < maxY; y++)
            {
                // for each pixel
                for (int x = 3, maxX = width - 3; x < maxX; x++, pos += bytesPerPixel)
                {
                    // get value of the nucleus
                    byte nucleusValue = image.Bytes[pos];
                    // usan - number of pixels with similar brightness
                    int usan = 0;
                    // center of gravity
                    int cx = 0, cy = 0;

                    // for each row of the mask
                    for (int i = -3; i <= 3; i++)
                    {
                        // determine row's radius
                        int r = rowRadius[i + 3];

                        // get pointer to the central pixel of the row
                        //byte ptr = image.Bytes[pos + stride * i];
                        int posCheck = pos + stride * i;

                        // for each element of the mask's row
                        for (int j = -r; j <= r; j++)
                        {
                            count++;

                            // differenceThreshold
                            if (System.Math.Abs(nucleusValue - image.Bytes[posCheck + j * bytesPerPixel]) <= differenceThreshold)
                            {
                                usan++;

                                cx += x + j;
                                cy += y + i;
                            }
                        }
                    }


                    // check usan size
                    if (usan < geometricalThreshold )
                    {
                        cx /= usan;
                        cy /= usan;


                        if ((x != cx) || (y != cy))
                        {
                            // cornersList.Add( new Point( x, y ) );
                            usan = (geometricalThreshold - usan);
                        }
                        else
                        {
                            usan = 0;
                        }
                    }
                    else
                    {
                        usan = 0;
                    }

                    // usan = ( usan < geometricalThreshold ) ? ( geometricalThreshold - usan ) : 0;
                    susanMap[y, x] = usan;
                }

                pos += 6 * bytesPerPixel ;
            }

            count = 0;

            // collect interesting points - only those points, which are local maximums
            _cornersList.Clear();

            // for each row
            for (int y = 2, maxY = height - 2; y < maxY; y++)
            {
                // for each pixel
                for (int x = 2, maxX = width - 2; x < maxX; x++)
                {
                    int currentValue = susanMap[y, x];

                    // for each windows' row
                    for (int i = -2; (currentValue != 0) && (i <= 2); i++)
                    {
                        // for each windows' pixel
                        for (int j = -2; j <= 2; j++)
                        {
                            if (susanMap[y + i, x + j] > currentValue)
                            {
                                currentValue = 0;
                                break;
                            }
                        }
                    }
                    count++;
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
