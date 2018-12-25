using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Distance
{
    class CalcDistanceCommandReport: Base.GrayscaleBaseTest
    {
        #region Constructors/Destructors
        public CalcDistanceCommandReport(int totalTests, string imageListFolder, IReport reportGenerator)
            : base (totalTests, imageListFolder, reportGenerator)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            command = new NAR.ImageProcessing.Distance.CalcDistanceCommand();

            base.Initialize(ref command);
        }
        #endregion
    }
}
