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

namespace NAR.ImageProcessing.Base
{
    public class Convolution2x2Command : Mask2x2Command, ICommand
    {
        #region Variables
        private short[,] _maskY = new short[2, 2];
        #endregion

        #region Constructors/Destructors
        public Convolution2x2Command(bool grayscale, byte divisor, byte threshold, byte offset, short[,] maskX, short[,] maskY)
            : base (grayscale, divisor, threshold, offset, maskX)
        {
            _maskY = maskY;
        }
        #endregion

        #region Methods
        protected Model.IImage Convolution(Model.IImage image, short[,] maskX, short[,] maskY, byte divisor, byte threshold, byte offset)
        {
            double value = 0;
            byte[] result = new byte[image.Bytes.Length];

            byte[] imageX = ApplyMask(image, _grayscale, maskX, divisor, threshold, offset);
            byte[] imageY = ApplyMask(image, _grayscale, maskY, divisor, threshold, offset);

            for (int c = 0; c < result.Length; c++)
            {
                value = Math.Sqrt((imageX[c] * imageX[c]) + (imageY[c] * imageY[c]));

                if (value > 255)
                    result[c] = 255;
                else if (value < 0)
                    result[c] = 0;
                else
                    result[c] = (byte)value;
            }

            Model.IImage imageResult = new Model.ImageBitmap(image.Image.Width, image.Image.Height, result);

            return imageResult;

        }
        #endregion

        #region ICommand Members
        public new Model.IImage Execute(Model.IImage image)
        {
            return Convolution(image, base.MaskX, _maskY, base.Divisor, base.Threshold, base.Offset);
        }
        #endregion
    }
}
