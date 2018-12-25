using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;


namespace NAR.Tests.ImageProcessing.Util
{
    [TestFixture]
    public class HistogramTests : BaseCommand
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
        public void TestCalculateFromGrayscale()
        {
            NAR.ImageProcessing.Utils.Histogram test = new NAR.ImageProcessing.Utils.Histogram();

            NAR.Model.IImage bitmap = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\Commands\\GrayscaleCommand.bmp"));

            byte limiar = 0;
            byte [] result = test.CalculateFromGrayscale(base.Model, out limiar);


            Assert.IsNotNull(result);
            Assert.AreEqual(256, result.Length);
            Assert.Greater(limiar, 0);
            
        }

        [Test]
        public void TestCalculateFromRGB()
        {
            NAR.ImageProcessing.Utils.Histogram test = new NAR.ImageProcessing.Utils.Histogram();

            byte[] red;
            byte[] green;
            byte[] blue;


            byte limiar = test.CalculateFromRGB(base.Model, out red, out green, out blue);


            Assert.IsNotNull(red);
            Assert.AreEqual(256, red.Length);

            Assert.IsNotNull(green);
            Assert.AreEqual(256, green.Length);

            Assert.IsNotNull(blue);
            Assert.AreEqual(256, blue.Length);

            Assert.Greater(limiar, 0);
        }

        #endregion
    }
}
