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
    /// The CAPTUREPARMS structure contains parameters that control the streaming video capture process. 
    /// This structure is used to get and set parameters that affect the capture rate, the number of buffers 
    /// to use while capturing, and how capture is terminated.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CaptureParms
    {
        #region Properties
        /// <summary>
        /// Requested frame rate, in microseconds. The default value is 66667, which corresponds to 15 frames per second.
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public Int32 dwRequestMicroSecPerFrame;
        /// <summary>
        /// User-initiated capture flag. If this member is TRUE, AVICap displays a dialog box prompting the user to 
        /// initiate capture. The default value is FALSE. 
        /// </summary>
        [MarshalAs(UnmanagedType.Bool)]
        public Boolean fMakeUserHitOKToCapture;
        /// <summary>
        /// Maximum allowable percentage of dropped frames during capture. Values range from 0 to 100. The default value is 10.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 wPercentDropForError;
        /// <summary>
        /// Yield flag. If this member is TRUE, the capture window spawns a separate background thread to perform step 
        /// and streaming capture. The default value is FALSE. 	Applications that set this flag must handle potential 
        /// reentry issues because the controls in the application are not disabled while capture is in progress.
        /// </summary>
        [MarshalAs(UnmanagedType.Bool)]
        public Boolean fYield;
        /// <summary>
        /// Maximum number of index entries in an AVI file. Values range from 1800 to 324,000. If set to 0, a default 
        /// value of 34,952 (32K frames plus a proportional number of audio buffers) is used. Each video frame or buffer 
        /// of waveform-audio data uses one index entry. The value of this entry establishes a limit for the number of 
        /// frames or audio buffers that can be captured. 
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public Int32 dwIndexSize;
        /// <summary>
        /// Logical block size, in bytes, of an AVI file. The value 0 indicates the current sector size is used as the granularity
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 wChunkGranularity;
        /// <summary>
        /// Not used in Win32 applications
        /// </summary>
        [MarshalAs(UnmanagedType.Bool)]
        public Boolean fUsingDOSMemory;
        /// <summary>
        /// Maximum number of video buffers to allocate. The memory area to place the buffers is specified with fUsingDOSMemory. 
        /// The actual number of buffers allocated might be lower if memory is unavailable
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 wNumVideoRequested;
        /// <summary>
        /// Capture audio flag. If this member is TRUE, audio is captured during streaming capture. This is the default value 
        /// if audio hardware is installed
        /// </summary>
        [MarshalAs(UnmanagedType.Bool)]
        public Boolean fCaptureAudio;
        /// <summary>
        /// Maximum number of audio buffers to allocate. The maximum number of buffers is 10
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 wNumAudioRequested;
        /// <summary>
        /// Virtual keycode used to terminate streaming capture. The default value is VK_ESCAPE. You must call the RegisterHotKey
        ///  function before specifying a keystroke that can abort a capture session. You can combine keycodes that include 
        ///  CTRL and SHIFT keystrokes by using the logical OR operator with the keycodes for CTRL (0x8000) and SHIFT (0x4000). 
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 vKeyAbort;
        /// <summary>
        /// Abort flag for left mouse button. If this member is TRUE, streaming capture stops if the left mouse button is pressed. 
        /// The default value is TRUE
        /// </summary>
        [MarshalAs(UnmanagedType.Bool)]
        public Boolean fAbortLeftMouse;
        /// <summary>
        /// Abort flag for right mouse button. If this member is TRUE, streaming capture stops if the right mouse button is pressed. 
        /// The default value is TRUE
        /// </summary>
        [MarshalAs(UnmanagedType.Bool)]
        public Boolean fAbortRightMouse;
        /// <summary>
        /// Time limit enabled flag. If this member is TRUE, streaming capture stops after the number of seconds in wTimeLimit has 
        /// elapsed. The default value is FALSE
        /// </summary>
        [MarshalAs(UnmanagedType.Bool)]
        public Boolean fLimitEnabled;
        /// <summary>
        /// Time limit for capture, in seconds. This parameter is used only if fLimitEnabled is TRUE
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 wTimeLimit;
        /// <summary>
        /// MCI device capture flag. If this member is TRUE, AVICap controls an	MCI-compatible video source during streaming capture. 
        /// MCI-compatible video sources include VCRs and laserdiscs. 
        /// </summary>
        [MarshalAs(UnmanagedType.Bool)]
        public Boolean fMCIControl;
        /// <summary>
        /// MCI device step capture flag. If this member is TRUE, step capture using an MCI device as a video source is enabled. 
        /// If it is FALSE, real-time capture using an MCI device is enabled. (If fMCIControl is FALSE, this member is ignored.) 
        /// </summary>
        [MarshalAs(UnmanagedType.Bool)]
        public Boolean fStepMCIDevice;
        /// <summary>
        /// Starting position, in milliseconds, of the MCI device for the capture sequence. (If fMCIControl is FALSE, this 
        /// member is ignored.) 
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public Int32 dwMCIStartTime;
        /// <summary>
        /// Stopping position, in milliseconds, of the MCI device for the capture sequence. When this position in the content 
        /// is reached, capture ends and the MCI device stops. (If fMCIControl is FALSE, this member is ignored.) 
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public Int32 dwMCIStopTime;
        /// <summary>
        /// Double-resolution step capture flag. If this member is TRUE, the capture hardware captures at twice the specified
        ///  resolution. (The resolution for the height and width is doubled.) 	Enable this option if the hardware does not 
        ///  support hardware-based decimation and you are capturing in the RGB format.
        /// </summary>
        [MarshalAs(UnmanagedType.Bool)]
        public Boolean fStepCaptureAt2x;
        /// <summary>
        /// Number of times a frame is sampled when creating a frame based on the average sample. A typical value for the number
        ///  of averages is 5.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 wStepCaptureAverageFrames;
        /// <summary>
        /// Audio buffer size. If the default value of zero is used, the size of each buffer will be the maximum of 0.5 
        /// seconds of audio or 10K bytes
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public Int32 dwAudioBufferSize;
        /// <summary>
        /// Not used in Win32 applications
        /// </summary>
        [MarshalAs(UnmanagedType.Bool)]
        public Boolean fDisableWriteCache;
        /// <summary>
        /// Indicates whether the audio stream controls the clock when writing an AVI file. If this member is set to 
        /// AVSTREAMMASTER_AUDIO, the audio stream is considered the master stream and the video stream duration is forced 
        /// to match the audio duration. If this member is set to AVSTREAMMASTER_NONE, the durations of audio and video 
        /// streams can differ. 
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 AVStreamMaster;
        #endregion
    }
}
