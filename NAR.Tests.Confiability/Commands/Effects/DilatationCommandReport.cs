using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Effects
{
    class DilatationCommandReport: Base.ColorBaseTest
    {
        #region Constructors/Destructors
        public DilatationCommandReport(int totalTests, string imageListFolder, IReport reportGenerator)
            : base (totalTests, imageListFolder, reportGenerator)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            int qtdPixel = 2;
            
            command = new NAR.ImageProcessing.Effects.DilatationCommand(qtdPixel);

            base.Initialize(ref command);
        }
        #endregion
    }
}
