using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;


namespace NAR.Forms.Tests
{
    public partial class frmMain : Form
    {
        #region Variables

        private NAR.Capture.Management.ICameraFacade _camera;
        private DateTime _lastCapture;
        private int _fps;
        private frmImage _frmImage;
        private frmHistogram _frmHistogramRGB;
        private frmHistogram _frmHistogramGrayScale;
        private frmHoughGraph _frmHoughGraph;
        private NAR.ImageProcessing.Utils.Histogram _histogram = new ImageProcessing.Utils.Histogram();

        #endregion

        #region Constructors/Destructors
       
        public frmMain()
        {
            InitializeComponent();


            //NAR.Capture.Drivers.IDriver apiWindows = new NAR.Capture.Drivers.WebCamWinAPI.WebCamWinDriver(
            //    new Model.ImageConfig(640, 480, System.Drawing.Imaging.PixelFormat.Format24bppRgb, false));
            
            NAR.Capture.Drivers.IDriverFacade apiWindows = new NAR.Capture.Drivers.WebCamWinAPI.WebCamWinDriver();
            NAR.Capture.Drivers.IDriverFacade directShowDriver = new NAR.Capture.Drivers.DirectShow.DirectShowDriver();
            NAR.Capture.Drivers.IDriverFacade aforgeDriver = new NAR.Capture.Drivers.AForge.AForgeDriver();
            NAR.Capture.Drivers.IDriverFacade echoDriver = new NAR.Capture.Drivers.Echo.EchoDriver();
            NAR.Capture.Drivers.IDriverFacade videoLib = new NAR.Capture.Drivers.DirectX.DSVideoLibDriver();



            _camera = new NAR.Capture.Management.CameraManager(videoLib);
            //_camera.OnFrameGrab += new NAR.Capture.Management.FrameGrab(OnFrameGrab);
            //_camera.PreviewControl = this;


        }

        #endregion

        #region Methods

        private void ControlFPS()
        {
            try
            {
                if (DateTime.Now.Second == _lastCapture.Second)
                    _fps++;
                else
                {
                    this.stblblFfpCurrent.Text = _fps.ToString("000");
                    _fps = 0;
                    _lastCapture = DateTime.Now;
                }
            }
            finally
            {
            }

            

            this.stblblFPS.Text = _camera.FPS.ToString();
            this.lblBuffer.Text = _camera.Buffer.ToString();
        }
        private void SetCamera(bool enabled)
        {
            this.mnuConnect.Enabled = !enabled;
            this.mnuDisconnect.Enabled = enabled;
            this.mnuGrabImage.Enabled = enabled;

            this.tmGrab.Enabled = false;

            if (enabled)
                this.tmGrab.Enabled = true;
            
        }
        private void ShowImage(Model.IImage image)
        {
            if (image == null || image.Image == null)
            {
            }
            else
            {
                try
                {
                    this.picImage.Image = (Image)image.Image.Clone();
                    //this.picImage.Show();
                }
                finally
                {
                }
            }
        }
        private void ApplyGrabbedImage(Model.IImage image)
        {
            for (int c = 0; c < _camera.Commands.Count; c++)
            {
                
            }
        }
        private void ShowDevices()
        {
            this.mnuDevices.DropDownItems.Clear();

            string[] devices = _camera.GetDevices();

            for (int c = 0; c < devices.Length; c++)
            {
                this.mnuDevices.DropDownItems.Add(devices[c]);
                this.mnuDevices.DropDownItems[this.mnuDevices.DropDownItems.Count - 1].Click += new EventHandler(dropDownItem_Click);

                if (c == 0)
                    ((ToolStripMenuItem)this.mnuDevices.DropDownItems[this.mnuDevices.DropDownItems.Count - 1]).Checked = true;

            }
            
        }

        #endregion

        #region Events

