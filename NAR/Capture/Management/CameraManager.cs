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
using System.Drawing.Imaging;
using System.Drawing;

namespace NAR.Capture.Management
{
    public class CameraManager : ICameraFacade
    {
        #region Constants
        private const int TotalBuffer = 10;
        #endregion

        #region Delegates / Events

        public event FrameGrab OnFrameGrab;
 
        #endregion

        #region Variables

        private Drivers.IDriverFacade _driver;
        private ImageProcessing.IImageInvoker _imageInvoker;
        private Model.ImageConfig _config;
        private Model.IImage[] _buffer;
        private int _totalBuffer = 0;
        private int _fpsCurrentValue = 0;
        private int _fpsLastValue = 0;
        private DateTime _fpsControl = DateTime.Now;

        #endregion

        #region Properties
        public int FPS
        {
            get { return _fpsLastValue; }
        }
        public int Buffer
        {
            get { return _totalBuffer; }
        }
        public ImageProcessing.CommandCollection Commands
        {
            get
            {
                return _imageInvoker.Commands;
            }
            set
            {
                _imageInvoker.Commands = value;
            }
        }
        public Model.ImageConfig ImageConfig
        {
            get
            {
                return _config;
            }
            set
            {
                _config = value;
            }
        }
        #endregion

        #region Constructors/Destructors

        public CameraManager(Drivers.IDriverFacade driver)
        {
            _driver = driver;
            _driver.OnDriverImageCapture += new Drivers.DriverImageCapture(Driver_OnDriverImageCapture);
            _config = driver.ImageConfig;
            _imageInvoker = new ImageProcessing.ImageInvoker();

            if (_config == null)
                _config = new Model.ImageConfig();


            _buffer = new Model.ImageBitmap[TotalBuffer];
        }

        

        #endregion

        #region Methods

        /// <summary>
        /// Rotates the buffer of images to grab only the last one, but keeking available to get it in the future.
        /// </summary>
        /// <param name="image">The image.</param>
        private void RotateBuffer(Model.IImage image)
        {
            if (_totalBuffer >= TotalBuffer - 1)
            {
                for (int c = 0; c < _totalBuffer - 1; c++)
                {
                    _buffer[c] = _buffer[c + 1];
                }
                _buffer[_totalBuffer - 1] = image;
            }
            else
            {
                _buffer[_totalBuffer++] = image;
            }

        }
        private Model.IImage GetBuffer()
        {
            Bitmap result = _driver.GrabFrame();

            //if (_totalBuffer == 0)
            //    return null;

            //return _buffer[_totalBuffer-- -1];        

            if (result != null)
            {
                return new Model.ImageBitmap(result);
            }
            return null;
        }
        #endregion

        #region ICamera Members

        public int Connect()
        {
            int result = _driver.Connect(new Model.ImageConfig(_config.Width,_config.Height, _config.GrabByEvent, _config.FrameRate));

            return result;
        }
        public bool Disconnect()
        {
            bool result = _driver.Disconnect();

            //If there is an event linked to the frame grabber
            //if (OnFrameGrab != null)
            //    _driver.OnDriverImageCapture -= new Drivers.DriverImageCapture(OnDriverImageCapture);


            return result;
        }
        public Model.IImage GrabFrame()
        {
            Model.IImage modelImage = GetBuffer();

            if (modelImage == null)
                return null;


            //Bitmap image = _driver.GrabFrame();
            
            //if (image == null)
            //    return null;

            //Model.IImage modelImage = new Model.ImageBitmap(image);

            //if (OnFrameGrab == null)
            //    RotateBuffer(modelImage);
            
            return _imageInvoker.ExecuteCommand(modelImage);

        }
        public bool RecordStart(bool audio, string file)
        {
            return true;
            //return _driver.RecordStart(audio);
        }
        public string[] GetDevices()
        {
            return _driver.GetDevices();
        }

        #endregion

        #region ICameraConfig members

        public void OpenDialogConfigVideoFormat()
        {
            bool result = _driver.ConfigVideoFormat();

        }
        public void OpenDialogConfigVideoCompression()
        {
            bool result = _driver.ConfigVideoCompression();
        }
        public void OpenDialogConfigVideoDisplay()
        {
            bool result = _driver.ConfigVideoDisplay();
        }
        public void OpenDialogConfigVideoSource()
        {
            bool result = _driver.ConfigVideoSource();
        }

        #endregion

        #region ICameraRecording members
        public bool RecordStart(bool audio, uint limitSec, string file)
        {
            return false;// _driver.RecordStart(audio, limitSec, file);
        }
        #endregion

        #region Events

        /// <summary>
        /// Handles the OnDriverImageCapture event of the Driver control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Drivers.Arguments.DriverImageEventArgs" /> instance containing the event data.</param>
        private void Driver_OnDriverImageCapture(object sender, Drivers.Arguments.DriverImageEventArgs e)
        {
            Model.IImage model;

            Bitmap newBitmap = (Bitmap)e.Image.Clone();


                if (e.Data == null)
                    model = new Model.ImageBitmap(newBitmap);
                else
                    model = new Model.ImageBitmap(newBitmap, e.Data);
            

            //RotateBuffer(model);


            DateTime current = DateTime.Now;

            if (_fpsControl.AddSeconds(1).CompareTo(current) < 0)
            {
                _fpsLastValue = _fpsCurrentValue;
                _fpsCurrentValue = 0;
                _fpsControl = current;
            }
            else
            {
                _fpsCurrentValue++;
            }

        }

        #endregion
    }
}
