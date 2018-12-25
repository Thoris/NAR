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
    /// Structure defines the dimensions and color information for a DIB
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct BitmapInfo
    {
        #region Properties
        /// <summary>
        /// Specifies a BITMAPINFOHEADER structure that contains information about the dimensions of color format
        /// </summary>
        [MarshalAs(UnmanagedType.Struct, SizeConst = 40)]
        public BitmapInfoHeader bmiHeader;
        /// <summary>
        /// The bmiColors member contains one of the following: 
        /// An array of RGBQUAD. The elements of the array that make up the color table. 
        /// An array of 16-bit unsigned integers that specifies indexes into the currently realized logical palette. 
        /// This use of bmiColors is allowed for functions that use DIBs. When bmiColors elements contain indexes to a 
        /// realized logical palette, they must also call the following bitmap functions: 
        /// CreateDIBitmap 
        /// CreateDIBPatternBrush 
        /// CreateDIBSection 
        /// The iUsage parameter of CreateDIBSection must be set to DIB_PAL_COLORS. 
        /// The number of entries in the array depends on the values of the biBitCount and biClrUsed members of the 
        /// BITMAPINFOHEADER structure
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
        public Int32[] bmiColors;
        #endregion
    }
}
