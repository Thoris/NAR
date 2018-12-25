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

namespace NAR.ImageProcessing.Pixels
{
    public class CountPixelCommand : PixelCounter, ICommand
    {
        #region Variables
        private int _x1;
        private int _x2;
        private int _y1;
        private int _y2;
        private bool _fill;
        private double _result;
        #endregion

        #region Properties
        public int X1
        {
            get { return _x1; }
            set { _x1 = value; }
        }
        public int X2
        {
            get { return _x2; }
            set { _x2 = value; }
        }
        public int Y1
        {
            get { return _y1; }
            set { _y1 = value; }
        }
        public int Y2
        {
            get { return _y2; }
            set { _y2 = value; }
        }
        public bool Fill
        {
            get { return _fill; }
            set { _fill = value; }
        }
        public double Result
        {
            get { return _result; }
        }
        #endregion

        #region Constructors/Destructors
        public CountPixelCommand(int x1, int x2, int y1, int y2, bool fill)
        {
            _x1 = x1;
            _x2 = x2;
            _y1 = y1;
            _y2 = y2;
            _fill = fill;

        }
        #endregion

        #region ICommand Members
        public new Model.IImage Execute(Model.IImage image)
        {
            Model.IImage newImage = null;

            _result = base.Count(image, _x1, _x2, _y1, _y2, _fill, out newImage);

            return newImage;
        }
        #endregion
    }
}
