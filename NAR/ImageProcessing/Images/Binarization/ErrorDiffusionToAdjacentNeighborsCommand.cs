using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ImageProcessing.Images.Binarization
{
    public class ErrorDiffusionToAdjacentNeighborsCommand : ErrorDiffusionDitheringCommand, IBinarization, ICommand
    {
        #region Variables
        // diffusion coefficients
        private int[][] _coefficients;
        // sum of all coefficients
        private int _coefficientsSum;

        #endregion

        #region Properties
        /// <summary>
        /// Diffusion coefficients.
        /// </summary>
        /// 
        /// <remarks>Set of coefficients, which are used for error diffusion to
        /// pixel's neighbors.</remarks>
        /// 
        public int[][] Coefficients
        {
            get { return _coefficients; }
            set
            {
                _coefficients = value;
                CalculateCoefficientsSum();
            }
        }
        #endregion

        #region Constructors/Destructors
        public ErrorDiffusionToAdjacentNeighborsCommand(int[][] coefficients)
        {
            this._coefficients = coefficients;
            CalculateCoefficientsSum();

        }
        #endregion

        #region Methods
        private void CalculateCoefficientsSum()
        {
            _coefficientsSum = 0;

            for (int i = 0, n = _coefficients.Length; i < n; i++)
            {
                int[] coefficientsRow = _coefficients[i];

                for (int j = 0, k = coefficientsRow.Length; j < k; j++)
                {
                    _coefficientsSum += coefficientsRow[j];
                }
            }
        }
        #endregion

        #region ICommand members
        public new Model.IImage Execute(Model.IImage image)
        {
            return base.Execute(image);
        }
        #endregion

        #region ErrorDiffusionDitheringCommand members
        protected override void Diffuse(int error, byte[] result, int pos, int width, int height)
        {
            int stride = width * 3;

            int ed;	// error diffusion

            // do error diffusion to right-standing neighbors
            int[] coefficientsRow = _coefficients[0];

            int newPos = 0;

            for (int jI = 1, jC = 0, k = coefficientsRow.Length; jC < k; jI++, jC++)
            {
                if (_x + jI >= width)
                    break;

                newPos = jI * 3;

                ed = result[newPos] + (error * coefficientsRow[jC]) / _coefficientsSum;
                ed = (ed < 0) ? 0 : ((ed > 255) ? 255 : ed);
                result[newPos] = (byte)ed;
                result[newPos + 1] = result[newPos];
                result[newPos + 2] = result[newPos];
            }

            // do error diffusion to bottom neigbors
            for (int i = 1, n = _coefficients.Length; i < n; i++)
            {
                if (_y + i >= height)
                    break;

                // move pointer to next image line
                pos += stride;

                // get coefficients of the row
                coefficientsRow = _coefficients[i];

                // process the row
                for (int jC = 0, k = coefficientsRow.Length, jI = -(k >> 1); jC < k; jI++, jC++)
                {
                    if (_x + jI >= width)
                        break;
                    if (_x + jI < 0)
                        continue;

                    newPos = pos + (jI * 3);


                    ed = result[newPos] + (error * coefficientsRow[jC]) / _coefficientsSum;
                    ed = (ed < 0) ? 0 : ((ed > 255) ? 255 : ed);
                    result[newPos] = (byte)ed;
                    result[newPos + 1] = result[newPos];
                    result[newPos + 2] = result[newPos];
                }
            }
        }
        #endregion
    }
}
