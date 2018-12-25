using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAR.Capture.Drivers.AForge.DirectShow;
using System.Drawing;

namespace NAR.Capture.Drivers.AForge
{
    public class AForgeDriver : IDriverFacade, IDisposable
    {
        #region Events/Delegates

        public event VideoStreamRecording OnVideoStreamRecording;
        public event DriverImageCapture OnDriverImageCapture;

        #endregion

        #region Variables

        private object _lock = new Object();
        private Model.ImageConfig _config;
        private VideoCaptureDevice _videoSource;
        private Bitmap _lastFrame;
        private bool _isContainsBuffer = false;

        #endregion

        #region Properties

        public Model.ImageConfig ImageConfig
        {
            get { return _config; }
        }

        #endregion

        #region Constructors/Destructors

        public AForgeDriver()
        {

        }

        #endregion

        #region Methods

        public int Connect(Model.ImageConfig config)
        {
            _config = config;

            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            _videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            _videoSource.NewFrame += new Video.NewFrameEventHandler(videoSource_NewFrame);
            //_videoSource.DesiredFrameRate = 30000;
            _videoSource.DesiredFrameSize = new Size(config.Width, config.Height);
            _videoSource.Start();
            
            /// videoDevices = new FilterInfoCollection( FilterCategory.VideoInputDevice );
            /// // create video source
            /// VideoCaptureDevice videoSource = new VideoCaptureDevice( videoDevices[0].MonikerString );
            /// // set NewFrame event handler
            /// videoSource.NewFrame += new NewFrameEventHandler( video_NewFrame );
            /// // start the video source
            /// videoSource.Start( );
            /// // ...
            /// // signal to stop when you no longer need capturing
            /// videoSource.SignalToStop( );
            return 0;
        }
        public bool Disconnect()
        {
            _videoSource.Stop();
            return true;
        }
        public string[] GetDevices()
        {
            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count == 0)
                return new string[0];

            string[] result = new string[videoDevices.Count];

            for (int c = 0; c < videoDevices.Count; c++)
            {
                result[c] = videoDevices[c].Name;
            }

            return result;
        }
        public System.Drawing.Bitmap GrabFrame()
        {
            Bitmap result = null;

            //if (!_isContainsBuffer)
            //    return null;

            _isContainsBuffer = false;

            result = (Bitmap)_lastFrame;

            return result;
        }
        public bool RecordStart(bool audio, uint limitSec, string file)
        {
            return false;
        }
        public bool ConfigVideoFormat()
        {
            return false;
        }
        public bool ConfigVideoCompression()
        {
            return false;
        }
        public bool ConfigVideoDisplay()
        {
            return false;
        }
        public bool ConfigVideoSource()
        {
            return false;
        }

        #endregion

        #region Events

        private void videoSource_NewFrame(object sender, Video.NewFrameEventArgs eventArgs)
        {
            _lastFrame = (Bitmap)eventArgs.Frame.Clone();

            _isContainsBuffer = true;

            if (OnDriverImageCapture != null)
                OnDriverImageCapture(this, new Arguments.DriverImageEventArgs(_lastFrame));


        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Disconnect();
        }

        #endregion
    }
}
