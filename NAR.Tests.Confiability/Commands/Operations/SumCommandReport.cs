using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Operations
{
    class SumCommandReport: Base.Color2ImagesBaseTest
    {
        #region Constructors/Destructors
        public SumCommandReport(int totalTests, string imageListFolder, IReport reportGenerator)
            : base (totalTests, imageListFolder, reportGenerator)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            command = new NAR.ImageProcessing.Operations.SumCommand();

            base.Initialize(ref command);
        }
        #endregion
    }
}
