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
    public class ContourBase
    {
        #region Constants

        public const int PIXEL_BLACK = 0;
        public const int PIXEL_WHITE = 255;
        public const int PIXEL_ON_GOING = 100;
        public const int PIXEL_DONE = 50;

        #endregion

        #region Variables

        protected bool _showPoints = true;
        protected IList<Point> _list;
        protected readonly int MaxIterations = 5000;
        protected IList<ContourComponent> _components;

        #endregion

        #region Properties
        public IList<ContourComponent> Components
        {
            get { return _components; }
        }
        #endregion

        #region Constructors/Destructors

        public ContourBase(bool showPoints)
        {
            _showPoints = showPoints;
        }

        #endregion

        #region Methods

        protected int GetPos(int x, int y, int stride, int bytesPerPixel)
        {
            return (y - 1) * stride + x * bytesPerPixel;
        }
        public Model.IImage ShowPoints(Model.IImage image, IList<Point> list)
        {
            int bytesPerPixel = 3;
            int width = image.Image.Width;
            int height = image.Image.Height;
            int size = width * height * bytesPerPixel;
            int stride = width * bytesPerPixel;
            int pos = 0;

            byte[] result = new byte[size];
            Array.Copy(image.Bytes, result, size);


            foreach (Point point in list)
            {
                pos = GetPos(point.X, point.Y, stride, bytesPerPixel);

                result[pos + Model.ImageBitmap.COLOR_RED] = 255;
                result[pos + Model.ImageBitmap.COLOR_GREEN] = 0;
                result[pos + Model.ImageBitmap.COLOR_BLUE] = 0;

            }

            return new Model.ImageBitmap(width, height, result);
        }
        protected Model.IImage SearchForInitial(Model.IImage image)
        {
            int bytesPerPixel = 3;
            int width = image.Image.Width;
            int height = image.Image.Height;
            int size = width * height * bytesPerPixel;
            int stride = width * bytesPerPixel;

            byte[] result = new byte[size];
            Array.Copy(image.Bytes, result, size);


            for (int line = height; line >= 0; line--)
            {
                int currentPos = GetPos(0, line, stride, bytesPerPixel);

                for (int column = 0; column < width; column++)
                {

                    //Found a black pixel
                    if (result[currentPos] == 0)
                    {
                        IList<Point> points = Extract(column, line, height, width, stride, bytesPerPixel, result);

                        _list = points;

                        if (_showPoints)
                            return ShowPoints(image, points);
                        else
                            return image;

                    }

                    currentPos += 3;
                }
            }
            return image;
        }
        protected virtual IList<Point> Extract(int initialX, int initialY, int height, int width, int stride, int bytesPerPixel, byte[] image)
        {
            return new List<Point>();
        }
        protected IList<ContourComponent> TrackIt(Model.IImage image)
        {
            IList<ContourComponent> resultList = new List<ContourComponent>();

            int bytesPerPixel = 3;
            int width = image.Image.Width;
            int height = image.Image.Height;
            int size = width * height * bytesPerPixel;
            int stride = width * bytesPerPixel;

            byte[] result = new byte[size];
            Array.Copy(image.Bytes, result, size);

            for (int line = height; line > 0; line--)
            {
                int currentPos = GetPos(0, line, stride, bytesPerPixel);

                for (int column = 0; column < width; column++)
                {
                    
                    //Found a black pixel
                    if (result[currentPos] == PIXEL_BLACK)
                    {
                        IList<Point> points = Extract(column, line, height, width, stride, bytesPerPixel, result);

                        _list = points;

                        //if (_showPoints)
                        //    return ShowPoints(image, points);
                        //else
                        //    return image;


                        for (int c = 0; c < points.Count; c++)
                        {
                            int pos = GetPos(points[c].X, points[c].Y, stride, bytesPerPixel);
                            result[pos+0] = PIXEL_DONE;
                            result[pos+1] = PIXEL_DONE;
                            result[pos+2] = PIXEL_DONE;
                        }

                        if (points.Count > 1 && points[0].X == points[points.Count - 1].X && points[0].Y == points[points.Count - 1].Y)
                        {

                            resultList.Add(new ContourComponent(points));
                        }

                    }

                    currentPos += 3;
                }
            }

            //image = new Model.ImageBitmap(width, height, result);


            return resultList;
        }

        #endregion
    }
}
