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
    /// structure is used by the capVideoStreamCallback function
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VideoHdr
    {
        #region Properties
        /// <summary>
        /// Pointer to locked data buffer
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public int lpData;
        /// <summary>
        /// Length of data buffer
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public int dwBufferLength;
        /// <summary>
        /// Bytes actually used
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public int dwBytesUsed;
        /// <summary>
        /// Milliseconds from start of stream
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public int dwTimeCaptured;
        /// <summary>
        /// User-defined data
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public int dwUser;
        /// <summary>
        /// The flags are defined as follows. Flag Meaning 
        /// VHDR_DONE Done bit 
        /// VHDR_PREPARED Set if this header has been prepared 
        /// VHDR_INQUEUE Reserved for driver 
        /// VHDR_KEYFRAME Key Frame  
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public int dwFlags;
        /// <summary>
        /// Reserved for driver 
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public int[] dwReserved;
        #endregion
    }
}
