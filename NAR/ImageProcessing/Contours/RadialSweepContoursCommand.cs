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
    public class RadialSweepContoursCommand : ContourBase, ICommand, IContoursDetector
    {
         #region Constructors/Destructors

        public RadialSweepContoursCommand(bool showPoints)
            : base (showPoints)
        {
        }

        #endregion

        #region Methods
        protected override IList<Point> Extract(int initialX, int initialY, int height, int width, int stride, int bytesPerPixel, byte[] image)
        {
            bool initial = true;
            int iteration = 0;
            IList<Point> list = new List<Point>();

            Direction direction = Direction.Down;
            Direction nextDirection = direction;

            int nextX = initialX;
            int nextY = initialY;

            int currentPosX = initialX;
            int currentPosY = initialY;

            list.Add(new Point(initialX, initialY));

            int currentPos = GetPos(initialX, initialY, stride, bytesPerPixel);
            image[currentPos] = 255;


            while ((nextX != initialX || nextY != initialY || initial) && ++iteration < base.MaxIterations)
            {
                initial = false;

                bool found = false;
                for (int c = 0; c < 8; c++)
                {
                    direction = nextDirection;

                    #region Checking next direction
                    if (nextDirection == Direction.Down)
                    {
                        nextX = currentPosX;
                        nextY = currentPosY + 1;
                        currentPos = GetPos(nextX, nextY, stride, bytesPerPixel);
                        nextDirection = Direction.DownLeft;
                    }
                    else if (nextDirection == Direction.DownLeft)
                    {
                        nextX = currentPosX - 1;
                        nextY = currentPosY + 1;
                        currentPos = GetPos(nextX, nextY, stride, bytesPerPixel);
                        nextDirection = Direction.Left;
                    }
                    else if (nextDirection == Direction.Left)
                    {
                        nextX = currentPosX - 1;
                        nextY = currentPosY;
                        currentPos = GetPos(nextX, nextY, stride, bytesPerPixel);
                        nextDirection = Direction.UpLeft;
                    }
                    else if (nextDirection == Direction.UpLeft)
                    {
                        nextX = currentPosX - 1;
                        nextY = currentPosY - 1;
                        currentPos = GetPos(nextX, nextY, stride, bytesPerPixel);
                        nextDirection = Direction.Up;
                    }
                    else if (nextDirection == Direction.Up)
                    {
                        nextX = currentPosX;
                        nextY = currentPosY - 1;
                        currentPos = GetPos(nextX, nextY, stride, bytesPerPixel);
                        nextDirection = Direction.UpRight;
                    }
                    else if (nextDirection == Direction.UpRight)
                    {
                        nextX = currentPosX + 1;
                        nextY = currentPosY - 1;
                        currentPos = GetPos(nextX, nextY, stride, bytesPerPixel);
                        nextDirection = Direction.Right;
                    }
                    else if (nextDirection == Direction.Right)
                    {
                        nextX = currentPosX + 1;
                        nextY = currentPosY;
                        currentPos = GetPos(nextX, nextY, stride, bytesPerPixel);
                        nextDirection = Direction.DownRight;
                    }
                    else if (nextDirection == Direction.DownRight)
                    {
                        nextX = currentPosX + 1;
                        nextY = currentPosY + 1;
                        currentPos = GetPos(nextX, nextY, stride, bytesPerPixel);
                        nextDirection = Direction.Down;
                    }
                    #endregion


                    //If found a black pixel
                    if (image[currentPos] == 0)
                    {
                        list.Add(new Point(nextX, nextY));
                        found = true;

                        image[currentPos] = 255;
                        currentPosX = nextX;
                        currentPosY = nextY;

                        switch (direction)
                        {
                            case Direction.Down:
                                nextDirection = Direction.Up;
                                break;
                            case Direction.DownLeft:
                                nextDirection = Direction.UpRight;
                                break;
                            case Direction.DownRight:
                                nextDirection = Direction.UpLeft;
                                break;
                            case Direction.Left:
                                nextDirection = Direction.Right;
                                break;
                            case Direction.Right:
                                nextDirection = Direction.Left;
                                break;
                            case Direction.Up:
                                nextDirection = Direction.Down;
                                break;
                            case Direction.UpLeft:
                                nextDirection = Direction.DownRight;
                                break;
                            case Direction.UpRight:
                                nextDirection = Direction.DownLeft;
                                break;
                        }

                        break;


                    }

                }

                if (!found)
                    return list;


            }





            return list;
        }
        #endregion

        #region ICommand Members

        public Model.IImage Execute(Model.IImage image)
        {
            return base.SearchForInitial(image);
        }

        #endregion
    
    }
}
