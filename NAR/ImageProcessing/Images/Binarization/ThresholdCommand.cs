using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ImageProcessing.Images.Binarization
{
    public class ThresholdCommand : IBinarization, ICommand
    {
        #region Variables
        /// <summary>
        /// Threshold value.
        /// </summary>
        protected int _threshold = 128;
        protected bool _grayscale = false;
        #endregion

        #region Constructors/Destructors
        public ThresholdCommand(int threshold, bool grayscale)
        {
            this._threshold = threshold;
            this._grayscale = grayscale;
        }
        #endregion

        #region ICommand members
        public Model.IImage Execute(Model.IImage image)
        {
            int width = image.Image.Width;
            int height = image.Image.Height;
            int size = width * height * 3;
            int limiar = _threshold;

            //Creating the size of bytes similar to the image considering 3 bytes of each pixel (RGB)
            byte[] result = new byte[size];

            Array.Copy(image.Bytes, result, size);


            //Foreach byte found in the image
            for (int c = 0; c < result.Length; c += 3)
            {
                if ((result[c] + result[c + 1] + result[c + 2]) / 3 > limiar)
                {
                    result[c] = 255;
                    result[c + 1] = 255;
                    result[c + 2] = 255;
                }
                else
                {
                    result[c] = 0;
                    result[c + 1] = 0;
                    result[c + 2] = 0;
                }

            }//end for c

            Model.IImage imageResult = new Model.ImageBitmap(width, height, result);

            return imageResult;
        }
        #endregion
    }
}
