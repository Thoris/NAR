using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Contours
{
    class RadialSweepContoursCommandReport: Base.BorderBaseTest
    {
        #region Constructors/Destructors
        public RadialSweepContoursCommandReport(int totalTests, string imageListFolder, IReport reportGenerator, NAR.ImageProcessing.Borders.IBorderDetector border)
            : base (totalTests, imageListFolder, reportGenerator, border)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            command = new NAR.ImageProcessing.Contours.RadialSweepContoursCommand(true);

            base.Initialize(ref command);
        }
        #endregion
    }
}
