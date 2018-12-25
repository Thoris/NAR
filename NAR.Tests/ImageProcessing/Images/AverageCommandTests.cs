using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NAR.Tests.ImageProcessing.Images
{
    [TestFixture]
    public class AverageCommandTests : BaseCommand
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
            NAR.ImageProcessing.Images.AverageCommand test = new NAR.ImageProcessing.Images.AverageCommand(true);

            NAR.Model.IImage result = test.Execute(base.Model);

            //result.Image.Save("C:\\temp\\cap.bmp");

            NAR.Model.IImage bitmap = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\Commands\\AverageCommand.bmp"));

            base.CheckAllBytes(result.Bytes, bitmap.Bytes);

        }
        [Test]
        public void TestExecuteGrayscale()
        {
            NAR.ImageProcessing.Images.AverageCommand test = new NAR.ImageProcessing.Images.AverageCommand(false);

            NAR.ImageProcessing.Images.GrayscaleCommand gray = new NAR.ImageProcessing.Images.GrayscaleCommand(false);
            NAR.Model.IImage result = gray.Execute(base.Model);

            result = test.Execute(result);

            result = test.Execute(base.Model);

            //result.Image.Save("C:\\temp\\capgray.bmp");

            NAR.Model.IImage bitmap = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\Commands\\AverageCommandGrayscale.bmp"));

            base.CheckAllBytes(result.Bytes, bitmap.Bytes);

        }
        
        #endregion
    }
}
