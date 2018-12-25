using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Textures
{
    class WoodTextureCommandReport : Base.ColorBaseTest
    {
        #region Constructors/Destructors
        public WoodTextureCommandReport(int totalTests, string imageListFolder, IReport reportGenerator)
            : base(totalTests, imageListFolder, reportGenerator)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            double rings = 12;

            command = new NAR.ImageProcessing.Textures.WoodTextureCommand(rings, true);

            base.Initialize(ref command);
        }
        #endregion
    }
}
