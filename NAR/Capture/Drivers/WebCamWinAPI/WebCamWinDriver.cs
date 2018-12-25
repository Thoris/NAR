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
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace NAR.Capture.Drivers.WebCamWinAPI
{
    public class WebCamWinDriver : IDriverFacade
    {
        #region Events/Delegates

        public event DriverImageCapture OnDriverImageCapture;
        public event VideoStreamRecording OnVideoStreamRecording;

        #endregion

        #region Variables

        private int _driverIndex = 0;
        private IWrapper _api;
        private Model.ImageConfig _config;
        private System.Windows.Forms.PictureBox _pictureBoxAux;


        private API.CaptureParms _params = new API.CaptureParms();
        private API.CapDriverCaps _caps = new API.CapDriverCaps();
        private API.WaveFormatex _waveFormatex = new API.WaveFormatex();
        private API.CapStatus _status = new API.CapStatus();
        private API.BitmapInfo _videoFormat = new API.BitmapInfo();
        private string _version = "";
        private string _captureFile = "";
        private string _mciDevice = "";

        private API.Function.ErrorCallback          callbackError;
        private API.Function.ControlCallback        callbackControl;
        private API.Function.StatusCallback         callbackStatus;
        private API.Function.VideoStreamCallback    callbackVideoStream;
        private API.Function.WaveStreamCallback     callbackWaveStream;
        private API.Function.YieldCallback          callbackYield;
        private API.Function.FrameCallback          callbackFrame;


        //private System.Timers.Timer _timer = new System.Timers.Timer(66);

        #endregion

        #region Properties

        public int DriverIndex
        {
            get { return _driverIndex; }
            set { _driverIndex = value; }
        }
        public Model.ImageConfig ImageConfig
        {
            get { return _config; }
        }

        #endregion

        #region Constructors/Destructors

        public WebCamWinDriver()
        {
            _pictureBoxAux = new PictureBox();



            //Initilizing the CallBack functions
            callbackFrame       = new API.Function.FrameCallback(FrameCallBack);
            callbackControl     = new API.Function.ControlCallback(ControlCallBack);
            callbackError       = new API.Function.ErrorCallback(ErrorCallBack);
            callbackStatus      = new API.Function.StatusCallback(StatusCallBack);
            callbackVideoStream = new API.Function.VideoStreamCallback(VideoStreamCallBack);
            callbackWaveStream  = new API.Function.WaveStreamCallback(WaveStreamCallBack);
            callbackYield       = new API.Function.YieldCallback(YieldCallBack);
        }
        
        #endregion

        #region Methods

        private IntPtr CreateCaptureWindow(IntPtr imageHandle, int width, int height)
        {
            if (imageHandle == IntPtr.Zero)
                //imageHandle = new System.Windows.Forms.PictureBox().Handle;
                imageHandle = _pictureBoxAux.Handle;

            IntPtr lpWndC = Wrapper.CreateCaptureWindow("Capture", width, height, imageHandle);

            return lpWndC;
        }
        private int SizeOf(object structure)
        {
            return Marshal.SizeOf(structure);
        }
        private object GetStructure(IntPtr ptr, ValueType structure)
        {
            return Marshal.PtrToStructure(ptr, structure.GetType());
        }
        private void Copy(IntPtr ptr, byte[] data)
        {

            //GCHandle pinnedArray = GCHandle.Alloc(data, GCHandleType.Pinned);
            //ptr = pinnedArray.AddrOfPinnedObject();
            ////do your stuff
            //pinnedArray.Free();

            //-----------------------------


            //ptr = Marshal.AllocHGlobal(data.Length);
            //Marshal.Copy(data, 0, ptr, data.Length);
            //// Call unmanaged code
            //Marshal.FreeHGlobal(ptr);

            //---------------------------
            //count++;            
            Marshal.Copy(ptr, data, 0, data.Length);
            
            //Marshal.CleanupUnusedObjectsInCurrentContext ();
        }
        private void Copy(int ptr, byte[] data)
        {
            Copy(new IntPtr(ptr), data);
        }
        
        #endregion

        #region CallBack Methods

        private void ErrorCallBack(IntPtr hWnd, short id, string description)
        {
        }
        /// <summary>
        /// Method used to convert the image captured from the camera to the Image in .Net format.
        /// This method is started such as an event, and every grab will call this method.        
        /// </summary>
        /// <param name="hWnd">Handle of the virtual window</param>
        /// <param name="hVHdr">Pointer to information of the image captured</param>
        private void FrameCallBack(IntPtr hWnd, IntPtr hVHdr)
        {
            if (OnDriverImageCapture == null)
                return;

            if (hVHdr == null || hWnd == null)
            {
                return;
            }

            try
            {
                API.VideoHdr vhr = new API.VideoHdr();
                vhr = (API.VideoHdr)GetStructure(hVHdr, vhr);

                if (vhr.dwBytesUsed == 0)
                {
                    return;
                }

                byte[] abiVideoData = new byte[vhr.dwBytesUsed];

                Copy(vhr.lpData, abiVideoData);

                Bitmap bmp = new Bitmap(_config.Width, _config.Height, 3 * _config.Width, System.Drawing.Imaging.PixelFormat.Format24bppRgb, new IntPtr(vhr.lpData));

                if (OnDriverImageCapture != null)
                    OnDriverImageCapture(this, new Arguments.DriverImageEventArgs(bmp, abiVideoData));
            }
            finally
            {
            }

        }
        private void ControlCallBack(IntPtr hWnd, bool state)
        {
        }
        private void StatusCallBack(IntPtr hWnd, short id, string description)
        {
            short statusId = id;
            string statusDescription = description;
        }
        private void VideoStreamCallBack(IntPtr hWnd, IntPtr lpVhdr)
        {

            API.VideoHdr videoHeader = new API.VideoHdr();
            videoHeader = (API.VideoHdr)GetStructure(lpVhdr, videoHeader);

            //Criando o vetor da imagem
            byte[] abiVideoData = new byte[videoHeader.dwBytesUsed];

            //Copiando os bytes da imagem
            //
            // TODO: Check the issue of this function
            // 
            Copy(videoHeader.lpData, abiVideoData);
            
            if (OnVideoStreamRecording != null)
            {
                Arguments.VideoStreamEventArgs videoEvent = new Arguments.VideoStreamEventArgs(false);

                OnVideoStreamRecording(this, videoEvent);

                if (videoEvent.StopRecording)
                {
                    _api.CaptureStop();
                    _api.CaptureAbort();
                }
            }

            
        }
        private void WaveStreamCallBack(IntPtr hWnd, IntPtr lpWhdr)
        {

            API.WaveFormatex wave = new API.WaveFormatex();
            wave = (API.WaveFormatex)GetStructure(lpWhdr, wave);
        }
        private void YieldCallBack(IntPtr hWnd)
        {
        }
        #endregion

        #region IDriver Members

        /// <summary>
        /// Connects to the camera
        /// </summary>
        /// <param name="config">Image's Configuration used to start working</param>
        /// <returns>
        /// Returns 0 if could connect sucessfully. Otherwise:
        /// - 1: If could not create the virtual window used in the capture.
        /// - 2: Could not connect to the specific driver.
        /// </returns>
        public int Connect(Model.ImageConfig config)
        {
            if (config == null)
                throw new ArgumentException("config");
            if (config.Width == 0)
                throw new ArgumentException("config.Width");
            if (config.Height == 0)
                throw new ArgumentException("config.Height");



            //Setting to the management the properly configuration of the image to be captured
            _config = config;

            //Creating a virtual window to be used in the camera management
            IntPtr driverHandle = CreateCaptureWindow(IntPtr.Zero, config.Width, config.Height);

            //If could create the handle
            if (driverHandle == IntPtr.Zero)
                return 1;
           
            //Creating the wrapper object used to send messages to windows
            _api = new Wrapper(driverHandle);

            //Connecting to the web cam driver
            if (!_api.DriverConnect(_driverIndex))
                return 2;

            bool res = false;

            res = _api.PreviewScale(true);
            res = _api.PreviewRate(33);
            

            //float FramesPerSec = 30.0;
            //capCaptureGetSetup(hWndC, &CaptureParms, sizeof(CAPTUREPARMS));
            //CaptureParms.dwRequestMicroSecPerFrame = (DWORD)(1.0e6 / FramesPerSec);


            _api.CaptureGetSetup(ref _params, SizeOf(_params));
            _params.dwRequestMicroSecPerFrame = 33333;  //30fps
            _api.CaptureSetSetup(_params, SizeOf(_params));

            _api.CaptureGetSetup(ref _params, SizeOf(_params));
            _api.DriverGetCaps(ref _caps, SizeOf(_caps));
            //_api.DriverGetVersion(ref _version, 100);
            //_api.FileGetCaptureFile (ref _captureFile, 100);
            _api.GetAudioFormat(ref _waveFormatex, SizeOf(_waveFormatex));
            _api.GetAudioFormatSize();
            _api.GetMCIDeviceName(ref _mciDevice, 100);
            _api.GetStatus(ref _status, SizeOf(_status));
            _api.GetVideoFormat(ref _videoFormat, SizeOf(_videoFormat));





            //Setting the callback frame to send the image capture
            _api.SetCallbackOnCapControl(callbackControl);
            _api.SetCallbackOnStatus(callbackStatus);
            _api.SetCallbackOnVideoStream(callbackVideoStream);
            _api.SetCallbackOnWaveStream(callbackWaveStream);
            _api.SetCallbackOnYield(callbackYield);
            _api.SetCallbackOnError(callbackError);
            res = _api.SetCallbackOnFrame(callbackFrame);

            //_timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
            //_timer.Enabled = true;

            

            //Returns 0 if could connect to the camera sucessfully
            return 0;
        }        
        /// <summary>
        /// Disconnect to the driver
        /// </summary>
        /// <returns>Returns true if could disconect sucessfully.</returns>
        public bool Disconnect()
        {
            //_api.CaptureStop();
            //_api.CaptureAbort();

            bool result = _api.DriverDisconnect();

            return result;
        }
        /// <summary>
        /// Methods which collect the devices allowed to connect to camera
        /// </summary>
        /// <returns>list of device names</returns>
        public string[] GetDevices()
        {
            if (_api == null)
            {
                //Creating a virtual window to be used in the camera management
                IntPtr driverHandle = CreateCaptureWindow(IntPtr.Zero, 320, 240);
                //Creating the wrapper object used to send messages to windows
                _api = new Wrapper(driverHandle);
            }


            IList<API.Device> list = _api.GetDevices();
            string[] result = new string[list.Count];


            for (int c = 0; c < list.Count; c++)
            {
                result[c] = list[c].Name;
            }


            return result;
        }

        #endregion

        #region IDriverVideo Members

        /// <summary>
        /// This method starts an event in the FrameCallBack to get the image from the driver
        /// </summary>
        /// <returns>Returns the Image captured, otherwise null if could not get the image.</returns>
        public Bitmap GrabFrame()
        {
            //_api.GrabFrameNoStop();
            //_api.GrabFrame();
            //return null;



            //Raising event to catch the image from web cam
            if (_api.GrabFrameNoStop())
            {
            


                //Openning the clipboard to put the image to be treat
                //_api.OpenClipboard();
                //Clipboard.Clear();

                //Copying to clipboard the image captured
                if (_api.EditCopy())
                {
                    if (Clipboard.ContainsImage())
                    {
                        return (Bitmap)Clipboard.GetImage();
                    }
                    else
                    {
                    }

                    //Copying the image from clipboard to the pointer used to generate the image
                    //IntPtr img = _api.GetClipboardData(2);
                    //Image x = Clipboard.GetImage();
                    //return x;
                    //Closing the clipboard
                    //_api.CloseClipboard();

                    //if (img == IntPtr.Zero)
                    //    return null;

                    //Creating the image from the pointer catch from clipboard
                    //return Image.FromHbitmap(img);


                    //System.Drawing.Bitmap bmp = new Bitmap(640, 480);
                    //System.Drawing.Graphics g = Graphics.FromImage(bmp);
                    //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                    //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                    //g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
                    //g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;

                    //g.DrawImage(Image.FromHbitmap(img), 0, 0, 640, 480);

                    //return bmp;

                    //Bitmap result = Image.FromHbitmap(img);

                    //return result;

                }//endif EdiCopy
          

            }//endif Grab Frame

            //return null if could not grab the image
            return null;
        }

        #endregion

        #region IDriverAudio Members
        #endregion

        #region IDriverRecording Members

        public bool RecordStart(bool audio, uint limitSec, string file)
        {
            
            API.CaptureParms par = new API.CaptureParms();

            bool result = _api.CaptureGetSetup(ref par, SizeOf(par));

            if (!result)
                return false;

            par.dwRequestMicroSecPerFrame = 33334;
            par.fMakeUserHitOKToCapture = false;
            par.fCaptureAudio = audio;

            if (limitSec != 0)
            {
                par.fLimitEnabled = true;
                par.wTimeLimit = limitSec;
            }

            result = _api.CaptureSetSetup(par, SizeOf(par));
            if (!result)
                return false;

            result = _api.CaptureSequence();
            //if (!result)
            //    return false;

            _api.CaptureStop();
            _api.CaptureAbort();


            result = _api.FileSaveAs(file);


            return result;
        }

        #endregion

        #region IDriverConfig Members

        public bool ConfigVideoFormat()
        {
            return _api.DlgVideoFormat();
        }
        public bool ConfigVideoCompression()
        {
            return _api.DlgVideoCompression();
        }
        public bool ConfigVideoDisplay()
        {
            return _api.DlgVideoDisplay();
        }
        public bool ConfigVideoSource()
        {
            return _api.DlgVideoSource();
        }

        #endregion

        #region Events

        //void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    _timer.Stop();
        //    _timer.Enabled = false;

        //    GrabFrame();

        //    _timer.Start();
        //    _timer.Enabled = true;
        //}

        #endregion



    }
}
