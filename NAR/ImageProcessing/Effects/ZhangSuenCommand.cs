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

namespace NAR.ImageProcessing.Effects
{
    public class ZhangSuenCommand : ICommand
    {
        #region Constructors/Destructors
        public ZhangSuenCommand()
        {
        }
        #endregion

        #region ICommand Members
        public Model.IImage Execute(Model.IImage image)
        {
            int width = image.Image.Width;
            int height = image.Image.Height;
            int size = width * height * 3;

            byte[,] matriz = new byte[3, 3];

            //Creating the size of bytes similar to the image considering 3 bytes of each pixel (RGB)
            byte[] result = new byte[size];

            Array.Copy(image.Bytes, result, size);



            long colEnd = (width * 3) - 3; //- 3;

            for (uint line = 1; line < (height) - 1; line++)
            {
                for (uint column = 3; column < colEnd; column += 3)
                {
                    int pixelVizinhos = 0;
                    int conectividade = 0;



                    long line1 = (column) + width * (line - 1) * 3;
                    long line2 = (column) + width * (line) * 3;
                    long line3 = (column) + width * (line + 1) * 3;

                    matriz[1, 1] = 255;

                    //If the pixel is black
                    if (result[line2] == 0)
                    {

                        matriz[1, 1] = 0;

                        #region Montando a matriz dos vizinhos

                        matriz[0, 0] = result[line1 - 3]; //+ COLOR_GRAY - 3];
                        matriz[0, 1] = result[line1]; //+ COLOR_GRAY];
                        matriz[0, 2] = result[line1 + 3]; //+ COLOR_GRAY + 3];

                        matriz[1, 0] = result[line2 - 3]; //+ COLOR_GRAY - 3];
                        //matriz[1,1] = result[line2]; //+ COLOR_GRAY];
                        matriz[1, 2] = result[line2 + 3]; //+ COLOR_GRAY + 3];

                        matriz[2, 0] = result[line3 - 3]; //+ COLOR_GRAY - 3];
                        matriz[2, 1] = result[line3]; // + COLOR_GRAY];
                        matriz[2, 2] = result[line3 + 3]; //+ COLOR_GRAY + 3];

                        #endregion

                        #region Checking for at least 1 connectivity
                        if (matriz[0, 1] == 255 && matriz[0, 2] == 0)
                            conectividade++;
                        if (matriz[0, 2] == 255 && matriz[1, 2] == 0)
                            conectividade++;
                        if (matriz[1, 2] == 255 && matriz[2, 2] == 0)
                            conectividade++;
                        if (matriz[2, 2] == 255 && matriz[2, 1] == 0)
                            conectividade++;
                        if (matriz[2, 1] == 255 && matriz[2, 0] == 0)
                            conectividade++;
                        if (matriz[2, 0] == 255 && matriz[1, 0] == 0)
                            conectividade++;
                        if (matriz[1, 0] == 255 && matriz[0, 0] == 0)
                            conectividade++;
                        if (matriz[0, 0] == 255 && matriz[0, 1] == 0)
                            conectividade++;
                        #endregion

                        #region Checking the existing of neighboors
                        if (matriz[0, 0] == 0)
                            pixelVizinhos++;
                        if (matriz[0, 1] == 0)
                            pixelVizinhos++;
                        if (matriz[0, 2] == 0)
                            pixelVizinhos++;
                        if (matriz[1, 0] == 0)
                            pixelVizinhos++;
                        if (matriz[1, 2] == 0)
                            pixelVizinhos++;
                        if (matriz[2, 0] == 0)
                            pixelVizinhos++;
                        if (matriz[2, 1] == 0)
                            pixelVizinhos++;
                        if (matriz[2, 2] == 0)
                            pixelVizinhos++;
                        #endregion


                        if (pixelVizinhos >= 2 && pixelVizinhos <= 6 && conectividade == 1)
                        {

                            //If the pixel is white
                            //0 * 0			0 * 0 
                            //* 0 *			* 0 0
                            //0 0 0			0 * 0
                            if ((matriz[0, 1] == 255 ||
                                matriz[1, 0] == 255 ||
                                matriz[1, 2] == 255) &&

                                (matriz[0, 1] == 255 ||
                                matriz[1, 0] == 255 ||
                                matriz[2, 1] == 255))
                            {

                                matriz[1, 1] = 255;
                            }

                        }

                    }


                    result[line2 + ImageBitmap.COLOR_GREEN] = matriz[1, 1];
                    result[line2 + ImageBitmap.COLOR_BLUE] = matriz[1, 1];
                    result[line2 + ImageBitmap.COLOR_RED] = matriz[1, 1];


                }
            }

            Model.IImage imageResult = new Model.ImageBitmap(image.Image.Width, image.Image.Height, result);

            return imageResult;
        }
        #endregion
    }
}
