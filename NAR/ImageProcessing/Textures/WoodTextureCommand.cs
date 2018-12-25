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
    /// Wood texture.
    /// </summary>
    /// 
    /// <remarks><para>The texture generator creates textures with effect of
    /// rings on trunk's shear. The <see cref="Rings"/> property allows to specify the
    /// desired amount of wood rings.</para>
    /// 
    /// <para>The generator is based on the <see cref="AForge.Math.PerlinNoise">Perlin noise function</see>.</para>
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
    ///
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/wood_texture.jpg" width="320" height="240" />
    /// </remarks>
    /// 
    public class WoodTextureCommand : RandomTextureBase,  ICommand, ITextureGenerator
    {
        #region Variables

        // Perlin noise function used for texture generation
        private PerlinNoise _noise = new PerlinNoise(8, 0.5, 1.0 / 32, 0.05);
        // rings amount
        private double _rings = 12;

        #endregion

        #region Properties

        /// <summary>
        /// Wood rings amount, ≥ 3.
        /// </summary>
        /// 
        /// <remarks><para>The property sets the amount of wood rings, which make effect of
        /// rings on trunk's shear.</para>
        /// 
        /// <para>Default value is set to <b>12</b>.</para></remarks>
        /// 
        public double Rings
        {
            get { return _rings; }
            set { _rings = Math.Max(3, value); }
        }

        #endregion

        #region Constructors/Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WoodTexture"/> class.
        /// </summary>
        /// 
        /// <param name="rings">Wood rings amount.</param>
        /// 
        public WoodTextureCommand( double rings, bool reset )
            : base(reset)
        {
            _rings = rings;
            
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
            int w2 = width / 2;
            int h2 = height / 2;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double xv = (double)(x - w2) / width;
                    double yv = (double)(y - h2) / height;

                    texture[y, x] =
                        Math.Min(1.0f, (float)
                        Math.Abs(Math.Sin(
                            (Math.Sqrt(xv * xv + yv * yv) + _noise.Function2D(x + _r, y + _r))
                                * Math.PI * 2 * _rings
                        ))
                        );
                }
            }
            return texture;
        }

        #endregion
    
    }
}
