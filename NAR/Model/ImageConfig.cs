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
using System.Drawing.Imaging;

namespace NAR.Model
{
    public class ImageConfig
    {
        #region Variables
        private int _width;
        private int _height;
        private PixelFormat _format;
        private bool _grabByEvent;
        private int _frameRate;
        #endregion

        #region Properties
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }
        public PixelFormat Format
        {
            get { return _format; }
            set { _format = value; }
        }
        public bool GrabByEvent
        {
            get { return _grabByEvent; }
            set { _grabByEvent = value; }
        }
        public int FrameRate
        {
            get { return _frameRate; }
            set { _frameRate = value; }
        }
        #endregion

        #region Constructors/Destructors
        public ImageConfig()
        {
        }
        public ImageConfig(int width, int height, PixelFormat forma, bool grabByEvent)
            : this (width, height, grabByEvent, 30)
        {
        }
        public ImageConfig(int width, int height, PixelFormat format, bool grabByEvent, int frameRate)
        {
            _width = width;
            _height = height;
            _format = format;
            _grabByEvent = grabByEvent;
            _frameRate = frameRate;
        }
        public ImageConfig(int width, int height, bool grabByEvent, int frameRate)
            : this (width, height, PixelFormat.Format24bppRgb, grabByEvent, frameRate)
        {
        }

        
    
    
        #endregion
    }
}
