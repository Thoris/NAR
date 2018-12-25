using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Images.Binarization
{
    public class ThresholdCommandReport: Base.GrayscaleBaseTest
    {
        #region Constructors/Destructors
        public ThresholdCommandReport(int totalTests, string imageListFolder, IReport reportGenerator)
            : base (totalTests, imageListFolder, reportGenerator)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            command = new NAR.ImageProcessing.Images.Binarization.ThresholdCommand(128, true);

            base.Initialize(ref command);
        }
        #endregion
    }
}
