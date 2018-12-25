using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ImageProcessing.CameraCalibration.Tsai.Math
{
    public class Compute
    {
        #region Methods

        public double[] Compute_U(int numberPoints, double[] xw, double[] yw, double[] xf, double[] yf)
        {
            int i;
            double[] U = new double[5];

            double[,] A = new double[numberPoints, 5];
            double[] B = new double[numberPoints];

            for (i = 0; i < numberPoints; i++)
            {
                A[i, 0] = yf[i] * xw[i];
                A[i, 1] = yf[i] * yw[i];
                A[i, 2] = yf[i];
                A[i, 3] = -xf[i] * xw[i];
                A[i, 4] = -xf[i] * yw[i];
                B[i] = xf[i];
            }

            U = Math.Quadratic.Calcule(numberPoints, 5, A, B);

            return U;
        }
        public double[] Compute_Tx_And_Ty(int np, double[] xw, double[] yw, double[] xf, double[] yf, double [] U)
        {

            double Ty_squared, Sr, distance, far_distance;
            double Tx, Ty, r1, r2, r4, r5, x, y;
            int i, far_point;

            double r1p = U[0],
                   r2p = U[1],
                   r4p = U[3],
                   r5p = U[4];

            double[] T = new double[3];


            if ((System.Math.Abs(r1p) < Util.EPSILON) && (System.Math.Abs(r2p) < Util.EPSILON))
                Ty_squared = 1 / (Util.SQR(r4p) + Util.SQR(r5p));
            else if ((System.Math.Abs(r4p) < Util.EPSILON) && (System.Math.Abs(r5p) < Util.EPSILON))
                Ty_squared = 1 / (Util.SQR(r1p) + Util.SQR(r2p));
            else if ((System.Math.Abs(r1p) < Util.EPSILON) && (System.Math.Abs(r4p) < Util.EPSILON))
                Ty_squared = 1 / (Util.SQR(r2p) + Util.SQR(r5p));
            else if ((System.Math.Abs(r2p) < Util.EPSILON) && (System.Math.Abs(r5p) < Util.EPSILON))
                Ty_squared = 1 / (Util.SQR(r1p) + Util.SQR(r4p));
            else
            {
                Sr = Util.SQR(r1p) + Util.SQR(r2p) + Util.SQR(r4p) + Util.SQR(r5p);
                Ty_squared = (Sr - System.Math.Sqrt(Util.SQR(Sr) - 4 * Util.SQR(r1p * r5p - r4p * r2p))) /
                             (2 * Util.SQR(r1p * r5p - r4p * r2p));
            }

            far_distance = 0;
            far_point = 0;
            for (i = 0; i < np; i++)
            {
                if ((distance = Util.SQR(xf[i]) + Util.SQR(yf[i])) > far_distance)
                {
                    far_point = i;
                    far_distance = distance;
                }
            }

            Ty = System.Math.Sqrt(Ty_squared);
            r1 = U[0] * Ty;
            r2 = U[1] * Ty;
            Tx = U[2] * Ty;
            r4 = U[3] * Ty;
            r5 = U[4] * Ty;
            x = r1 * xw[far_point] + r2 * yw[far_point] + Tx;
            y = r4 * xw[far_point] + r5 * yw[far_point] + Ty;

            if ((Util.SIGNBIT(x) != Util.SIGNBIT(xf[far_point])) || (Util.SIGNBIT(y) != Util.SIGNBIT(yf[far_point])))
                Ty = -Ty;

            T[0] = U[2] * Ty;
            T[1] = Ty;

            return T;
        }
        public double[] Compute_R(double[] T, double[] U)
        {
            double[] r = new double[9];

            r[0] = U[0] * T[1];
            r[1] = U[1] * T[1];
            r[2] = System.Math.Sqrt(1 - Util.SQR(r[0]) - Util.SQR(r[1]));

            r[3] = U[3] * T[1];
            r[4] = U[4] * T[1];
            r[5] = System.Math.Sqrt(1 - Util.SQR(r[3]) - Util.SQR(r[4]));
            if (!Util.SIGNBIT(r[0] * r[3] + r[1] * r[4]))
                r[5] = -r[5];

            r[6] = r[1] * r[5] - r[2] * r[4];
            r[7] = r[2] * r[3] - r[0] * r[5];
            r[8] = r[0] * r[4] - r[1] * r[3];

            return r;
        }
        public double [] Compute_F_And_Tz(int np, double[] xw, double[] yw, double[] xf, double[] yf, ref double[] T, ref double[] r)
        {
            double [] f = null;

            double[,] A = new double[Util.MAX_POINTS, 5];
            double[] B = new double[Util.MAX_POINTS];
            int i;

            for (i = 0; i < np; i++)
            {
                A[i, 0] = r[0] * xw[i] + r[1] * yw[i] + T[0];
                A[i, 1] = -xf[i];
                B[i] = (r[6] * xw[i] + r[7] * yw[i]) * xf[i];
                A[i + np, 0] = r[3] * xw[i] + r[4] * yw[i] + T[1];
                A[i + np, 1] = -yf[i];
                B[i + np] = (r[6] * xw[i] + r[7] * yw[i]) * yf[i];
            }

            //if (!MinQuad(2 * np, 2, A, ref B))
            //    return false;

            double [] U = Quadratic.Calcule(2 * np, 2, A, B);

            if (U == null)
                return null;


            //*f = B[0];
            f = new double[U.Length];

            for (int c = 0; c < U.Length; c++)
                f[c] = U[c];


            T[2] = U[1];

            return f;

        }

        public int Compute_Tz(int np, double[] xw, double[] yw, double[] xf, double[] yf, double f, double[] T, double[] r)
        {
            double[,] A = new double[np, 5];
            double[] B = new double[np];
            int i;

            for (i = 0; i < np; i++)
            {
                A[i, 0] = -xf[i];
                B[i] = (r[6] * xw[i] + r[7] * yw[i]) * xf[i] - f * (r[0] * xw[i] + r[1] * yw[i] + T[0]);
                A[i + np, 0] = -yf[i];
                B[i + np] = (r[6] * xw[i] + r[7] * yw[i]) * yf[i] - f * (r[3] * xw[i] + r[4] * yw[i] + T[1]);
            }

            double [] result = Math.Quadratic.Calcule(2 * np, 1, A, B);

            if (result == null)
                return 0;

            T[2] = result[0];

            return 1;
        }
        
        #endregion
    }
}
