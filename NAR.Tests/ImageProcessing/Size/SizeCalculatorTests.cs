using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NAR.Tests.ImageProcessing.Size
{
    [TestFixture]
    public class SizeCalculatorTests : BaseCommand
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
        public void TestCalculate()
        {
            NAR.ImageProcessing.Size.SizeCalculator test = new NAR.ImageProcessing.Size.SizeCalculator();

            int x1 = 0;
            int x2 = 256;
            int y1 = 125;
            int y2 = 125;
            double mmPixel = 10;
            bool fill = true;

            NAR.Model.IImage newImage = null;

            double result = test.Calculate(base.Pen, x1, x2, y1, y2, mmPixel, fill, out newImage);

            Assert.AreEqual(220, result);
            //newImage.Image.Save("C:\\temp\\cap.bmp");

            NAR.Model.IImage bitmap = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\Commands\\SizeCalculatorCommand.bmp"));

            base.CheckAllBytes(newImage.Bytes, bitmap.Bytes);
        }

        #endregion

    }
}
