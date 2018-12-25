using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ImageProcessing.Images.Binarization
{
    public class BurkesDitheringCommand : ErrorDiffusionToAdjacentNeighborsCommand, IBinarization, ICommand
    {
        #region Constructors/Destructors
        public BurkesDitheringCommand()
          : base( 
                new int[2][] 
                {
                    new int[2] { 8, 4 },
                    new int[5] { 2, 4, 8, 4, 2 }
                }
            )
        {
        }
        #endregion

        #region ICommand members
        public new Model.IImage Execute(Model.IImage image)
        {
            return base.Execute(image);
        }
        #endregion
    }
}
