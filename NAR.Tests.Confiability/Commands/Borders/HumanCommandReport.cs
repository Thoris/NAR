using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Borders
{
    public class HumanCommandReport: Base.ColorBaseTest
    {
        #region Constructors/Destructors
        public HumanCommandReport(int totalTests, string imageListFolder, IReport reportGenerator)
            : base ( totalTests,  imageListFolder, reportGenerator)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            command = new NAR.ImageProcessing.Borders.HumanCommand();

            base.Initialize(ref command);
        }
        #endregion
    }
}
