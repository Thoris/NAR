using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Effects
{
    class WaterCommandReport: Base.ColorBaseTest
    {
        #region Constructors/Destructors
        public WaterCommandReport(int totalTests, string imageListFolder, IReport reportGenerator)
            : base (totalTests, imageListFolder, reportGenerator)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            short water = 5;
            command = new NAR.ImageProcessing.Effects.WaterCommand(water);

            base.Initialize(ref command);
        }
        #endregion
    }
}
