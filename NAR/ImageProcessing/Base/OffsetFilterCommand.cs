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
using System.Drawing;
using NAR.Model;

namespace NAR.ImageProcessing.Base
{
    public class OffsetFilterCommand : ICommand
    {
        #region Variables
        private bool _abs;
        private Point[,] _offsetPoints;
        #endregion

        #region Properties
        public bool Abs
        {
            get { return _abs; }
        }
        public Point[,] OffsetPoints
        {
            get { return _offsetPoints; }
            set { _offsetPoints = value; }
        }
        #endregion

        #region Constructors/Destructors
        public OffsetFilterCommand()
            : this (false)
        {
        }
        public OffsetFilterCommand(bool abs)
        {
            _abs = abs;
        }
        #endregion

        #region Methods
        public Model.IImage OffSetAbs(Model.IImage image)
        {
            int width = image.Image.Width;
            int height = image.Image.Height;
            int size = width * height * 3;
            int offSetX, offSetY;

            //Creating the size of bytes similar to the image considering 3 bytes of each pixel (RGB)
            byte[] result = new byte[size];

            Array.Copy(image.Bytes, result, size);

            //Getting the size of the line
            long scanLine = (width * 3); //- 3;


            for (int line = 0; line < height; line++)
            {
                for (int column = 0; column < width; column++)
                {
                    long linePos = (column * 3) + scanLine * (line);


                    offSetX = _offsetPoints[column, line].X;
                    offSetY = _offsetPoints[column, line].Y;

                    if (offSetY >= 0 && offSetY < height && offSetX >= 0 && offSetX < width)
                    {

                        result[linePos + ImageBitmap.COLOR_BLUE] = image.Bytes[(offSetY * scanLine) + (offSetX * 3) + ImageBitmap.COLOR_BLUE];
                        result[linePos + ImageBitmap.COLOR_GREEN] = image.Bytes[(offSetY * scanLine) + (offSetX * 3) + ImageBitmap.COLOR_GREEN];
                        result[linePos + ImageBitmap.COLOR_RED] = image.Bytes[(offSetY * scanLine) + (offSetX * 3) + ImageBitmap.COLOR_RED];
                    }


                }
            }

            Model.IImage imageResult = new Model.ImageBitmap(width, height, result);

            return imageResult;
        }
        public Model.IImage OffSetNormal(Model.IImage image)
        {
            int width = image.Image.Width;
            int height = image.Image.Height;
            int size = width * height * 3;
            int offSetX, offSetY;

            //Creating the size of bytes similar to the image considering 3 bytes of each pixel (RGB)
            byte[] result = new byte[size];

            Array.Copy(image.Bytes, result, size);

            //Getting the size of the line
            long scanLine = (width * 3); //- 3;


            for (int line = 0; line < height; line++)
            {
                for (int column = 0; column < width; column++)
                {
                    long linePos = (column * 3) + scanLine * (line);


                    offSetX = _offsetPoints[column, line].X;
                    offSetY = _offsetPoints[column, line].Y;

                    if (line + offSetY >= 0 && line + offSetY < height &&
                        column + offSetX >= 0 && column + offSetX < width)
                    {
                        result[linePos + ImageBitmap.COLOR_BLUE] = image.Bytes[((line + offSetY) * scanLine) + ((column + offSetX) * 3) + ImageBitmap.COLOR_BLUE];
                        result[linePos + ImageBitmap.COLOR_GREEN] = image.Bytes[((line + offSetY) * scanLine) + ((column + offSetX) * 3) + ImageBitmap.COLOR_GREEN];
                        result[linePos + ImageBitmap.COLOR_RED] = image.Bytes[((line + offSetY) * scanLine) + ((column + offSetX) * 3) + ImageBitmap.COLOR_RED];
                    }

                }
            }

            Model.IImage imageResult = new Model.ImageBitmap(width, height, result);

            return imageResult;
        }
        #endregion

        #region ICommand Members
        public Model.IImage Execute(Model.IImage image)
        {
            if (_abs)
                return OffSetAbs(image);
            else
                return OffSetNormal(image);

        }
        #endregion
    }
}
