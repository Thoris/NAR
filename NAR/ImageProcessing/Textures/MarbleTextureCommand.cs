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

namespace NAR.ImageProcessing.Textures
{

    /// <summary>
    /// Marble texture.
    /// </summary>
    /// 
    /// <remarks><para>The texture generator creates textures with effect of marble.
    /// The <see cref="XPeriod"/> and <see cref="YPeriod"/> properties allow to control the look
    /// of marble texture in X/Y directions.</para>
    /// 
    /// <para>The generator is based on the <see cref="AForge.Math.PerlinNoise">Perlin noise function</see>.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create texture generator
    /// MarbleTexture textureGenerator = new MarbleTexture( );
    /// // generate new texture
    /// float[,] texture = textureGenerator.Generate( 320, 240 );
    /// // convert it to image to visualize
    /// Bitmap textureImage = TextureTools.ToBitmap( texture );
    /// </code>
    ///
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/marble_texture.jpg" width="320" height="240" />
    /// </remarks>
    /// 
    public class MarbleTextureCommand : RandomTextureBase, ICommand, ITextureGenerator
    {

        #region Variables

        private double _xPeriod = 5.0;
        private double _yPeriod = 10.0;

        // Perlin noise function used for texture generation
        private PerlinNoise _noise = new PerlinNoise(2, 0.65, 1.0 / 32, 1.0);


        #endregion

        #region Properties
        /// <summary>
        /// X period value, ≥ 2.
        /// </summary>
        /// 
        /// <remarks>Default value is set to <b>5</b>.</remarks>
        /// 
        public double XPeriod
        {
            get { return _xPeriod; }
            set { _xPeriod = Math.Max(2.0, value); }
        }

        /// <summary>
        /// Y period value, ≥ 2.
        /// </summary>
        /// 
        /// <remarks>Default value is set to <b>10</b>.</remarks>
        /// 
        public double YPeriod
        {
            get { return _yPeriod; }
            set { _yPeriod = Math.Max(2.0, value); }
        }
        #endregion

        #region Constructors/Destructors

        public MarbleTextureCommand(double xPeriod, double yPeriod, bool reset)
            : base(reset)
        {
        }

        #endregion

        #region ICommand Members

        public Model.IImage Execute(Model.IImage image)
        {

            return base.ToBitmap(Generate(image.Image.Width, image.Image.Height));
        }

        #endregion

        #region ITextureGenerator

        /// <summary>
        /// Generate texture.
        /// </summary>
        /// 
        /// <param name="width">Texture's width.</param>
        /// <param name="height">Texture's height.</param>
        /// 
        /// <returns>Two dimensional array of intensities.</returns>
        /// 
        /// <remarks>Generates new texture of the specified size.</remarks>
        /// 
        public float[,] Generate(int width, int height)
        {
            float[,] texture = new float[height, width];
            double xFact = _xPeriod / width;
            double yFact = _yPeriod / height;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    texture[y, x] =
                        Math.Min(1.0f, (float)
                            Math.Abs(Math.Sin(
                                (x * xFact + y * yFact + _noise.Function2D(x + _r, y + _r)) * Math.PI
                            ))
                        );

                }
            }
            return texture;
        }

        #endregion
    }
}
