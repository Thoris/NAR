using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NAR.Tests.ImageProcessing.Borders
{
    [TestFixture]
    public class MaskCommandTests : BaseCommand
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

            NAR.ImageProcessing.Borders.MaskData mask = new NAR.ImageProcessing.Borders.MaskData(3,3);

            NAR.ImageProcessing.Borders.MaskCommand test = new NAR.ImageProcessing.Borders.MaskCommand(mask);

            


            //NAR.Model.IImage result = test.Execute(base.Model);

            //NAR.Model.IImage bitmap = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\Commands\\BlackWhiteCommand.bmp"));

            //base.CheckAllBytes(result.Bytes, bitmap.Bytes);

        }

        #endregion
    }
}
