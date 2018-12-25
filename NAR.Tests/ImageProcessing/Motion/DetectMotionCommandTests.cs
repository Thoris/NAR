using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NAR.Tests.ImageProcessing.Motion
{
    [TestFixture]
    public class DetectMotionCommandTests : BaseCommand
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
            int resX = 256;
            int resY = 256;
            byte relevance = 10;
            int qtdPixel = 90;
            bool showInImage = true;


            NAR.ImageProcessing.Motion.DetectMotionCommand test = new NAR.ImageProcessing.Motion.DetectMotionCommand(
                resX, resY, relevance, qtdPixel, showInImage);

            NAR.Model.IImage result = test.Execute(base.Model);

            result = test.Execute(base.Motion);

            NAR.Model.IImage bitmap = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\Commands\\DetectMotionCommand.bmp"));

            base.CheckAllBytes(result.Bytes, bitmap.Bytes);
        }
        #endregion
    }
}
