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
    /// Hough circle.
    /// </summary>
    /// 
    /// <remarks>Represents circle of Hough transform.</remarks>
    /// 
    /// <seealso cref="HoughCircleTransformation"/>
    /// 
    public class HoughCircle : IComparable
    {
        /// <summary>
        /// Circle center's X coordinate.
        /// </summary>
        public readonly int X;

        /// <summary>
        /// Circle center's Y coordinate.
        /// </summary>
        public readonly int Y;

        /// <summary>
        /// Circle's radius.
        /// </summary>
        public readonly int Radius;

        /// <summary>
        /// Line's absolute intensity.
        /// </summary>
        public readonly short Intensity;

        /// <summary>
        /// Line's relative intensity.
        /// </summary>
        public readonly double RelativeIntensity;

        /// <summary>
        /// Initializes a new instance of the <see cref="HoughCircle"/> class.
        /// </summary>
        /// 
        /// <param name="x">Circle's X coordinate.</param>
        /// <param name="y">Circle's Y coordinate.</param>
        /// <param name="radius">Circle's radius.</param>
        /// <param name="intensity">Circle's absolute intensity.</param>
        /// <param name="relativeIntensity">Circle's relative intensity.</param>
        /// 
        public HoughCircle(int x, int y, int radius, short intensity, double relativeIntensity)
        {
            X = x;
            Y = y;
            Radius = radius;
            Intensity = intensity;
            RelativeIntensity = relativeIntensity;
        }

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
            return (-Intensity.CompareTo(((HoughCircle)value).Intensity));
        }
    }
}
