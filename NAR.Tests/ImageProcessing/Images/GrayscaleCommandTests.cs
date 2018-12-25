using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NAR.Tests.ImageProcessing.Images
{
    [TestFixture]
    public class GrayscaleCommandTests : BaseCommand
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
        public void TestExecuteByAverage()
        {
            NAR.ImageProcessing.Images.GrayscaleCommand test = new NAR.ImageProcessing.Images.GrayscaleCommand(false);

            NAR.Model.IImage result = test.Execute(base.Model);
            
            NAR.Model.IImage bitmap = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\Commands\\GrayscaleCommand.bmp"));

            base.CheckAllBytes(result.Bytes, bitmap.Bytes);

        }
        [Test]
        public void TestExecuteByLuminance()
        {
            NAR.ImageProcessing.Images.GrayscaleCommand test = new NAR.ImageProcessing.Images.GrayscaleCommand(true);

            NAR.Model.IImage result = test.Execute(base.Model);

            NAR.Model.IImage bitmap = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\Commands\\GrayscaleCommandLuminance.bmp"));

            base.CheckAllBytes(result.Bytes, bitmap.Bytes);

        }

        #endregion
    }
}
