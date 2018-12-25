using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Effects
{
    class SwirlCommandReport: Base.ColorBaseTest
    {
        #region Constructors/Destructors
        public SwirlCommandReport(int totalTests, string imageListFolder, IReport reportGenerator)
            : base (totalTests, imageListFolder, reportGenerator)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            double degree = 0.05;

            command = new NAR.ImageProcessing.Effects.SwirlCommand(degree);

            base.Initialize(ref command);
        }
        #endregion
    }
}
