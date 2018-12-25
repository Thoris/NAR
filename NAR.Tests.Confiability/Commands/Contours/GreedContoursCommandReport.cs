using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Contours
{
    class GreedContoursCommandReport: Base.BorderBaseTest
    {
        #region Constructors/Destructors
        public GreedContoursCommandReport(int totalTests, string imageListFolder, IReport reportGenerator, NAR.ImageProcessing.Borders.IBorderDetector border)
            : base (totalTests, imageListFolder, reportGenerator, border)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            command = new NAR.ImageProcessing.Contours.GreedContoursCommand(true);

            base.Initialize(ref command);
        }
        #endregion
    }
}
