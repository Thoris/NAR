using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Borders
{
    public class BaseBorderReport : Base.GrayscaleBaseTest
    {
        #region Constructors/Destructors
        public BaseBorderReport(int totalTests, string imageListFolder, IReport reportGenerator)
            : base ( totalTests, imageListFolder, reportGenerator)
        {

            NewCommands = new ImageProcessing.ICommand[2];
            NewCommands[0] = new NAR.ImageProcessing.Images.BlackWhiteCommand(true);
            NewCommands[1] = new NAR.ImageProcessing.Images.InvertCommand();
        }
        #endregion
    }
}
