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
using NAR.Model;
using System.Drawing;
using System.Windows.Forms;

namespace NAR.ImageProcessing.Motion
{
    public class MotionDetector : IMotion
    {
        #region Variables
        private int _resX;
        private int _resY;
        private byte _relevance;
        private int _qtdPixel;
        private Model.IImage _lastImage;
        #endregion

        #region Properties
        public Model.IImage LastImage
        {
            get { return _lastImage; }
        }
        #endregion

        #region Constructors/Destructors
        public MotionDetector(int resX, int resY, byte relevance, int qtdPixel)
        {
            _relevance = relevance;
            _resX = resX;
            _resY = resY;
            _qtdPixel = qtdPixel;
        }
        #endregion

        #region Methods

        private int FillRegionMotion(ref byte[,] motion, int resX, int resY, int px, int py, int depth,
                ref int minX, ref int maxX, ref int minY, ref int maxY, int regionArea)
        {
            if (motion[px, py] == 1)
            {
                motion[px, py] = 255;

                regionArea++;


                if (px < minX)
                    minX = px;
                if (px > maxX)
                    maxX = px;
                if (py < minY)
                    minY = py;
                if (py > maxY)
                    maxY = py;

                //If didn't find 100 routines
                if (depth < 100)
                {
                    //--------------
                    //    o # o
                    //    o o o
                    //    o o o 
                    //--------------
                    if (py > 0)
                        regionArea = FillRegionMotion(ref motion,
                            resX, resY,
                            px, py - 1,
                            depth + 1,
                            ref minX, ref maxX, ref minY, ref maxY,
                            regionArea);
                    //--------------
                    //    o o #
                    //    o o o
                    //    o o o 
                    //--------------
                    if (px < resX - 1 && py > 0)
                        regionArea = FillRegionMotion(ref motion,
                            resX, resY,
                            px + 1, py - 1,
                            depth + 1,
                            ref minX, ref maxX, ref minY, ref maxY,
                            regionArea);
                    //--------------
                    //    o o o
                    //    o o #
                    //    o o o 
                    //--------------
                    if (px < resX - 1)
                        regionArea = FillRegionMotion(ref motion,
                            resX, resY,
                            px + 1, py,
                            depth + 1,
                            ref minX, ref maxX, ref minY, ref maxY,
                            regionArea);
                    //--------------
                    //    o o o
                    //    o o o
                    //    o o # 
                    //--------------
                    //if (px < resX -1 && py < resY - 2 )
                    //	regionArea = FillRegionMotion (ref motion, resX, resY, px + 1, py + 1, depth + 1, ref minX,ref maxX,ref minY,ref maxY,  regionArea);
                    //--------------
                    //    o o o
                    //    o o o
                    //    o # o 
                    //--------------
                    if (py < resY - 1)
                        regionArea = FillRegionMotion(ref motion,
                            resX, resY,
                            px, py + 1,
                            depth + 1,
                            ref minX, ref maxX, ref minY, ref maxY,
                            regionArea);
                    //--------------
                    //    o o o
                    //    # o o
                    //    o o o 
                    //--------------
                    if (px > 0)
                        regionArea = FillRegionMotion(ref motion,
                            resX, resY,
                            px - 1, py,
                            depth + 1,
                            ref minX, ref maxX, ref minY, ref maxY,
                            regionArea);
                    //--------------
                    //    # o o
                    //    o o o
                    //    o o o 
                    //--------------
                    if (px > 0 && py > 0)
                        regionArea = FillRegionMotion(ref motion,
                            resX, resY,
                            px - 1, py - 1,
                            depth + 1,
                            ref minX, ref maxX, ref minY, ref maxY,
                            regionArea);
                    //--------------
                    //    o o o
                    //    o o o
                    //    # o o 
                    //--------------
                    if (px > 0 && py < resY - 1)
                        regionArea = FillRegionMotion(ref motion,
                            resX, resY,
                            px - 1, py + 1,
                            depth + 1,
                            ref minX, ref maxX, ref minY, ref maxY,
                            regionArea);
                    //--------------
                    //    o o o
                    //    o o o
                    //    o o #
                    //--------------
                    if (px < resX - 1 && py < resY - 1)
                        regionArea = FillRegionMotion(ref motion,
                            resX, resY,
                            px + 1, py + 1,
                            depth + 1,
                            ref minX, ref maxX, ref minY, ref maxY,
                            regionArea);

                }//endif depth < 100
            }//endif Motion == 1

            return regionArea;
        }

