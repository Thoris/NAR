using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NAR.ImageProcessing.Segmentation;

namespace NAR.Tests.ImageProcessing.Segmentation
{
    [TestFixture]
    public class HoughCircleTransformationTests : BaseCommand
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
        public void TestCollectCircles()
        {
            NAR.ImageProcessing.Segmentation.HoughCircleTransformation test = new NAR.ImageProcessing.Segmentation.HoughCircleTransformation(35);

            NAR.Model.IImage sample = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\HoughSample.bmp"));

            NAR.Model.IImage result = test.Execute(sample);

            HoughCircle[] circles = test.GetCirclesByRelativeIntensity(0.5);

            Assert.IsNotNull(circles);
            Assert.AreEqual(circles.Length, 2);

        }

        [Test]
        public void TestHoughGraphics()
        {
            NAR.ImageProcessing.Segmentation.HoughCircleTransformation test = new NAR.ImageProcessing.Segmentation.HoughCircleTransformation(35);

            //NAR.ImageProcessing.Images.BlackWhiteCommand bw = new NAR.ImageProcessing.Images.BlackWhiteCommand(false);

            NAR.Model.IImage sample = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\HoughSample.bmp"));

            //NAR.Model.IImage result = bw.Execute(sample);

            NAR.Model.IImage result = test.Execute(sample);

            NAR.Model.IImage graph = test.CreateGraph();

            NAR.Model.IImage compare = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\Commands\\HoughResultCircle.bmp"));

            base.CheckAllBytes(compare.Bytes, graph.Bytes);

        }
       

        #endregion

    }
}
