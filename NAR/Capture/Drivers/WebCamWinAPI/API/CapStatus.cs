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

namespace NAR.Capture.Drivers.WebCamWinAPI.API
{
    /// <summary>
    /// The CAPSTATUS structure defines the current state of the capture window
    /// </summary>
    public struct CapStatus
    {
        #region Properties
        /// <summary>
        /// Image width, in pixels
        /// </summary>
        public ushort uiImageWidth;
        /// <summary>
        /// Image height, in pixels 
        /// </summary>
        public ushort uiImageHeight;
        /// <summary>
        /// Live window flag. The value of this member is TRUE if the window is displaying video 
        /// using the preview method
        /// </summary>
        public bool fLiveWindow;
        /// <summary>
        /// Overlay window flag. The value of this member is TRUE if the window is displaying video 
        /// using hardware overlay
        /// </summary>
        public bool fOverlayWindow;
        /// <summary>
        /// Input scaling flag. The value of this member is TRUE if the window is scaling the input 
        /// video to the client area when displaying video using preview. This parameter has no effect 
        /// when displaying video using overlay
        /// </summary>
        public bool fScale;
        /// <summary>
        /// The x- and y-offset of the pixel displayed in the upper left corner of the client area of the window
        /// </summary>
        public Point ptScroll;
        /// <summary>
        /// Default palette flag. The value of this member is TRUE if the capture driver is using its default palette
        /// </summary>
        public bool fUsingDefaultPalette;
        /// <summary>
        /// Audio hardware flag. The value of this member is TRUE if the system has waveform-audio hardware installed
        /// </summary>
        public bool fAudioHardware;
        /// <summary>
        /// Capture file flag. The value of this member is TRUE if a valid capture file has been generated
        /// </summary>
        public bool fCapFileExists;
        /// <summary>
        /// Number of frames processed during the current (or most recent) streaming capture. This count 
        /// includes dropped frames
        /// </summary>
        public double dwCurrentVideoFrame;
        /// <summary>
        /// Number of frames dropped during the current (or most recent) streaming capture. Dropped frames occur 
        /// when the capture rate exceeds the rate at which frames can be saved to file. In this case, 
        /// the capture driver has no buffers available for storing data. Dropping frames does not affect 
        /// synchronization because the previous frame is displayed in place of the dropped frame
        /// </summary>
        public double dwCurrentVideoFramesDropped;
        /// <summary>
        /// Number of waveform-audio samples processed during the current (or most recent) streaming capture
        /// </summary>
        public double dwCurrentWaveSamples;
        /// <summary>
        /// Time, in milliseconds, since the start of the current (or most recent) streaming capture
        /// </summary>
        public double dwCurrentTimeElapsedMS;
        /// <summary>
        /// Handle to current palette
        /// </summary>
        //HPALETTE	hPalCurrent; 
        public IntPtr hPalCurrent;
        /// <summary>
        /// Capturing flag. The value of this member is TRUE when capturing is in progress
        /// </summary>
        public bool fCapturingNow;
        /// <summary>
        /// Error return values. Use this member if your application does not support an error callback function
        /// </summary>
        public double dwReturn;
        /// <summary>
        /// Number of video buffers allocated. This value might be less than the number specified in 
        /// the wNumVideoRequested member of the CAPTUREPARMS structure
        /// </summary>
        public ushort wNumVideoAllocated;
        /// <summary>
        /// Number of audio buffers allocated. This value might be less than the number specified in the 
        /// wNumAudioRequested member of the CAPTUREPARMS structure
        /// </summary>
        public ushort wNumAudioAllocated;
        #endregion
    }
}
