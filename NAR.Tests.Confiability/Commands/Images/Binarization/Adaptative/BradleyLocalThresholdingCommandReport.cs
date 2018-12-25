using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Images.Binarization.Adaptative
{
    public class BradleyLocalThresholdingCommandReport: Base.GrayscaleBaseTest
    {
        #region Constructors/Destructors
        public BradleyLocalThresholdingCommandReport(int totalTests, string imageListFolder, IReport reportGenerator)
            : base (totalTests, imageListFolder, reportGenerator)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            command = new NAR.ImageProcessing.Images.Binarization.Adaptative.BradleyLocalThresholdingCommand(41, 0.15f);

            base.Initialize(ref command);
        }
        #endregion
    }
}
