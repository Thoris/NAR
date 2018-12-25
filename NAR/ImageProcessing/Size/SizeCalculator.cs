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

namespace NAR.ImageProcessing.Size
{
    public class SizeCalculator : Images.BlackWhiteCommand, ISizeCalculator
    {
        #region Constructors/Destructors
        public SizeCalculator()
            : base(false)
        {
        }
        #endregion

        #region ISizeCalculator members
        public double Calculate(Model.IImage image, int x1, int x2, int y1, int y2, double mmPixel, bool fill, out Model.IImage resultImage)
        {
            int width = image.Image.Width;
            int height = image.Image.Height;
            int size = width * height * 3;

            //Creating the size of bytes similar to the image considering 3 bytes of each pixel (RGB)
            byte[] result = new byte[size];

            Model.IImage newImage = base.Execute(image);


            Array.Copy(newImage.Bytes, result, size);


            int iX1 = x1;
            int iX2 = x2;
            int iY1 = y1;
            int iY2 = y2;

            int iInit = -1, iEnd = -1;


            bool bContinue = true;
            while (bContinue)
            {
                #region Column

                if (y1 != y2)
                {

                    int posX = iX1 * 3;


                    if (iInit == -1)
                    {
                        int iPosNow = (iY1 * width * 3) + posX;

                        if (result[iPosNow] == 0)
                        {
                            iInit = iY1;

                        }

                        iY1++;


                        if (fill)
                        {
                            result[iPosNow] = 255;
                            result[iPosNow + 1] = 0;
                            result[iPosNow + 2] = 0;
                        }//endif fill

                    }

                    if (iEnd == -1)
                    {
                        int iPosNow = (iY2 * width * 3) + posX;

                        if (result[iPosNow] == 0)
                        {
                            iEnd = iY2;
                        }

                        iY2--;



                        if (fill)
                        {
                            result[iPosNow] = 255;
                            result[iPosNow + 1] = 0;
                            result[iPosNow + 2] = 0;
                        }//endif fill
                    }

                    if ((iInit != -1 && iEnd != -1) || (iY1 > iY2))
                    {
                        bContinue = false;
                        //break;
                    }
                }
                #endregion

                #region Line
                else
                {

                    int iPosY = iY1 * width * 3;


                    if (iInit == -1)
                    {
                        int iPosNow = (iX1 * 3) + iPosY;

                        if (result[iPosNow] == 0)
                        {
                            iInit = iX1;
                        }


                        iX1++;



                        if (fill)
                        {
                            result[iPosNow] = 255;
                            result[iPosNow + 1] = 0;
                            result[iPosNow + 2] = 0;
                        }//endif fill
                    }

                    if (iEnd == -1)
                    {
                        int iPosNow = (iX2 * 3) + iPosY;

                        if (result[iPosNow] == 0)
                        {
                            iEnd = iX2;
                        }

                        iX2--;



                        if (fill)
                        {
                            result[iPosNow] = 255;
                            result[iPosNow + 1] = 0;
                            result[iPosNow + 2] = 0;

                        }//endif fill

                    }

                    if ((iInit != -1 && iEnd != -1) || iX1 > iX2)
                    {
                        bContinue = false;
                        //break;
                    }
                }//endif 

                #endregion

            }//end while

            resultImage = new Model.ImageBitmap(image.Image.Width, image.Image.Height, result);

            if (iInit != -1 && iEnd != -1)
            {

                int iQtd = iEnd - iInit;

                return (double)iQtd * mmPixel;
            }

            return 0;
        }
        #endregion
    }
}
