namespace NAR.Forms.Tests.Controls
{
    partial class CommandManager
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.ctlAvailableItems = new NAR.Forms.Tests.Controls.AvailableItems();
            this.ctlCommandList = new NAR.Forms.Tests.Controls.CommandList();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.ctlAvailableItems);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.ctlCommandList);
            this.splitContainer1.Size = new System.Drawing.Size(430, 383);
            this.splitContainer1.SplitterDistance = 130;
            this.splitContainer1.TabIndex = 0;
            // 
            // ctlAvailableItems
            // 
            this.ctlAvailableItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctlAvailableItems.Location = new System.Drawing.Point(0, 0);
            this.ctlAvailableItems.Name = "ctlAvailableItems";
            this.ctlAvailableItems.Size = new System.Drawing.Size(130, 383);
            this.ctlAvailableItems.TabIndex = 0;
            this.ctlAvailableItems.OnInsertItem += new NAR.Forms.Tests.Controls.AvailableItems.InsertItem(this.ctlAvailableItems_OnInsertItem);
            // 
            // ctlCommandList
            // 
            this.ctlCommandList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctlCommandList.Location = new System.Drawing.Point(0, 0);
            this.ctlCommandList.Name = "ctlCommandList";
            this.ctlCommandList.Size = new System.Drawing.Size(296, 383);
            this.ctlCommandList.TabIndex = 0;
            this.ctlCommandList.OnListChanged += new NAR.Forms.Tests.Controls.CommandList.ListChanged(this.ctlCommandList_OnListChanged);
            // 
            // CommandManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "CommandManager";
            this.Size = new System.Drawing.Size(430, 383);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private AvailableItems ctlAvailableItems;
        private CommandList ctlCommandList;
    }
}
