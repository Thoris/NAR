using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Operators
{
    class NandCommandReport: Base.Color2ImagesBaseTest
    {
        #region Constructors/Destructors
        public NandCommandReport(int totalTests, string imageListFolder, IReport reportGenerator)
            : base (totalTests, imageListFolder, reportGenerator)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            command = new NAR.ImageProcessing.Operators.NandCommand();

            base.Initialize(ref command);
        }
        #endregion
    }
}
