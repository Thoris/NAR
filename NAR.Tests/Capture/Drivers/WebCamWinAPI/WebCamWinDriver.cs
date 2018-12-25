using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Drawing;

namespace NAR.Tests.Capture.Drivers.WebCamWinAPI
{
    [TestFixture]
    public class WebCamWinDriver
    {
        #region Constants
        private const int Width = 640;
        private const int Height = 480;
        #endregion

        #region Constructors/Destructors

        [TestFixtureSetUp]
        public void Init()
        {
        }
        [TearDown]
        public void Cleanup()
        {
        }

        #endregion

        #region Methods

        [Test]
        public void TestConnectDisconnect()
        {
            NAR.Capture.Drivers.IDriverFacade driver = new NAR.Capture.Drivers.WebCamWinAPI.WebCamWinDriver();

            int result = driver.Connect(new NAR.Model.ImageConfig(Width, Height, false, 66));

            Assert.AreEqual(0, result);

            bool resultDisconnect = driver.Disconnect();

            Assert.IsTrue(resultDisconnect);
        }
        [Test]
        public void TestGrabFrame()
        {
            NAR.Capture.Drivers.IDriverFacade driver = new NAR.Capture.Drivers.WebCamWinAPI.WebCamWinDriver();

            driver.Connect(new NAR.Model.ImageConfig(Width, Height, false, 66));



            System.Drawing.Image image = driver.GrabFrame();

            //image.Save("C:\\temp\\test.bmp");

            //Assert.IsNull(image);

            image = driver.GrabFrame();
            //Assert.IsNull(image);
            //image.Save("C:\\temp\\test.bmp");

            image = driver.GrabFrame();
            //Assert.IsNull(image);
            //image.Save("C:\\temp\\test.bmp");

            image = driver.GrabFrame();
            Assert.IsNotNull(image);
            //image.Save("C:\\temp\\test.bmp");


            //Model.ImageBitmap img = new Model.ImageBitmap(image);
            //img.FillProperties();

            //Bitmap bmp = new Bitmap(image);
            //bmp.Save("C:\\Temp\\Capture.bmp");

            driver.Disconnect();

            Assert.IsNotNull(image);
            
        }
        [Test]
        public void TestRecordStartAndStopByEvent()
        {
            NAR.Capture.Drivers.IDriverFacade driver = new NAR.Capture.Drivers.WebCamWinAPI.WebCamWinDriver();

            driver.OnVideoStreamRecording += new NAR.Capture.Drivers.VideoStreamRecording(driver_OnVideoStreamRecording);

            driver.Connect(new NAR.Model.ImageConfig(Width, Height, false, 66));

            bool result = driver.RecordStart(true, 10, ".\\RecStopByEvent.avi");

            driver.Disconnect();
        }

        private void driver_OnVideoStreamRecording(object sender, NAR.Capture.Drivers.Arguments.VideoStreamEventArgs e)
        {
            e.StopRecording = true;
        }

        [Test]
        public void TestRecordStartAndStopByTimeLimit()
        {
            NAR.Capture.Drivers.IDriverFacade driver = new NAR.Capture.Drivers.WebCamWinAPI.WebCamWinDriver();

            driver.Connect(new NAR.Model.ImageConfig(Width, Height, false, 66));

            bool result = driver.RecordStart(true, 2, ".\\RecStopByTimeLimit.avi");

            driver.Disconnect();
        }
        [Test]
        public void TestGrabbingTime()
        {
            NAR.Capture.Drivers.IDriverFacade driver = new NAR.Capture.Drivers.WebCamWinAPI.WebCamWinDriver();

            driver.Connect(new NAR.Model.ImageConfig(Width, Height, false, 66));

            DateTime startDate = DateTime.Now;


            while (DateTime.Compare(startDate.AddSeconds(5), DateTime.Now) > 0)
            {
                System.Drawing.Image image = driver.GrabFrame();
            }

            driver.Disconnect();

        }

        #endregion
    }
}
