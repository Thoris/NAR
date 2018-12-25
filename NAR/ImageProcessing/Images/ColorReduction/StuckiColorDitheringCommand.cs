using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ImageProcessing.Images.ColorReduction
{
    public class StuckiColorDitheringCommand : IColorReduction, ICommand
    {
        #region ICommand members
        public Model.IImage Execute(Model.IImage image)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
