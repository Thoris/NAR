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
using NAR.Model;

namespace NAR.ImageProcessing.Utils
{
    public class Histogram
    {
        #region Methods

        public byte[] CalculateFromGrayscale(Model.IImage image)
        {
            byte limiar = 0;
            return CalculateFromGrayscale(image, out limiar); 
        }
        public byte[] CalculateFromGrayscale(Model.IImage image, out byte limiar)
        {
            byte[] result = new byte[256];
            long count = 0;
            limiar = 0;

            //Getting the value for only 1 byte of each pixel because in 
            //  Grayscale the image contains the same value for RGB
            for (uint c = 0; c < image.Bytes.Length; c += 3)
            {
                result[image.Bytes[c]]++;
                count += image.Bytes[c];
            }

            //Calculating the limiar
            limiar = (byte)((count) / (image.Bytes.Length / 3));

            return result;
        }
        public byte CalculateFromRGB(Model.IImage image, out byte[] red, out byte[] green, out byte[] blue)
        {
            long count = 0;
            red = new byte[256];
            green = new byte[256];
            blue = new byte[256];

            for (uint c = 0; c < image.Bytes.Length; c += 3)
            {
                red[image.Bytes[c + ImageBitmap.COLOR_RED]]++;
                green[image.Bytes[c + ImageBitmap.COLOR_GREEN]]++;
                blue[image.Bytes[c + ImageBitmap.COLOR_BLUE]]++;

                count += image.Bytes[c + ImageBitmap.COLOR_RED] + image.Bytes[c + ImageBitmap.COLOR_GREEN] + image.Bytes[c + ImageBitmap.COLOR_BLUE];
            }

            return (byte)(count / image.Bytes.Length);


        }

        #endregion
    }
}