        public Model.IImage FillImageMotion(Model.IImage image, IList<MotionPosition> list)
        {
            Bitmap imgManage = (Bitmap) image.Image.Clone();


            Graphics graph  = Graphics.FromImage(imgManage);

            foreach (MotionPosition pos in list)
            {
                graph.DrawRectangle(new Pen(Color.Red, 2), pos.X, pos.Y, pos.Width - pos.X, pos.Height - pos.Y);
            }

            Model.IImage newImage = new Model.ImageBitmap(imgManage);

            return newImage;


        }

        #endregion

        #region IMotion Members
        public IList<MotionPosition> Detect(Model.IImage currentImage)
        {
            IList<MotionPosition> result = new List<MotionPosition>();
            byte[,] motion = new byte[_resX, _resY];

            if (_lastImage == null)
            {
                _lastImage = currentImage;
                return result;
            }


            int width = currentImage.Image.Width;
            int height = currentImage.Image.Height;
            int size = width * height * 3;
            
            //Creating the size of bytes similar to the image considering 3 bytes of each pixel (RGB)
            byte[] entry = new byte[size];


            for (int c = 0; c < entry.Length; c++)
            {
                entry[c] = (byte)(_lastImage.Bytes[c] ^ currentImage.Bytes[c]);
            }

            int line = 3 * width;


            int y2 = 0, x2 = 0;

            int sx = width / _resX;
            int sy = height / _resY;



            for (int x = sx; x < width - 1; x += sx)
            {
                y2 = 0;
                for (int y = sy; y < height - 1; y += sy)
                {
                    int pos = (line * y) + (3 * x);

                    if (entry[pos + ImageBitmap.COLOR_RED] > _relevance &&
                        entry[pos + ImageBitmap.COLOR_GREEN] > _relevance &&
                        entry[pos + ImageBitmap.COLOR_BLUE] > _relevance)

                        motion[x2, y2] = 1; //Pixel presente
                    else
                        motion[x2, y2] = 0; //Pixel Ausente

                    y2++;
                }//end for y
                x2++;
            }//end for x




            int regionArea = 0,	maxX, maxY, minX, minY;		

            for (int x = 0; x < _resX; x++)
            {
                for (int y = 0; y < _resY; y++)
                {
                    if (motion[x, y] == 1)
                    {
                        regionArea = 0;
                        minX = x;
                        maxX = x;
                        minY = y;
                        maxY = y;
                        
                        regionArea = FillRegionMotion(ref motion, _resX, _resY, x, y, 0, ref minX, ref maxX, ref minY, ref maxY, 0);

                        if (regionArea > _qtdPixel)
                        {
                            //int[] aiPos = new int[4];
                            //aiPos[0] = (minX) * sx;
                            //aiPos[1] = (maxX) * sx;
                            //aiPos[2] = (minY) * sy;
                            //aiPos[3] = (maxY) * sy;

                            MotionPosition position = new MotionPosition();
                            position.X = (minX) * sx;
                            position.Width = (maxX) * sx;
                            position.Y = (minY) * sy;
                            position.Height = (maxY) * sy;

                            result.Add(position);
                        }//endif iRegionArea >

                    }
                }//end for y
            }//end for x





            _lastImage = currentImage;
            return result;
        }
        #endregion
    }
}
