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
    public class LuminosityCommand : ICommand
    {
        #region Variables

        private double _contrast;
        private double _brightness;

        #endregion

        #region Properties

        public double Contrast
        {
            get { return _contrast; }
        }
        public double Brightness
        {
            get { return _brightness; }
        }

        #endregion

        #region Constructors/Destructors

        public LuminosityCommand(double contrast, double brightness)
        {
            _contrast = contrast;
            _brightness = brightness;
        }

        #endregion


        #region ICommand Members
        public Model.IImage Execute(Model.IImage image)
        {
            int width = image.Image.Width;
            int height = image.Image.Height;
            int size = width * height * 3;

            //Creating the size of bytes similar to the image considering 3 bytes of each pixel (RGB)
            byte[] result = new byte[size];

            //Foreach byte found in the image
            for (int c = 0; c < result.Length; c ++)
            {
                double value = ((double)image.Bytes[c] * _contrast + _brightness);

                if (value < 0)
                    result[c] = 0;
                else if (value > 255)
                    result[c] = 255;
                else
                    result[c] = (byte)value;

            }//end for c

            Model.IImage imageResult = new Model.ImageBitmap(width, height, result);

            return imageResult;
        }
        #endregion
    }
}
