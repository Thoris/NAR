
//http://msdn.microsoft.com/en-us/library/windows/desktop/dd390352(v=vs.85).aspx

using System.Drawing;
using NAR.Capture.Drivers.DirectShow.DShowNET;

namespace NAR.Capture.Drivers.DirectShow
{
    public class DirectShowDriver : IDriverFacade
    {
        #region Events/Delegates

        public event DriverImageCapture OnDriverImageCapture;
        public event VideoStreamRecording OnVideoStreamRecording;

        #endregion

        #region Variables

        private Model.ImageConfig _config;
        private DSCapture.Capture _capture;
        private System.Windows.Forms.PictureBox _pictureBoxAux;
        private Bitmap _lastBitmap;

        #endregion

        #region Properties

        public Model.ImageConfig ImageConfig
        {
            get { return _config; }
        }
        

        #endregion

        #region Constructors/Destructors

        public DirectShowDriver()
        {
            _pictureBoxAux = new System.Windows.Forms.PictureBox();
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
            DSCapture.Filters filters = new DSCapture.Filters();
            
            _capture = new DSCapture.Capture(filters.VideoInputDevices[0], filters.AudioInputDevices[0]);
            //_capture.FrameEvent2 += new DSCapture.Capture.HeFrame(_capture_FrameEvent2);
            _capture.CaptureComplete +=new System.EventHandler(_capture_CaptureComplete);
            _capture.FrameSize = new Size(config.Width, config.Height);
            _capture.FrameRate = 30000;
            _capture.PreviewWindow = _pictureBoxAux;
            //_capture.AllowSampleGrabber = true;


            _capture.Cue();
            
            _capture.Start();
            

            return 0;
        }
        /// <summary>
        /// Disconnect to the driver
        /// </summary>
        /// <returns>Returns true if could disconect sucessfully.</returns>
        public bool Disconnect()
        {
            _capture.Stop();

            return false;
        }
        /// <summary>
        /// Methods which collect the devices allowed to connect to camera
        /// </summary>
        /// <returns>list of device names</returns>
        public string[] GetDevices()
        {
            DSCapture.Filters filters = new DSCapture.Filters();

            if (filters.VideoInputDevices == null)
                return new string[0];


            string[] result = new string[filters.VideoInputDevices.Count];

            for (int c = 0; c < filters.VideoInputDevices.Count; c++)
            {
                result[c] = filters.VideoInputDevices[c].Name;
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

            if (_lastBitmap != null)
                return new Bitmap(_pictureBoxAux.Image);

            return _lastBitmap;
        }

        #endregion

        #region IDriverAudio Members
        #endregion

        #region IDriverRecording Members

        public bool RecordStart(bool audio, uint limitSec, string file)
        {


            return false ;
        }

        #endregion

        #region IDriverConfig Members

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

        private void _capture_FrameEvent2(Bitmap BM)
        {
            _lastBitmap = BM;

            if (OnDriverImageCapture != null)
                OnDriverImageCapture(this, new Arguments.DriverImageEventArgs( _lastBitmap));
        }
        private void _capture_CaptureComplete(object sender, System.EventArgs e)
        {

        }

        #endregion



    }
}
    