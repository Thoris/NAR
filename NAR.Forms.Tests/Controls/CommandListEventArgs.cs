using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Forms.Tests.Controls
{
    public class CommandListEventArgs : EventArgs
    {
        #region Variables
        private NAR.ImageProcessing.CommandCollection _list;
        #endregion

        #region Properties
        public NAR.ImageProcessing.CommandCollection List
        {
            get { return _list; }
        }
        #endregion

        #region Constructors/Destructors
        public CommandListEventArgs(NAR.ImageProcessing.CommandCollection list)
        {
            _list = list;
        }
        #endregion
    }
}
