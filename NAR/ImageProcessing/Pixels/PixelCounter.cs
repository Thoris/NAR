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

namespace NAR.ImageProcessing.Pixels
{
    public class PixelCounter : Images.BlackWhiteCommand, ICountPixels
    {
        #region Constructors/Destructors
        public PixelCounter()
            : base (false)
        {
        }
        #endregion

        #region ICountPixels Members
        public double Count(Model.IImage image, int x1, int x2, int y1, int y2, bool fill, out Model.IImage resultImage)
        {
            double result = 0;

            uint qtd = 0; 
            uint not = 0; 
            

            int width = image.Image.Width;
            int height = image.Image.Height;
            int size = width * height * 3;


            //Creating the size of bytes similar to the image considering 3 bytes of each pixel (RGB)
            byte[] newImage = new byte[size];

            resultImage = base.Execute(image);

            Array.Copy(resultImage.Bytes, newImage, size);

            for (int l = y1; l < y2; l++)
            {
                int iX1 = x1 * 3;
                int iX2 = x2 * 3;

                long lPos = width * l * 3;

                for (int c = iX1; c < iX2; c += 3)
                {
                    if (resultImage.Bytes[c + lPos] == 0)
                    {
                        qtd++;


                        if (fill)
                        {
                            newImage[c + lPos] = 255;
                            newImage[c + lPos + 1] = 0;
                            newImage[c + lPos + 2] = 0;
                        }//endif _bFill

                    }
                    else 
                    {
                        not++;

                    }
                }//end for c;
            }//end for l

            resultImage = new Model.ImageBitmap(image.Image.Width, image.Image.Height, newImage);

            if (not == 0)
                return 100;

             result = (double)(((double)qtd / (double)((double)not + (double)qtd)) * 100);


            return result;
        }
        #endregion
    }
}
