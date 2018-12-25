using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace NAR.Capture.Drivers.DirectX
{
    public class DSVideoLibDriver : IDriverFacade
    {
        #region Variables
        private Model.ImageConfig _config;
        private GraphManager _graph;
        #endregion

        #region Constructors/Destructors
        public DSVideoLibDriver()
        {

        }
        #endregion

        #region IDriverFacade Members

        public event DriverImageCapture OnDriverImageCapture;
        public Model.ImageConfig ImageConfig
        {
            get { return _config; }
        }
        public int Connect(Model.ImageConfig config)
        {
            _config = config;
            string config_default = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><dsvl_input><camera show_format_dialog=\"true\" friendly_name=\"\"><pixel_format><RGB32 flip_h=\"false\" flip_v=\"true\"/></pixel_format></camera></dsvl_input>";

            _graph = new GraphManager();
            _graph.BuildGraphFromXMLString(config_default);

            _graph.EnableMemoryBuffer(3, 3);

            _graph.Run();
            

            return 0;
        }
        public bool Disconnect()
        {
            if (_graph.bufferCheckedOut)
            {
                if (_graph.CheckinMemoryBuffer(true) != 0)
                    return false;

                _graph.bufferCheckedOut = false;
            }

            if (_graph.Stop() == 0)
                return true;
            
            return false;
        }
        public string[] GetDevices()
        {
            return new string[] { "" };
        }

        #endregion

        #region IDriverVideo Members

        public System.Drawing.Bitmap GrabFrame()
        {
            try
            {
                int wait_result;

                if (_graph.bufferCheckedOut)
                {
                    int hr = _graph.CheckinMemoryBuffer(false);
                    if (hr != 0)
                        return null;
                    _graph.bufferCheckedOut = false;
                }
                wait_result = _graph.WaitForNextSample(0xFFFFFFFF);

                byte[] buffer = null;
                if (wait_result == 0x00000000L)
                {
                    int width = 0, height = 0;
                    PIXELFORMAT pxFormat = PIXELFORMAT.PIXELFORMAT_RGB32;
                    TimeSpan tm = new TimeSpan();
                    int hr = _graph.CheckoutMemoryBuffer(ref buffer, ref width, ref height, ref pxFormat, ref tm);

                    if (hr == 0)
                    {
                        _graph.bufferCheckedOut = true;


                        //Creating the new bitmap to be populed from byte array
                        Bitmap image = new Bitmap(width, height);

                        //Starting the image attributes locking that
                        BitmapData BitMap1Data = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                        //Getting the address from the first pixel
                        IntPtr dataPtr = BitMap1Data.Scan0; //This assumes you've created a IntPtr dataPtr somewhere else. 

                        //Copying the byte array
                        Marshal.Copy(buffer, 0, dataPtr, width * height * 3);

                        //Unlocking the image
                        image.UnlockBits(BitMap1Data);


                        return image;
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                }
            }
            
            return (null);
        }

        #endregion

        #region IDriverRecording Members

        public event VideoStreamRecording OnVideoStreamRecording;
        public bool RecordStart(bool audio, uint limitSec, string file)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDriverConfig Members

        public bool ConfigVideoFormat()
        {
            throw new NotImplementedException();
        }
        public bool ConfigVideoCompression()
        {
            throw new NotImplementedException();
        }
        public bool ConfigVideoDisplay()
        {
            throw new NotImplementedException();
        }
        public bool ConfigVideoSource()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
