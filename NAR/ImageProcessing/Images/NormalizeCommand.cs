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

namespace NAR.ImageProcessing.Images
{
    public class NormalizeCommand : ICommand
    {
        #region Variables
        private bool _grayscale;
        #endregion

        #region Properties
        public bool Grayscale
        {
            get { return _grayscale; }
        }
        #endregion

        #region Constructors/Destructors
        public NormalizeCommand(bool grayscale)
        {
            _grayscale = grayscale;
        }
        #endregion

        #region Methods
        public Model.IImage NormalizeRGB(Model.IImage image)
        {
            int width = image.Image.Width;
            int height = image.Image.Height;
            int size = width * height * 3;
            byte minorRed = 255;
            byte majorRed = 0;
            byte minorGreen = 255;
            byte majorGreen = 0;
            byte minorBlue = 255;
            byte majorBlue = 0;
            byte newValue = 0;
            

            //Creating the size of bytes similar to the image considering 3 bytes of each pixel (RGB)
            byte[] result = new byte[size];

            Array.Copy(image.Bytes, result, size);

            //Foreach byte found in the image
            for (int c = 0; c < result.Length; c += 3)
            {
                if (majorRed < result[c + Model.ImageBitmap.COLOR_RED])
                    majorRed = result[c + Model.ImageBitmap.COLOR_RED];
                if (minorRed > result[c + Model.ImageBitmap.COLOR_RED])
                    minorRed = result[c + Model.ImageBitmap.COLOR_RED];


                if (majorGreen < result[c + Model.ImageBitmap.COLOR_GREEN])
                    majorGreen = result[c + Model.ImageBitmap.COLOR_GREEN];
                if (minorGreen > result[c + Model.ImageBitmap.COLOR_GREEN])
                    minorGreen = result[c + Model.ImageBitmap.COLOR_GREEN];


                if (majorBlue < result[c + Model.ImageBitmap.COLOR_BLUE])
                    majorBlue = result[c + Model.ImageBitmap.COLOR_BLUE];
                if (minorBlue > result[c + Model.ImageBitmap.COLOR_BLUE])
                    minorBlue = result[c + Model.ImageBitmap.COLOR_BLUE];

            }//end for c


            int divisorRed = majorRed - minorRed;
            if (divisorRed == 0)
                return image;

            int divisorGreen = majorGreen - minorGreen;
            if (divisorGreen == 0)
                return image;

            int divisorBlue = majorBlue - minorBlue;
            if (divisorBlue == 0)
                return image;



            for (uint c = 0; c < result.Length; c += 3)
            {
                newValue = (byte)((255 / divisorRed) * (result[c + Model.ImageBitmap.COLOR_RED] - minorRed));
                result[c + Model.ImageBitmap.COLOR_RED] = newValue;

                newValue = (byte)((255 / divisorGreen) * (result[c + Model.ImageBitmap.COLOR_GREEN] - minorGreen));
                result[c + Model.ImageBitmap.COLOR_GREEN] = newValue;

                newValue = (byte)((255 / divisorBlue) * (result[c + Model.ImageBitmap.COLOR_BLUE] - minorBlue));
                result[c + Model.ImageBitmap.COLOR_BLUE] = newValue;
            }


            Model.IImage imageResult = new Model.ImageBitmap(width, height, result);

            return imageResult;

        }
        public Model.IImage NormalizeGrayscale(Model.IImage image)
        {
            int width = image.Image.Width;
            int height = image.Image.Height;
            int size = width * height * 3;
            byte minor = 255;
            byte major = 0;
            byte newValue = 0;


            //Creating the size of bytes similar to the image considering 3 bytes of each pixel (RGB)
            byte[] result = new byte[size];

            Array.Copy(image.Bytes, result, size);

            //Foreach byte found in the image
            for (int c = 0; c < result.Length; c += 3)
            {
                if (major < result[c])
                    major = result[c];

                if (minor > result[c])
                    minor = result[c];

            }//end for c


            int divisor = major - minor;
            if (divisor == 0)
                return image;


            for (uint c = 0; c < result.Length; c += 3)
            {
                newValue = (byte)((255 / divisor) * (result[c] - minor));
                result[c + Model.ImageBitmap.COLOR_BLUE] = newValue;
                result[c + Model.ImageBitmap.COLOR_GREEN] = newValue;
                result[c + Model.ImageBitmap.COLOR_RED] = newValue;
            }


            Model.IImage imageResult = new Model.ImageBitmap(width, height, result);

            return imageResult;
        }
        #endregion

        #region ICommand Members
        public Model.IImage Execute(Model.IImage image)
        {

            if (_grayscale)
                return NormalizeGrayscale(image);
            else
                return NormalizeRGB(image);

        }
        #endregion
    }
}
