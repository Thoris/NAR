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

namespace NAR.ImageProcessing.Corner
{
    public class HarrisCornersCommand : CornerBase, ICommand, ICornerDetector
    {
        #region Variables
        //protected int[] gray;                //gray scale image vector

        //protected int[] _gradX;               //gradient on x axe
        //protected int[] _gradY;               //gradient on y axe


        //order 2 moment matrix (structured tensor)
        // M = | Mx  Mxy |
        //     | Mxy My  |
        //
        //protected int[] _mx;
        //protected int[] _my;
        //protected int[] _mxy;

        private double _k;
        private int _cornerTreshold;
        private int _edgeTreshold;
        private int _supressWindowSize;


        protected readonly int _filterSize = 3;
        protected readonly int[] _dx = new int[9] 
            {
                -1,  0,  1, 
                -2,  0,  2, 
                -1,  0,  1 
            };
        protected readonly int[] _dy = new int[9] 
            { 
                -1, -2, -1, 
                 0,  0,  0, 
                 1,  2,  1
            };

        protected readonly int _gaussianKernelWindowSize = 5;
        protected readonly double[] GaussianKernel = new double[25] { 
            ((double)1 / (double)84), ((double)2 / (double)84), ((double)3 / (double)84), ((double)2 / (double)84), ((double)1 / (double)84), 
            ((double)2 / (double)84), ((double)5 / (double)84), ((double)6 / (double)84), ((double)5 / (double)84), ((double)2 / (double)84),
            ((double)3 / (double)84), ((double)6 / (double)84), ((double)8 / (double)84), ((double)6 / (double)84), ((double)3 / (double)84),
            ((double)2 / (double)84), ((double)5 / (double)84), ((double)6 / (double)84), ((double)5 / (double)84), ((double)2 / (double)84), 
            ((double)1 / (double)84), ((double)2 / (double)84), ((double)3 / (double)84), ((double)2 / (double)84), ((double)1 / (double)84) 
        };
        
        

        #endregion

        #region Constructors/Destructors

        public HarrisCornersCommand(double k, int cornerTreshold, int edgeTreshold, int supressWindowSize, bool showPoints)
            : base (showPoints)
        {
            _k = k;
            _cornerTreshold = cornerTreshold;
            _edgeTreshold = edgeTreshold;
            _supressWindowSize = supressWindowSize;
            _showPoints = showPoints;

        }

        #endregion

        #region Methods

