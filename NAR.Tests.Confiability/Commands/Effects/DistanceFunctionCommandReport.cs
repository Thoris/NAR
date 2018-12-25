using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Effects
{
    class DistanceFunctionCommandReport: Base.ColorBaseTest
    {
        #region Constructors/Destructors
        public DistanceFunctionCommandReport(int totalTests, string imageListFolder, IReport reportGenerator)
            : base (totalTests, imageListFolder, reportGenerator)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            //command = new NAR.ImageProcessing.Effects.DistanceFunctionCommand();

            base.Initialize(ref command);
        }
        #endregion
    }
}
