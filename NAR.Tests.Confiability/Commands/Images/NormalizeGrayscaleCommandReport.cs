using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Images
{
    class NormalizeGrayscaleCommandReport: Base.GrayscaleBaseTest
    {
        #region Constructors/Destructors
        public NormalizeGrayscaleCommandReport(int totalTests, string imageListFolder, IReport reportGenerator)
            : base (totalTests, imageListFolder, reportGenerator)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            double contrast = 1;
            double brightness = 1;
            command = new NAR.ImageProcessing.Images.NormalizeCommand(true);

            base.Initialize(ref command);
        }
        #endregion
    }
}
