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

namespace NAR.ImageProcessing.Borders
{
    public class HorizontalCommand : ICommand, IBorderDetector
    {
        #region Constructors/Destructors
        public HorizontalCommand()
        {
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

            long stride = width * 3;
            int pixel = 0;

            for (uint l = 1; l < height - 1; ++l)
            {
                for (uint c = 9; c < ((width - 1) * 3); ++c)
                {
                    long lLine1 = l * stride + c;
                    long lLine2 = (l - 1) * stride + c;

                    //Calculando o valor do pixel
                    pixel = image.Bytes[lLine1 - 9] +
                        image.Bytes[lLine1 - 6] +
                        image.Bytes[lLine1 - 3] +
                        image.Bytes[lLine1] +
                        image.Bytes[lLine1 + 3] +
                        image.Bytes[lLine1 + 6] +
                        image.Bytes[lLine1 + 9] -
                        image.Bytes[lLine2 - 9] -
                        image.Bytes[lLine2 - 6] -
                        image.Bytes[lLine2 - 3] -
                        image.Bytes[lLine2] -
                        image.Bytes[lLine2 + 3] -
                        image.Bytes[lLine2 + 6] -
                        image.Bytes[lLine2 + 9];

                    if (pixel > 255)
                        pixel = 255;
                    else
                        if (pixel < 0)
                            pixel = 0;

                    result[lLine1] = (byte)pixel;

                }//end for c
            }//end for l


            Model.IImage imageResult = new Model.ImageBitmap(width, height, result);

            return imageResult;
        }
        #endregion
    }
}
