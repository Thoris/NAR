using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Images.Binarization
{
    public class FloydSteinbergDitheringCommandReport: Base.GrayscaleBaseTest
    {
        #region Constructors/Destructors
        public FloydSteinbergDitheringCommandReport(int totalTests, string imageListFolder, IReport reportGenerator)
            : base (totalTests, imageListFolder, reportGenerator)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            command = new NAR.ImageProcessing.Images.Binarization.FloydSteinbergDitheringCommand();

            base.Initialize(ref command);
        }
        #endregion
    }
}
