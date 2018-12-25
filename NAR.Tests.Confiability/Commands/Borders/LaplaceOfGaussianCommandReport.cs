using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Borders
{
    public class LaplaceOfGaussianCommandReport : BaseBorderReport
    {
        #region Constructors/Destructors
        public LaplaceOfGaussianCommandReport(int totalTests, string imageListFolder, IReport reportGenerator)
            : base (totalTests, imageListFolder, reportGenerator)
        {
            ((NAR.ImageProcessing.Images.BlackWhiteCommand)base.NewCommands[0]).CalculateLimiar = false;
            ((NAR.ImageProcessing.Images.BlackWhiteCommand)base.NewCommands[0]).Limiar = 20;

        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            command = new NAR.ImageProcessing.Borders.LaplaceOfGaussianCommand(true);

            base.Initialize(ref command);
        }
        #endregion
    }
}
