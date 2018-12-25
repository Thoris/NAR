using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Images
{
    class LuminosityCommandReport: Base.ColorBaseTest
    {
        #region Constructors/Destructors
        public LuminosityCommandReport(int totalTests, string imageListFolder, IReport reportGenerator)
            : base (totalTests, imageListFolder, reportGenerator)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            double contrast = 1;
            double brightness = 1;
            command = new NAR.ImageProcessing.Images.LuminosityCommand(contrast, brightness);

            base.Initialize(ref command);
        }
        #endregion
    }
}
