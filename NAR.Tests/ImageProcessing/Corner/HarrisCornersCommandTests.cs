using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NAR.Tests.ImageProcessing.Corner
{
    [TestFixture]
    public class HarrisCornersCommandTests : BaseCommand
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

            double constantK = 0.05;
            int treshold = 9000000;
            int edgeTreshold = 0;
            int supressWindowSize = 11;



            //NAR.Model.IImage bitmapInput = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\HoughSample.bmp"));

            NAR.Model.IImage bitmapInput = new NAR.Model.ImageBitmap(base.Original);


            NAR.ImageProcessing.Images.GrayscaleCommand gray = new NAR.ImageProcessing.Images.GrayscaleCommand(false);
            //NAR.ImageProcessing.Borders.SobelCommand border = new NAR.ImageProcessing.Borders.SobelCommand(true);
            //NAR.ImageProcessing.Effects.GaussianCommand gaussian = new NAR.ImageProcessing.Effects.GaussianCommand(true);
            //NAR.ImageProcessing.Images.BlackWhiteCommand bw = new NAR.ImageProcessing.Images.BlackWhiteCommand(true);
            //NAR.ImageProcessing.Segmentation.HoughLineTransformationCommand seg = new NAR.ImageProcessing.Segmentation.HoughLineTransformationCommand(true);


            NAR.ImageProcessing.Corner.HarrisCornersCommand test =
                new NAR.ImageProcessing.Corner.HarrisCornersCommand(constantK, treshold, edgeTreshold, supressWindowSize, true);




            NAR.Model.IImage result = gray.Execute(bitmapInput);
            //result = border.Execute(result);
            //result = bw.Execute(result);
            //result = seg.Execute(result);
            //result = gaussian.Execute(result);
            //result = test.Execute(result);

            result = test.Execute(result);

            //result.Image.Save("C:\\temp\\result.bmp");

            NAR.Model.IImage bitmap = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\Commands\\HarrisCornersCommand.bmp"));

            base.CheckAllBytes(result.Bytes, bitmap.Bytes);

        }

        #endregion
    }
}