        public Model.IImage Execute(Model.IImage image)
        {

            int width = image.Image.Width;
            int height = image.Image.Height;
            int sizeGray = width * height;
            int size = width * height * 3;

            int[] gray = new int[sizeGray];
            int [] gradX;
            int [] gradY;
            int [] mx;
            int [] my;
            int [] mxy;


            //Creating the size of bytes similar to the image considering 3 bytes of each pixel (RGB)
            //byte[] result = new byte[size];

            for (int c = 0, pos = 0; c < size; c+=3, pos++)
                gray[pos] = image.Bytes[c];


            //compute image grayscale gradient
            BuildGradient(width, height, gray, _dx, _dy, _filterSize, out gradX, out gradY);

            //compute order 2 moment matrix;
            BuildOrder2MomentMatrix(width, height, gradX, gradY, out mx, out my, out mxy);

            //compute harris reponse for corners
            BuildHarrisResponse(width, height, gray, mx, my, mxy, _k, _cornerTreshold);

            //apply non-maximum supression algorithm to eliminate the weak corners
            ApplyNONMaxSupression(width, height, _supressWindowSize, ref gray);
    
            return WriteResultImage(width, height, image, gray);

            //return null;
        }
        /// <summary>
        /// Builds the gradient of the image.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="gray">The gray vector indicating the image.</param>
        /// <param name="dx">convolution operation between grayscale image and soebel operator for x axe.</param>
        /// <param name="dy">convolution operation between grayscale image and soebel operator for y axe.</param>
        /// <param name="kernelSize">Size of the kernel.</param>
        /// <param name="gradX">The grad X.</param>
        /// <param name="gradY">The grad Y.</param>
        private void BuildGradient(int width, int height, int [] gray, int [] dx, int [] dy, int kernelSize, out int [] gradX, out int [] gradY)
        {
            int sizeGray = width * height;
            gradX = new int[sizeGray];
            gradY = new int[sizeGray];
            
            int lineCoord = 0;

            for (int i = width + 1; i < (height * width) - width - 1; i++)
            {
                //avoid to exceed image left and right margins and loss of information
                lineCoord = i % width;
                if (lineCoord <= width - 1 && lineCoord >= 1)
                {
                    gradX[i] = GradSpatialConvolution(width, height, gray, i, dx, kernelSize);
                    gradY[i] = GradSpatialConvolution(width, height, gray, i, dy, kernelSize);
                }

            }
        }
        /// <summary>
        /// Build spatial convolution of data[x]
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="data">The data of the image.</param>
        /// <param name="k"></param>
        /// <param name="kernelFilter">The kernel filter.</param>
        /// <param name="kernelSize">Size of the kernel.</param>
        /// <returns></returns>
        protected int GradSpatialConvolution(int width, int height, int[] data, int k, int[] kernelFilter, int kernelSize)
        {
            int output = 0;
            int l = 0;
            for (int i = -(kernelSize / 2); i <= (kernelSize / 2); i++)
            {
                for (int j = -(kernelSize / 2); j <= (kernelSize / 2); j++)
                {
                    output += data[k + i * width + j] * kernelFilter[l];
                    l++;
                }
            }

            return output;

        }
        /// <summary>
        /// Build order 2 moment matrix (structured tensor) using gradient on x and y axes
        /// 
        ///    M  = | G conv (grad_x*grad_x)  G conv (grad_x*grad_y) |
        ///         | G conv (grad_x*grad_y)  G conv (grad_y*grad_y) |
        ///         
        ///    G = gaussian kernel
        ///    conv = convolution operation;
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="gradX">Vector with contains the X result</param>
        /// <param name="gradY">Vector with contains the Y result</param>
        /// <param name="mx">The result of Mx.</param>
        /// <param name="my">The result of My.</param>
        /// <param name="mxy">The result of Mxy.</param>
        private void BuildOrder2MomentMatrix(int width, int height, int []  gradX, int [] gradY, out int [] mx, out int [] my, out int [] mxy)
        {
            int sizeGray = width * height;
            
            mx = new int[sizeGray];
            my = new int[sizeGray];
            mxy = new int[sizeGray];


            int line_coord = 0;
            for (int i = (2 + 1) * width + 2; i < (height * width) - (2 + 1) * width - 2; i++)
            {
                //avoid to exceed image left and right margins and loss of information
                line_coord = i % width;
                if (line_coord <= width - 2 - 1 && line_coord >= 2 + 1)
                {
                    mx[i] = MSpatialConvolution(width, height, gradX, gradX, i, GaussianKernel, 5);
                    my[i] = MSpatialConvolution(width, height, gradY, gradY, i, GaussianKernel, 5);
                    mxy[i] = MSpatialConvolution(width, height, gradX, gradY, i, GaussianKernel, 5);
                }
            }
        }
        /// <summary>
        /// Spatial convolution for 1 element of the order 2 moment matrix.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="data1">Array which are multiplied element by element.</param>
        /// <param name="data2">Array which are multiplied element by element.</param>
        /// <param name="k">order coordinate for the center pixel.</param>
        /// <param name="kernelFilter">kernel filter for convolution.</param>
        /// <param name="kernelSize">dimension of the kernel (ex if kernel is 5x5 then kernelsize = 5).</param>
        /// <returns></returns>
        protected int MSpatialConvolution(int width, int height, int[] data1, int[] data2, int k, double[] kernelFilter, int kernelSize)
        {
            int output = 0;
            int l = 0;
            for (int i = -(kernelSize / 2); i <= (kernelSize / 2); i++)
            {
                for (int j = -(kernelSize / 2); j <= (kernelSize / 2); j++)
                {
                    output += (int)(data1[k + i * width + j] * data2[k + i * width + j] * kernelFilter[l]);
                    l++;
                }
            }

            return output;

        }
        /// <summary>
        /// Compute the harris response and apply a treshold to the result to diferentiate pixels which are on a corner,
        ///    which are on a flat region, or which are on an edge
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="gray">The gray vector with contains gray image.</param>
        /// <param name="mx">The mx vector.</param>
        /// <param name="my">The my vector.</param>
        /// <param name="mxy">The mxy vector.</param>
        /// <param name="k">Order coordinate for the center pixel.</param>
        /// <param name="cornerTreshold">The corner treshold.</param>
        private void BuildHarrisResponse(int width, int height, int [] gray, int [] mx, int [] my, int[] mxy, double k, int cornerTreshold)
        {
            for (int i = 0; i < width * height; i++)
            {
                gray[i] = (int)((double)(mx[i] * my[i]) - (mxy[i] * mxy[i]) - (double)k * ((double)(mx[i] + my[i]) * (mx[i] + my[i])));

                //apply the treshold to diferentiate corners pixels from edge pixels or from flat regions pixels
                if (gray[i] < cornerTreshold)
                {
                    gray[i] = 0;

                }

            }
        }
        /// <summary>
        /// Apply non-maximum supression algorithm for corners and edge.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="supressWindowSize">Size of the supress window.</param>
        /// <param name="corners">The corners vector with contains the points.</param>
        private void ApplyNONMaxSupression(int width, int height, int supressWindowSize, ref int [] corners)
        {
            int lineCoord = 0;

            for (int i = (supressWindowSize / 2 + 1) * width + (supressWindowSize / 2);
                     i < (height * width) - (supressWindowSize / 2 + 1) * width - (supressWindowSize / 2);
                     i++)
            {
                lineCoord = i % width;
                if (lineCoord <= width - (supressWindowSize / 2 + 1) && lineCoord >= (supressWindowSize / 2 + 1))
                {

                    if (corners[i] != 0)
                    {
                        //verify i pixel suround based on nm_supress_window_size
                        for (int kk = -(supressWindowSize / 2); kk <= (supressWindowSize / 2); kk++)
                        {
                            for (int ll = -(supressWindowSize / 2); ll <= (supressWindowSize / 2); ll++)
                            {
                                if (corners[i + kk * width + ll] > corners[i])
                                {
                                    corners[i] = 0;
                                    break;
                                }
                            }
                        }

                    }
                }
            }
        }

