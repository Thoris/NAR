using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ImageProcessing.CameraCalibration.Tsai.Math
{
    public class Util
    { 
        #region Constants

        public const int MAX_POINTS = 128;
        public const double EPSILON = 1.0E-8;
        public const int MAXDIM = 9 + MAX_POINTS;
        public const double TOL = 1e-10;

        #endregion


        #region Methods

        public static double SQR(double x)
        {
            return x * x;
        }
        public static bool SIGNBIT(double a)
        {
            if (a > 0)
                return false;
            else
                return true;
        }
        public static void SWAP(ref double a, ref double b)
        {
            double temp = (a);
            (a) = (b);
            (b) = temp;

        }

        #endregion
    }
}
