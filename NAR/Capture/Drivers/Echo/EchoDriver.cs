using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using NAR.Capture.Drivers.Echo.Items;

namespace NAR.Capture.Drivers.Echo
{
    public class EchoDriver : IDriverFacade
    {
        #region Constants
        public const int MAX_ITEMS = 10;
        #endregion

        #region Events/Delegates

        public event DriverImageCapture OnDriverImageCapture; 
        public event VideoStreamRecording OnVideoStreamRecording;


        #endregion

        #region Variables

        private Model.ImageConfig _config;
        private System.Timers.Timer _tick;
        private IDataItem [] _items;
        private Image _lastImage;
        
    
        #endregion

        #region Properties

        public Model.ImageConfig ImageConfig
        {
            get { return _config; }
        }

        #endregion

        #region Constructors/Destructors

        public EchoDriver()
        {
            Random rnd = new Random();


            _items = new IDataItem[MAX_ITEMS];
            for (int c=0; c < MAX_ITEMS; c++)
            {
                if (c >= MAX_ITEMS / 2)
                    _items[c] = new QuadData();
                else
                    _items[c] = new CircleData();

                if (rnd.Next(2) == 1)
                    _items[c].Fill = true;
                else
                    _items[c].Fill = false;
                
            }
        }

        #endregion

        #region IDriverFacade Members

        public int Connect(Model.ImageConfig config)
        {
            _config = config;

            Random rnd = new Random();

            
            for (int c = 0; c < MAX_ITEMS; c++)
            {
                _items[c].MaximumPos = new Point(config.Width, config.Height);
                _items[c].CurrentPos = new Point(rnd.Next(config.Width - 50), rnd.Next(config.Height - 50));

                
            }

            _tick = new System.Timers.Timer(config.FrameRate);
            _tick.Elapsed += new System.Timers.ElapsedEventHandler(_tick_Elapsed);
            _tick.Enabled = true;

            return 0;
        }
        public bool Disconnect()
        {
            _tick.Enabled = false;
            _tick.Stop();

            return true;
        }
        public string[] GetDevices()
        {
            return new string[] { "Echo Driver" };
        }

        #endregion

        #region IDriverVideo Members

        public System.Drawing.Bitmap GrabFrame()
        {
            return (Bitmap)_lastImage;
        }

        #endregion

        #region IDriverRecording Members

        public bool RecordStart(bool audio, uint limitSec, string file)
        {
            throw new NotImplementedException();
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

        #region Methods

        private Image Draw()
        {
            Bitmap img = new Bitmap(_config.Width, _config.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);


            for (int c = 0; c < MAX_ITEMS; c++)
            {
                _items[c].Move();
                img = _items[c].Draw(img);
            }


            return (Image)img;

        }
        //private Bitmap DrawCircle(Bitmap source)
        //{
        //    Bitmap img = (Bitmap)source.Clone();

        //    Graphics g = Graphics.FromImage(img);

        //    for (int c = 0; c < MAX_ITEMS; c++)
        //    {

        //        Pen myPen = new Pen(_circleData[c].Forecolor, _circleData[c].LineLenght);

        //        _circleData[c].Move();

        //        g.DrawEllipse(myPen, _circleData[c].Pos.X, _circleData[c].Pos.Y,
        //            _circleData[c].Pos.Width, _circleData[c].Pos.Height);

        //        myPen.Dispose();
            
        //    }

        //    g.Dispose();

        //    return img;

        //}

        #endregion

        #region Events

        private void _tick_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _tick.Enabled = false;

            try
            {
                Image newImage = Draw();

                _lastImage = newImage;

                if (OnDriverImageCapture != null)
                    OnDriverImageCapture(this, new Arguments.DriverImageEventArgs((Bitmap)newImage.Clone()));

            }
            finally
            {
                _tick.Enabled = true;
            }
        }

        #endregion
    }
}
