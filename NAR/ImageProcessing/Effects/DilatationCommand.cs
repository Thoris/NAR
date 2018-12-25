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

namespace NAR.ImageProcessing.Effects
{
    public class DilatationCommand : ICommand
    {
        #region Variables
        private int _qtdPixel;
        #endregion

        #region Properties
        public int QtdPixel
        {
            get { return _qtdPixel; }
        }
        #endregion

        #region Constructors/Destructors
        public DilatationCommand(int qtdPixel)
        {
            _qtdPixel = qtdPixel;
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


            long colEnd = (width * 3) - 3; //- 3;

            for (uint line = 1; line < (height) - 1; line++)
            {
                for (uint column = 3; column < colEnd; column += 3)
                {

                    long lLine1 = (column) + width * (line - 1) * 3;
                    long lLine2 = (column) + width * (line) * 3;
                    long lLine3 = (column) + width * (line + 1) * 3;
                    int contPixel = 0;

                    //* 0 0 
                    //0 0 0
                    //0 0 0
                    if (image.Bytes[lLine1 - 3] == 0)
                        contPixel++;
                    //0 * 0 
                    //0 0 0
                    //0 0 0
                    if (image.Bytes[lLine1] == 0)
                        contPixel++;
                    //0 0 * 
                    //0 0 0
                    //0 0 0
                    if (image.Bytes[lLine1 + 3] == 0)
                        contPixel++;
                    //0 0 0 
                    //* 0 0
                    //0 0 0
                    if (image.Bytes[lLine2 - 3] == 0)
                        contPixel++;
                    //0 0 0 
                    //0 * 0
                    //0 0 0
                    if (image.Bytes[lLine2] == 0)
                        contPixel++;
                    //0 0 0 
                    //0 0 *
                    //0 0 0
                    if (image.Bytes[lLine2 + 3] == 0)
                        contPixel++;
                    //0 0 0 
                    //0 0 0
                    //* 0 0
                    if (image.Bytes[lLine3 - 3] == 0)
                        contPixel++;
                    //0 0 0 
                    //0 0 0
                    //0 * 0
                    if (image.Bytes[lLine3] == 0)
                        contPixel++;
                    //0 0 0 
                    //0 0 0
                    //0 0 *
                    if (image.Bytes[lLine3 + 3] == 0)
                        contPixel++;


                    if (contPixel >= _qtdPixel)
                    {
                        result[lLine2 + 0] = 0;
                        result[lLine2 + 1] = 0;
                        result[lLine2 + 2] = 0;

                    }
                    else
                    {
                        result[lLine2 + 0] = image.Bytes[lLine2];
                        result[lLine2 + 1] = image.Bytes[lLine2];
                        result[lLine2 + 2] = image.Bytes[lLine2];

                    }//endif pixels vizinhos

                }//end for iIndex2
            } //end for iIndex1

            Model.IImage imageResult = new Model.ImageBitmap(width, height, result);

            return imageResult;
        }
        #endregion
    }
}
