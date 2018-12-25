using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Pixels
{
    class CountPixelCommandReport: Base.BlackWhiteBaseTest
    {
        #region Constructors/Destructors
        public CountPixelCommandReport(int totalTests, string imageListFolder, IReport reportGenerator)
            : base (totalTests, imageListFolder, reportGenerator)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            int x1 = 10;
            int x2 = 200;
            int y1 = 10;
            int y2 = 200;
            bool fill = true;

            command = new NAR.ImageProcessing.Pixels.CountPixelCommand(x1,x2,y1,y2,fill);

            base.Initialize(ref command);
        }
        #endregion
    }
}
