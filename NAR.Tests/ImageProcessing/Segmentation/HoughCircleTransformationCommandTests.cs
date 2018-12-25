using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NAR.Tests.ImageProcessing.Segmentation
{
    [TestFixture]
    public class HoughCircleTransformationCommandTests : BaseCommand
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
            NAR.ImageProcessing.Segmentation.HoughCircleTransformationCommand test = new NAR.ImageProcessing.Segmentation.HoughCircleTransformationCommand();

            NAR.ImageProcessing.Images.GrayscaleCommand gray = new NAR.ImageProcessing.Images.GrayscaleCommand(false);
            NAR.ImageProcessing.Borders.SobelCommand border = new NAR.ImageProcessing.Borders.SobelCommand(true);
            NAR.ImageProcessing.Images.BlackWhiteCommand bw = new NAR.ImageProcessing.Images.BlackWhiteCommand(true);

            NAR.Model.IImage input = gray.Execute(base.Model);
            input = border.Execute(input);
            input = bw.Execute(input);
            
            NAR.Model.IImage result = test.Execute(input);

            //result.Image.Save("C:\\temp\\t.bmp");

            //NAR.Model.IImage bitmap = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\Commands\\HoughLineTransformationCommand.bmp"));

            //base.CheckAllBytes(result.Bytes, bitmap.Bytes);

        }

        [Test]
        public void TestShowCircles()
        {
            NAR.ImageProcessing.Segmentation.HoughLineTransformationCommand test = new NAR.ImageProcessing.Segmentation.HoughLineTransformationCommand(true);

            NAR.Model.IImage sample = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\HoughSample.bmp"));

            NAR.Model.IImage result = test.Execute(sample);

            //NAR.Model.IImage compare = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\HoughSampleLine.bmp"));
            //base.CheckAllBytes(compare.Bytes, result.Bytes);
        }
        #endregion

    }
}
