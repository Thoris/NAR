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
    public class BlackWhiteCommand : ICommand
    {
        #region Variables

        private byte _limiar = 125;
        private bool _calculateLimiar = false;

        #endregion

        #region Properties

        public byte Limiar
        {
            get { return _limiar; }
            set { _limiar = value; }
        }
        public bool CalculateLimiar
        {
            get { return _calculateLimiar; }
            set { _calculateLimiar = value; }
        }

        #endregion

        #region Constructors/Destructors
        //public BlackWhiteCommand()
        //    : this(false)
        //{
        //}
        public BlackWhiteCommand(bool calculateLimiar)
        {
            _calculateLimiar = calculateLimiar;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Calculates the limiar of the image from the array of bytes.
        /// </summary>
        /// <param name="bytes">The array of bytes.</param>
        /// <returns>value of limiar found</returns>
        public byte CalculateLimiarImage(byte[] bytes)
        {
            int min = 255;     //Minimum value found 
            int max = 0;       //Maximum value found
            int result = 0;    //limiar's results

            byte current;

            for (int c = 0; c < bytes.Length; c += 3)
            {

                current = (byte)((bytes[c] + bytes[c + 1] + bytes[c + 2]) / (byte)3);


                if (current > max)
                    max = bytes[c];

                if (current < min)
                    min = bytes[c];
            }

            result = (max + min) / 2;

            return (byte)result;
        }

        #endregion

        #region ICommand Members
        public Model.IImage Execute(Model.IImage image)
        {
            int width = image.Image.Width;
            int height = image.Image.Height;
            int size = width * height * 3;
            byte limiar = _limiar;

            //Creating the size of bytes similar to the image considering 3 bytes of each pixel (RGB)
            byte[] result = new byte[size];

            Array.Copy(image.Bytes, result, size);

            //if should calculate the limiar before to create the image
            if (_calculateLimiar)
                limiar = CalculateLimiarImage(result);

            //Foreach byte found in the image
            for (int c = 0; c < result.Length; c+=3)
            {

                if ((result[c] + result[c + 1] + result[c + 2]) / 3 > limiar)
                {
                    result[c] = 255;
                    result[c + 1] = 255;
                    result[c + 2] = 255;
                }
                else
                {
                    result[c] = 0;
                    result[c+1] = 0;
                    result[c+2] = 0;
                }

            }//end for c

            Model.IImage imageResult = new Model.ImageBitmap(width, height, result);
            
            return imageResult;
        }
        #endregion
    }
}
