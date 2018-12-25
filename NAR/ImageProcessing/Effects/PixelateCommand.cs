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

namespace NAR.ImageProcessing.Effects
{
    public class PixelateCommand : Base.OffsetFilterCommand, ICommand
    {
        #region Variables
        private short _pixel;
        private bool _grid;
        #endregion

        #region Properties
        public short Pixel
        {
            get { return _pixel; }
        }
        public bool Grid
        {
            get { return _grid; }
        }
        #endregion

        #region Constructors/Destructors
        public PixelateCommand(short pixel, bool grid)
            : base()
        {
            _pixel = pixel;
            _grid = grid;
        }
        #endregion

        #region ICommand Members
        public new Model.IImage Execute(Model.IImage image)
        {
            int width = image.Image.Width;
            int height = image.Image.Height;

            Point[,] pt = new Point[width, height];

            int newX, newY;
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    newX = _pixel - x % _pixel;

                    if (_grid && newX == _pixel)
                        pt[x, y].X = -x;
                    else if (x + newX > 0 && x + newX < width)
                        pt[x, y].X = newX;
                    else
                        pt[x, y].X = 0;

                    newY = _pixel - y % _pixel;

                    if (_grid && newY == _pixel)
                        pt[x, y].Y = -y;
                    else if (y + newY > 0 && y + newY < height)
                        pt[x, y].Y = newY;
                    else
                        pt[x, y].Y = 0;
                }
            }


            base.OffsetPoints = pt;

            return base.Execute(image);
            
        }
        #endregion
    }
}
