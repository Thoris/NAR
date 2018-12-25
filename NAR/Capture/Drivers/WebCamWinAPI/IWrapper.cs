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

namespace NAR.Capture.Drivers.WebCamWinAPI
{
    public interface IWrapper
    {

        /// <summary>
        /// Connects a capture window to a capture driver
        /// </summary>
        /// <param name="driverNumber">Index of the capture driver. The index can range from 0 through 9.</param>
        /// <returns>Returns TRUE if successful or FALSE if the specified capture driver cannot be connected to the capture window.</returns>
        /// <remarks>Connecting a capture driver to a capture window automatically disconnects any previously connected capture driver</remarks>
        bool DriverConnect(int driverNumber);

        /// <summary>
        /// Initiates streaming video and audio capture to a file
        /// </summary>
        /// <returns>Returns TRUE if successful or FALSE otherwise.
        /// If an error occurs and an error callback function is set using the capSetCallbackOnError macro, 
        /// the error callback function is called.</returns>
        /// <remarks>If you want to alter the parameters controlling streaming capture, use the capCaptureSetSetup macro prior to 
        /// starting the capture. By default, the capture window does not allow other applications to continue running during capture. 
        /// To override this, either set the fYield member of the CAPTUREPARMS structure to TRUE, or install a yield callback function.
        /// During streaming capture, the capture window can optionally issue notifications to your application of specific types of 
        /// conditions. To install callback procedures for these notifications, use the following macros:
        /// 		capSetCallbackOnError
        /// 		capSetCallbackOnStatus
        /// 		capSetCallbackOnVideoStream
        /// 		capSetCallbackOnWaveStream
        /// 		capSetCallbackOnYield</remarks>
        bool CaptureSequence();
        /// <summary>
        /// Sets the frame display rate in preview mode
        /// </summary>
        /// <param name="rate">Rate, in milliseconds, at which new frames are captured and displayed.</param>
        /// <returns>Returns TRUE if successful or FALSE if the capture window is not connected to a capture driver.</returns>
        /// <remarks>The preview mode uses substantial CPU resources. Applications can disable preview or lower the 
        /// preview rate when another application has the focus. During streaming video capture, the previewing task is lower 
        /// priority than writing frames to disk, and preview frames are displayed only if no other buffers are available for writing</remarks>
        bool PreviewRate(int rate);
        /// <summary>
        /// enables or disables preview mode. In preview mode, frames are transferred from the capture hardware 
        /// to system memory and then displayed in the capture window using GDI functions.
        /// </summary>
        /// <param name="preview">Preview flag. Specify TRUE for this parameter to enable preview mode or FALSE to disable it.</param>
        /// <returns>Returns TRUE if successful or FALSE otherwise.</returns>
        /// <remarks>The preview mode uses substantial CPU resources. Applications can disable preview or lower the preview rate 
        /// when another application has the focus. The fLiveWindow member of the CAPSTATUS structure indicates if preview mode 
        /// is currently enabled. Enabling preview mode automatically disables overlay mode.</remarks>
        bool Preview(bool preview);
        /// <summary>
        /// Enables or disables overlay mode. In overlay mode, video is displayed using hardware overlay
        /// </summary>
        /// <param name="overlay">Overlay flag. Specify TRUE for this parameter to enable overlay mode or FALSE to disable it.</param>
        /// <returns>Returns TRUE if successful or FALSE otherwise</returns>
        /// <remarks>Using an overlay does not require CPU resources. 
        /// The fHasOverlay member of the CAPDRIVERCAPS structure indicates whether the device is capable of overlay. 
        /// The fOverlayWindow member of the CAPSTATUS structure indicates whether overlay mode is currently enabled. 
        /// Enabling overlay mode automatically disables preview mode. </remarks>
        bool Overlay(bool overlay);
        /// <summary>
        /// Enables or disables scaling of the preview video images. If scaling is enabled, the captured video frame is 
        /// stretched to the dimensions of the capture window
        /// </summary>
        /// <param name="scaling">Preview scaling flag. Specify TRUE for this parameter to stretch preview frames to the size of the 
        /// capture window or FALSE to display them at their natural size. </param>
        /// <returns>Returns TRUE if successful or FALSE otherwise</returns>
        /// <remarks>Scaling preview images controls the immediate presentation of captured frames within the capture window. 
        /// It has no effect on the size of the frames saved to file.Scaling has no effect when using overlay to display video 
        /// in the frame buffer.</remarks>
        bool PreviewScale(bool scaling);
        /// <summary>
        /// Copies the contents of the video frame buffer and associated palette to the clipboard
        /// </summary>
        /// <returns>Returns TRUE if successful or FALSE otherwise</returns>
        bool EditCopy();
        bool CopyToClipboard();

