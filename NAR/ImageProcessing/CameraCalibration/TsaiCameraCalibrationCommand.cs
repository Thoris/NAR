using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ImageProcessing.CameraCalibration
{
    public class TsaiCameraCalibrationCommand : ICommand
    {
        #region ICommand Members

        public Model.IImage Execute(Model.IImage image)
        {
            return image;
        }

        #endregion
    }
}
