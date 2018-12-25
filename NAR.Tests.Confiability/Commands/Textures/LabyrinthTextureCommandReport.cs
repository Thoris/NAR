using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Textures
{
    class LabyrinthTextureCommandReport : Base.ColorBaseTest
    {
        #region Constructors/Destructors
        public LabyrinthTextureCommandReport(int totalTests, string imageListFolder, IReport reportGenerator)
            : base(totalTests, imageListFolder, reportGenerator)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            command = new NAR.ImageProcessing.Textures.LabyrinthTextureCommand(true);

            base.Initialize(ref command);
        }
        #endregion
    }
}
