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
using System.Runtime.InteropServices;

//http://social.msdn.microsoft.com/Forums/en/csharpgeneral/thread/77504ef3-d15f-4d3e-9e27-c6d6267434d9
//http://www.experts-exchange.com/Programming/Languages/C_Sharp/Q_24622577.html
//http://www.codeproject.com/Articles/20519/Webcamera-Multithreading-and-VFW
//http://en.verysource.com/code/2564810_1/classshowvideo.cs.html

namespace NAR.Capture.Drivers.WebCamWinAPI
{
    public class Wrapper : IWrapper
    {
        #region Variables
        private IntPtr _hwnd;
        #endregion

        #region Properties

        public IntPtr Hwnd
        {
            get { return _hwnd; }
        }

        #endregion

        #region Constructors/Destructors

        public Wrapper(IntPtr hwnd)
        {
            _hwnd = hwnd;
        }

        #endregion

        #region Internal Methods

        private bool SendMessage(IntPtr hwnd, int message, int wparam, long lparam)
        {
            if (API.Function.IsWindow(hwnd))
                return Convert.ToBoolean(API.Function.SendMessage(hwnd, Convert.ToUInt32(message), wparam, (IntPtr)lparam));
            else
                return Convert.ToBoolean (0);
        }
        private bool SendMessage(IntPtr hwnd, int message, int wparam, IntPtr lparam)
        {

            if (API.Function.IsWindow(hwnd))
                return Convert.ToBoolean(API.Function.SendMessage(hwnd, Convert.ToUInt32(message), wparam, lparam));
            else
                return Convert.ToBoolean(0);
        }
        private bool SendMessage(IntPtr hwnd, int message, int wparam, ref string sparam)
        {
            if (API.Function.IsWindow(hwnd))
                return Convert.ToBoolean(API.Function.SendMessage(hwnd, Convert.ToUInt32(message), wparam, sparam));
            else
                return Convert.ToBoolean(0);            
        }
        private IntPtr VarPtr(object _object)
        {
            IntPtr lpAdrress;

            //Aloca o objeto na memória
            GCHandle GCH = GCHandle.Alloc(_object, GCHandleType.Pinned);
            
            //Pega o endereço do mesmo
            lpAdrress = GCH.AddrOfPinnedObject();
            
            //Libera o objeto da memória
            GCH.Free();

            //Retorna o endereço encontrado
            return lpAdrress;
        }


        public int OpenClipboard()
        {
            return API.Function.OpenClipboard((int)_hwnd);
        }
        public int EmptyClipboard()
        {
            return API.Function.EmptyClipboard();
        }
        public int CloseClipboard()
        {
            return API.Function.CloseClipboard();
        }
        public IntPtr GetClipboardData(uint uFormat)
        {
            return API.Function.GetClipboardData(uFormat);
        }

        #endregion

        #region Methods

