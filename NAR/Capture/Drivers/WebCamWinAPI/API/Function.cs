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
    //HELP
    //http://www.pinvoke.net/default.aspx/user32.sendmessage
    //
    public abstract class Function
    {
        #region Delegates/Events
        /// <summary>
        /// function is the error callback function used with video capture. The capErrorCallback error callback function is a 
        /// placeholder for the application-supplied function name
        /// </summary>
        /// <param name="hWnd">Handle to the capture window associated with the callback function</param>
        /// <param name="nID">Error identification number</param>
        /// <param name="lpsz">Pointer to a textual description of the returned error</param>
        public delegate void ErrorCallback(IntPtr hWnd, short nID, string lpsz);
        /// <summary>
        /// function is the callback function used for precision control to begin and end streaming capture. 
        /// The capControlCallback callback function is a placeholder for the application-supplied function name
        /// </summary>
        /// <param name="hWnd">Handle to the capture window associated with the callback function</param>
        /// <param name="nState">Current state of the capture operation. The CONTROLCALLBACK_PREROLL value is sent 
        /// initially to enable prerolling of the video sources and to return control to the capture application at the 
        /// exact moment recording is to begin. The CONTROLCALLBACK_CAPTURING value is sent once per captured frame to 
        /// indicate that streaming capture is in progress and to enable the application to end capture.</param>
        public delegate void ControlCallback(IntPtr hWnd, bool bState);
        ///<summary>
        /// function is the status callback function used with video capture. The capStatusCallback status callback function is 
        ///a placeholder for the application-supplied function name
        ///</summary>
        /// <param name="hWnd">Handle to the capture window associated with the callback function</param>
        /// <param name="nID">Message identification number</param>
        /// <param name="lpsz">Pointer to a textual description of the returned status</param>
        public delegate void StatusCallback(IntPtr hWnd, short nId, string lpsz);
        /// <summary>
        ///  function is the callback function used with streaming capture to optionally process a frame of captured video. 
        ///  It is registered by the capSetCallbackOnFrame macro. The capVideoStreamCallback callback function is 
        ///  a placeholder for the application-supplied function name
        /// </summary>
        /// <param name="hWnd">Handle to the capture window associated with the callback function</param>
        /// <param name="lpVhdr">Pointer to a VIDEOHDR structure containing information about the captured frame</param> 
        //public delegate void capVideoStreamCallback(IntPtr hWnd, VIDEOHDR lpVhdr);
        public delegate void VideoStreamCallback(IntPtr hWnd, IntPtr lpVhdr);
        /// <summary>
        ///  function is the callback function used with streaming capture to optionally process buffers of audio data. 
        ///  The capWaveStreamCallback callback function is a placeholder for the application-supplied function name
        /// </summary>
        /// <param name="hWnd">Handle to the capture window associated with the callback function</param>
        /// <param name="lpWhdr">Pointer to a WAVEHDR structure containing information about the captured audio data</param> 
        //public delegate void capWaveStreamCallback(IntPtr hWnd, WAVEFORMATEX lpWhdr);
        public delegate void WaveStreamCallback(IntPtr hWnd, IntPtr lpWhdr);
        /// <summary>
        /// function is the yield callback function used with video capture. The capYieldCallback yield callback function 
        /// is a placeholder for the application-supplied function name
        /// </summary>
        /// <param name="hWnd">Handle to the capture window associated with the callback function</param>
        public delegate void YieldCallback(IntPtr hWnd);
        /// <summary>
        /// function is the frame callback function used with video capture. The capYieldCallback yield callback function 
        /// is a placeholder for the application-supplied function name
        /// </summary>
        /// <param name="hWnd">Handle to the capture window associated with the callback function</param>
        /// <param name="lpVHdr"/>Bytes used by image</param>
        public delegate void FrameCallback(IntPtr hWnd, IntPtr lpVHdr);
        //public delegate void FrameCallback(IntPtr hWnd, API.VideoHdr lpVHdr);


        //public delegate void FrameEventHandler(IntPtr lwnd, IntPtr lpVHdr);

        #endregion

        #region Methods

        #region API's SendMessage
        //[DllImport("user32.dll")]
        //public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, IntPtr lParam);

        // The SendMessage function sends the specified message to a 
        // window or windows. It calls the window procedure for the specified 
        // window and does not return until the window procedure has processed the message. 
        [DllImport("User32.dll")]
        public static extern Int32 SendMessage(
            IntPtr hWnd,               // handle to destination window
            UInt32 Msg,                // message
            UInt32 wParam,             // first message parameter
            Int32 lParam               // second message parameter
            );

        [DllImport("User32.dll")]
        public static extern Int32 SendMessage(
            IntPtr hWnd,               // handle to destination window
            UInt32 Msg,                // message
            int wParam,					// first message parameter
            IntPtr lParam              // second message parameter
            );

        [DllImport("User32.dll")]
        public static extern Int32 SendMessage(
            IntPtr hWnd,
            UInt32 Msg,
            int wParam,
            ControlCallback fpFunc);

        [DllImport("User32.dll")]
        public static extern Int32 SendMessage(
            IntPtr hWnd,
            UInt32 Msg,
            int wParam,
            ErrorCallback fpFunc);

        [DllImport("User32.dll")]
        public static extern Int32 SendMessage(
            IntPtr hWnd,
            UInt32 Msg,
            int wParam,
            FrameCallback fpFunc);

        [DllImport("User32.dll")]
        public static extern Int32 SendMessage(
            IntPtr hWnd,
            UInt32 Msg,
            int wParam,
            VideoStreamCallback fpFunc);

        [DllImport("User32.dll")]
        public static extern Int32 SendMessage(
            IntPtr hWnd,
            UInt32 Msg,
            int wParam,
            WaveStreamCallback fpFunc);

        [DllImport("User32.dll")]
        public static extern Int32 SendMessage(
            IntPtr hWnd,
            UInt32 Msg,
            int wParam,
            YieldCallback fpFunc);

        [DllImport("User32.dll")]
        public static extern Int32 SendMessage(
            IntPtr hWnd,
            UInt32 Msg,
            int wParam,
            StatusCallback fpFunc);

        [DllImport("User32.dll")]
        public static extern Int32 SendMessage(
            IntPtr hWnd,
            UInt32 Msg,
            int wParam,
            ref BitmapInfo _bitmap);

        [DllImport("User32.dll")]
        public static extern Int32 SendMessage(
            IntPtr hWnd,
            UInt32 Msg,
            int wParam,
            ref CapDriverCaps _capdrivercaps);

        [DllImport("User32.dll")]
        public static extern Int32 SendMessage(
            IntPtr hWnd,
            UInt32 Msg,
            int wParam,
            ref CapStatus _capstatus);

        [DllImport("User32.dll")]
        public static extern Int32 SendMessage(
            IntPtr hWnd,
            UInt32 Msg,
            int wParam,
            ref WaveFormatex _waveformatex);

        [DllImport("User32.dll")]
        public static extern Int32 SendMessage(
            IntPtr hWnd,
            UInt32 Msg,
            int wParam,
            //[MarshalAs(UnmanagedType.LPStr)]
            String ptr);

        [DllImport("User32.dll")]
        public static extern Int32 SendMessage(
            IntPtr hWnd,
            UInt32 Msg,
            int wParam,
            ref CaptureParms _captureparms);

        #endregion


        /// <summary>
        /// function retrieves the version description of the capture driver
        /// </summary>
        /// <param name="wDriverIndex">Index of the capture driver. The index can range from 0 through 9.
        /// Plug-and-Play capture drivers are enumerated first, followed by capture drivers listed in the registry, 
        /// which are then followed by capture drivers listed in SYSTEM.INI</param>
        /// <param name="lpszName">Pointer to a buffer containing a null-terminated string corresponding to the capture driver name</param>
        /// <param name="cbName">Length, in bytes, of the buffer pointed to by lpszName</param>
        /// <param name="lpszVer">Pointer to a buffer containing a null-terminated string corresponding to the description of the capture driver</param>
        /// <param name="cbVer">Length, in bytes, of the buffer pointed to by lpszVer</param>
        /// <returns>Returns TRUE if successful or FALSE otherwise</returns>
        [DllImport("avicap32.dll")]
        //public static extern bool capGetDriverDescription(int wDriverIndex, byte[] lpszName, int cbName, byte[] lpszVer, int cbVer);
        public static extern bool capGetDriverDescription(short wDriver, string lpszName, int cbName, string lpszVer, int cbVer);

        /// <summary>
        /// The capCreateCaptureWindow function creates a capture window
        /// </summary>
        /// <param name="lpszWindowName">Null-terminated string containing the name used for the capture window</param>
        /// <param name="dwStyle">Window styles used for the capture window. Window styles are described with the CreateWindowEx function</param>
        /// <param name="x">The x-coordinate of the upper left corner of the capture window</param>
        /// <param name="y">The y-coordinate of the upper left corner of the capture window</param>
        /// <param name="nWidth">Width of the capture window</param>
        /// <param name="nHeight">Height of the capture window</param>
        /// <param name="hWnd">Handle to the parent window</param>
        /// <param name="nID">Window identifier</param>
        /// <returns>Returns a handle of the capture window if successful or NULL otherwise</returns>
        [DllImport("avicap32.dll")]
        public static extern IntPtr capCreateCaptureWindow(string lpszWindowName, int dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWnd, int nID);

        //[DllImport("avicap32.dll", EntryPoint="capCreateCaptureWindowA")]
        //public static extern int capCreateCaptureWindowA(string lpszWindowName, int dwStyle, int X, int Y, int nWidth, int nHeight, int hwndParent, int nID);

        [DllImport("user32", EntryPoint = "OpenClipboard")]
        public static extern int OpenClipboard(int hWnd);

        [DllImport("user32", EntryPoint = "EmptyClipboard")]
        public static extern int EmptyClipboard();

        [DllImport("user32", EntryPoint = "CloseClipboard")]
        public static extern int CloseClipboard();

        [DllImport("user32", EntryPoint = "GetClipboardData")]
        public static extern IntPtr GetClipboardData(uint uFormat);

        //[DllImport("avicap32.dll", EntryPoint="capGetDriverDescriptionA")]
        //public static extern int capGetDriverDescriptionA(ushort wDriverIndex, string lpszName, short cbName, string lpszVer, short cbVer);

        [DllImport("user32.dll")]
        public static extern bool IsWindow(IntPtr hWnd /* handle to window*/);


















        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern int DeleteDC(IntPtr hdc);

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern int BitBlt(IntPtr hdcDst, int xDst, int yDst, int w, int h, IntPtr hdcSrc, int xSrc, int ySrc, int rop);
        public static int SRCCOPY = 0x00CC0020;

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern IntPtr CreateDIBSection(IntPtr hdc, ref BitmapInfoHeader bmi, uint Usage, out IntPtr bits, IntPtr hSection, uint dwOffset);
        //static uint BI_RGB = 0;
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct BITMAPINFO
        {
            public uint biSize;
            public int biWidth, biHeight;
            public short biPlanes, biBitCount;
            public uint biCompression, biSizeImage;
            public int biXPelsPerMeter, biYPelsPerMeter;
            public uint biClrUsed, biClrImportant;
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 256)]
            public uint[] cols;
        }

        public static uint MAKERGB(int r, int g, int b)
        {
            return ((uint)(b & 255)) | ((uint)((r & 255) << 8)) | ((uint)((g & 255) << 16));
        }





        #endregion
    }
}
