using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ImageProcessing.Images.Binarization
{
    public class SierraDitheringCommand : ErrorDiffusionToAdjacentNeighborsCommand, IBinarization, ICommand
    {
        #region Constructors/Destructors
        public SierraDitheringCommand()
            : base
            ( 
                new int[3][] 
                {
                    new int[2] { 5, 3 },
                    new int[5] { 2, 4, 5, 4, 2 },
                    new int[3] { 2, 3, 2 }
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
