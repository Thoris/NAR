using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ArManagement.Config
{
    public class ARConfiguration
    {
        #region Constants
        public const int SquareMax = 30;
        #endregion

        #region Variables
        private ARImageProcMode _imageProcMode = ARImageProcMode.Full;
        private static ARConfiguration _current = new ARConfiguration();
        private bool _debug;
        #endregion

        #region Properties
        public ARImageProcMode ImageProcMode 
        {
            get { return _imageProcMode; }
            set { _imageProcMode = value; }
        }
        public static ARConfiguration Current
        {
            get { return _current; }
            set { _current = value; }
        }
        public bool Debug
        {
            get { return _debug; }
            set { _debug = value; }
        }
        #endregion
    }
}
