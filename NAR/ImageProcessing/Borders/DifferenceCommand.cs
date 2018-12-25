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
    public class DifferenceCommand : ICommand, IBorderDetector
    {
        #region Constructors/Destructors
        public DifferenceCommand()
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



            int pixel = 0;	
            int pixelMax = 0;

            long stride = width * 3;

            for (uint l = 1; l < height - 1; ++l)
            {
                for (uint c = 3; c < ((width - 1) * 3); ++c)
                {
                    long lLine1 = (l - 1) * stride + c;
                    long lLine2 = (l) * stride + c;
                    long lLine3 = (l + 1) * stride + c;

                    pixelMax = Math.Abs(image.Bytes[lLine1 + 3] - image.Bytes[lLine3 - 3]);
                    pixel = Math.Abs(image.Bytes[lLine3 + 3] - image.Bytes[lLine1 - 3]);
                    if (pixel > pixelMax)
                        pixelMax = pixel;

                    pixel = Math.Abs(image.Bytes[lLine1] - image.Bytes[lLine3]);
                    if (pixel > pixelMax)
                        pixelMax = pixel;

                    pixel = Math.Abs(image.Bytes[lLine2 + 3] - image.Bytes[lLine2 - 3]);
                    if (pixel > pixelMax)
                        pixelMax = pixel;

                    if (pixelMax < 0)
                        pixelMax = 0;

                    result[lLine1] = (byte)pixelMax;


                }//end for c
            }//end for l

            Model.IImage imageResult = new Model.ImageBitmap(width, height, result);

            return imageResult;
        }
        #endregion
    }
}
