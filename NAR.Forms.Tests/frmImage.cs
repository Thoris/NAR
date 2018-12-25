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
    public partial class frmImage : Form
    {
        #region Variables
        private Model.IImage _image;
        #endregion

        #region Properties
        public Model.IImage Image
        {
            get { return _image; }
        }
        #endregion

        #region Constructors/Destructors
        public frmImage(Model.IImage image)
        {
            InitializeComponent();

            _image = image;
        }
        #endregion

        #region Methods
        private void ShowImage(Model.IImage image)
        {
            this.picImage.Image = image.Image;
            this.Width = image.Image.Width + 20;
            this.Height = image.Image.Height + 40;
            this.picImage.Show();
        }
        #endregion

        #region Events
        private void frmImage_Load(object sender, EventArgs e)
        {
            ShowImage(_image);
            this.Refresh();
        }
        #endregion
    }
}
