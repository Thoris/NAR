using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Effects
{
    class MoireCommandReport: Base.ColorBaseTest
    {
        #region Constructors/Destructors
        public MoireCommandReport(int totalTests, string imageListFolder, IReport reportGenerator)
            : base (totalTests, imageListFolder, reportGenerator)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            byte factor = 15;

            command = new NAR.ImageProcessing.Effects.MoireCommand(factor);

            base.Initialize(ref command);
        }
        #endregion
    }
}
