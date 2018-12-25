using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ImageProcessing.Images.Binarization
{
    public abstract class ErrorDiffusionDitheringCommand
    {
        #region Variables
        private byte _threshold = 128;
        /// <summary>
        /// Current processing X coordinate.
        /// </summary>
        protected int _x;

        /// <summary>
        /// Current processing Y coordinate.
        /// </summary>
        protected int _y;
        #endregion

        #region Properties
        /// <summary>
        /// Threshold value.
        /// </summary>
        /// 
        /// <remarks>Default value is 128.</remarks>
        /// 
        public byte ThresholdValue
        {
            get { return _threshold; }
            set { _threshold = value; }
        }
        #endregion

        #region Constructors/Destructors
        public ErrorDiffusionDitheringCommand()
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Do error diffusion.
        /// </summary>
        /// 
        /// <param name="error">Current error value.</param>
        /// <param name="ptr">Pointer to current processing pixel.</param>
        /// 
        /// <remarks>All parameters of the image and current processing pixel's coordinates
        /// are initialized in protected members.</remarks>
        /// 
        protected abstract void Diffuse(int error, byte [] result, int pos, int width, int height);
        #endregion

        #region ICommand members
        public Model.IImage Execute(Model.IImage image)
        {
            int height = image.Image.Height;
            int width = image.Image.Width;
            int size = width * height * 3;
            int stride = width * 3;

            //Creating the size of bytes similar to the image considering 3 bytes of each pixel (RGB)
            byte[] result = new byte[size];

            Array.Copy(image.Bytes, result, size);
            
            int pos = 0;


            // pixel value and error value
            int v, error;


            //For each line
            for (_y = 0; _y < height; _y++)
            {
                //For each pixel
                for (_x = 0; _x < width; _x++)
                {
                    v = result[pos];

                    // fill the next destination pixel
                    if (v >= _threshold)
                    {
                        result[pos] = 255;
                        result[pos + 1] = 255;
                        result[pos + 2] = 255;
                        error = v - 255;
                    }
                    else
                    {
                        result[pos] = 0;
                        result[pos + 1] = 0;
                        result[pos + 2] = 0;
                        error = v;
                    }

                    Diffuse(error, result, pos, width, height);

                    pos += 3;
                }

                //pos += stride;


            }//end for line

            return new Model.ImageBitmap(width, height, result);

        }
        #endregion
    }
}