        /// <summary>
        /// Sets an error callback function in the client application. AVICap calls this procedure when errors occur
        /// </summary>
        /// <param name="proc">Pointer to the error callback function. Specify NULL for this parameter to disable a previously 
        /// installed error callback function</param>
        /// <returns>Returns TRUE if successful or FALSE if streaming capture or a single-frame capture session is in progress</returns>
        /// <remarks>Applications can optionally set an error callback function. If set, AVICap calls the error procedure in 
        /// the following situations: 
        /// 		The disk is full. 
        /// 		A capture window cannot be connected with a capture driver. 
        /// 		A waveform-audio device cannot be opened. 
        /// 		The number of frames dropped during capture exceeds the specified percentage. 
        /// 		The frames cannot be captured due to vertical synchronization interrupt problems</remarks>
        bool SetCallbackOnError(API.Function.ErrorCallback proc);
        /// <summary>
        /// Sets a status callback function in the application. AVICap calls this procedure whenever the capture window status changes
        /// </summary>
        /// <param name="proc">Pointer to the status callback function. Specify NULL for this parameter to disable a previously ]
        /// installed status callback function</param>
        /// <returns>Returns TRUE if successful or FALSE if streaming capture or a single-frame capture session is in progress</returns>
        /// <remarks>Applications can optionally set a status callback function. If set, AVICap calls this procedure in the following 
        /// situations: 
        /// 		A capture session is completed. 
        /// 		A capture driver successfully connected to a capture window. 
        /// 		An optimal palette is created. 
        /// 		The number of captured frames is reported</remarks>
        bool SetCallbackOnStatus(API.Function.StatusCallback proc);

        /// <summary>
        /// Sets a callback function in the application. AVICap calls this procedure when the capture window yields during streaming capture
        /// </summary>
        /// <param name="proc">Pointer to the yield callback function. Specify NULL for this parameter to disable a previously installed 
        /// yield callback function</param>
        /// <returns>Returns TRUE if successful or FALSE if streaming capture or a single-frame capture session is in progress</returns>
        /// <remarks>Applications can optionally set a yield callback function. The yield callback function is called at least 
        /// once for each video frame captured during streaming capture. If a yield callback function is installed, it will be 
        /// called regardless of the state of the fYield member of the CAPTUREPARMS structure.
        ///		If the yield callback function is used, it must be installed before starting the capture session and it must remain enabled 
        ///	for the duration of the session. It can be disabled after streaming capture ends.
        ///	Applications typically perform some type of message processing in the callback function consisting of a PeekMessage, 
        ///	TranslateMessage, DispatchMessage loop, as in the message loop of a WinMain function. The yield callback function must also 
        ///	filter and remove messages that can cause reentrancy problems.
        ///	 An application typically returns TRUE in the yield procedure to continue streaming capture. If a yield callback function 
        ///	 returns FALSE, the capture window stops the capture process</remarks>
        bool SetCallbackOnYield(API.Function.YieldCallback proc);
        /// <summary>
        /// Sets a preview callback function in the application. AVICap calls this procedure when the capture window captures preview frames
        /// </summary>
        /// <param name="proc">Pointer to the preview callback function. Specify NULL for this parameter to disable a previously installed 
        /// callback function</param>
        /// <returns>Returns TRUE if successful or FALSE if streaming capture or a single-frame capture session is in progress</returns>
        /// <remarks>The capture window calls the callback function before displaying preview frames. This allows an application to modify the frame if desired. 
        /// This callback function is not used during streaming video capture</remarks>
        bool SetCallbackOnFrame(API.Function.FrameCallback proc);
        /// <summary>
        /// Sets a callback function in the application. AVICap calls this procedure during streaming capture when a video buffer is filled
        /// </summary>
        /// <param name="proc">Pointer to the video-stream callback function. Specify NULL for this parameter to disable a previously 
        /// installed video-stream callback function</param>
        /// <returns>Returns TRUE if successful or FALSE if streaming capture or a single-frame capture session is in progress</returns>
        /// <remarks>The capture window calls the callback function before writing the captured frame to disk. This allows applications 
        /// to modify the frame if desired. If a video stream callback function is used for streaming capture, the procedure must 
        /// be installed before starting the capture session and it must remain enabled for the duration of the session. It can 
        /// be disabled after streaming capture ends </remarks>
        bool SetCallbackOnVideoStream(API.Function.VideoStreamCallback proc);
        /// <summary>
        /// Sets a callback function in the application. AVICap calls this procedure during streaming capture when a new audio
        ///  buffer becomes available
        /// </summary>
        /// <param name="proc">Pointer to the wave stream callback function. Specify NULL for this parameter to disable a previously 
        /// installed wave stream callback function</param>
        /// <returns>Returns TRUE if successful or FALSE if streaming capture or a single-frame capture session is in progress</returns>
        bool SetCallbackOnWaveStream(API.Function.WaveStreamCallback proc);
        /// <summary>
        /// Sets a callback function in the application giving it precise recording control
        /// </summary>
        /// <param name="proc">Pointer to the callback function. Specify NULL for this parameter to disable a previously installed 
        /// callback function</param>
        /// <returns>Returns TRUE if successful or FALSE if streaming capture or a single-frame capture session is in progress</returns>
        /// <remarks>The capture window calls the procedure before writing the audio buffer to disk. This allows applications 
        /// to modify the audio buffer if desired. If a wave stream callback function is used, it must be installed before starting 
        /// the capture session and it must remain enabled for the duration of the session. It can be disabled after streaming 
        /// capture ends</remarks>
        bool SetCallbackOnCapControl(API.Function.ControlCallback proc);




