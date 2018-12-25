using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ImageProcessing.CameraCalibration.Tsai.Parameters
{
    public class CalibrationConstants
    {
        #region Properties
        
        public double f { get; set; }   //mm
        public double kappa1 { get; set; }  // 1/mm^2
        public double p1 { get; set; }  //1/mm
        public double p2 { get; set; }  //1/mm
        public double Tx { get; set; }  //mm
        public double Ty { get; set; }  //mm
        public double Tz { get; set; }  //mm
        public double Rx { get; set; }  //rad
        public double Ry { get; set; }  //rad
        public double Rz { get; set; }  //rad
        public double r1 { get; set; }
        public double r2 { get; set; }
        public double r3 { get; set; }
        public double r4 { get; set; }
        public double r5 { get; set; }
        public double r6 { get; set; }
        public double r7 { get; set; }
        public double r8 { get; set; }
        public double r9 { get; set; }

        #endregion
    }
}
