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
    public class GrayscaleCommand : ICommand
    {
        #region Variables
        private bool _luminance;
        #endregion

        #region Properties
        public bool Luminance
        {
            get { return _luminance; }
            set { _luminance = value; }
        }
        #endregion

        #region Constructors/Destructors
        public GrayscaleCommand(bool luminance)
        {
            _luminance = luminance;
        }
        #endregion

        #region Methods
        private byte [] ExecuteByAverage(byte [] image, int size)
        {
            byte newPixel = 0;

            //Creating the size of bytes similar to the image considering 3 bytes of each pixel (RGB)
            byte[] result = new byte[size];

            Array.Copy(image, result, size);


            //Foreach byte found in the image
            for (int c = 0; c < result.Length; c += 3)
            {
                //Creating the average to set the pixel
                newPixel = (byte)((result[c] + result[c + 1] + result[c + 2]) / 3);

                result[c] = newPixel;
                result[c + 1] = newPixel;
                result[c + 2] = newPixel;

            }//end for c

            return result;
        }
        private byte[] ExecuteByLuminance(byte[] image, int size)
        {
            byte newPixel = 0;

            //Creating the size of bytes similar to the image considering 3 bytes of each pixel (RGB)
            byte[] result = new byte[size];

            Array.Copy(image, result, size);


            //Foreach byte found in the image
            for (int c = 0; c < result.Length; c += 3)
            {
                newPixel = (byte) (
                    0.299 * image[c + Model.ImageBitmap.COLOR_RED] + 
                    0.587 * image[c + Model.ImageBitmap.COLOR_RED] +
                    0.114 * image[c + Model.ImageBitmap.COLOR_RED]);

                result[c] = newPixel;
                result[c + 1] = newPixel;
                result[c + 2] = newPixel;

            }//end for c

            return result;
        }
        #endregion

        #region ICommand Members
        public Model.IImage Execute(Model.IImage image)
        {
            int width = image.Image.Width;
            int height = image.Image.Height;
            int size = width * height * 3;

            byte[] result = null;

            if (!_luminance)
                result = ExecuteByAverage(image.Bytes, size);
            else
                result = ExecuteByLuminance(image.Bytes, size);
            
            
            

            Model.IImage imageResult = new Model.ImageBitmap(width, height, result);
            
            return imageResult;
        }
        #endregion
    }
}
