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
    /// Perlin noise function.
    /// </summary>
    /// 
    /// <remarks><para>The class implements 1-D and 2-D Perlin noise functions, which represent
    /// sum of several smooth noise functions with different frequency and amplitude. The description
    /// of Perlin noise function and its calculation may be found on
    /// <a href="http://freespace.virgin.net/hugo.elias/models/m_perlin.htm" target="_blank">Hugo Elias's page</a>.
    /// </para>
    /// 
    /// <para>The number of noise functions, which comprise the resulting Perlin noise function, is
    /// set by <see cref="Octaves"/> property. Amplitude and frequency values for each octave
    /// start from values, which are set by <see cref="InitFrequency"/> and <see cref="InitAmplitude"/>
    /// properties.</para>
    /// 
    /// <para>Sample usage (clouds effect):</para>
    /// <code>
    /// // create Perlin noise function
    /// PerlinNoise noise = new PerlinNoise( 8, 0.5, 1.0 / 32 );
    /// // generate clouds effect
    /// float[,] texture = new float[height, width];
    /// 
    /// for ( int y = 0; y &lt; height; y++ )
    /// {
    /// 	for ( int x = 0; x &lt; width; x++ )
    /// 	{
    /// 		texture[y, x] = 
    /// 			Math.Max( 0.0f, Math.Min( 1.0f,
    /// 				(float) noise.Function2D( x, y ) * 0.5f + 0.5f
    /// 			) );
    /// 	}
    /// }
    /// </code>
    /// </remarks>
    /// 
    public class PerlinNoise
    {
        #region Variables

        private double _initFrequency = 1.0;
        private double _initAmplitude = 1.0;
        private double _persistence = 0.65;
        private int _octaves = 4;

        #endregion

        #region Properties

        /// <summary>
        /// Initial frequency.
        /// </summary>
        /// 
        /// <remarks><para>The property sets initial frequency of the first octave. Frequencies for
        /// next octaves are calculated using the next equation:<br />
        /// frequency<sub>i</sub> = <see cref="InitFrequency"/> * 2<sup>i</sup>,
        /// where i = [0, <see cref="Octaves"/>).
        /// </para>
        /// 
        /// <para>Default value is set to <b>1</b>.</para>
        /// </remarks>
        /// 
        public double InitFrequency
        {
            get { return _initFrequency; }
            set { _initFrequency = value; }
        }

        /// <summary>
        /// Initial amplitude.
        /// </summary>
        /// 
        /// <remarks><para>The property sets initial amplitude of the first octave. Amplitudes for
        /// next octaves are calculated using the next equation:<br />
        /// amplitude<sub>i</sub> = <see cref="InitAmplitude"/> * <see cref="Persistence"/><sup>i</sup>,
        /// where i = [0, <see cref="Octaves"/>).
        /// </para>
        /// 
        /// <para>Default value is set to <b>1</b>.</para>
        /// </remarks>
        ///
        public double InitAmplitude
        {
            get { return _initAmplitude; }
            set { _initAmplitude = value; }
        }

        /// <summary>
        /// Persistence value.
        /// </summary>
        ///
        /// <remarks><para>The property sets so called persistence value, which controls the way
        /// how <see cref="InitAmplitude">amplitude</see> is calculated for each octave comprising
        /// the Perlin noise function.</para>
        /// 
        /// <para>Default value is set to <b>0.65</b>.</para>
        /// </remarks>
        ///
        public double Persistence
        {
            get { return _persistence; }
            set { _persistence = value; }
        }

        /// <summary>
        /// Number of octaves, [1, 32].
        /// </summary>
        /// 
        /// <remarks><para>The property sets the number of noise functions, which sum up the resulting
        /// Perlin noise function.</para>
        /// 
        /// <para>Default value is set to <b>4</b>.</para>
        /// </remarks>
        /// 
        public int Octaves
        {
            get { return _octaves; }
            set { _octaves = System.Math.Max(1, System.Math.Min(32, value)); }
        }

        #endregion

        #region Constructors/Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PerlinNoise"/> class.
        /// </summary>
        /// 
        public PerlinNoise() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerlinNoise"/> class.
        /// </summary>
        /// 
        /// <param name="octaves">Number of octaves (see <see cref="Octaves"/> property).</param>
        /// <param name="persistence">Persistence value (see <see cref="Persistence"/> property).</param>
        /// 
        public PerlinNoise(int octaves, double persistence)
        {
            _octaves = octaves;
            _persistence = persistence;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PerlinNoise"/> class.
        /// </summary>
        /// 
        /// <param name="octaves">Number of octaves (see <see cref="Octaves"/> property).</param>
        /// <param name="persistence">Persistence value (see <see cref="Persistence"/> property).</param>
        /// <param name="initFrequency">Initial frequency (see <see cref="InitFrequency"/> property).</param>
        /// <param name="initAmplitude">Initial amplitude (see <see cref="InitAmplitude"/> property).</param>
        /// 
        public PerlinNoise(int octaves, double persistence, double initFrequency, double initAmplitude)
        {
            _octaves = octaves;
            _persistence = persistence;
            _initFrequency = initFrequency;
            _initAmplitude = initAmplitude;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 1-D Perlin noise function.
        /// </summary>
        /// 
        /// <param name="x">x value.</param>
        /// 
        /// <returns>Returns function's value at point <paramref name="x"/>.</returns>
        /// 
        public double Function(double x)
        {
            double frequency = _initFrequency;
            double amplitude = _initAmplitude;
            double sum = 0;

            // octaves
            for (int i = 0; i < _octaves; i++)
            {
                sum += SmoothedNoise(x * frequency) * amplitude;

                frequency *= 2;
                amplitude *= _persistence;
            }
            return sum;
        }

        /// <summary>
        /// 2-D Perlin noise function.
        /// </summary>
        /// 
        /// <param name="x">x value.</param>
        /// <param name="y">y value.</param>
        /// 
        /// <returns>Returns function's value at point (<paramref name="x"/>, <paramref name="y"/>).</returns>
        /// 
        public double Function2D(double x, double y)
        {
            double frequency = _initFrequency;
            double amplitude = _initAmplitude;
            double sum = 0;

            // octaves
            for (int i = 0; i < _octaves; i++)
            {
                sum += SmoothedNoise(x * frequency, y * frequency) * amplitude;

                frequency *= 2;
                amplitude *= _persistence;
            }
            return sum;
        }


        /// <summary>
        /// Ordinary noise function
        /// </summary>
        private double Noise(int x)
        {
            int n = (x << 13) ^ x;

            return (1.0 - ((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0);
        }
        private double Noise(int x, int y)
        {
            int n = x + y * 57;
            n = (n << 13) ^ n;

            return (1.0 - ((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0);
        }


        /// <summary>
        /// Smoothed noise.
        /// </summary>
        private double SmoothedNoise(double x)
        {
            int xInt = (int)x;
            double xFrac = x - xInt;

            return CosineInterpolate(Noise(xInt), Noise(xInt + 1), xFrac);
        }
        private double SmoothedNoise(double x, double y)
        {
            int xInt = (int)x;
            int yInt = (int)y;
            double xFrac = x - xInt;
            double yFrac = y - yInt;

            // get four noise values
            double x0y0 = Noise(xInt, yInt);
            double x1y0 = Noise(xInt + 1, yInt);
            double x0y1 = Noise(xInt, yInt + 1);
            double x1y1 = Noise(xInt + 1, yInt + 1);

            // x interpolation
            double v1 = CosineInterpolate(x0y0, x1y0, xFrac);
            double v2 = CosineInterpolate(x0y1, x1y1, xFrac);
            // y interpolation
            return CosineInterpolate(v1, v2, yFrac);
        }

        /// <summary>
        /// Cosine interpolation.
        /// </summary>
        private double CosineInterpolate(double x1, double x2, double a)
        {
            double f = (1 - Math.Cos(a * Math.PI)) * 0.5;

            return x1 * (1 - f) + x2 * f;
        }

        #endregion
    }
}
