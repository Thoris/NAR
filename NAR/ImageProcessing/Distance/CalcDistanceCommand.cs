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

namespace NAR.ImageProcessing.Distance
{
    public class CalcDistanceCommand : Images.BlackWhiteCommand, ICommand
    {
        /*
        #region Variables
        private int _x1;
        private int _x2;
        private int _y1;
        private int _y2;
        private bool _vertical;
        private int _posCheck;
        #endregion

        #region Constructors/Destructors
        public CalcDistanceCommand(int x1, int x2, int y1, int y2, int posCheck, bool vertical)
        {
            _x1 = x1;
            _x2 = x2;
            _y1 = y1;
            _y2 = y2;
            _posCheck = posCheck;
            _vertical = vertical;
        }
         * */

        #region Constructors/Destructors
        public CalcDistanceCommand()
            : base (false)
        {
        }
        #endregion

        #region ICommand Members        
        public new Model.IImage Execute(Model.IImage image)
        {

            int width = image.Image.Width;
            int height = image.Image.Height;
            int size = width * height * 3;

            //Creating the size of bytes similar to the image considering 3 bytes of each pixel (RGB)
            byte[] result = new byte[size];

            Model.IImage newImage = base.Execute(image);

            Array.Copy(newImage.Bytes, result, size);

            int[,] matriz = new int[height, width];

            int col = 0, line = 0;
            int infinito = width * height;

            for (int c = 0; c < result.Length; c += 3)
            {
                if (result[c] == 255)
                    matriz[line, col] = 0;
                else
                    matriz[line, col] = infinito;

                if (++col >= width)
                {
                    line++;
                    col = 0;
                }//endif iCol > Width
            }//end for


            int interacoes = 1;
            int minPixel = infinito;
            int maxPixel = 0;

            while (interacoes != 0)
            {
                interacoes = 0;

                for (int l = 1; l < height - 1; l++)
                {
                    for (int c = 1; c < width - 1; c++)
                    {
                        if (matriz[l, c] != 0)
                        {
                            //P2  P3  P4
                            //P1  P0  *
                            //*   *   *
                            minPixel = matriz[l, c];
                            // P1
                            if (matriz[l, c - 1] + 1 < minPixel)
                                minPixel = matriz[l, c - 1] + 1;
                            // P2
                            if (matriz[l - 1, c - 1] + 1 < minPixel)
                                minPixel = matriz[l - 1, c - 1] + 1;
                            // P3
                            if (matriz[l - 1, c] + 1 < minPixel)
                                minPixel = matriz[l - 1, c] + 1;
                            // P4
                            if (matriz[l - 1, c + 1] + 1 < minPixel)
                                minPixel = matriz[l - 1, c + 1] + 1;

                            matriz[l, c] = minPixel;

                        }//endif pixel branco

                    }//end for c
                }//end for l


                for (int l = height - 2; l >= 1; l--)
                {
                    for (int c = width - 2; c >= 1; c--)
                    {
                        if (matriz[l, c] != 0)
                        {
                            //*   *   *
                            //*   P0  P1
                            //P4  P3  P2
                            minPixel = matriz[l, c];
                            // P1
                            if (matriz[l, c + 1] + 1 < minPixel)
                                minPixel = matriz[l, c + 1] + 1;
                            // P2
                            if (matriz[l + 1, c + 1] + 1 < minPixel)
                                minPixel = matriz[l + 1, c + 1] + 1;
                            // P3
                            if (matriz[l + 1, c] + 1 < minPixel)
                                minPixel = matriz[l + 1, c] + 1;
                            // P4
                            if (matriz[l + 1, c - 1] + 1 < minPixel)
                                minPixel = matriz[l + 1, c - 1] + 1;

                            matriz[l, c] = minPixel;

                            if (minPixel > maxPixel)
                                maxPixel = minPixel;

                            if (matriz[l, c] > minPixel)
                                interacoes++;

                        }//endif pixel branco

                    }//end for c
                }//end for l


            }//end while


            uint count = 0;
            for (int l = 0; l < height; l++)
            {
                for (int c = 0; c < width; c++)
                {
                    if (matriz[l, c] != 0)
                    {
                        int color = matriz[l, c] * (255 / maxPixel);
                        result[count + 0] = (byte)color;
                        result[count + 1] = (byte)color;
                        result[count + 2] = (byte)color;

                    }
                    else
                    {
                        result[count + 0] = (byte)matriz[l, c];
                        result[count + 1] = (byte)matriz[l, c];
                        result[count + 2] = (byte)matriz[l, c];
                    }
                    count += 3;
                }//end for c
            }//end for l



            return new Model.ImageBitmap(image.Image.Width, image.Image.Height, result);
        }
    
        #endregion
    }
}
