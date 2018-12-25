using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Effects
{
    class ZhangSuenCommandReport: Base.ColorBaseTest
    {
        #region Constructors/Destructors
        public ZhangSuenCommandReport(int totalTests, string imageListFolder, IReport reportGenerator)
            : base (totalTests, imageListFolder, reportGenerator)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            command = new NAR.ImageProcessing.Effects.ZhangSuenCommand();

            base.Initialize(ref command);
        }
        #endregion
    }
}
