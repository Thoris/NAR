using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Reports
{
    public class FileReportBase
    {
        #region Variables
        private string _moduleName;
        private string _commandName;
        private string _folder;        
        #endregion

        #region Properties
        public string ModuleName
        {
            get { return _moduleName; }
            set { _moduleName = value; }
        }
        public string CommandName
        {
            get { return _commandName; }
            set { _commandName = value; }
        }
        public string Folder
        {
            get { return _folder; }
        }
        public string FullFolder
        {
            get
            {
                string folder = System.IO.Path.Combine(_folder, _moduleName);

                //string folder = _folder + "\\" + _moduleName;

                return folder;
            }
        }
        public string FullFileName
        {
            get 
            {
                string folder = this.FullFolder;

                return System.IO.Path.Combine(folder, this.GetType().Name);
            }
        }
        #endregion

        #region Constructors/Destructors       
        public FileReportBase(string folder)
        {
            _folder = folder;
        }
        #endregion
    }
}
