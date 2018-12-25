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

namespace NAR.ImageProcessing.Corner
{
    public class CornerBase
    {
        #region Variables

        protected bool _showPoints;
        protected IList<Point> _cornersList = new List<Point>();

        #endregion

        #region Properties

        public IList<Point> Corners
        {
            get { return _cornersList; }
        }

        #endregion

        #region Constructors/Destructors

        public CornerBase(bool showPoints)
        {
            _showPoints = showPoints;
        }

        #endregion

        #region Methods

        public Model.IImage ShowPoints(Model.IImage image)
        {
            int width = image.Image.Width;
            int height = image.Image.Height;


            int size = width * height * 3;
            byte[] result = new byte[size];
            Array.Copy(image.Bytes, result, size);


            //int kk = 0;
            //int ll = 0;

            for (int c = 0; c < _cornersList.Count; c++)
            {
                int line = _cornersList[c].Y * 3 * width;
                //int pos = line + (_cornersList[c].X * 3);


                for (int ll = -1; ll < 2; ll++)
                {
                    int currentLine = line + width * ll * 3 ;
                    for (int kk = -1; kk < 2; kk++)
                    {
                        int currentColumn = currentLine + _cornersList[c].X * 3 + kk * 3;


                        result[currentColumn + ImageBitmap.COLOR_RED] = 255;
                        result[currentColumn + ImageBitmap.COLOR_GREEN] = 0;
                        result[currentColumn + ImageBitmap.COLOR_BLUE] = 0;

                    }
                }




                //kk = i / width; //col number
                //ll = i % width; //row number


                //for (int ii = -2; ii <= 2; ii++)
                //{
                //    for (int jj = -2; jj <= 2; jj++)
                //    {
                //        result[(i + ii * width + jj) * 3] = 255;
                //        result[(i + ii * width + jj) * 3 + 1] = 0;
                //        result[(i + ii * width + jj) * 3 + 2] = 0;
                //    }
                //}

            }


            return new Model.ImageBitmap(width, height, result);

        }
        #endregion

    }
}
