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
using System.Text;
using System.Runtime.InteropServices;

namespace NAR.Capture.Drivers.WebCamWinAPI.API
{

    /// <summary>
    /// structure contains information about the dimensions and color format of a device-independent bitmap (DIB). 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct BitmapInfoHeader
    {
        #region Properties
        /// <summary>
        /// Specifies the number of bytes required by the structure
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public Int32 biSize;
        /// <summary>
        /// Specifies the width of the bitmap. 
        /// For RGB formats, the width is specified in pixels.  
        ///   The same is true for YUV formats if the bitdepth is an even power of 2. 
        ///   For YUV formats where the bitdepth is not an even power of 2, however, the width is specified in bytes. 
        ///   Decoders and video sources should propose formats where biWidth is the width of the image. 
        ///   If the video renderer is using DirectDraw, it modifies the format so that biWidth equals the stride of the surface, 
        ///   and the rcTarget member of the VIDEOINFOHEADER or VIDEOINFOHEADER2 structure specifies the image width. 
        ///   Then it proposes the modified format using IPin::QueryAccept. For RGB and even-power-of-2 YUV formats, if the video 
        ///   renderer does not specify the stride, then round the width up to the nearst DWORD boundardy to find the strid
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public Int32 biWidth;
        /// <summary>
        /// Specifies the height of the bitmap, in pixels
        /// For RGB bitmaps, if biHeight is positive, the bitmap is a bottom-up DIB with the origin at the lower left corner. 
        /// If biHeight is negative, the bitmap is a top-down DIB with the origin at the upper left corner. 
        /// For YUV bitmaps, the bitmap is always top-down, regardless of the sign of biHeight.
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public Int32 biHeight;
        /// <summary>
        /// Specifies the number of planes for the target device. This value must be set to 1
        /// </summary>
        [MarshalAs(UnmanagedType.I2)]
        public short biPlanes;
        /// <summary>
        /// Specifies the number of bits per pixel
        /// </summary>
        [MarshalAs(UnmanagedType.I2)]
        public short biBitCount;
        /// <summary>
        /// If the bitmap is compressed, this member is a FOURCC the specifies the compression. For uncompressed formats, ]
        /// the following values are possible:
        /// Value Description:
        /// 	BI_RGB - Uncompressed RGB. 
        /// 	BI_BITFIELDS - Uncompressed RGB with color masks. Valid for 16-bpp and 32-bpp bitmaps 
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public Int32 biCompression;
        /// <summary>
        /// Specifies the size, in bytes, of the image. This can be set to 0 for uncompressed RGB bitmaps
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public Int32 biSizeImage;
        /// <summary>
        /// Specifies the horizontal resolution, in pixels per meter, of the target device for the bitmap
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public Int32 biXPelsPerMeter;
        /// <summary>
        /// Specifies the vertical resolution, in pixels per meter, of the target device for the bitmap
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public Int32 biYPelsPerMeter;
        /// <summary>
        /// Specifies the number of color indices in the color table that are actually used by the bitmap
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public Int32 biClrUsed;
        /// <summary>
        /// Specifies the number of color indices that are considered important for displaying the bitmap. If this value 
        /// is zero, all colors are important
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public Int32 biClrImportant;

        //
        // TODO: Check it
        // ????????????????????????????????????????????????
        //
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 256)]
        public uint[] cols;
        #endregion
    }
}
