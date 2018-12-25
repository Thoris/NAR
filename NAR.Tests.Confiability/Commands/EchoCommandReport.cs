using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands
{
    public class EchoCommandReport : Base.ReportBase
    {
        #region Constructors/Destructors
        public EchoCommandReport(int totalTests, string imageListFolder, IReport reportGenerator)
            : base (totalTests,  imageListFolder, reportGenerator)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            command = new EchoCommand();

            base.Initialize(ref command);
        }
        #endregion
    }
}
