using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NAR.ImageProcessing.Segmentation;

namespace NAR.Tests.ImageProcessing.Segmentation
{
    [TestFixture]
    public class HoughLineTransformationTests : BaseCommand
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
        public void TestCollectLines()
        {
            NAR.ImageProcessing.Segmentation.HoughLineTransformation test = new NAR.ImageProcessing.Segmentation.HoughLineTransformation();

            NAR.Model.IImage sample = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\HoughSample.bmp"));

            NAR.Model.IImage result = test.Execute(sample);

            HoughLine[] list = test.GetLinesByRelativeIntensity(0.5);

            Assert.IsNotNull(list);
            Assert.AreEqual(list.Length, 4);

        }

        [Test]
        public void TestHoughGraphics()
        {
            NAR.ImageProcessing.Segmentation.HoughLineTransformation test = new NAR.ImageProcessing.Segmentation.HoughLineTransformation();

            NAR.Model.IImage sample = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\HoughSample.bmp"));

            NAR.Model.IImage result = test.Execute(sample);


            NAR.Model.IImage graph = test.CreateGraph();

            NAR.Model.IImage compare = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\Commands\\HoughResult.bmp"));

            base.CheckAllBytes(compare.Bytes, graph.Bytes);

        }

        #endregion

    }
}
