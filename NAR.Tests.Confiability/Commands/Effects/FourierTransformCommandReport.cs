using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Effects
{
    class FourierTransformCommandReport: Base.ColorBaseTest
    {
        #region Constructors/Destructors
        public FourierTransformCommandReport(int totalTests, string imageListFolder, IReport reportGenerator)
            : base (totalTests, imageListFolder, reportGenerator)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            //command = new NAR.ImageProcessing.Effects.FourierTransformCommand();

            base.Initialize(ref command);
        }
        #endregion
    }
}
