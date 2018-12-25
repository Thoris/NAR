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
    public abstract class WinMessage
    {
        #region Constants

        public const int User = 1024;

        #endregion

        public abstract class Cap
        {
            #region Constants

            /// <summary>
            /// The WM_CAP_DRIVER_CONNECT message connects a capture window to a capture driver. You can send this message explicitly or by using the capDriverConnect macro.
            /// </summary>
            public const int Connect = 1034;
            /// <summary>
            /// The WM_CAP_DRIVER_DISCONNECT message disconnects a capture driver from a capture window. You can send this message explicitly or by using the capDriverDisconnect macro.
            /// </summary>
            public const int Disconnect = 1035;
            /// <summary>
            /// The WM_CAP_GRAB_FRAME message retrieves and displays a single frame from the capture driver. After capture, overlay and preview are disabled. You can send this message explicitly or by using the capGrabFrame macro.
            /// </summary>
            public const int GetFrame= 1084;
            /// <summary>
            /// The WM_CAP_EDIT_COPY message copies the contents of the video frame buffer and associated palette to the clipboard. You can send this message explicitly or by using the capEditCopy macro.
            /// </summary>
            public const int Copy = 1054;

            public const int Start = WinMessage.User;
            public const int UnicodeStart = WinMessage.User + 100;

            public const int GetCapStreamPtr = WinMessage.Cap.Start + 1;

            /// <summary>
            /// The WM_CAP_GET_USER_DATA message retrieves a LONG_PTR data value associated with a capture window. You can send this message explicitly or by using the capGetUserData macro.
            /// </summary>
            public const int GetUserData = Cap.Start + 8;
            /// <summary>
            /// The WM_CAP_SET_USER_DATA message associates a LONG_PTR data value with a capture window. You can send this message explicitly or by using the capSetUserData macro.
            /// </summary>
            public const int SetUserData = Cap.Start + 9;

            /// <summary>
            /// The WM_CAP_EDIT_COPY message copies the contents of the video frame buffer and associated palette to the clipboard. You can send this message explicitly or by using the capEditCopy macro.
            /// </summary>
            public const int EditCopy = Cap.Start + 30;


            /// <summary>
            /// The WM_CAP_SET_PREVIEW message enables or disables preview mode. In preview mode, frames are transferred from the capture hardware to system memory and then displayed in the capture window using GDI functions. You can send this message explicitly or by using the capPreview macro.
            /// </summary>
            public const int SetPreview = Cap.Start + 50;
            /// <summary>
            /// The WM_CAP_SET_OVERLAY message enables or disables overlay mode. In overlay mode, video is displayed using hardware overlay. You can send this message explicitly or by using the capOverlay macro.
            /// </summary>
            public const int SetOverlay = Cap.Start + 51;
            /// <summary>
            /// The WM_CAP_SET_PREVIEWRATE message sets the frame display rate in preview mode. You can send this message explicitly or by using the capPreviewRate macro.
            /// </summary>
            public const int SetPreviewRate = Cap.Start + 52;
            /// <summary>
            /// The WM_CAP_SET_SCALE message enables or disables scaling of the preview video images. If scaling is enabled, the captured video frame is stretched to the dimensions of the capture window. You can send this message explicitly or by using the capPreviewScale macro.
            /// </summary>
            public const int SetScale = Cap.Start + 53;
            /// <summary>
            /// The WM_CAP_GET_STATUS message retrieves the status of the capture window. You can send this message explicitly or by using the capGetStatus macro.
            /// </summary>
            public const int GetStatus = Cap.Start + 54;
            /// <summary>
            /// The WM_CAP_SET_SCROLL message defines the portion of the video frame to display in the capture window. This message sets the upper left corner of the client area of the capture window to the coordinates of a specified pixel within the video frame. You can send this message explicitly or by using the capSetScrollPos macro.
            /// </summary>
            public const int SetScroll = Cap.Start + 55;
            
            
            /// <summary>
            /// The WM_CAP_GRAB_FRAME message retrieves and displays a single frame from the capture driver. After capture, overlay and preview are disabled. You can send this message explicitly or by using the capGrabFrame macro.
            /// </summary>
			public const int GrabFrame = Cap.Start +  60;
            /// <summary>
            /// The WM_CAP_GRAB_FRAME_NOSTOP message fills the frame buffer with a single uncompressed frame from the capture device and displays it. Unlike with the WM_CAP_GRAB_FRAME message, the state of overlay or preview is not altered by this message. You can send this message explicitly or by using the capGrabFrameNoStop macro.
            /// </summary>
			public const int GrabFrameNoStop = Cap.Start +  61;


            /// <summary>
            /// The WM_CAP_STOP message stops the capture operation. You can send this message explicitly or by using the capCaptureStop macro.
            /// In step frame capture, the image data that was collected before this message was sent is retained in the capture file. An equivalent duration of audio data is also retained in the capture file if audio capture was enabled.
            /// </summary>
            public const int Stop = Cap.Start + 68;
            /// <summary>
            /// The WM_CAP_ABORT message stops the capture operation. In the case of step capture, the image data collected up to the point of the WM_CAP_ABORT message will be retained in the capture file, but audio will not be captured. You can send this message explicitly or by using the capCaptureAbort macro.
            /// </summary>
            public const int Abort = Cap.Start + 69;

            /// <summary>
            /// The WM_CAP_SINGLE_FRAME_OPEN message opens the capture file for single-frame capturing. Any previous information in the capture file is overwritten. You can send this message explicitly or by using the capCaptureSingleFrameOpen macro.
            /// </summary>
            public const int SingleFrameOpen = Cap.Start + 70;
            /// <summary>
            /// The WM_CAP_SINGLE_FRAME_CLOSE message closes the capture file opened by the WM_CAP_SINGLE_FRAME_OPEN message. You can send this message explicitly or by using the capCaptureSingleFrameClose macro.
            /// </summary>
            public const int SingleFrameClose = Cap.Start + 71;
            /// <summary>
            /// The WM_CAP_SINGLE_FRAME message appends a single frame to a capture file that was opened using the WM_CAP_SINGLE_FRAME_OPEN message. You can send this message explicitly or by using the capCaptureSingleFrame macro.
            /// </summary>
            public const int SingleFrame = Cap.Start + 72;



            // Defines end of the message range
            public const int UnicodeEnd = Pal.Save;
            public const int End = UnicodeEnd;

            #endregion

            public abstract class CallBack
            {
                #region Constants

                //public const int SetErrorW = Cap.UnicodeStart + 2;
                //public const int SetStatusW = Cap.UnicodeStart + 3;
                //public const int SetErrorA = Cap.Start + 2;
                //public const int SetStatusA= Cap.Start + 3;

                /// <summary>
                /// The WM_CAP_SET_CALLBACK_ERROR message sets an error callback function in the client application. AVICap calls this procedure when errors occur. You can send this message explicitly or by using the capSetCallbackOnError macro.
                /// </summary>
                public const int SetError = Cap.Start + 2;
                /// <summary>
                /// The WM_CAP_SET_CALLBACK_STATUS message sets a status callback function in the application. AVICap calls this procedure whenever the capture window status changes. You can send this message explicitly or by using the capSetCallbackOnStatus macro.
                /// </summary>
                public const int SetStatus = Cap.Start + 3;

                /// <summary>
                /// The WM_CAP_SET_CALLBACK_YIELD message sets a callback function in the application. AVICap calls this procedure when the capture window yields during streaming capture. You can send this message explicitly or by using the capSetCallbackOnYield macro.
                /// </summary>
                public const int SetYield = Cap.Start + 4;
                /// <summary>
                /// The WM_CAP_SET_CALLBACK_FRAME message sets a preview callback function in the application. AVICap calls this procedure when the capture window captures preview frames. You can send this message explicitly or by using the capSetCallbackOnFrame macro.
                /// </summary>
                public const int SetFrame = Cap.Start + 5;
                /// <summary>
                /// The WM_CAP_SET_CALLBACK_VIDEOSTREAM message sets a callback function in the application. AVICap calls this procedure during streaming capture when a video buffer is filled. You can send this message explicitly or by using the capSetCallbackOnVideoStream macro.
                /// </summary>
                public const int SetVideoStream = Cap.Start + 6;
                /// <summary>
                /// The WM_CAP_SET_CALLBACK_WAVESTREAM message sets a callback function in the application. AVICap calls this procedure during streaming capture when a new audio buffer becomes available. You can send this message explicitly or by using the capSetCallbackOnWaveStream macro.
                /// </summary>
                public const int SetWaveStream = Cap.Start + 7;


                // Following added post VFW 1.1
                /// <summary>
                /// The WM_CAP_SET_CALLBACK_CAPCONTROL message sets a callback function in the application giving it precise recording control. You can send this message explicitly or by using the capSetCallbackOnCapControl macro.
                /// </summary>
                public const int SetCapControl = Cap.Start + 85;

                #endregion

            }//end class CallBack

            public abstract class Driver
            {
                #region Constants

                public const int Connect = Cap.Start + 10;
                public const int Disconnect = Cap.Start + 11;

                //public const int GetNameA = Cap.Start + 12;
                //public const int GetVersionA = Cap.Start + 13;
                public const int GetNameW = Cap.UnicodeStart + 12;
                public const int GetVersionW = Cap.UnicodeStart + 13;

                //Not Unicode
                /// <summary>
                /// The WM_CAP_DRIVER_GET_NAME message returns the name of the capture driver connected to the capture window. You can send this message explicitly or by using the capDriverGetName macro.
                /// </summary>
                public const int GetName = Cap.Start + 12;
                /// <summary>
                /// The WM_CAP_DRIVER_GET_VERSION message returns the version information of the capture driver connected to a capture window. You can send this message explicitly or by using the capDriverGetVersion macro.
                /// </summary>
                public const int GetVersion = Cap.Start + 13;

                /// <summary>
                /// The WM_CAP_DRIVER_GET_CAPS message returns the hardware capabilities of the capture driver currently connected to a capture window. You can send this message explicitly or by using the capDriverGetCaps macro.
                /// </summary>
                public const int GetCaps = Cap.Start + 14;

                #endregion

            }//end class Driver

            public abstract class File
            {
                #region Constants

                //public const int SetCaptureFileA = Cap.Start + 20;
                //public const int GetCaptureFileA = Cap.Start + 21;
                //public const int SaveAsA = Cap.Start + 23;
                //public const int SaveDiba = Cap.Start + 25;
                //public const int SetCaptureFileW = Cap.UnicodeStart + 20;
                //public const int GetCaptureFileW = Cap.UnicodeStart + 21;
                //public const int SaveAsW = Cap.UnicodeStart + 23;
                //public const int SaveDibW = Cap.UnicodeStart + 25;

                //Not Unicode
                /// <summary>
                /// The WM_CAP_FILE_SET_CAPTURE_FILE message names the file used for video capture. You can send this message explicitly or by using the capFileSetCaptureFile macro.
                /// </summary>
                public const int SetCaptureFile = Cap.Start + 20;
                /// <summary>
                /// The WM_CAP_FILE_GET_CAPTURE_FILE message returns the name of the current capture file. You can send this message explicitly or by using the capFileGetCaptureFile macro.
                /// </summary>
                public const int GetCaptureFile = Cap.Start + 21;
                /// <summary>
                /// The WM_CAP_FILE_SAVEAS message copies the contents of the capture file to another file. You can send this message explicitly or by using the capFileSaveAs macro.
                /// </summary>
                public const int SaveAs = Cap.Start + 23;
                /// <summary>
                /// The WM_CAP_FILE_SAVEDIB message copies the current frame to a DIB file. You can send this message explicitly or by using the capFileSaveDIB macro.
                /// </summary>
                public const int SaveDib = Cap.Start + 25;

                // out of order to save on ifdefs
                /// <summary>
                /// The WM_CAP_FILE_ALLOCATE message creates (preallocates) a capture file of a specified size. You can send this message explicitly or by using the capFileAlloc macro.
                /// </summary>
                public const int Allocate = Cap.Start + 22;
                /// <summary>
                /// The WM_CAP_FILE_SET_INFOCHUNK message sets and clears information chunks. Information chunks can be inserted in an AVI file during capture to embed text strings or custom data. You can send this message explicitly or by using the capFileSetInfoChunk macro.
                /// </summary>
                public const int SetInfoChunk = Cap.Start + 24;

                #endregion

            }//end class File

            public abstract class Audio
            {
                #region Constants

                /// <summary>
                /// The WM_CAP_SET_AUDIOFORMAT message sets the audio format to use when performing streaming or step capture. You can send this message explicitly or by using the capSetAudioFormat macro.
                /// </summary>
                public const int SetFormat  = Cap.Start + 35;
                /// <summary>
                /// The WM_CAP_GET_AUDIOFORMAT message obtains the audio format or the size of the audio format. You can send this message explicitly or by using the capGetAudioFormat and capGetAudioFormatSize macros.
                /// </summary>
                public const int GetFormat = Cap.Start + 36;

                #endregion

            }//end class Audio

            public abstract class Video
            {
                #region Constants

                /// <summary>
                /// The WM_CAP_DLG_VIDEOFORMAT message displays a dialog box in which the user can select the video format. The Video Format dialog box might be used to select image dimensions, bit depth, and hardware compression options. You can send this message explicitly or by using the capDlgVideoFormat macro.
                /// </summary>
                public const int DlgFormat = Cap.Start + 41;
                /// <summary>
                /// The WM_CAP_DLG_VIDEOSOURCE message displays a dialog box in which the user can control the video source. The Video Source dialog box might contain controls that select input sources; alter the hue, contrast, brightness of the image; and modify the video quality before digitizing the images into the frame buffer. You can send this message explicitly or by using the capDlgVideoSource macro.
                /// </summary>
                public const int DlgSource = Cap.Start + 42;
                /// <summary>
                /// The WM_CAP_DLG_VIDEODISPLAY message displays a dialog box in which the user can set or adjust the video output. This dialog box might contain controls that affect the hue, contrast, and brightness of the displayed image, as well as key color alignment. You can send this message explicitly or by using the capDlgVideoDisplay macro.
                /// </summary>
                public const int DlgDisplay = Cap.Start + 43;
                /// <summary>
                /// The WM_CAP_GET_VIDEOFORMAT message retrieves a copy of the video format in use or the size required for the video format. You can send this message explicitly or by using the capGetVideoFormat and capGetVideoFormatSize macros.
                /// </summary>
                public const int GetFormat = Cap.Start + 44;
                /// <summary>
                /// The WM_CAP_SET_VIDEOFORMAT message sets the format of captured video data. You can send this message explicitly or by using the capSetVideoFormat macro.
                /// </summary>
                public const int SetFormat = Cap.Start + 45;
                /// <summary>
                /// The WM_CAP_DLG_VIDEOCOMPRESSION message displays a dialog box in which the user can select a compressor to use during the capture process. The list of available compressors can vary with the video format selected in the capture driver's Video Format dialog box. You can send this message explicitly or by using the capDlgVideoCompression macro.
                /// </summary>
                public const int DlgCompression = Cap.Start + 46;

                #endregion

            }//end class Video

            public abstract class Sequence
            {
                #region Constants

                /// <summary>
                /// The WM_CAP_SEQUENCE message initiates streaming video and audio capture to a file. You can send this message explicitly or by using the capCaptureSequence macro.
                /// </summary>
                public const int Value = Cap.Start + 62;
                /// <summary>
                /// The WM_CAP_SEQUENCE_NOFILE message initiates streaming video capture without writing data to a file. You can send this message explicitly or by using the capCaptureSequenceNoFile macro.
                /// </summary>
                public const int SequenceNoFile = Cap.Start + 63;
                /// <summary>
                /// The WM_CAP_SET_SEQUENCE_SETUP message sets the configuration parameters used with streaming capture. You can send this message explicitly or by using the capCaptureSetSetup macro.
                /// </summary>
                public const int SetSetup = Cap.Start + 64;
                /// <summary>
                /// The WM_CAP_GET_SEQUENCE_SETUP message retrieves the current settings of the streaming capture parameters. You can send this message explicitly or by using the capCaptureGetSetup macro.
                /// </summary>
                public const int GetSetup = Cap.Start + 65;

                #endregion

            }//end class Sequence

            public abstract class Mci
            {
                #region Constants


                //public const int SetDeviceA = Cap.Start + 66;
                //public const int GetDeviceA = Cap.Start + 67;
                //public const int SetDeviceW = Cap.UnicodeStart + 66;
                //public const int SetDeviewW = Cap.UnicodeStart + 67;

                //Not Unicode
                /// <summary>
                /// The WM_CAP_SET_MCI_DEVICE message specifies the name of the MCI video device to be used to capture data. You can send this message explicitly or by using the capSetMCIDeviceName macro.
                /// </summary>
                public const int SetDevice = Cap.Start + 66;
                /// <summary>
                /// The WM_CAP_GET_MCI_DEVICE message retrieves the name of an MCI device previously set with the WM_CAP_SET_MCI_DEVICE message. You can send this message explicitly or by using the capGetMCIDeviceName macro.
                /// </summary>
                public const int GetDevice = Cap.Start + 67;

                #endregion
            
            }//end class Mci

            public abstract class Pal
            {
                #region Constants


                //public const int OpenA = Cap.Start + 80;
                //public const int SaveA = Cap.Start + 81;
                //public const int OpenW = Cap.UnicodeStart+ 80;
                //public const int SaveW= Cap.UnicodeStart + 81;

                /// <summary>
                /// The WM_CAP_PAL_OPEN message loads a new palette from a palette file and passes it to a capture driver. Palette files typically use the filename extension .PAL. A capture driver uses a palette when required by the specified digitized image format. You can send this message explicitly or by using the capPaletteOpen macro.
                /// </summary>
                public const int Open = Cap.Start + 80;
                /// <summary>
                /// The WM_CAP_PAL_SAVE message saves the current palette to a palette file. Palette files typically use the filename extension .PAL. You can send this message explicitly or by using the capPaletteSave macro.
                /// </summary>
                public const int Save = Cap.Start + 81;

                /// <summary>
                /// The WM_CAP_PAL_PASTE message copies the palette from the clipboard and passes it to a capture driver. You can send this message explicitly or by using the capPalettePaste macro.
                /// </summary>
                public const int Paste = Cap.Start + 82;
                /// <summary>
                /// The WM_CAP_PAL_AUTOCREATE message requests that the capture driver sample video frames and automatically create a new palette. You can send this message explicitly or by using the capPaletteAuto macro.
                /// </summary>
                public const int AutoCreate = Cap.Start + 83;
                /// <summary>
                /// The WM_CAP_PAL_MANUALCREATE message requests that the capture driver manually sample video frames and create a new palette. You can send this message explicitly or by using the capPaletteManual macro.
                /// </summary>
                public const int ManualCreate = Cap.Start + 84;

                #endregion

            }//end class Pal

        }//end class Cap
    }//end class WinMessage
}
