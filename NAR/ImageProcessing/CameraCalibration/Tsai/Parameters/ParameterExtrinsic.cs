using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ImageProcessing.CameraCalibration.Tsai.Parameters
{
    public class Extrinsic
    {

        #region Properties
        /// <summary>
        /// Translation vector (Size: 3)
        /// </summary>
        public double[] Translation { get; set; }
        /// <summary>
        /// Rotation Vector (Matrix 3x3)
        /// </summary>
        public double[] Rotation { get; set; }
        #endregion

        #region Constructors/Destructors
        public Extrinsic()
        {
        }
        #endregion
    }
}
