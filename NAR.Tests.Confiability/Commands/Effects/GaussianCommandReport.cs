using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Effects
{
    class GaussianCommandReport: Base.GrayscaleBaseTest
    {
        #region Constructors/Destructors
        public GaussianCommandReport(int totalTests, string imageListFolder, IReport reportGenerator)
            : base (totalTests, imageListFolder, reportGenerator)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            command = new NAR.ImageProcessing.Effects.GaussianCommand(true);

            base.Initialize(ref command);
        }
        #endregion
    }
}
