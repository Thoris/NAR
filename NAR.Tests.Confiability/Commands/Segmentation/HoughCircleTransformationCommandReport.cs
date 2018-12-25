using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Segmentation
{
    class HoughCircleTransformationCommandReport: Base.BlackWhiteBorderBaseTest
    {
        #region Constructors/Destructors
        public HoughCircleTransformationCommandReport(int totalTests, string imageListFolder, IReport reportGenerator, NAR.ImageProcessing.Borders.IBorderDetector border)
            : base (totalTests, imageListFolder, reportGenerator, border)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            command = new NAR.ImageProcessing.Segmentation.HoughCircleTransformationCommand();

            base.Initialize(ref command);
        }
        #endregion
    }
}
