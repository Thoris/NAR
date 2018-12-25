using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NAR.Forms.Tests
{
    public partial class frmHoughGraph : Form
    {
        #region Properties
        public Image Graph
        {
            get { return this.picGraph.Image; }
            set 
            {
                if (this.picGraph.Image == null)
                {
                    this.Width = value.Width + 20;
                    this.Height = value.Height + 40;
                }


                this.picGraph.Image = value;
                this.picGraph.Invalidate();
            }
        }
        #endregion

        #region Constructors/Destructors
        public frmHoughGraph()
        {
            InitializeComponent();
        }
        #endregion
    }
}
