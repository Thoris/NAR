using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Forms.Tests.Controls
{
    public class InsertSelectedItemEventArgs : EventArgs
    {
        #region Variables
        private AvailableEntry _entry;
        #endregion

        #region Properties
        public AvailableEntry Entry
        {
            get { return _entry; }
        }
        #endregion

        #region Constructors/Destructors
        public InsertSelectedItemEventArgs(AvailableEntry entry)
        {
            _entry = entry;
        }
        #endregion
    }
}