        /// <summary>
        /// Associates a LONG data value with a capture window
        /// </summary>
        /// <param name="user">Data value to associate with a capture window</param>
        /// <returns>Returns TRUE if successful or FALSE if streaming capture is in progress</returns>
        bool SetUserData(long user);
        /// <summary>
        /// Retrieves a LONG data value associated with a capture window
        /// </summary>
        void GetUserData();


        /// <summary>
        /// Disconnects a capture driver from a capture window
        /// </summary>
        /// <returns>Returns TRUE if successful or FALSE if the capture window is not connected to a capture driver</returns>
        bool DriverDisconnect();
        /// <summary>
        /// Returns the name of the capture driver connected to the capture window
        /// </summary>
        /// <param name="name">Pointer to an application-defined buffer used to return the device name as a null-terminated string</param>
        /// <param name="size">Size, in bytes, of the buffer referenced by szName</param>
        /// <returns>Returns TRUE if successful or FALSE if the capture window is not connected to a capture driver</returns>
        bool DriverGetName(ref string name, int size);


        /// <summary>
        /// Returns the version information of the capture driver connected to a capture window
        /// </summary>
        /// <param name="version">Pointer to an application-defined buffer used to return the version information as a null-terminated string</param>
        /// <param name="size">Size, in bytes, of the application-defined buffer referenced by szVer</param>
        /// <returns>Returns TRUE if successful or FALSE if the capture window is not connected to a capture driver</returns>
        bool DriverGetVersion(ref string version, int size);
        /// <summary>
        /// Returns the hardware capabilities of the capture driver currently connected to a capture window
        /// </summary>
        /// <param name="capDriverCaps">Pointer to the CAPDRIVERCAPS structure to contain the hardware capabilities</param>
        /// <param name="size">Size, in bytes, of the structure referenced by psCaps</param>
        /// <returns>Returns TRUE if successful or FALSE if the capture window is not connected to a capture driver</returns>
        bool DriverGetCaps(ref API.CapDriverCaps capDriverCaps, int size);

