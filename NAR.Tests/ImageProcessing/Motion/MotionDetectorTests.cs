using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NAR.Tests.ImageProcessing.Motion
{
    [TestFixture]
    public class MotionDetectorTests : BaseCommand
    {
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
        public void TestDetect()
        {
            int resX = 256;
            int resY = 256;
            byte relevance = 10;
            int qtdPixel = 90;

            NAR.ImageProcessing.Motion.MotionDetector test = new NAR.ImageProcessing.Motion.MotionDetector(resX, resY, relevance, qtdPixel);

            IList<NAR.ImageProcessing.Motion.MotionPosition> result = test.Detect(base.Model);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);

            result = test.Detect(base.Motion);

            Assert.IsNotNull(result);
            Assert.Greater(result.Count, 0);
            
            NAR.Model.IImage newImage = test.FillImageMotion(base.Motion, result);

            Assert.IsNotNull(newImage);
            //newImage.Image.Save("C:\\temp\\cap.bmp");


            //NAR.Model.IImage result = test.Execute(base.Model);

            NAR.Model.IImage bitmap = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\Commands\\DetectMotionCommand.bmp"));

            base.CheckAllBytes(newImage.Bytes, bitmap.Bytes);
        }
        #endregion
    }
}
