using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NAR.Tests.ImageProcessing.Distance
{
    [TestFixture]
    public class CalcDistanceCommandTests : BaseCommand
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
        public void TestCalcule()
        {


            NAR.ImageProcessing.Distance.CalcDistanceCommand test =
                new NAR.ImageProcessing.Distance.CalcDistanceCommand();

            NAR.Model.IImage result = test.Execute(base.Model);

            Assert.IsNotNull(result);

            //result.Image.Save("C:\\temp\\cap.bmp");


            //NAR.Model.IImage result = test.Execute(base.Model);

            NAR.Model.IImage bitmap = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\Commands\\CalcDistanceCommand.bmp"));

            base.CheckAllBytes(result.Bytes, bitmap.Bytes);
        }
        #endregion
    }
}
