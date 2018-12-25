using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands.Effects
{
    class RandomJitterCommandReport: Base.ColorBaseTest
    {
        #region Constructors/Destructors
        public RandomJitterCommandReport(int totalTests, string imageListFolder, IReport reportGenerator)
            : base (totalTests, imageListFolder, reportGenerator)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            command = new NAR.ImageProcessing.Effects.RandomJitterCommand();

            base.Initialize(ref command);
        }
        #endregion
    }
}
