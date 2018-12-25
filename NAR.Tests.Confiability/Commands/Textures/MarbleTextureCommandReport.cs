using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Textures
{
    class MarbleTextureCommandReport : Base.ColorBaseTest
    {
        #region Constructors/Destructors
        public MarbleTextureCommandReport(int totalTests, string imageListFolder, IReport reportGenerator)
            : base(totalTests, imageListFolder, reportGenerator)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            double xPeriod = 5;
            double yPeriod = 10;

            command = new NAR.ImageProcessing.Textures.MarbleTextureCommand(xPeriod, yPeriod, true);

            base.Initialize(ref command);
        }
        #endregion
    }
}
