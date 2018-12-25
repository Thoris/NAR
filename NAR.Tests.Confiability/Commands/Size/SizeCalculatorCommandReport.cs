using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Size
{
    class SizeCalculatorCommandReport: Base.BlackWhiteBaseTest
    {
        #region Constructors/Destructors
        public SizeCalculatorCommandReport(int totalTests, string imageListFolder, IReport reportGenerator)
            : base (totalTests, imageListFolder, reportGenerator)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {

            int x1 = 0;
            int x2 = 256;
            int y1 = 125;
            int y2 = 125;
            double mmPixel = 10;
            bool fill = true;

            command = new NAR.ImageProcessing.Size.SizeCalculatorCommand(x1,x2,y1,y2,mmPixel,fill);

            base.Initialize(ref command);
        }
        #endregion
    }
}
