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

namespace NAR.ImageProcessing.Contours
{
    public class SquareContoursCommand : ContourBase, ICommand, IContoursDetector
    {
        #region Constructors/Destructors

        public SquareContoursCommand(bool showPoints)
            : base (showPoints)
        {
        }

        #endregion

        #region Methods
        protected override IList<Point> Extract(int initialX, int initialY, int height, int width, int stride, int bytesPerPixel, byte[] image)
        {
            int iteration = 0;
            IList<Point> list = new List<Point>();

            Direction direction = Direction.Left;

            int nextX = initialX - 1;
            int nextY = initialY;

            list.Add(new Point(initialX, initialY));

            int currentPos = GetPos(initialX, initialY, stride, bytesPerPixel);

            //image[currentPos + Model.ImageBitmap.COLOR_RED] = 255;
            //image[currentPos + Model.ImageBitmap.COLOR_GREEN] = 0;
            //image[currentPos + Model.ImageBitmap.COLOR_BLUE] = 0;


            while ((nextX != initialX || nextY != initialY) && ++iteration < base.MaxIterations)
            {

                currentPos = GetPos(nextX, nextY, stride, bytesPerPixel);
                

                if (image[currentPos] == ContourBase.PIXEL_BLACK ||
                    image[currentPos] == ContourBase.PIXEL_ON_GOING)
                {
                 
                    if (image[currentPos] == ContourBase.PIXEL_BLACK)
                        list.Add(new Point(nextX, nextY));


                    image[currentPos] = ContourBase.PIXEL_ON_GOING;
                    //image[currentPos+1] = ContourBase.PIXEL_ON_GOING;
                    //image[currentPos+2] = ContourBase.PIXEL_ON_GOING;

                    //image[currentPos + Model.ImageBitmap.COLOR_RED] = 255;
                    //image[currentPos + Model.ImageBitmap.COLOR_GREEN] = 0;
                    //image[currentPos + Model.ImageBitmap.COLOR_BLUE] = 0;

                    switch (direction)
                    {
                        case Direction.Up:

                            direction = Direction.Left;
                            nextX--;

                            break;

                        case Direction.Left:

                            direction = Direction.Down;
                            nextY++;

                            break;

                        case Direction.Right:

                            direction = Direction.Up;
                            nextY--;

                            break;

                        case Direction.Down:

                            direction = Direction.Right;
                            nextX++;

                            break;
                    }

                }
                else if (image[currentPos] == ContourBase.PIXEL_WHITE)
                {
                    switch (direction)
                    {
                        case Direction.Up:

                            direction = Direction.Right;
                            nextX++;

                            break;

                        case Direction.Left:

                            direction = Direction.Up;
                            nextY--;

                            break;

                        case Direction.Right:

                            direction = Direction.Down;
                            nextY++;

                            break;

                        case Direction.Down:

                            direction = Direction.Left;
                            nextX--;

                            break;
                    }

                }


            }

            if (list.Count > 1 && list[0].X == nextX && list[0].Y == nextY)
                list.Add (new Point(nextX, nextY));


            return list;
        }
        #endregion

        #region ICommand Members

        public Model.IImage Execute(Model.IImage image)
        {
            //return base.SearchForInitial(image);
            IList<ContourComponent> list = base.TrackIt(image);


            base._components = list;

            if (base._showPoints)
            {
                Model.IImage result = new Model.ImageBitmap(image.Image);
                for (int c = 0; c < list.Count; c++)
                {
                    result = base.ShowPoints(result, list[c].Points);
                }
                return result;
            }

            return image;            
        }

        #endregion
    
    }
}
