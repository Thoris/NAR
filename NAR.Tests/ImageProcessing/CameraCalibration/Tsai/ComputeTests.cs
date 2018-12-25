using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NAR.Tests.ImageProcessing.CameraCalibration.Tsai
{
    [TestFixture]
    public class ComputeTests : BaseCommand
    {
        #region Constructors/Destructors

        [TestFixtureSetUp]
        public void Init()
        {
        }
        [TearDown]
        public void Cleanup()
        {
        }

        #endregion

        #region Methods
        [Test]
        public void TestCalibration()
        {
            
            int np = 8;

            double[] xw = new double[np];
            double[] yw = new double[np];

            xw[0] = 29.5; xw[1] = -30.5; xw[2] = -30.5; xw[3] = 29.5; xw[4] = 10.5; xw[5] = -19.5; xw[6] = -19.5; xw[7] = 10.5;
            yw[0] = -34.5; yw[1] = 35.5; yw[2] = -34.5; yw[3] = 35.5; yw[4] = -22.5; yw[5] = 12.5; yw[6] = -22.5; yw[7] = 12.5;


            int[,] VectorPoints = new int[np, 2];

            int count = 0;
            //for (int x = 0; x < 9; x++)
            //{
            //    for (int y = 0; y < 9; y++)
            //    {
            //        if (x % 2  && y % 2)
            //        {
            //            //GEOM_line_intersection(p, &lv[x], &lh[y]);
            //            xi[count] = p[0];
            //            yi[count] = p[1];
            //            whichx[count] = x;
            //            whichy[count] = y;
            //            count++;
            //        }
            //    }
            //}

            double[] xf = new double[np];
            double[] yf = new double[np];

            double [] T = new double[3];
            double [] r = new double[9];
            double[] focal = null;

            xw[0] = 10; xw[1] = 50; xw[2] = 10; xw[3] = 50; xw[4] = 1; xw[5] = 60; xw[6] = 1; xw[7] = 60;
            yw[0] = 10; yw[1] = 50; yw[2] = 50; yw[3] = 10; yw[4] = 1; yw[5] = 60; yw[6] = 60; yw[7] = 1;


            


            // xf yf points of world
            //xf[0] = (double)VectorPoints[0,1] - 160; xf[1] = (double)VectorPoints[1,1] - 160;
            //yf[0] = (double)VectorPoints[0,0] - 120; yf[1] = (double)VectorPoints[1,0] - 120;
            //xf[2] = (double)VectorPoints[2,1] - 160; xf[3] = (double)VectorPoints[3,1] - 160;
            //yf[2] = (double)VectorPoints[2,0] - 120; yf[3] = (double)VectorPoints[3,0] - 120;
            //xf[4] = (double)VectorPoints[4,1] - 160; xf[5] = (double)VectorPoints[5,1] - 160;
            //yf[4] = (double)VectorPoints[4,0] - 120; yf[5] = (double)VectorPoints[5,0] - 120;
            //xf[6] = (double)VectorPoints[6,1] - 160; xf[7] = (double)VectorPoints[7,1] - 160;
            //yf[6] = (double)VectorPoints[6,0] - 120; yf[7] = (double)VectorPoints[7,0] - 120;


            xf[0] = 29.5; xf[1] = -30.5; xf[2] = -30.5; xf[3] = 29.5; xf[4] = 10.5; xf[5] = -19.5; xf[6] = -19.5; xf[7] = 10.5;
            yf[0] = -34.5; yf[1] = 35.5; yf[2] = -34.5; yf[3] = 35.5; yf[4] = -22.5; yf[5] = 12.5; yf[6] = -22.5; yf[7] = 12.5;


            



            NAR.ImageProcessing.CameraCalibration.Tsai.Compute compute = new NAR.ImageProcessing.CameraCalibration.Tsai.Compute();
            compute.TsaiCalibration2D(np, xw, yw, xf, yf, ref focal, ref T, ref r);
            //tsaiCalibration2D(np, xw, yw, xf, yf, &focal, T, r);

        }
        #endregion

    }
}
