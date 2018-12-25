using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Base
{
    public class ReportBase : ListBaseTest
    {
        #region Variables
        private IReport _reportGenerator;
        #endregion

        #region Properties
        public NAR.ImageProcessing.ICommand Command
        {
            get { return base._command; }
        }
        #endregion

        #region Constructors/Destructors

        public ReportBase(int totalTests, string imageListFolder, IReport reportGenerator)
            : base (totalTests,  imageListFolder)
        {
            _reportGenerator = reportGenerator;
        }
        #endregion

        #region Methods
        public new void Initialize()
        {
            base.Initialize();
        }
        public void Save()
        {

            if (string.IsNullOrEmpty(_reportGenerator.ModuleName))
                _reportGenerator.ModuleName = _command.GetType().Namespace;

            if (string.IsNullOrEmpty(_reportGenerator.CommandName))
                _reportGenerator.CommandName = _command.GetType().Name;

            _reportGenerator.Save(base.Results);
        }
        #endregion
    }
}
