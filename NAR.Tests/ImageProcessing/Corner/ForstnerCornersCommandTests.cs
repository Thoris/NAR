using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NAR.Tests.ImageProcessing.Corner
{
    [TestFixture]
    public class ForstnerCornersCommandTests : BaseCommand
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

            //NAR.Model.IImage bitmapInput = new NAR.Model.ImageBitmap(base.Original);

            //NAR.ImageProcessing.Images.GrayscaleCommand gray = new NAR.ImageProcessing.Images.GrayscaleCommand(false);

            NAR.ImageProcessing.Corner.ForstnerCornersCommand test =
                new NAR.ImageProcessing.Corner.ForstnerCornersCommand(true);

            //NAR.Model.IImage result = gray.Execute(bitmapInput);

            //result = test.Execute(result);


            //NAR.Model.IImage bitmap = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\Commands\\SusanCornersCommand.bmp"));

            //base.CheckAllBytes(result.Bytes, bitmap.Bytes);

        }

        #endregion
    }
}
