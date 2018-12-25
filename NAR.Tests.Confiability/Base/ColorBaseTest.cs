using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Base
{
    public class ColorBaseTest : ReportBase
    {
        #region Constructors/Destructors
        public ColorBaseTest(int totalTests, string imageListFolder, IReport reportGenerator)
            : base (totalTests,  imageListFolder, reportGenerator)
        {
        }
        #endregion

        #region Methods
        public new void Initialize()
        {
            base.Initialize();
        }
        #endregion
    }
}
