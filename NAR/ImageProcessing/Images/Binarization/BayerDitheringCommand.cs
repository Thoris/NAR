using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ImageProcessing.Images.Binarization
{
    public class BayerDitheringCommand : OrderedDitheringCommand, IBinarization, ICommand
    {
        #region Constructors/Destructors
        public BayerDitheringCommand()
            : base( new byte[,] {
								{   0, 192,  48, 240 },
								{ 128,  64, 176, 112 },
								{  32, 224,  16, 208 },
								{ 160,  96, 144,  80 } } )
        {
        }
        #endregion

        #region ICommand Members

        public new Model.IImage Execute(Model.IImage image)
        {
            return base.Execute(image);
        }

        #endregion
    }
}
