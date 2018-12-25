using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NAR.Tests.ImageProcessing.Contours
{
    [TestFixture]
    public class SquareBlobCommandTests : BaseCommand
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
        public void TestExecute()
        {

            NAR.Model.IImage bitmapInput = new NAR.Model.ImageBitmap (base.ReadBitmap(".\\Resources\\Contours.png"));

            NAR.ImageProcessing.Images.GrayscaleCommand gray = new NAR.ImageProcessing.Images.GrayscaleCommand(false);
            NAR.ImageProcessing.Borders.SobelCommand border = new NAR.ImageProcessing.Borders.SobelCommand(false);
            NAR.ImageProcessing.Images.InvertCommand invert = new NAR.ImageProcessing.Images.InvertCommand();
            NAR.ImageProcessing.Images.BlackWhiteCommand bw = new NAR.ImageProcessing.Images.BlackWhiteCommand(true);


            NAR.ImageProcessing.Contours.SquareContoursCommand test =
                new NAR.ImageProcessing.Contours.SquareContoursCommand(true);

            NAR.Model.IImage result = gray.Execute(bitmapInput);
            result = border.Execute(result);
            result = invert.Execute(result);
            result = bw.Execute(result);
            
            result = test.Execute(result);

            //result.Image.Save("C:\\temp\\cap.bmp");

            NAR.Model.IImage bitmap = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\Commands\\SquareContoursCommand.bmp"));

            base.CheckAllBytes(result.Bytes, bitmap.Bytes);

        }

        #endregion
    }
}