        /// <summary>
        /// Names the file used for video capture
        /// </summary>
        /// <param name="name">Pointer to the null-terminated string that contains the name of the capture file to use</param>
        /// <returns>Returns TRUE if successful or FALSE if the filename is invalid or if streaming or single-frame capture is in progress</returns>
        bool FileSetCaptureFile(string name);
        /// <summary>
        /// Returns the name of the current capture file
        /// </summary>
        /// <param name="name">Pointer to an application-defined buffer used to return the name of the capture file as a null-terminated string</param>
        /// <param name="size">Size, in bytes, of the application-defined buffer referenced by szName</param>
        /// <returns>Returns TRUE if successful or FALSE otherwise</returns>
        bool FileGetCaptureFile(ref string name, int size);
        /// <summary>
        /// Creates (preallocates) a capture file of a specified size
        /// </summary>
        /// <param name="size">Size, in bytes, to create the capture file</param>
        /// <returns>Returns TRUE if successful or FALSE otherwise.	
        /// If an error occurs and an error callback function is set using the capSetCallbackOnError macro, the error 
        /// callback function is called </returns>
        bool FileAlloc(uint size);
        /// <summary>
        /// Copies the contents of the capture file to another file
        /// </summary>
        /// <param name="name">Pointer to the null-terminated string that contains the name of the destination file used to copy the file</param>
        /// <returns></returns>
        bool FileSaveAs(string name);

        /*
        /// <summary>
        /// sets and clears information chunks. Information chunks can be inserted in an AVI file during capture 
        /// to embed text strings or custom data
        /// </summary>
        /// <param name="lpInfoChunk">Pointer to a CAPINFOCHUNK structure defining the information chunk to be created or deleted</param>
        /// <returns>Returns TRUE if successful or FALSE otherwise. 
        /// If an error occurs and an error callback function is set using the capSetCallbackOnError macro, the error callback 
        /// function is called</returns>
        bool capFileSetInfoChunk(object lpInfoChunk) ;
        */

        /// <summary>
        /// Copies the current frame to a DIB file
        /// </summary>
        /// <param name="name">Pointer to the null-terminated string that contains the name of the destination DIB file</param>
        /// <returns>Returns TRUE if successful or FALSE otherwise.
        ///		If an error occurs and an error callback function is set using the capSetCallbackOnError macro, 
        ///		the error callback function is called</returns>
        bool FileSaveDIB(string name);


        /// <summary>
        /// Sets the audio format to use when performing streaming or step capture
        /// </summary>
        /// <param name="waveFormatex">Pointer to a WAVEFORMATEX or PCMWAVEFORMAT structure that defines the audio format</param>
        /// <param name="size">Size, in bytes, of the structure referenced by s</param>
        /// <returns>Returns TRUE if successful or FALSE otherwise</returns>
        bool SetAudioFormat(API.WaveFormatex waveFormatex, int size);
        /// <summary>
        /// Obtains the audio format
        /// </summary>
        /// <param name="waveFormatex">Pointer to a WAVEFORMATEX structure, or NULL. If the value is NULL, the size, in bytes, required to hold the WAVEFORMATEX structure is returned</param>
        /// <param name="size">Size, in bytes, of the structure referenced by s</param>
        /// <returns>Returns the size, in bytes, of the audio format</returns>
        bool GetAudioFormat(ref API.WaveFormatex waveFormatex, int size);

        /// <summary>
        /// Obtains the size of the audio format
        /// </summary>
        /// <returns>Returns the size, in bytes, of the audio format</returns>
        bool GetAudioFormatSize();


        /// <summary>
        /// Displays a dialog box in which the user can select the video format. 
        /// The Video Format dialog box might be used to select image dimensions, bit depth, and hardware compression options
        /// </summary>
        /// <returns>Returns TRUE if successful or FALSE otherwise</returns>
        bool DlgVideoFormat();
        /// <summary>
        /// Displays a dialog box in which the user can control the video source. The Video Source dialog box might contain 
        /// controls that select input sources; alter the hue, contrast, brightness of the image; and modify the video 
        /// quality before digitizing the images into the frame buffer
        /// </summary>
        /// <returns>Returns TRUE if successful or FALSE otherwise</returns>
        bool DlgVideoSource();
        /// <summary>
        /// Displays a dialog box in which the user can set or adjust the video output. This dialog box might contain 
        /// controls that affect the hue, contrast, and brightness of the displayed image, as well as key color alignment
        /// </summary>
        /// <returns>Returns TRUE if successful or FALSE otherwise</returns>
        bool DlgVideoDisplay();
        /// <summary>
        /// Displays a dialog box in which the user can select a compressor to use during the capture process. 
        /// The list of available compressors can vary with the video format selected in the capture driver's Video Format dialog box
        /// </summary>
        /// <returns>Returns TRUE if successful or FALSE otherwise</returns>
        bool DlgVideoCompression();

