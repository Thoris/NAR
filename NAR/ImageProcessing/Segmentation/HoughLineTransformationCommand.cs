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

namespace NAR.ImageProcessing.Segmentation
{
    public class HoughLineTransformationCommand : HoughLineTransformation , ICommand
    {
        #region Properties
        public Pen Pen
        {
            get { return _pen; }
            set { _pen = value; }
        }

        public bool ShowLines
        {
            get { return _showLines; }
            set { _showLines = value; }
        }
        #endregion

        #region Variables
        private bool _showLines = true;
        private Pen _pen = new Pen(Color.Red, 1);
        #endregion

        #region Constructors/Destructors
        public HoughLineTransformationCommand(bool showLines)
        {

        }
        #endregion

        #region ICommand Members
        public new Model.IImage Execute(Model.IImage image)
        {
            Model.IImage result = base.Execute(image) ;

            Model.IImage newData = (Model.IImage)result.Clone();


            if (_showLines)
            {
                HoughLine[] list = GetLinesByRelativeIntensity(0.5);

                Graphics graph = Graphics.FromImage(newData.Image);

                foreach (HoughLine line in list)
                {

                    // get line's radius and theta values
                    int r = line.Radius;
                    double t = line.Theta;

                    // check if line is in lower part of the image
                    if (r < 0)
                    {
                        t += 180;
                        r = -r;
                    }

                    // convert degrees to radians
                    t = (t / 180) * Math.PI;

                    // get image centers (all coordinate are measured relative
                    // to center)
                    int w2 = image.Image.Width / 2;
                    int h2 = image.Image.Height / 2;

                    double x0 = 0, x1 = 0, y0 = 0, y1 = 0;

                    if (line.Theta != 0)
                    {
                        // none vertical line
                        x0 = -w2; // most left point
                        x1 = w2;  // most right point

                        // calculate corresponding y values
                        y0 = (-Math.Cos(t) * x0 + r) / Math.Sin(t);
                        y1 = (-Math.Cos(t) * x1 + r) / Math.Sin(t);
                    }
                    else
                    {
                        // vertical line
                        x0 = line.Radius;
                        x1 = line.Radius;

                        y0 = h2;
                        y1 = -h2;
                    }


                    graph.DrawLine(_pen,
                        (int)(x0 + w2), (int)(h2 - y0),
                        (int)(x1 + w2), (int)(h2 - y1));

                }
            }

            return newData;


        }
        #endregion
    }
}
