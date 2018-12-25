using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NAR.Tests.ImageProcessing.Images
{
    [TestFixture]
    public class BlackWhiteCommandTests : BaseCommand
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
            NAR.ImageProcessing.Images.BlackWhiteCommand test = new NAR.ImageProcessing.Images.BlackWhiteCommand(false);

            NAR.Model.IImage result = test.Execute(base.Model);

            NAR.Model.IImage bitmap = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\Commands\\BlackWhiteCommand.bmp"));

            base.CheckAllBytes(result.Bytes, bitmap.Bytes);

        }
        [Test]
        public void TestExecuteCalculatingLimiar()
        {
            NAR.ImageProcessing.Images.BlackWhiteCommand test = new NAR.ImageProcessing.Images.BlackWhiteCommand(false);

            test.CalculateLimiar = true;

            NAR.Model.IImage result = test.Execute(base.Model);

            NAR.Model.IImage bitmap = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\Commands\\BlackWhiteCommandWithLimiar.bmp"));

            base.CheckAllBytes(result.Bytes, bitmap.Bytes);

        }


        #endregion
    }
}
