using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ImageProcessing.CameraCalibration.Tsai.Parameters
{
    class CalibrationData
    {
        #region Properties

        int PointCount { get; set; } //points
        double[] xw { get; set; }  //mm
        double[] yw { get; set; }  //mm
        double[] zw { get; set; }  //mm
        double[] Xf { get; set; }  //pix
        double[] Yf { get; set; }  //pix

        #endregion

    }
}
