using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Contours
{
    class MooreNeighborContoursCommandReport: Base.BorderBaseTest
    {
        #region Constructors/Destructors
        public MooreNeighborContoursCommandReport(int totalTests, string imageListFolder, IReport reportGenerator, NAR.ImageProcessing.Borders.IBorderDetector border)
            : base (totalTests, imageListFolder, reportGenerator, border)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            command = new NAR.ImageProcessing.Contours.MooreNeighborContoursCommand(true);

            base.Initialize(ref command);
        }
        #endregion
    }
}
