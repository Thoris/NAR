using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NAR.Tests.ImageProcessing.Images.ColorReduction
{
    [TestFixture]
    [Ignore]
    public class SierraColorDitheringCommandTests : BaseCommand
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
        public void TestExecuteGrayScale()
        {
            NAR.ImageProcessing.Images.ColorReduction.SierraColorDitheringCommand test = new NAR.ImageProcessing.Images.ColorReduction.SierraColorDitheringCommand();

            NAR.Model.IImage result = test.Execute(base.Model);

            //result.Image.Save("C:\\temp\\cap.bmp");

            NAR.Model.IImage bitmap = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\Commands\\AverageCommand.bmp"));

            base.CheckAllBytes(result.Bytes, bitmap.Bytes);

        }

        #endregion
    }
}
