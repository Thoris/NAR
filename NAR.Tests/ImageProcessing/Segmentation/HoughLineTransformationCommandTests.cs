using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NAR.Tests.ImageProcessing.Segmentation
{
    [TestFixture]
    [Ignore]
    public class HoughLineTransformationCommandTests : BaseCommand
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
            NAR.ImageProcessing.Segmentation.HoughLineTransformationCommand test = new NAR.ImageProcessing.Segmentation.HoughLineTransformationCommand(true);

            //NAR.Model.IImage sample = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\HoughSample.bmp"));

            NAR.ImageProcessing.Images.GrayscaleCommand gray = new NAR.ImageProcessing.Images.GrayscaleCommand(false);
            NAR.ImageProcessing.Borders.SobelCommand border = new NAR.ImageProcessing.Borders.SobelCommand(true);
            NAR.ImageProcessing.Images.BlackWhiteCommand bw = new NAR.ImageProcessing.Images.BlackWhiteCommand(true);

            NAR.Model.IImage input = gray.Execute(base.Model);
            //input.Image.Save("C:\\temp\\1.bmp");
            input = border.Execute(input);
            //input.Image.Save("C:\\temp\\2.bmp");
            input = bw.Execute(input);
            //input.Image.Save("C:\\temp\\3.bmp");
            


            NAR.Model.IImage result = test.Execute(input);

            //result.Image.Save("C:\\temp\\result.bmp");
            
            NAR.Model.IImage bitmap = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\Commands\\HoughLineTransformationCommand.bmp"));

            base.CheckAllBytes(result.Bytes, bitmap.Bytes);

        }

        [Test]
        public void TestShowLines()
        {
            NAR.ImageProcessing.Segmentation.HoughLineTransformationCommand test = new NAR.ImageProcessing.Segmentation.HoughLineTransformationCommand(true);

            NAR.Model.IImage sample = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\HoughSample.bmp"));

            NAR.Model.IImage result = test.Execute(sample);

            //result.Image.Save("C:\\temp\\test.bmp");

            NAR.Model.IImage compare = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\HoughSampleLine.bmp"));

            base.CheckAllBytes(compare.Bytes, result.Bytes);
        }
        #endregion

    }
}
