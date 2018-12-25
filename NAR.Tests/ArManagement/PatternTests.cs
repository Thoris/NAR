using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NAR.Tests.ArManagement
{
    [TestFixture]
    public class PatternTests
    {
        #region Constants
        public const string DefaultFileName = @".\Resources\Data\patt.hiro";
        #endregion

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
        public void TestLoadFile()
        {
            NAR.ArManagement.Pattern pattern = new NAR.ArManagement.Pattern();
            int result = pattern.Load(DefaultFileName);


            //NAR.Model.IImage image = new NAR.Model.ImageBitmap(base.Original);

            //byte[] result = image.Bytes;

            //int totalBytes = base.Original.Width * base.Original.Height * 3;

            //Assert.IsNotNull(result);
            //Assert.AreEqual(totalBytes, result.Length);


        }

        #endregion
    }
}
