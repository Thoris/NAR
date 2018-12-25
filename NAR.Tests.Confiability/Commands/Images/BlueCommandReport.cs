using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Images
{
    class BlueCommandReport: Base.ColorBaseTest
    {
        #region Constructors/Destructors
        public BlueCommandReport(int totalTests, string imageListFolder, IReport reportGenerator)
            : base (totalTests, imageListFolder, reportGenerator)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            command = new NAR.ImageProcessing.Images.BlueCommand();

            base.Initialize(ref command);
        }
        #endregion
    }
}
