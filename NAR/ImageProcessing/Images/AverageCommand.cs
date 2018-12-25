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
    //http://users.ecs.soton.ac.uk/msn/book/new_demo/averaging/

    public class AverageCommand : ICommand
    {
        #region Variables
        private bool _colored;
        #endregion

        #region Constructors/Destructors
        public AverageCommand(bool colored)
        {
            _colored = colored;
        }
        #endregion

        #region ICommand members
        public Model.IImage Execute(Model.IImage image)
        {
            int width = image.Image.Width;
            int height = image.Image.Height;
            int size = width * height * 3;

            int[] auxByte = new int[3];
            byte[] newByte = new byte[3];
            
            //Creating the vector of positions from byte array
            long[] lines = new long[3];

            //Creating the size of bytes similar to the image considering 3 bytes of each pixel (RGB)
            byte[] result = new byte[size];
            byte[] newBytes = new byte[size];

            Array.Copy(image.Bytes, result, size);


            //Calculating the end of the column from the byte array
            long colEnd = (width * 3) - 3;

            //Foreach line in the byte array
            for (int line = 1; line < height - 1; line++)
            {
                //Foreach pixel from the mask
                for (int col = 3; col < colEnd; col += 3)
                {
                    //Calculating the position of near to the focal point
                    lines[0] = (col) + width * (line - 1) * 3;
                    lines[1] = (col) + width * (line) * 3;
                    lines[2] = (col) + width * (line + 1) * 3;


                    


                    if (_colored)
                    {
                        auxByte[Model.ImageBitmap.COLOR_BLUE] = (
                            result[lines[0] - 3 + Model.ImageBitmap.COLOR_BLUE] +
                            result[lines[0] + Model.ImageBitmap.COLOR_BLUE] +
                            result[lines[0] + 3 + Model.ImageBitmap.COLOR_BLUE] +
                            result[lines[1] - 3 + Model.ImageBitmap.COLOR_BLUE] +
                            result[lines[1] + Model.ImageBitmap.COLOR_BLUE] +
                            result[lines[1] + 3 + Model.ImageBitmap.COLOR_BLUE] +
                            result[lines[2] - 3 + Model.ImageBitmap.COLOR_BLUE] +
                            result[lines[2] + Model.ImageBitmap.COLOR_BLUE] +
                            result[lines[2] + 3 + Model.ImageBitmap.COLOR_BLUE]);

                        newByte[Model.ImageBitmap.COLOR_BLUE] = (byte)(auxByte[Model.ImageBitmap.COLOR_BLUE] / 9);


                        auxByte[Model.ImageBitmap.COLOR_GREEN] = (
                            result[lines[0] - 3 + Model.ImageBitmap.COLOR_GREEN] +
                            result[lines[0] + Model.ImageBitmap.COLOR_GREEN] +
                            result[lines[0] + 3 + Model.ImageBitmap.COLOR_GREEN] +
                            result[lines[1] - 3 + Model.ImageBitmap.COLOR_GREEN] +
                            result[lines[1] + Model.ImageBitmap.COLOR_GREEN] +
                            result[lines[1] + 3 + Model.ImageBitmap.COLOR_GREEN] +
                            result[lines[2] - 3 + Model.ImageBitmap.COLOR_GREEN] +
                            result[lines[2] + Model.ImageBitmap.COLOR_GREEN] +
                            result[lines[2] + 3 + Model.ImageBitmap.COLOR_GREEN]);

                        newByte[Model.ImageBitmap.COLOR_GREEN] = (byte)(auxByte[Model.ImageBitmap.COLOR_GREEN] / 9);


                        auxByte[Model.ImageBitmap.COLOR_RED] = (
                            result[lines[0] - 3 + Model.ImageBitmap.COLOR_RED] +
                            result[lines[0] + Model.ImageBitmap.COLOR_RED] +
                            result[lines[0] + 3 + Model.ImageBitmap.COLOR_RED] +
                            result[lines[1] - 3 + Model.ImageBitmap.COLOR_RED] +
                            result[lines[1] + Model.ImageBitmap.COLOR_RED] +
                            result[lines[1] + 3 + Model.ImageBitmap.COLOR_RED] +
                            result[lines[2] - 3 + Model.ImageBitmap.COLOR_RED] +
                            result[lines[2] + Model.ImageBitmap.COLOR_RED] +
                            result[lines[2] + 3 + Model.ImageBitmap.COLOR_RED]);

                        newByte[Model.ImageBitmap.COLOR_RED] = (byte)(auxByte[Model.ImageBitmap.COLOR_RED] / 9);


                        newBytes[lines[1] + Model.ImageBitmap.COLOR_BLUE] = newByte[Model.ImageBitmap.COLOR_BLUE];
                        newBytes[lines[1] + Model.ImageBitmap.COLOR_GREEN] = newByte[Model.ImageBitmap.COLOR_GREEN];
                        newBytes[lines[1] + Model.ImageBitmap.COLOR_RED] = newByte[Model.ImageBitmap.COLOR_RED];
                    }
                    else
                    {
                        auxByte[0] = (
                            result[lines[0] - 3] +
                            result[lines[0]] +
                            result[lines[0] + 3] +
                            result[lines[1] - 3] +
                            result[lines[1]] +
                            result[lines[1] + 3] +
                            result[lines[2] - 3] +
                            result[lines[2]] +
                            result[lines[2] + 3]) ;

                        newByte[0] = (byte)( auxByte[0] / 9 );


                        newBytes[lines[1] + Model.ImageBitmap.COLOR_BLUE] = newByte[0];
                        newBytes[lines[1] + Model.ImageBitmap.COLOR_GREEN] = newByte[0];
                        newBytes[lines[1] + Model.ImageBitmap.COLOR_RED] = newByte[0];

                    }



                }//end for col
            }//end for line

            return new Model.ImageBitmap(width, height, newBytes);
        }
        #endregion
    }
}