        /// <summary>
        /// Retrieves a copy of the video format in use
        /// </summary>
        /// <param name="bitmapInfo">Pointer to a BITMAPINFO structure. You can also specify NULL to retrieve the number of bytes needed by BITMAPINFO</param>
        /// <param name="size">Size, in bytes, of the structure referenced by s</param>
        /// <returns>Returns the size, in bytes, of the video format or zero if the capture window is not connected to a capture driver. 
        /// For video formats that require a palette, the current palette is also returned</returns>
        bool GetVideoFormat(ref API.BitmapInfo bitmapInfo, int size);
        /// <summary>
        /// Retrieves the size required for the video format
        /// </summary>
        /// <returns>Returns the size, in bytes, of the video format or zero if the capture window is not connected to a capture driver. 
        /// For video formats that require a palette, the current palette is also returned</returns>
        bool GetVideoFormatSize();
        /// <summary>
        /// Sets the format of captured video data
        /// </summary>
        /// <param name="bitmapInfo">Pointer to a BITMAPINFO structure</param>
        /// <param name="size">Size, in bytes, of the structure referenced by s</param>
        /// <returns>Returns TRUE if successful or FALSE otherwise</returns>
        bool SetVideoFormat(API.BitmapInfo bitmapInfo, int size);


        /// <summary>
        /// Retrieves the status of the capture window
        /// </summary>
        /// <param name="capStatus">Pointer to a CAPSTATUS structure</param>
        /// <param name="size">Size, in bytes, of the structure referenced by s</param>
        /// <returns>Returns TRUE if successful or FALSE if the capture window is not connected to a capture driver</returns>
        bool GetStatus(ref API.CapStatus capStatus, int size);
        /// <summary>
        /// Defines the portion of the video frame to display in the capture window. This message sets the upper left corner
        ///  of the client area of the capture window to the coordinates of a specified pixel within the video frame
        /// </summary>
        /// <param name="address">Address to contain the desired scroll position</param>
        /// <returns>Returns TRUE if successful or FALSE otherwise</returns>
        bool SetScrollPos(IntPtr address);


        /// <summary>
        /// Retrieves and displays a single frame from the capture driver. After capture, overlay and preview are disabled
        /// </summary>
        /// <returns>Returns TRUE if successful or FALSE otherwise</returns>
        bool GrabFrame();
        /// <summary>
        /// Fills the frame buffer with a single uncompressed frame from the capture device and displays it. 
        /// Unlike with the capGrabFrame macro, the state of overlay or preview is not altered by this message
        /// </summary>
        /// <returns>Returns TRUE if successful or FALSE otherwise</returns>
        bool GrabFrameNoStop();

        /// <summary>
        /// Initiates streaming video capture without writing data to a file
        /// </summary>
        /// <returns>Returns TRUE if successful or FALSE otherwise</returns>
        bool CaptureSequenceNoFile();
        /// <summary>
        /// Stops the capture operation.
        /// In step frame capture, the image data that was collected before this message was sent is retained 
        /// in the capture file. An equivalent duration of audio data is also retained in the capture file if 
        /// audio capture was enabled.
        /// </summary>
        /// <returns>Returns TRUE if successful or FALSE otherwise</returns>
        bool CaptureStop();
        /// <summary>
        ///  Stops the capture operation
        /// </summary>
        /// <returns>Returns TRUE if successful or FALSE otherwise</returns>
        bool CaptureAbort();


        /// <summary>
        /// Opens the capture file for single-frame capturing. Any previous information in the capture file is overwritten
        /// </summary>
        /// <returns>Returns TRUE if successful or FALSE otherwise</returns>
        bool CaptureSingleFrameOpen();
        /// <summary>
        /// Closes the capture file opened by the capCaptureSingleFrameOpen macro
        /// </summary>
        /// <returns>Returns TRUE if successful or FALSE otherwise</returns>
        bool CaptureSingleFrameClose();
        /// <summary>
        /// Appends a single frame to a capture file that was opened using the capCaptureSingleFrameOpen macro
        /// </summary>
        /// <returns>Returns TRUE if successful or FALSE otherwise</returns>
        bool CaptureSingleFrame();

