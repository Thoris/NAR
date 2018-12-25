using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ImageProcessing.CameraCalibration.Tsai
{
    public class TsaiCameraCalibration
    {
        #region Variables
        private Math.Compute _compute;
        private Parameters.Extrinsic _extrinsic;
        private Parameters.Intrinsic _intrinsic;
        #endregion

        #region Properties
        #endregion

        #region Constructors/Destructors
        public TsaiCameraCalibration()
        {
            _compute = new Math.Compute();
            _extrinsic = new Parameters.Extrinsic();
            _intrinsic = new Parameters.Intrinsic();
        }
        #endregion

        #region Methods
        public void Calibrate(int numberPoints, double[] xw, double[] yw, double[] xf, double[] yf)
        {
            double[] U = _compute.Compute_U(numberPoints, xw, yw, xf, yf);
            double[] T = _compute.Compute_Tx_And_Ty(numberPoints, xw, yw, xf, yf, U);
            double[] r = _compute.Compute_R(T, U);
            double[] f = _compute.Compute_F_And_Tz(numberPoints, xw, yw, xf, yf, ref T, ref r);

            if (f[0] < 0)
            {
                r[2] = -r[2];
                r[5] = -r[5];
                r[6] = -r[6];
                r[7] = -r[7];
                f = _compute.Compute_F_And_Tz(numberPoints, xw, yw, xf, yf, ref T, ref r);
            }


            //_intrinsic.F



            Coordinate3d cam =  CameraPosition(T, r);

            if (cam.x == 0)
            {
            }


        }
        public Coordinate2d Project(double xw, double yw, double zw, double f, double [] T, double [] r)
        {
            double xc = r[0] * xw + r[1] * yw + r[2] * zw + T[0];
            double yc = r[3] * xw + r[4] * yw + r[5] * zw + T[1];
            double zc = r[6] * xw + r[7] * yw + r[8] * zw + T[2];

            Coordinate2d result = new Coordinate2d();
            result.x = f * xc / zc;
            result.y = f * yc / zc;

            return result;

        }
        public Coordinate3d UnProject(double xf, double yf, double zw, double f, double[] T, double[] r)
        {
            double common_denominator;
            Coordinate3d result = new Coordinate3d();


            common_denominator = ((r[0] * r[7] - r[1] * r[6]) * yf + (r[4] * r[6] - r[3] * r[7]) * xf - f * r[0] * r[4] + f * r[1] * r[3]);

            result.x = (((r[1] * r[8] - r[2] * r[7]) * yf + (r[5] * r[7] - r[4] * r[8]) * xf - f * r[1] * r[5] + f * r[2] * r[4]) * zw +
                   (r[1] * T[2] - r[7] * T[0]) * yf + (r[7] * T[1] - r[4] * T[2]) * xf - f * r[1] * T[1] + f * r[4] * T[0]) / common_denominator;

            result.y = -(((r[0] * r[8] - r[2] * r[6]) * yf + (r[5] * r[6] - r[3] * r[8]) * xf - f * r[0] * r[5] + f * r[2] * r[3]) * zw +
                    (r[0] * T[2] - r[6] * T[0]) * yf + (r[6] * T[1] - r[3] * T[2]) * xf - f * r[0] * T[1] + f * r[3] * T[0]) / common_denominator;


            
            return result;
        }


        public Coordinate3d CameraPosition(double[] T, double[] r)
        {
            Coordinate3d result = new Coordinate3d();


            double[] X = new double[3];
            double[,] M = new double[3, 3];

            M[0, 0] = r[0]; M[0, 1] = r[1]; M[0, 2] = r[2];
            M[1, 0] = r[3]; M[1, 1] = r[4]; M[1, 2] = r[5];
            M[2, 0] = r[6]; M[2, 1] = r[7]; M[2, 2] = r[8];

            X[0] = -T[0];
            X[1] = -T[1];
            X[2] = -T[2];

            Math.Gauss.Calcule(3, M, ref X);


            result.x = X[0];
            result.y = X[1];
            result.z = -X[2];


            return result;
        }
        #endregion

    }
}