        public static IntPtr CreateCaptureWindow(string caption, int width, int height, IntPtr hWnd)
        {
            return API.Function.capCreateCaptureWindow(
                caption,
                API.Layout.WS_CHILD + API.Layout.WS_VISIBLE,
                0,
                0,
                width,
                height,
                hWnd,
                0);
        }
        public IList<API.Device> GetDevices()
        {

            IList<API.Device> list = new List<API.Device>();
            string name = new string(' ', 80);
            string ver = new string(' ', 80);

            for (short i = 0; API.Function.capGetDriverDescription(i, name, 80, ver, 80); i++)
            {
                list.Add(new API.Device(i, name.Trim(), ver.Trim()));
            }
            return list.ToArray();

        }
        public bool DriverConnect(int driverNumber)
        {
            return ((bool)SendMessage(_hwnd, API.WinMessage.Cap.Driver.Connect, (int)driverNumber, 0L)); 
        }
        public bool CaptureSequence()
        {
            return ((bool)SendMessage(_hwnd, API.WinMessage.Cap.Sequence.Value, (int)0, (long)0L)); 
        }
        public bool PreviewRate(int rate)
        {
            return ((bool)SendMessage(_hwnd, API.WinMessage.Cap.SetPreviewRate, (int)(rate), 0));
        }
        public bool Preview(bool preview)
        {
            return ((bool)SendMessage(_hwnd, API.WinMessage.Cap.SetPreview, Convert.ToInt32(preview), 0L)); 
        }
        public bool Overlay(bool overlay)
        {
            return ((bool)SendMessage(_hwnd, API.WinMessage.Cap.SetOverlay, Convert.ToInt32(overlay), 0L));
        }
        public bool PreviewScale(bool scaling)
        {
            return ((bool)SendMessage(_hwnd, API.WinMessage.Cap.SetScale, Convert.ToInt32(scaling), 0L));
        }
        public bool EditCopy()
        {
            return ((bool)SendMessage(_hwnd, API.WinMessage.Cap.EditCopy, 0, 0L)); 
        }
        public bool CopyToClipboard()
        {
            return ((bool)SendMessage(_hwnd, API.WinMessage.Cap.Copy, 0, 0L)); 
        }
        public bool SetCallbackOnError(API.Function.ErrorCallback proc)
        {
            if (API.Function.IsWindow(_hwnd))
                return Convert.ToBoolean(API.Function.SendMessage(_hwnd, API.WinMessage.Cap.CallBack.SetError, 0, proc));
            else
                return false;
        }
        public bool SetCallbackOnStatus(API.Function.StatusCallback proc)
        {
            if (API.Function.IsWindow(_hwnd))
                return Convert.ToBoolean(API.Function.SendMessage(_hwnd, API.WinMessage.Cap.CallBack.SetStatus, 0, proc));
            else
                return false;
        }
        public bool SetCallbackOnYield(API.Function.YieldCallback proc)
        {
            if (API.Function.IsWindow(_hwnd))
                return Convert.ToBoolean(API.Function.SendMessage(_hwnd, API.WinMessage.Cap.CallBack.SetYield, 0, proc));
            else
                return false;
        }
        public bool SetCallbackOnFrame(API.Function.FrameCallback proc)
        {
            if (API.Function.IsWindow(_hwnd))
                return Convert.ToBoolean(API.Function.SendMessage(_hwnd, API.WinMessage.Cap.CallBack.SetFrame, 0, proc));
            else
                return false;
        }
        public bool SetCallbackOnVideoStream(API.Function.VideoStreamCallback proc)
        {
            if (API.Function.IsWindow(_hwnd))
                return Convert.ToBoolean(API.Function.SendMessage(_hwnd, API.WinMessage.Cap.CallBack.SetVideoStream, 0, proc));
            else
                return false;
        }
        public bool SetCallbackOnWaveStream(API.Function.WaveStreamCallback proc)
        {
            if (API.Function.IsWindow(_hwnd))
                return Convert.ToBoolean(API.Function.SendMessage(_hwnd, API.WinMessage.Cap.CallBack.SetWaveStream, 0, proc));
            else
                return false;
        }
        public bool SetCallbackOnCapControl(API.Function.ControlCallback proc)
        {
            if (API.Function.IsWindow(_hwnd))
                return Convert.ToBoolean(API.Function.SendMessage(_hwnd, API.WinMessage.Cap.CallBack.SetCapControl, 0, proc));
            else
                return false;
        }
        public bool SetUserData(long user)
        {
            return (bool)SendMessage(_hwnd, API.WinMessage.Cap.SetUserData, 0, user); 
        }
        public void GetUserData()
        {
            SendMessage(_hwnd, API.WinMessage.Cap.GetUserData, 0, 0); 
        }
        public bool DriverDisconnect()
        {
            return (bool)SendMessage(_hwnd, API.WinMessage.Cap.Driver.Disconnect, 0, 0);
        }
        public bool DriverGetName(ref string name, int size)
        {
            return (bool)SendMessage(_hwnd, API.WinMessage.Cap.Driver.GetName, size, ref name); 	
        }
        public bool DriverGetVersion(ref string version, int size)
        {
            return (bool)SendMessage(_hwnd, API.WinMessage.Cap.Driver.GetVersion, size, ref version); 
        }
        public bool DriverGetCaps(ref API.CapDriverCaps capDriverCaps, int size)
        {
            if (API.Function.IsWindow(_hwnd))
                return Convert.ToBoolean(API.Function.SendMessage(_hwnd, API.WinMessage.Cap.Driver.GetCaps, size, ref capDriverCaps));
            else
                return false;
        }
        public bool FileSetCaptureFile(string name)
        {
            return (bool)SendMessage(_hwnd, API.WinMessage.Cap.File.SetCaptureFile, 0, ref name); 
        }
        public bool FileGetCaptureFile(ref string name, int size)
        {
            return (bool)SendMessage(_hwnd, API.WinMessage.Cap.File.GetCaptureFile, size, ref name); 
        }
        public bool FileAlloc(uint size)
        {
            return (bool)SendMessage(_hwnd, API.WinMessage.Cap.File.Allocate , 0, size); 
        }
        public bool FileSaveAs(string name)
        {
            return (bool)SendMessage(_hwnd, API.WinMessage.Cap.File.SaveAs, 0, ref name); 
        }
        public bool FileSaveDIB(string name)
        {
            return (bool)SendMessage(_hwnd, API.WinMessage.Cap.File.SaveDib, 0, ref name); 
        }
        public bool SetAudioFormat(API.WaveFormatex waveFormatex, int size)
        {
            if (API.Function.IsWindow(_hwnd))
                return Convert.ToBoolean(API.Function.SendMessage(_hwnd, API.WinMessage.Cap.Audio.SetFormat, size, ref waveFormatex));
            else
                return false;
        }
        public bool GetAudioFormat(ref API.WaveFormatex waveFormatex, int size)
        {
            if (API.Function.IsWindow(_hwnd))
                return Convert.ToBoolean(API.Function.SendMessage(_hwnd, API.WinMessage.Cap.Audio.GetFormat, size, ref waveFormatex));
            else
                return false;
        }
        public bool GetAudioFormatSize()
        {
            return (bool)SendMessage(_hwnd, API.WinMessage.Cap.Audio.GetFormat, 0, 0L); 
        }
        public bool DlgVideoFormat()
        {
            return (bool)SendMessage(_hwnd, API.WinMessage.Cap.Video.DlgFormat, 0, 0L); 
        }
        public bool DlgVideoSource()
        {
            return (bool)SendMessage(_hwnd, API.WinMessage.Cap.Video.DlgSource, 0, 0L); 
        }
        public bool DlgVideoDisplay()
        {
            return (bool)SendMessage(_hwnd, API.WinMessage.Cap.Video.DlgDisplay , 0, 0L); 
        }
        public bool DlgVideoCompression()
        {
            return (bool)SendMessage(_hwnd, API.WinMessage.Cap.Video.DlgCompression, 0, 0L);
        }
        public bool GetVideoFormat(ref API.BitmapInfo bitmapInfo, int size)
        {
            if (API.Function.IsWindow(_hwnd))
                return Convert.ToBoolean(API.Function.SendMessage(_hwnd, API.WinMessage.Cap.Video.GetFormat, size, ref bitmapInfo));
            else
                return false;
        }
        public bool GetVideoFormatSize()
        {
            return (bool)SendMessage(_hwnd, API.WinMessage.Cap.Video.GetFormat, 0, 0L); 
        }
        public bool SetVideoFormat(API.BitmapInfo bitmapInfo, int size)
        {
            if (API.Function.IsWindow(_hwnd))
                return Convert.ToBoolean(API.Function.SendMessage(_hwnd, API.WinMessage.Cap.Video.SetFormat, size, ref bitmapInfo));
            else
                return false;
        }
        public bool GetStatus(ref API.CapStatus capStatus, int size)
        {
            if (API.Function.IsWindow(_hwnd))
                return Convert.ToBoolean(API.Function.SendMessage(_hwnd, API.WinMessage.Cap.GetStatus , size, ref capStatus));
            else
                return false;
        }
        public bool SetScrollPos(IntPtr address)
        {
            return (bool)SendMessage(_hwnd, API.WinMessage.Cap.SetScroll, 0, address); 
        }
        public bool GrabFrame()
        {
            return (bool)SendMessage(_hwnd, API.WinMessage.Cap.GrabFrame, 0, 0L); 
        }
        public bool GrabFrameNoStop()
        {
            return (bool)SendMessage(_hwnd, API.WinMessage.Cap.GrabFrameNoStop, 0, 0L); 
        }
        public bool CaptureSequenceNoFile()
        {
            return (bool)SendMessage(_hwnd, API.WinMessage.Cap.Sequence.SequenceNoFile, 0, 0L); 
        }
        public bool CaptureStop()
        {
            return (bool)SendMessage(_hwnd, API.WinMessage.Cap.Stop, 0, 0L); 
        }
        public bool CaptureAbort()
        {
            return (bool)SendMessage(_hwnd, API.WinMessage.Cap.Abort, 0, 0L); 
        }
        public bool CaptureSingleFrameOpen()
        {
            return (bool)SendMessage(_hwnd, API.WinMessage.Cap.SingleFrameOpen, 0, 0L); 
        }
        public bool CaptureSingleFrameClose()
        {
            return (bool)SendMessage(_hwnd, API.WinMessage.Cap.SingleFrameClose, 0, 0L); 
        }
        public bool CaptureSingleFrame()
        {
            return (bool)SendMessage(_hwnd, API.WinMessage.Cap.SingleFrame, 0, 0L); 
        }
        public bool CaptureGetSetup(ref API.CaptureParms captureParms, int size)
        {
            if (API.Function.IsWindow(_hwnd))
                return Convert.ToBoolean(API.Function.SendMessage(_hwnd, API.WinMessage.Cap.Sequence.GetSetup, size, ref captureParms));
            else
                return false;
        }
        public bool CaptureSetSetup(API.CaptureParms captureParms, int size)
        {
            if (API.Function.IsWindow(_hwnd))
                return Convert.ToBoolean(API.Function.SendMessage(_hwnd, API.WinMessage.Cap.Sequence.SetSetup, size, ref captureParms));
            else
                return false;
        }
        public bool SetMCIDeviceName(string name)
        {
            return (bool)SendMessage(_hwnd, API.WinMessage.Cap.Mci.SetDevice, 0, VarPtr(name)); 
        }
        public bool GetMCIDeviceName(ref string name, int size)
        {
            return (bool)SendMessage(_hwnd, API.WinMessage.Cap.Mci.GetDevice, size, ref name); 
        }
        public bool PaletteOpen(string name)
        {
            return (bool)SendMessage(_hwnd, API.WinMessage.Cap.Pal.Open, 0, ref name); 
        }
        public bool PaletteSave(string name)
        {
            return (bool)SendMessage(_hwnd, API.WinMessage.Cap.Pal.Save, 0, ref name); 
        }
        public bool PalettePaste()
        {
            return (bool)SendMessage(_hwnd, API.WinMessage.Cap.Pal.Paste, 0, 0L); 
        }
        public bool PaletteAuto(int frames, int colors)
        {
            return (bool)SendMessage(_hwnd, API.WinMessage.Cap.Pal.AutoCreate, frames, colors); 
        }
        public bool PaletteManual(int paletteHistogram, int numColors)
        {
            return (bool)SendMessage(_hwnd, API.WinMessage.Cap.Pal.AutoCreate, paletteHistogram, numColors);
        }
    
        #endregion        
    
    }
}
