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

namespace NAR.Model
{
    public class ImageBitmap : IImage, ICloneable
    {
        #region Constants

        public const int COLOR_RED = 2;
        public const int COLOR_GREEN = 1;
        public const int COLOR_BLUE = 0;

        #endregion

        #region Variables

        private Bitmap _image;
        //private Capture.Management.ImageExtensions _extension;
        private byte[] _bytes;

        #endregion

        #region Properties

        public Bitmap Image
        {
            get { return _image; }
        }
        public byte[] Bytes
        {
            get { return _bytes; }
        }

        #endregion

        #region Constructors/Destructors

        public ImageBitmap(Bitmap image)
        {
            _image = image;
            _bytes = FillBytes(image);
        }
        public ImageBitmap(int width, int height, byte[] bytes)
        {
            _bytes = bytes;

            _image = FillImage(width, height, bytes);
        }
        public ImageBitmap(Bitmap image, byte[] bytes)
        {
            _bytes = bytes;
            _image = image;
        }

        #endregion

        #region Methods

        private byte [] FillBytes(Bitmap bitmap)
        {
            return Capture.Management.ImageExtensions.ConvertBitmapToByteArray(bitmap);            
        }
        private Bitmap FillImage(int width, int height, byte [] bytes)
        {
            return Capture.Management.ImageExtensions.ConvertByteArrayToBitmap(width, height, bytes);
        }

        #endregion


        #region ICloneable members

        public object Clone()
        {
            byte[] data = new byte[_bytes.Length];

            Array.Copy(_bytes, data, _bytes.Length);

            return new ImageBitmap(this.Image.Width, this.Image.Height, data); 

        }

        #endregion
    }
}
