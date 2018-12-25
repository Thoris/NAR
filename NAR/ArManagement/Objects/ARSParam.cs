using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ArManagement.Objects
{
    public class ARSParam
    {
        public int xsize, ysize;
        public double[,] matL = new double[3, 4];
        public double[,] matR = new double[3, 4];
        public double[,] matL2R = new double[3, 4];
        public double[] dist_factorL = new double[4];
        public double[] dist_factorR = new double[4];
    }
}