        /// <summary>
        /// Retrieves the current settings of the streaming capture parameters
        /// </summary>
        /// <param name="captureParms">Pointer to a CAPTUREPARMS structure</param>
        /// <param name="size">Size, in bytes, of the structure referenced by s</param>
        /// <returns>Returns TRUE if successful or FALSE otherwise</returns>
        bool CaptureGetSetup(ref API.CaptureParms captureParms, int size);
        /// <summary>
        /// Sets the configuration parameters used with streaming capture
        /// </summary>
        /// <param name="captureParms">Pointer to a CAPTUREPARMS structure</param>
        /// <param name="size">Size, in bytes, of the structure referenced by s</param>
        /// <returns>Returns TRUE if successful or FALSE otherwise</returns>
        bool CaptureSetSetup(API.CaptureParms captureParms, int size);


        /// <summary>
        /// Specifies the name of the MCI video device to be used to capture data
        /// </summary>
        /// <param name="name">Pointer to a null-terminated string containing the name of the device</param>
        /// <returns>Returns TRUE if successful or FALSE otherwise</returns>
        bool SetMCIDeviceName(string name);
        /// <summary>
        /// Retrieves the name of an MCI device previously set with the capSetMCIDeviceName macro
        /// </summary>
        /// <param name="name">Pointer to a null-terminated string that contains the MCI device name</param>
        /// <param name="size">Length, in bytes, of the buffer referenced by szName </param>
        /// <returns>Returns TRUE if successful or FALSE otherwise</returns>
        bool GetMCIDeviceName(ref string name, int size);


        /// <summary>
        /// Loads a new palette from a palette file and passes it to a capture driver. Palette files typically use the 
        /// filename extension .PAL. A capture driver uses a palette when required by the specified digitized image format
        /// </summary>
        /// <param name="name">Pointer to a null-terminated string containing the palette filename</param>
        /// <returns>Returns TRUE if successful or FALSE otherwise. 
        /// If an error occurs and an error callback function is set using the capSetCallbackOnError macro, 
        /// the error callback function is called </returns>
        bool PaletteOpen(string name);
        /// <summary>
        /// Saves the current palette to a palette file. Palette files typically use the filename extension .PAL
        /// </summary>
        /// <param name="name">Pointer to a null-terminated string containing the palette filename</param>
        /// <returns>Returns TRUE if successful or FALSE otherwise.	If an error occurs and an error callback function 
        /// is set using the capSetCallbackOnError macro, the error callback function is called</returns>
        bool PaletteSave(string name);

        /// <summary>
        /// Copies the palette from the clipboard and passes it to a capture driver
        /// </summary>
        /// <returns>Returns TRUE if successful or FALSE otherwise. If an error occurs and an error callback function 
        /// is set using the capSetCallbackOnError macro, the error callback function is called </returns>
        bool PalettePaste();
        /// <summary>
        /// Requests that the capture driver sample video frames and automatically create a new palette
        /// </summary>
        /// <param name="frames">Number of frames to sample</param>
        /// <param name="colors">Number of colors in the palette. The maximum value for this parameter is 256</param>
        /// <returns>Returns TRUE if successful or FALSE otherwise.	If an error occurs and an error callback function 
        /// is set using the capSetCallbackOnError macro, the error callback function is called</returns>
        bool PaletteAuto(int frames, int colors);
        /// <summary>
        /// Requests that the capture driver manually sample video frames and create a new palette
        /// </summary>
        /// <param name="paletteHistogram">Palette histogram flag. Set this parameter to TRUE for each frame included in creating the optimal palette. 
        /// After the last frame has been collected, set this parameter to FALSE to calculate the optimal palette and send it to the 
        /// capture driver</param>
        /// <param name="numColors">Number of colors in the palette. The maximum value for this parameter is 256. This value is used 
        /// only during collection of the first frame in a sequence</param>
        /// <returns>Returns TRUE if successful or FALSE otherwise.If an error occurs and an error callback function is 
        /// set using the capSetCallbackOnError macro, the error callback function is called</returns>
        bool PaletteManual(int paletteHistogram, int numColors);




        int OpenClipboard();
        int EmptyClipboard();
        int CloseClipboard();
        IntPtr GetClipboardData(uint uFormat);


        IList<API.Device> GetDevices();

    }
}