        public Model.IImage WriteResultImage(int width, int height, Model.IImage original, int [] corners)
        {

            int size = width * height * 3;
            byte[] result = new byte[size];
            Array.Copy(original.Bytes, result, size);


            int kk = 0;
            int ll = 0;

            //for (int i = 0; i < img_height * img_width; i++)
            //{
            //    rgb[i * 3] = (byte)corners[i];
            //    rgb[i * 3 + 1] = (byte)corners[i];
            //    rgb[i * 3 + 2] = (byte)corners[i];
            //}


            for (int i = 2 * width + 2; i < width * height - 2 * width - 2; i++)
            {
                kk = i / width; //col number
                ll = i % width; //row number
                //rgb[i * 3] = (byte)corners[ll * img_height + kk];
                //rgb[i * 3 + 1] = (byte)corners[ll * img_height + kk];
                //rgb[i * 3 + 2] = (byte)corners[ll * img_height + kk];
                if ((byte)corners[i] != 0)
                {
                    for (int ii = -2; ii <= 2; ii++)
                    {
                        for (int jj = -2; jj <= 2; jj++)
                        {
                            result[(i + ii * width + jj) * 3] = 255;
                            result[(i + ii * width + jj) * 3 + 1] = 0;
                            result[(i + ii * width + jj) * 3 + 2] = 0;
                        }
                    }


                }
            }

            //System.IO.MemoryStream ms = new System.IO.MemoryStream(rgb);
            //Bitmap bmp = new Bitmap(img_width, img_height, PixelFormat.Format24bppRgb);
            //BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);
            //Marshal.Copy(rgb, 0, bmpData.Scan0, rgb.Length);
            //bmp.UnlockBits(bmpData);
            //bmp.Save(file_path);

            return new Model.ImageBitmap(width, height, result);

        }

        #endregion
    }
}
