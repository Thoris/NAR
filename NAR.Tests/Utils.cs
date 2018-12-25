using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace NAR.Tests
{
    public class Utils
    {
        #region Constants

        public const string ImageFile = ".\\Resources\\Sample.bmp";
        public const string ImageFile2 = ".\\Resources\\Sample2.bmp";
        public const string ImageMotion = ".\\Resources\\SampleMotion.bmp";
        public const string ImagePen = ".\\Resources\\ImagePen.bmp";

        #endregion

        #region Constructors/Destructors

        public Utils()
        {
        }

        #endregion

        #region Methods

        public Bitmap ReadSampleImage(string filePath)
        {
            Bitmap image = (Bitmap)Bitmap.FromFile(filePath, false);           

            return image;
        }
        public Bitmap ReadSampleImage()
        {
            return ReadSampleImage(ImageFile);
        }
        public Bitmap ReadSampleImage2()
        {
            return ReadSampleImage(ImageFile2);
        }
        public Bitmap ReadSampleMotion()
        {
            return ReadSampleImage(ImageMotion);
        }
        public Bitmap ReadSamplePen()
        {
            return ReadSampleImage(ImagePen);
        }

        #endregion
    }
}
