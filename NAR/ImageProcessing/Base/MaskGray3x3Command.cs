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
    public class MaskGray3x3Command : ICommand
    {
        #region Variables
        private byte[,] _mask;
        private byte _divisor;
        private byte _threshold;
        private byte _offset;
        #endregion

        #region Properties
        protected byte[,] Mask
        {
            get { return _mask; }
            set { _mask = value; }
        }
        public byte Divisor
        {
            get { return _divisor; }
        }
        public byte Threshold
        {
            get { return _threshold; }
        }
        public byte Offset
        {
            get { return _offset; }
        }
        #endregion

        #region Constructors/Destructors
        public MaskGray3x3Command(byte [,] mask, byte divisor, byte threshold, byte offset)
        {
            _mask = mask;
            _divisor = divisor;
            _threshold = threshold;
            _offset = offset;
        }
        #endregion

        #region ICommand Members
        public Model.IImage Execute(Model.IImage image)
        {
            int width = image.Image.Width;
            int height = image.Image.Height;
            int size = width * height * 3;

            int auxByte = 0;
            byte newByte = 0;

            //Creating the vector of positions from byte array
            long[] lines = new long[3];

            //Creating the size of bytes similar to the image considering 3 bytes of each pixel (RGB)
            byte[] result = new byte[size];

            Array.Copy(image.Bytes, result, size);


            //Calculating the end of the column from the byte array
            long colEnd = (width * 3) - 3;

            //Foreach line in the byte array
            for (int line = 1; line < result.Length - 1; line++)
            {
                //Foreach pixel from the mask
                for (int col = 3; col < colEnd; col += 3)
                {
                    //Calculando as posições vizinhas
                    lines[0] = (col) + width * (line - 1) * 3;
                    lines[1] = (col) + width * (line) * 3;
                    lines[2] = (col) + width * (line + 1) * 3;


                    auxByte = result[lines[0] - 3] * _mask[0, 0];
                    auxByte += result[lines[1] - 3] * _mask[0, 1];
                    auxByte += result[lines[2] - 3] * _mask[0, 2];
                    auxByte += result[lines[0]] * _mask[1, 0];
                    auxByte += result[lines[1]] * _mask[1, 1];
                    auxByte += result[lines[2]] * _mask[1, 2];
                    auxByte += result[lines[0] + 3] * _mask[2, 0];
                    auxByte += result[lines[1] + 3] * _mask[2, 1];
                    auxByte += result[lines[2] + 3] * _mask[2, 2];

                    newByte = (byte)System.Math.Abs((auxByte / _divisor) + _offset) > _threshold ? (byte)255 : (byte)0;

                    result[lines[1] + Model.ImageBitmap.COLOR_RED] = newByte;
                    result[lines[1] + Model.ImageBitmap.COLOR_GREEN] = newByte;
                    result[lines[1] + Model.ImageBitmap.COLOR_BLUE] = newByte;

                }//end for each pixel from the mask

            }//end for line

            Model.IImage imageResult = new Model.ImageBitmap(width, height, result);


            return imageResult;
        }
        #endregion
    }
}
