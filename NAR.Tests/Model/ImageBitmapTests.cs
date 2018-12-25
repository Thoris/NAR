using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NAR.Tests.ImageProcessing;

namespace NAR.Tests.Model
{

    [TestFixture]
    public class ImageBitmapTests : BaseCommand
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
        public void TestBytesFromConstructorByImage()
        {
            NAR.Model.IImage image = new NAR.Model.ImageBitmap(base.Original);

            byte[] result = image.Bytes;

            int totalBytes = base.Original.Width * base.Original.Height * 3;

            Assert.IsNotNull(result);
            Assert.AreEqual(totalBytes, result.Length);


        }
        [Test]
        public void TestImageFromConstructorByByteArray()
        {
            int width = base.Original.Width;
            int height = base.Original.Height;

            byte [] newImage = new byte [width * height * 3];

            NAR.Model.IImage image = new NAR.Model.ImageBitmap(width, height, newImage);

            Assert.IsNotNull(image.Image);
            Assert.AreEqual(width, image.Image.Width);
            Assert.AreEqual(height, image.Image.Height);

        }
        [Test]
        public void TestImageAndByteArrayFromConstryctorByImageAndByteArray()
        {

            int width = base.Original.Width;
            int height = base.Original.Height;


            byte[] newImage = new byte[width * height * 3];


            NAR.Model.IImage image = new NAR.Model.ImageBitmap(base.Original, newImage);

            Assert.IsNotNull(image.Image);
            Assert.IsNotNull(image.Bytes);

            Assert.AreEqual(width * height * 3, image.Bytes.Length);
            Assert.AreEqual(width, image.Image.Width);
            Assert.AreEqual(height, image.Image.Height);


        }
        
        #endregion
    }
}
