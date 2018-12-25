using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Operations
{
    class MultiplicationCommandReport: Base.Color2ImagesBaseTest
    {
        #region Constructors/Destructors
        public MultiplicationCommandReport(int totalTests, string imageListFolder, IReport reportGenerator)
            : base (totalTests, imageListFolder, reportGenerator)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            command = new NAR.ImageProcessing.Operations.MultiplicationCommand();

            base.Initialize(ref command);
        }
        #endregion
    }
}
