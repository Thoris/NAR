using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NAR.Tests.ImageProcessing.Borders
{
    [TestFixture]
    public class RichardsCommandTests : BaseCommand
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
            NAR.ImageProcessing.Borders.RichardsCommand test = new NAR.ImageProcessing.Borders.RichardsCommand(false);

            NAR.Model.IImage result = test.Execute(base.Model);

            //result.Image.Save("C:\\temp\\cap.bmp");

            NAR.Model.IImage bitmap = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\Commands\\RichardsCommand.bmp"));

            base.CheckAllBytes(result.Bytes, bitmap.Bytes);
        }

        [Test]
        public void TestExecuteInGrayScale()
        {
            NAR.ImageProcessing.Borders.RichardsCommand test = new NAR.ImageProcessing.Borders.RichardsCommand(true);

            NAR.ImageProcessing.Images.GrayscaleCommand gray = new NAR.ImageProcessing.Images.GrayscaleCommand(false);
            NAR.Model.IImage result = gray.Execute(base.Model);

            result = test.Execute(result);


            //result.Image.Save("C:\\temp\\RichardsGrayScaleCommand.bmp");

            NAR.Model.IImage bitmap = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\Commands\\RichardsGrayScaleCommand.bmp"));

            base.CheckAllBytes(result.Bytes, bitmap.Bytes);
        }

        #endregion
    }
}
