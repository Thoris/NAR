using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ImageProcessing.Images.Binarization
{
    public class FloydSteinbergDitheringCommand : ErrorDiffusionToAdjacentNeighborsCommand, IBinarization, ICommand
    {
        #region Constructors/Destructors
        public FloydSteinbergDitheringCommand()
            :  base
            ( 
                new int[2][] 
                {
                    new int[1] { 7 },
                    new int[3] { 3, 5, 1 }
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
