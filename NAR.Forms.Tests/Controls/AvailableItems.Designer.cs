namespace NAR.Forms.Tests.Controls
{
    partial class AvailableItems
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.treAvailable = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // treAvailable
            // 
            this.treAvailable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treAvailable.Location = new System.Drawing.Point(0, 0);
            this.treAvailable.Name = "treAvailable";
            this.treAvailable.Size = new System.Drawing.Size(270, 366);
            this.treAvailable.TabIndex = 0;
            this.treAvailable.DoubleClick += new System.EventHandler(this.treAvailable_DoubleClick);
            this.treAvailable.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treAvailable_KeyDown);
            // 
            // AvailableItems
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treAvailable);
            this.Name = "AvailableItems";
            this.Size = new System.Drawing.Size(270, 366);
            this.Load += new System.EventHandler(this.AvailableItems_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treAvailable;
    }
}
