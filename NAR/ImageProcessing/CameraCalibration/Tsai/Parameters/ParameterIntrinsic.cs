using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ImageProcessing.CameraCalibration.Tsai.Parameters
{
    public class Intrinsic
    {
        
        #region Properties
        /// <summary>
        /// Effective focal length of the pin hole camera
        /// </summary>
        public double F { get; set; }
        /// <summary>
        /// Coordinates of center of radial lens distortion (also used as the piercing point of the camera coordinate frame's Z axis with the camera's sensor plane)
        /// </summary>
        public double Ox { get; set; }
        /// <summary>
        /// Coordinates of center of radial lens distortion (also used as the piercing point of the camera coordinate frame's Z axis with the camera's sensor plane)
        /// </summary>
        public double Oy { get; set; }
        /// <summary>
        /// Uncertainty factor for scale of horizontal scanline
        /// </summary>
        public double Sx { get; set; }
        /// <summary>
        /// Uncertainty factor for scale of vertical scanline
        /// </summary>
        public double Sy { get; set; }
        /// <summary>
        /// 1st order radial lens distortion coefficient
        /// </summary>
        public double K1 { get; set; }
        /// <summary>
        /// Distorcion coeficient (If requested)
        /// </summary>
        public double K2 { get; set; }

        #endregion

        #region Constructors/Destructors
        public Intrinsic()
        {
        }
        #endregion
    }
}
