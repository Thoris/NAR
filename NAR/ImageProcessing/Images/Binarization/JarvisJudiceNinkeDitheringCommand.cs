using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ImageProcessing.Images.Binarization
{
    public class JarvisJudiceNinkeDitheringCommand : ErrorDiffusionToAdjacentNeighborsCommand, IBinarization, ICommand
    {
        #region Constructors/Destructors
        public JarvisJudiceNinkeDitheringCommand ()
            : base
            (
                new int[3][] 
                {
                    new int[2] { 7, 5 },
                    new int[5] { 3, 5, 7, 5, 3 },
                    new int[5] { 1, 3, 5, 3, 1 }
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
