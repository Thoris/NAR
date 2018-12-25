using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NAR.Forms.Tests.Controls
{
    public partial class CommandManager : UserControl
    {
        #region Events / Delegates

        public delegate void ListChanged(object sender, CommandListEventArgs e);
        public event ListChanged OnListChanged;

        #endregion

        #region Properties
        public Model.IImage ComparedImage
        {
            get { return this.ctlCommandList.ComparedImage; }
            set { this.ctlCommandList.ComparedImage = value; }
        }
        #endregion

        #region Constructors/Destructors
        public CommandManager()
        {
            InitializeComponent();
        }
        #endregion

        #region Methods
        
        #endregion

        #region Events
        private void ctlAvailableItems_OnInsertItem(object sender, InsertSelectedItemEventArgs e)
        {
            this.ctlCommandList.AddNewEntry(e.Entry);

            if (OnListChanged != null)
                OnListChanged(this, new CommandListEventArgs(this.ctlCommandList.GetList()));
        }
        private void ctlCommandList_OnListChanged(object sender, CommandListEventArgs e)
        {
            if (OnListChanged != null)
                OnListChanged(this, e);
        }
        #endregion

    }

    
}
