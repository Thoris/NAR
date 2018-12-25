using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using NUnit.Framework;

namespace NAR.Tests.ImageProcessing
{
    public class BaseCommand
    {
        #region Variables

        private Bitmap _originalImage;
        private Utils _util;
        private NAR.Model.IImage _model;
        private NAR.Model.IImage _model2;
        private NAR.Model.IImage _motion;		
        private NAR.Model.IImage _pen;

        #endregion

        #region Properties

        protected Bitmap Original
        {
            get { return _originalImage; }
        }
        protected NAR.Model.IImage Model
        {
            get { return _model; }
        }
        protected NAR.Model.IImage Model2
        {
            get { return _model2; }
        }
        protected NAR.Model.IImage Motion
        {
            get { return _motion; }
        }		
        protected NAR.Model.IImage Pen
        {
            get { return _pen; }
        }

        #endregion

        #region Constructors/Destructors

        public BaseCommand()
        {
            _util = new Utils();
            ReadSample();
            ReadSample2();
            ReadMotion();
            ReadSamplePen();
            _model = ConvertToModel(_originalImage);
        }

        #endregion

        #region Methods

        protected void ReadSample()
        {
            _originalImage = _util.ReadSampleImage();
        }
        protected void ReadSample2()
        {
            Bitmap model2 = _util.ReadSampleImage2();

            _model2 = ConvertToModel(model2);
        }
        protected void ReadMotion()
        {
            Bitmap motion = _util.ReadSampleMotion();

            _motion = ConvertToModel(motion);
        }
        protected void ReadSamplePen()
        {
            Bitmap pen = _util.ReadSamplePen();

            _pen = ConvertToModel(pen);
        }
        protected Bitmap ReadBitmap(string filePath)
        {
            return _util.ReadSampleImage(filePath);
        }
        protected NAR.Model.IImage ConvertToModel(Bitmap image)
        {
            NAR.Model.IImage result = new NAR.Model.ImageBitmap(image);

            //result.FillProperties();

            return result;
        }
        protected void CheckAllBytes(byte[] image, byte[] compared)
        {

            Assert.IsNotNull(image);
            Assert.IsNotNull(compared);
            Assert.Greater(image.Length, 0);
            Assert.Greater(compared.Length, 0);
            Assert.AreEqual(image.Length, compared.Length);

            for (int c = 0; c < image.Length; c++)
            {
                Assert.AreEqual(image[c], compared[c]);
            }
        }

        #endregion
    }
}
