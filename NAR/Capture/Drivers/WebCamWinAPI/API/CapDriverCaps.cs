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
    /// structure defines the capabilities of the capture driver
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CapDriverCaps
    {
        #region Properties
        /// <summary>
        /// Index of the capture driver. An index value can range from 0 to 9
        /// </summary>
        public UInt32 wDeviceIndex; // Driver index in system.ini
        /// <summary>
        /// Video-overlay flag. The value of this member is TRUE if the device supports video overlay
        /// </summary>
        public Int16 fHasOverlay; // Can device overlay?
        /// <summary>
        /// Video source dialog flag. The value of this member is TRUE if the device supports a dialog box for 
        /// selecting and controlling the video source
        /// </summary>
        public Int16 fHasDlgVideoSource; // Has Video source dlg?
        /// <summary>
        /// Video format dialog flag. The value of this member is TRUE if the device supports a dialog box for 
        /// selecting the video format
        /// </summary>
        public Int16 fHasDlgVideoFormat; // Has Format dlg?
        /// <summary>
        /// Video display dialog flag. The value of this member is TRUE if the device supports a dialog box for controlling 
        /// the redisplay of video from the capture frame buffer
        /// </summary>
        public Int16 fHasDlgVideoDisplay; // Has External out dlg?
        /// <summary>
        /// Capture initialization flag. The value of this member is TRUE if a capture device has been successfully connected
        /// </summary>
        public Int16 fCaptureInitialized; // Driver ready to capture?
        /// <summary>
        /// Driver palette flag. The value of this member is TRUE if the driver can create palettes
        /// </summary>
        public Int16 fDriverSuppliesPalettes; // Can driver make palettes?
        // following always NULL on Win32.
        /// <summary>
        /// Not used in Win32 applications
        /// </summary>
        public IntPtr hVideoIn; // Driver In channel
        /// <summary>
        /// Not used in Win32 applications
        /// </summary>
        public IntPtr hVideoOut; // Driver Out channel
        /// <summary>
        /// Not used in Win32 applications
        /// </summary>
        public IntPtr hVideoExtIn; // Driver Ext In channel
        /// <summary>
        /// Not used in Win32 applications
        /// </summary>
        public IntPtr hVideoExtOut; // Driver Ext Out channel
        #endregion
    }
}
