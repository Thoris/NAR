using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Operators
{
    class XorCommandReport: Base.Color2ImagesBaseTest
    {
        #region Constructors/Destructors
        public XorCommandReport(int totalTests, string imageListFolder, IReport reportGenerator)
            : base (totalTests, imageListFolder, reportGenerator)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            command = new NAR.ImageProcessing.Operators.XorCommand();

            base.Initialize(ref command);
        }
        #endregion
    }
}
