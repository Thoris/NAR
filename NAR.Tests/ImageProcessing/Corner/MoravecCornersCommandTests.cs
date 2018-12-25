using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NAR.Tests.ImageProcessing.Corner
{
    [TestFixture]
    public class MoravecCornersCommandTests : BaseCommand
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
            int windowSize = 3;
            int threshold = 500;


            //NAR.Model.IImage bitmapInput = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\HoughSample.bmp"));
            //NAR.Model.IImage bitmapInput = new NAR.Model.ImageBitmap(base.ReadBitmap("C:\\Temp\\Corner.bmp"));

            NAR.Model.IImage bitmapInput = new NAR.Model.ImageBitmap(base.Original);


            NAR.ImageProcessing.Images.GrayscaleCommand gray = new NAR.ImageProcessing.Images.GrayscaleCommand(false);
            

            NAR.ImageProcessing.Corner.MoravecCornersCommand test =
                new NAR.ImageProcessing.Corner.MoravecCornersCommand(threshold, windowSize, true);




            NAR.Model.IImage result = gray.Execute(bitmapInput);
            

            result = test.Execute(result);

            //result.Image.Save("C:\\temp\\result.bmp");

            NAR.Model.IImage bitmap = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\Commands\\MoravecCornersCommand.bmp"));

            base.CheckAllBytes(result.Bytes, bitmap.Bytes);

        }

        #endregion
    }
}
