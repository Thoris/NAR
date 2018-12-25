using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Images
{
    class GrayscaleLuminanceCommandReport: Base.ColorBaseTest
    {
        #region Constructors/Destructors
        public GrayscaleLuminanceCommandReport(int totalTests, string imageListFolder, IReport reportGenerator)
            : base (totalTests, imageListFolder, reportGenerator)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            command = new NAR.ImageProcessing.Images.GrayscaleCommand(true);

            base.Initialize(ref command);
        }
        #endregion
    }
}
