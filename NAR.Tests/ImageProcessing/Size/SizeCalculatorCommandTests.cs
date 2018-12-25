using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NAR.Tests.ImageProcessing.Size
{
    [TestFixture]
    public class SizeCalculatorCommandTests : BaseCommand
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
        [Ignore]
        public void TestExecute()
        {

            int x1 = 0;
            int x2 = 256;
            int y1 = 125;
            int y2 = 125;
            double mmPixel = 10;
            bool fill = true;

            NAR.ImageProcessing.Size.SizeCalculatorCommand test = new NAR.ImageProcessing.Size.SizeCalculatorCommand(x1,x2,y1,y2,mmPixel,fill);


            NAR.Model.IImage newImage = null;

            newImage = test.Execute(base.Pen);

            //newImage.Image.Save("C:\\temp\\cap.bmp");

            NAR.Model.IImage bitmap = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\Commands\\SizeCalculatorCommand.bmp"));

            base.CheckAllBytes(newImage.Bytes, bitmap.Bytes);
        }

        #endregion

    }
}
