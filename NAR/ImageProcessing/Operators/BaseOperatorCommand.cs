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

namespace NAR.ImageProcessing.Operators
{
    public class BaseOperatorCommand : Base.BaseFileCompared, ICommand
    {
        #region Enumerations
        public enum Operators
        {
            And,
            Nand,
            Nor,
            Not,
            Or,
            Xor,
        }
        #endregion

        #region Variables
        private Operators _operator;
        #endregion

        #region Properties
        private Operators Operator
        {
            get { return _operator; }
        }
        #endregion

        #region Constructors/Destructors
        public BaseOperatorCommand(Operators operatorAction)
        {
            _operator = operatorAction;
        }
        #endregion

        #region ICommand Members

        public Model.IImage Execute(Model.IImage image)
        {
            int width = image.Image.Width;
            int height = image.Image.Height;
            int size = width * height * 3;
            int newByte = 0;

            //Creating the size of bytes similar to the image considering 3 bytes of each pixel (RGB)
            byte[] result = new byte[size];

            //Array.Copy(image.Bytes, result, size);


            //Foreach byte found in the image
            for (int c = 0; c < result.Length; c ++)
            {
                //Checking which kind of operator should execute in the image
                if (_operator == Operators.And)
                {
                    newByte = image.Bytes[c] & _imageBase.Bytes[c];
                }
                else if (_operator == Operators.Nand)
                {
                    newByte = ~(image.Bytes[c] & _imageBase.Bytes[c]);
                }
                else if (_operator == Operators.Nor)
                {
                    newByte = ~(image.Bytes[c] | _imageBase.Bytes[c]);                
                }
                else if (_operator == Operators.Not)
                {
                    newByte = ~image.Bytes[c];
                }
                else if (_operator == Operators.Or)
                {
                    newByte = image.Bytes[c] | _imageBase.Bytes[c];                
                }
                else if (_operator == Operators.Xor)
                {
                    newByte = image.Bytes[c] ^ _imageBase.Bytes[c];                
                
                }//end if _operator


                //Checking if it passed through the limit
                if (newByte > 255) result[c] = 255;
                else if (newByte < 0) result[c] = 0;
                else result[c] = (byte)newByte;
                


            }//end for c

            Model.IImage imageResult = new Model.ImageBitmap(width, height, result);

            return imageResult;


        }

        #endregion
    }
}
