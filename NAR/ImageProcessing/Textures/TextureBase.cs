/******************************************************************************
* Copyright (c) 2012, TAP Consulting Group
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither TAP Consulting Group nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY TAP Consulting Group ''AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL TAP Consulting Group BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace NAR.ImageProcessing.Textures
{
    /// <summary>
    /// Texture tools.
    /// </summary>
    /// 
    /// <remarks><para>The class represents collection of different texture tools, like
    /// converting a texture to/from grayscale image.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create texture generator
    /// WoodTexture textureGenerator = new WoodTexture( );
    /// // generate new texture
    /// float[,] texture = textureGenerator.Generate( 320, 240 );
    /// // convert it to image to visualize
    /// Bitmap textureImage = TextureTools.ToBitmap( texture );
    /// </code>
    /// </remarks>
    /// 
    public class TextureBase 
    {
        #region Methods
        /// <summary>
        /// Convert texture to grayscale bitmap.
        /// </summary>
        /// 
        /// <param name="texture">Texture to convert to bitmap.</param>
        /// 
        /// <returns>Returns bitmap of the texture.</returns>
        /// 
        public Model.IImage ToBitmap(float[,] texture)
        {
            // get texture dimension
            int width = texture.GetLength(1);
            int height = texture.GetLength(0);
            int size = width * height * 3;

            

            byte[] result = new byte[size];


            int pos = 0;



            // for each line
            for (int y = 0; y < height; y++)
            {
                // for each pixel
                for (int x = 0; x < width; x++)
                {
                    byte resultByte = (byte)(texture[y, x] * 255.0f);

                    result[pos + Model.ImageBitmap.COLOR_RED] = resultByte;
                    result[pos + Model.ImageBitmap.COLOR_GREEN] = resultByte;
                    result[pos + Model.ImageBitmap.COLOR_BLUE] = resultByte;
                    
                    pos += 3;
                }
            }
            
            return new Model.ImageBitmap (width, height, result);
        }

        ///// <summary>
        ///// Convert grayscale bitmap to texture.
        ///// </summary>
        ///// 
        ///// <param name="image">Image to convert to texture.</param>
        ///// 
        ///// <returns>Returns texture as 2D float array.</returns>
        ///// 
        ///// <exception cref="UnsupportedImageFormatException">Only grayscale (8 bpp indexed images) are supported.</exception>
        ///// 
        //public static float[,] FromBitmap(Bitmap image)
        //{
        //    // lock source bitmap data
        //    BitmapData imageData = image.LockBits(
        //        new Rectangle(0, 0, image.Width, image.Height),
        //        ImageLockMode.ReadOnly, image.PixelFormat);

        //    // process the image
        //    float[,] texture = FromBitmap(imageData);

        //    // unlock source image
        //    image.UnlockBits(imageData);

        //    return texture;
        //}

        ///// <summary>
        ///// Convert grayscale bitmap to texture
        ///// </summary>
        ///// 
        ///// <param name="imageData">Image data to convert to texture</param>
        ///// 
        ///// <returns>Returns texture as 2D float array.</returns>
        ///// 
        ///// <exception cref="UnsupportedImageFormatException">Only grayscale (8 bpp indexed images) are supported.</exception>
        ///// 
        //public static float[,] FromBitmap(BitmapData imageData)
        //{
        //    return FromBitmap(new Model.ImageBitmap(imageData));
        //}

        /// <summary>
        /// Convert grayscale bitmap to texture.
        /// </summary>
        /// 
        /// <param name="image">Image data to convert to texture.</param>
        /// 
        /// <returns>Returns texture as 2D float array.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Only grayscale (8 bpp indexed images) are supported.</exception>
        /// 
        public static float[,] FromBitmap(Model.IImage image)
        {


            // get source image dimension
            int width = image.Image.Width;
            int height = image.Image.Height;

            // create texture array
            float[,] texture = new float[height, width];




            int pos = 0;

            // for each line
            for (int y = 0; y < height; y++)
            {
                // for each pixel
                for (int x = 0; x < width; x++)
                {
                    texture[y, x] = (float)image.Bytes[pos] / 255.0f;
                    pos += 3;
                }
            }


            return texture;
        }
        #endregion
    }
}
