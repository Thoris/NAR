using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ImageProcessing.CameraCalibration.Tsai.Math
{
    public class Quadratic
    {
        #region Methods

        public static double[] Calcule(int numberPoints, int numberEquations, double[,] A, double[] B)
        {
            double[,] M = new double[5, 5];
            double[] X = new double[5];


            int i, j, k;

            for (i = 0; i < numberEquations; i++)
            {
                for (j = 0; j < numberEquations; j++)
                {
                    M[i, j] = A[0, j] * A[0, i];

                    for (k = 1; k < numberPoints; k++)                    
                        M[i, j] += A[k, j] * A[k, i];                    
                }
            }

            for (i = 0; i < numberEquations; i++)
            {
                X[i] = A[0, i] * B[0];

                for (k = 1; k < numberPoints; k++)
                    X[i] += A[k, i] * B[k];            
            }


            int result = Math.Gauss.Calcule(numberEquations, M, ref X);

            if (result != 0)
                return null;


            return X;
        }

        #endregion
    }
}