        private void OnFrameGrab(object sender, Capture.Management.Arguments.FrameEventArgs e)
        {
            
        }  
        private void mnuConnect_Click(object sender, EventArgs e)
        {
            _camera.ImageConfig.Width = 640;
            _camera.ImageConfig.Height = 480;
            _camera.ImageConfig.FrameRate = 1;
            

            int result = _camera.Connect();

            this.tmGrab.Interval = _camera.ImageConfig.FrameRate;
            this.tmGrab.Start();
            



            SetCamera(true);

        }
        private void mnuDisconnect_Click(object sender, EventArgs e)
        {
            this.tmGrab.Stop();

            bool result = _camera.Disconnect();

            SetCamera(false);
        }
        private void mnuGrabImage_Click(object sender, EventArgs e)
        {
            Model.IImage image = _camera.GrabFrame();

            this.cltCommandManager.ComparedImage = image;

            if (_frmImage != null)
            {
                _frmImage.Close();
                _frmImage.Dispose();
                _frmImage = null;
            }
            
            _frmImage = new Tests.frmImage(image);
            _frmImage.Show();
            _frmImage.Refresh();

        }
        private void tmGrab_Tick(object sender, EventArgs e)
        {
            this.tmGrab.Enabled = false;
            this.tmGrab.Stop();

            Model.IImage image = _camera.GrabFrame();

            if (image != null)
            {
                ShowImage(image);


                if (_frmHistogramGrayScale != null)
                {
                    _frmHistogramGrayScale.SetGrayScale(_histogram.CalculateFromGrayscale(image));
                }
                else if (_frmHistogramRGB != null)
                {
                    byte[] red;
                    byte[] green;
                    byte[] blue;
                    _histogram.CalculateFromRGB(image, out red, out green, out blue);

                    _frmHistogramRGB.SetRGB(red, green, blue);
                }
                if (_frmHoughGraph != null)
                {
                    for (int c = 0; c < _camera.Commands.Count; c++)
                    {
                        if (_camera.Commands[c].GetType() == typeof(NAR.ImageProcessing.Segmentation.HoughLineTransformationCommand))
                        {
                            NAR.ImageProcessing.Segmentation.HoughLineTransformationCommand command =
                                (NAR.ImageProcessing.Segmentation.HoughLineTransformationCommand)_camera.Commands[c];

                            _frmHoughGraph.Graph = command.CreateGraph().Image;
                        }
                    }
                }

                ControlFPS();


                ShowImage(image);

            }
            


            this.tmGrab.Enabled = true;
            this.tmGrab.Start();
        }
        private void cltCommandManager_OnListChanged(object sender, Controls.CommandListEventArgs e)
        {

            _camera.Commands.Clear();
            _camera.Commands.AddRange(e.List);


        }
        private void mnuCapture_Click(object sender, EventArgs e)
        {
            
        }
        private void mnuHistogramRGB_Click(object sender, EventArgs e)
        {
            if (_frmHistogramRGB != null)
            {
                _frmHistogramRGB.Close();
                _frmHistogramRGB.Dispose();
                _frmHistogramRGB = null;
            }

            _frmHistogramRGB = new frmHistogram();
            _frmHistogramRGB.Show();
        }
        private void mnuHistogramGrayscale_Click(object sender, EventArgs e)
        {
            if (_frmHistogramGrayScale != null)
            {
                _frmHistogramGrayScale.Close();
                _frmHistogramGrayScale.Dispose();
                _frmHistogramGrayScale = null;
            }

            _frmHistogramGrayScale = new frmHistogram();
            _frmHistogramGrayScale.Show();

        }
        private void mnuConfigVideoCompression_Click(object sender, EventArgs e)
        {
            _camera.OpenDialogConfigVideoCompression();
        }
        private void mnuConfigVideoDisplay_Click(object sender, EventArgs e)
        {
            _camera.OpenDialogConfigVideoDisplay();
        }
        private void mnuConfigVideoSource_Click(object sender, EventArgs e)
        {
            _camera.OpenDialogConfigVideoSource();
        }
        private void mnuConfigVideoFormat_Click(object sender, EventArgs e)
        {
            _camera.OpenDialogConfigVideoFormat();
        }
        private void mnuHoughGraph_Click(object sender, EventArgs e)
        {
            if (_frmHoughGraph != null)
            {
                _frmHoughGraph.Close();
                _frmHoughGraph.Dispose();
                _frmHoughGraph = null;
            }

            _frmHoughGraph = new frmHoughGraph();
            _frmHoughGraph.Show();
        }
        private void mnuStartVideo_Click(object sender, EventArgs e)
        {
            _camera.RecordStart(true, 5, "test.avi");
            //NAR.Capture.Drivers.IDriverFacade driver = new NAR.Capture.Drivers.WebCamWinAPI.WebCamWinDriver();

            //driver.OnVideoStreamRecording += new NAR.Capture.Drivers.VideoStreamRecording(driver_OnVideoStreamRecording);

            //driver.Connect(new NAR.Model.ImageConfig(Width, Height, false));

            
            //bool result = driver.RecordStart(true, 10, ".\\RecStopByEvent.avi");

            //driver.Disconnect();
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            ShowDevices();
        }
        private void dropDownItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            if (!item.Selected)
            {
                for (int c = 0; c < this.mnuDevices.DropDownItems.Count; c++)
                {
                    ((ToolStripMenuItem)this.mnuDevices.DropDownItems[c]).Checked = false ;
                }
            }

            item.Checked = true;

        }

        #endregion


    }
}
