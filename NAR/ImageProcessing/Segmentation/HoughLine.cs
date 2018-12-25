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

namespace NAR.ImageProcessing.Segmentation
{

    /// <summary>
    /// Hough line.
    /// </summary>
    /// 
    /// <remarks><para>Represents line of Hough Line transformation using
    /// <a href="http://en.wikipedia.org/wiki/Polar_coordinate_system">polar coordinates</a>.
    /// See <a href="http://en.wikipedia.org/wiki/Polar_coordinate_system#Converting_between_polar_and_Cartesian_coordinates">Wikipedia</a>
    /// for information on how to convert polar coordinates to Cartesian coordinates.
    /// </para>
    /// 
    /// <para><note><see cref="HoughLineTransformation">Hough Line transformation</see> does not provide
    /// information about lines start and end points, only slope and distance from image's center. Using
    /// only provided information it is not possible to draw the detected line as it exactly appears on
    /// the source image. But it is possible to draw a line through the entire image, which contains the
    /// source line (see sample code below).
    /// </note></para>
    /// 
    /// <para>Sample code to draw detected Hough lines:</para>
    /// <code>
    /// HoughLineTransformation lineTransform = new HoughLineTransformation( );
    /// // apply Hough line transofrm
    /// lineTransform.ProcessImage( sourceImage );
    /// Bitmap houghLineImage = lineTransform.ToBitmap( );
    /// // get lines using relative intensity
    /// HoughLine[] lines = lineTransform.GetLinesByRelativeIntensity( 0.5 );
    /// 
    /// foreach ( HoughLine line in lines )
    /// {
    ///     // get line's radius and theta values
    ///     int    r = line.Radius;
    ///     double t = line.Theta;
    ///     
    ///     // check if line is in lower part of the image
    ///     if ( r &lt; 0 )
    ///     {
    ///         t += 180;
    ///         r = -r;
    ///     }
    ///     
    ///     // convert degrees to radians
    ///     t = ( t / 180 ) * Math.PI;
    ///     
    ///     // get image centers (all coordinate are measured relative
    ///     // to center)
    ///     int w2 = image.Width /2;
    ///     int h2 = image.Height / 2;
    ///     
    ///     double x0 = 0, x1 = 0, y0 = 0, y1 = 0;
    ///     
    ///     if ( line.Theta != 0 )
    ///     {
    ///         // none-vertical line
    ///         x0 = -w2; // most left point
    ///         x1 = w2;  // most right point
    ///     
    ///         // calculate corresponding y values
    ///         y0 = ( -Math.Cos( t ) * x0 + r ) / Math.Sin( t );
    ///         y1 = ( -Math.Cos( t ) * x1 + r ) / Math.Sin( t );
    ///     }
    ///     else
    ///     {
    ///         // vertical line
    ///         x0 = line.Radius;
    ///         x1 = line.Radius;
    ///     
    ///         y0 = h2;
    ///         y1 = -h2;
    ///     }
    ///     
    ///     // draw line on the image
    ///     Drawing.Line( sourceData,
    ///         new IntPoint( (int) x0 + w2, h2 - (int) y0 ),
    ///         new IntPoint( (int) x1 + w2, h2 - (int) y1 ),
    ///         Color.Red );
    /// }
    /// </code>
    /// 
    /// <para>To clarify meaning of <see cref="Radius"/> and <see cref="Theta"/> values
    /// of detected Hough lines, let's take a look at the below sample image and
    /// corresponding values of radius and theta for the lines on the image:
    /// </para>
    /// 
    /// <img src="img/imaging/sample15.png" width="400" height="300" />
    /// 
    /// <para>Detected radius and theta values (color in corresponding colors):
    /// <list type="bullet">
    /// <item><font color="#FF0000">Theta = 90, R = 125, I = 249</font>;</item>
    /// <item><font color="#00FF00">Theta = 0, R = -170, I = 187</font> (converts to Theta = 180, R = 170);</item>
    /// <item><font color="#0000FF">Theta = 90, R = -58, I = 163</font> (converts to Theta = 270, R = 58);</item>
    /// <item><font color="#FFFF00">Theta = 101, R = -101, I = 130</font> (converts to Theta = 281, R = 101);</item>
    /// <item><font color="#FF8000">Theta = 0, R = 43, I = 112</font>;</item>
    /// <item><font color="#FF80FF">Theta = 45, R = 127, I = 82</font>.</item>
    /// </list>
    /// </para>
    /// 
    /// </remarks>
    /// 
    /// <seealso cref="HoughLineTransformation"/>
    /// 
    public class HoughLine : IComparable
    {
        #region Properties

        /// <summary>
        /// Line's slope - angle between polar axis and line's radius (normal going
        /// from pole to the line). Measured in degrees, [0, 180).
        /// </summary>
        public readonly double Theta;

        /// <summary>
        /// Line's distance from image center, (−∞, +∞).
        /// </summary>
        /// 
        /// <remarks><note>Negative line's radius means, that the line resides in lower
        /// part of the polar coordinates system. This means that <see cref="Theta"/> value
        /// should be increased by 180 degrees and radius should be made positive.
        /// </note></remarks>
        /// 
        public readonly short Radius;

        /// <summary>
        /// Line's absolute intensity, (0, +∞).
        /// </summary>
        /// 
        /// <remarks><para>Line's absolute intensity is a measure, which equals
        /// to number of pixels detected on the line. This value is bigger for longer
        /// lines.</para>
        /// 
        /// <para><note>The value may not be 100% reliable to measure exact number of pixels
        /// on the line. Although these value correlate a lot (which means they are very close
        /// in most cases), the intensity value may slightly vary.</note></para>
        /// </remarks>
        /// 
        public readonly short Intensity;

        /// <summary>
        /// Line's relative intensity, (0, 1].
        /// </summary>
        /// 
        /// <remarks><para>Line's relative intensity is relation of line's <see cref="Intensity"/>
        /// value to maximum found intensity. For the longest line (line with highest intesity) the
        /// relative intensity is set to 1. If line's relative is set 0.5, for example, this means
        /// its intensity is half of maximum found intensity.</para>
        /// </remarks>
        /// 
        public readonly double RelativeIntensity;

        #endregion

        #region Constructors/Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HoughLine"/> class.
        /// </summary>
        /// 
        /// <param name="theta">Line's slope.</param>
        /// <param name="radius">Line's distance from image center.</param>
        /// <param name="intensity">Line's absolute intensity.</param>
        /// <param name="relativeIntensity">Line's relative intensity.</param>
        /// 
        public HoughLine(double theta, short radius, short intensity, double relativeIntensity)
        {
            Theta = theta;
            Radius = radius;
            Intensity = intensity;
            RelativeIntensity = relativeIntensity;
        }

        #endregion

        #region IComparable Members

        /// <summary>
        /// Compare the object with another instance of this class.
        /// </summary>
        /// 
        /// <param name="value">Object to compare with.</param>
        /// 
        /// <returns><para>A signed number indicating the relative values of this instance and <b>value</b>: 1) greater than zero - 
        /// this instance is greater than <b>value</b>; 2) zero - this instance is equal to <b>value</b>;
        /// 3) greater than zero - this instance is less than <b>value</b>.</para>
        /// 
        /// <para><note>The sort order is descending.</note></para></returns>
        /// 
        /// <remarks>
        /// <para><note>Object are compared using their <see cref="Intensity">intensity</see> value.</note></para>
        /// </remarks>
        /// 
        public int CompareTo(object value)
        {
            return (-Intensity.CompareTo(((HoughLine)value).Intensity));
        }

        #endregion
    }
}
