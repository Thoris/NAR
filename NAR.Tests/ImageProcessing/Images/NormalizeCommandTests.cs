using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NAR.Tests.ImageProcessing.Images
{
    [TestFixture]
    public class NormalizeCommandTests : BaseCommand
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
        public void TestExecuteRGB()
        {
            NAR.ImageProcessing.Images.NormalizeCommand test = new NAR.ImageProcessing.Images.NormalizeCommand(false);

            NAR.Model.IImage result = test.Execute(base.Model);

            //result.Image.Save("C:\\temp\\cap.bmp");

            NAR.Model.IImage bitmap = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\Commands\\NormalizeCommandRGB.bmp"));

            base.CheckAllBytes(result.Bytes, bitmap.Bytes);
        }

        [Test]
        public void TestExecuteGrayscale()
        {
            NAR.ImageProcessing.Images.NormalizeCommand test = new NAR.ImageProcessing.Images.NormalizeCommand(true);

            NAR.Model.IImage result = test.Execute(base.Model);

            NAR.Model.IImage bitmap = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\Commands\\NormalizeCommandGrayscale.bmp"));

            base.CheckAllBytes(result.Bytes, bitmap.Bytes);
        }

        #endregion
    }
}
