using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Effects
{
    class PixelateCommandReport: Base.ColorBaseTest
    {
        #region Constructors/Destructors
        public PixelateCommandReport(int totalTests, string imageListFolder, IReport reportGenerator)
            : base (totalTests, imageListFolder, reportGenerator)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            bool grid = true;
            short pixel = 10;

            command = new NAR.ImageProcessing.Effects.PixelateCommand(pixel, grid);

            base.Initialize(ref command);
        }
        #endregion
    }
}
