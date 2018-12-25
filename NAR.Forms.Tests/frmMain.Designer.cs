namespace NAR.Forms.Tests
{
    partial class frmMain
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.stbMain = new System.Windows.Forms.StatusStrip();
            this.stblblFPSLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.stblblFPS = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblBufferDesc = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblBuffer = new System.Windows.Forms.ToolStripStatusLabel();
            this.mnuMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuConnect = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDisconnect = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDevices = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuConfiguration = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuConfigVideoFormat = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuConfigVideoSource = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuConfigVideoDisplay = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuConfigVideoCompression = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuImage = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuGrabImage = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHistogram = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHistogramRGB = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHistogramGrayscale = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHoughGraph = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuVideo = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuStartVideo = new System.Windows.Forms.ToolStripMenuItem();
            this.picImage = new System.Windows.Forms.PictureBox();
            this.tmGrab = new System.Windows.Forms.Timer(this.components);
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.stbFPSCurrentDesc = new System.Windows.Forms.ToolStripStatusLabel();
            this.stblblFfpCurrent = new System.Windows.Forms.ToolStripStatusLabel();
            this.cltCommandManager = new NAR.Forms.Tests.Controls.CommandManager();
            this.stbMain.SuspendLayout();
            this.mnuMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picImage)).BeginInit();
            this.SuspendLayout();
            // 
            // stbMain
            // 
            this.stbMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stblblFPSLabel,
            this.stblblFPS,
            this.lblBufferDesc,
            this.lblBuffer,
            this.stbFPSCurrentDesc,
            this.stblblFfpCurrent});
            this.stbMain.Location = new System.Drawing.Point(0, 558);
            this.stbMain.Name = "stbMain";
            this.stbMain.Size = new System.Drawing.Size(866, 22);
            this.stbMain.TabIndex = 0;
            this.stbMain.Text = "statusStrip1";
            // 
            // stblblFPSLabel
            // 
            this.stblblFPSLabel.Name = "stblblFPSLabel";
            this.stblblFPSLabel.Size = new System.Drawing.Size(26, 17);
            this.stblblFPSLabel.Text = "FPS";
            // 
            // stblblFPS
            // 
            this.stblblFPS.Name = "stblblFPS";
            this.stblblFPS.Size = new System.Drawing.Size(13, 17);
            this.stblblFPS.Text = "0";
            // 
            // lblBufferDesc
            // 
            this.lblBufferDesc.Name = "lblBufferDesc";
            this.lblBufferDesc.Size = new System.Drawing.Size(39, 17);
            this.lblBufferDesc.Text = "Buffer";
            // 
            // lblBuffer
            // 
            this.lblBuffer.Name = "lblBuffer";
            this.lblBuffer.Size = new System.Drawing.Size(13, 17);
            this.lblBuffer.Text = "0";
            // 
            // mnuMain
            // 
            this.mnuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.mnuConfiguration,
            this.mnuImage,
            this.mnuVideo});
            this.mnuMain.Location = new System.Drawing.Point(0, 0);
            this.mnuMain.Name = "mnuMain";
            this.mnuMain.Size = new System.Drawing.Size(866, 24);
            this.mnuMain.TabIndex = 1;
            this.mnuMain.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuConnect,
            this.mnuDisconnect,
            this.mnuDevices});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // mnuConnect
            // 
            this.mnuConnect.Name = "mnuConnect";
            this.mnuConnect.Size = new System.Drawing.Size(133, 22);
            this.mnuConnect.Text = "&Connect";
            this.mnuConnect.Click += new System.EventHandler(this.mnuConnect_Click);
            // 
            // mnuDisconnect
            // 
            this.mnuDisconnect.Enabled = false;
            this.mnuDisconnect.Name = "mnuDisconnect";
            this.mnuDisconnect.Size = new System.Drawing.Size(133, 22);
            this.mnuDisconnect.Text = "&Disconnect";
            this.mnuDisconnect.Click += new System.EventHandler(this.mnuDisconnect_Click);
            // 
            // mnuDevices
            // 
            this.mnuDevices.Name = "mnuDevices";
            this.mnuDevices.Size = new System.Drawing.Size(133, 22);
            this.mnuDevices.Text = "&Devices";
            // 
            // mnuConfiguration
            // 
            this.mnuConfiguration.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuConfigVideoFormat,
            this.mnuConfigVideoSource,
            this.mnuConfigVideoDisplay,
            this.mnuConfigVideoCompression});
            this.mnuConfiguration.Name = "mnuConfiguration";
            this.mnuConfiguration.Size = new System.Drawing.Size(93, 20);
            this.mnuConfiguration.Text = "&Configuration";
            // 
            // mnuConfigVideoFormat
            // 
            this.mnuConfigVideoFormat.Name = "mnuConfigVideoFormat";
            this.mnuConfigVideoFormat.Size = new System.Drawing.Size(186, 22);
            this.mnuConfigVideoFormat.Text = "Video &Format...";
            this.mnuConfigVideoFormat.Click += new System.EventHandler(this.mnuConfigVideoFormat_Click);
            // 
            // mnuConfigVideoSource
            // 
            this.mnuConfigVideoSource.Name = "mnuConfigVideoSource";
            this.mnuConfigVideoSource.Size = new System.Drawing.Size(186, 22);
            this.mnuConfigVideoSource.Text = "Video &Source...";
            this.mnuConfigVideoSource.Click += new System.EventHandler(this.mnuConfigVideoSource_Click);
            // 
            // mnuConfigVideoDisplay
            // 
            this.mnuConfigVideoDisplay.Name = "mnuConfigVideoDisplay";
            this.mnuConfigVideoDisplay.Size = new System.Drawing.Size(186, 22);
            this.mnuConfigVideoDisplay.Text = "Video &Display...";
            this.mnuConfigVideoDisplay.Click += new System.EventHandler(this.mnuConfigVideoDisplay_Click);
            // 
            // mnuConfigVideoCompression
            // 
            this.mnuConfigVideoCompression.Name = "mnuConfigVideoCompression";
            this.mnuConfigVideoCompression.Size = new System.Drawing.Size(186, 22);
            this.mnuConfigVideoCompression.Text = "Video &Compression...";
            this.mnuConfigVideoCompression.Click += new System.EventHandler(this.mnuConfigVideoCompression_Click);
            // 
            // mnuImage
            // 
            this.mnuImage.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuGrabImage,
            this.mnuHistogram,
            this.mnuHoughGraph});
            this.mnuImage.Name = "mnuImage";
            this.mnuImage.Size = new System.Drawing.Size(52, 20);
            this.mnuImage.Text = "&Image";
            // 
            // mnuGrabImage
            // 
            this.mnuGrabImage.Enabled = false;
            this.mnuGrabImage.Name = "mnuGrabImage";
            this.mnuGrabImage.Size = new System.Drawing.Size(146, 22);
            this.mnuGrabImage.Text = "&Grab Image";
            this.mnuGrabImage.Click += new System.EventHandler(this.mnuGrabImage_Click);
            // 
            // mnuHistogram
            // 
            this.mnuHistogram.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHistogramRGB,
            this.mnuHistogramGrayscale});
            this.mnuHistogram.Name = "mnuHistogram";
            this.mnuHistogram.Size = new System.Drawing.Size(146, 22);
            this.mnuHistogram.Text = "&Histogram";
            // 
            // mnuHistogramRGB
            // 
            this.mnuHistogramRGB.Name = "mnuHistogramRGB";
            this.mnuHistogramRGB.Size = new System.Drawing.Size(124, 22);
            this.mnuHistogramRGB.Text = "&RGB";
            this.mnuHistogramRGB.Click += new System.EventHandler(this.mnuHistogramRGB_Click);
            // 
            // mnuHistogramGrayscale
            // 
            this.mnuHistogramGrayscale.Name = "mnuHistogramGrayscale";
            this.mnuHistogramGrayscale.Size = new System.Drawing.Size(124, 22);
            this.mnuHistogramGrayscale.Text = "&Grayscale";
            this.mnuHistogramGrayscale.Click += new System.EventHandler(this.mnuHistogramGrayscale_Click);
            // 
            // mnuHoughGraph
            // 
            this.mnuHoughGraph.Name = "mnuHoughGraph";
            this.mnuHoughGraph.Size = new System.Drawing.Size(146, 22);
            this.mnuHoughGraph.Text = "H&ough Graph";
            this.mnuHoughGraph.Click += new System.EventHandler(this.mnuHoughGraph_Click);
            // 
            // mnuVideo
            // 
            this.mnuVideo.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuStartVideo});
            this.mnuVideo.Name = "mnuVideo";
            this.mnuVideo.Size = new System.Drawing.Size(49, 20);
            this.mnuVideo.Text = "&Video";
            // 
            // mnuStartVideo
            // 
            this.mnuStartVideo.Name = "mnuStartVideo";
            this.mnuStartVideo.Size = new System.Drawing.Size(155, 22);
            this.mnuStartVideo.Text = "S&tart Recording";
            this.mnuStartVideo.Click += new System.EventHandler(this.mnuStartVideo_Click);
            // 
            // picImage
            // 
            this.picImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picImage.Location = new System.Drawing.Point(0, 24);
            this.picImage.Name = "picImage";
            this.picImage.Size = new System.Drawing.Size(866, 534);
            this.picImage.TabIndex = 2;
            this.picImage.TabStop = false;
            // 
            // tmGrab
            // 
            this.tmGrab.Interval = 10;
            this.tmGrab.Tick += new System.EventHandler(this.tmGrab_Tick);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(363, 24);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 534);
            this.splitter1.TabIndex = 4;
            this.splitter1.TabStop = false;
            // 
            // stbFPSCurrentDesc
            // 
            this.stbFPSCurrentDesc.Name = "stbFPSCurrentDesc";
            this.stbFPSCurrentDesc.Size = new System.Drawing.Size(69, 17);
            this.stbFPSCurrentDesc.Text = "FPS Current";
            // 
            // stblblFfpCurrent
            // 
            this.stblblFfpCurrent.Name = "stblblFfpCurrent";
            this.stblblFfpCurrent.Size = new System.Drawing.Size(13, 17);
            this.stblblFfpCurrent.Text = "0";
            // 
            // cltCommandManager
            // 
            this.cltCommandManager.ComparedImage = null;
            this.cltCommandManager.Dock = System.Windows.Forms.DockStyle.Right;
            this.cltCommandManager.Location = new System.Drawing.Point(366, 24);
            this.cltCommandManager.Name = "cltCommandManager";
            this.cltCommandManager.Size = new System.Drawing.Size(500, 534);
            this.cltCommandManager.TabIndex = 3;
            this.cltCommandManager.OnListChanged += new NAR.Forms.Tests.Controls.CommandManager.ListChanged(this.cltCommandManager_OnListChanged);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(866, 580);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.cltCommandManager);
            this.Controls.Add(this.picImage);
            this.Controls.Add(this.stbMain);
            this.Controls.Add(this.mnuMain);
            this.MainMenuStrip = this.mnuMain;
            this.Name = "frmMain";
            this.Text = "Main";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.stbMain.ResumeLayout(false);
            this.stbMain.PerformLayout();
            this.mnuMain.ResumeLayout(false);
            this.mnuMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip stbMain;
        private System.Windows.Forms.MenuStrip mnuMain;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuConnect;
        private System.Windows.Forms.ToolStripMenuItem mnuDisconnect;
        private System.Windows.Forms.PictureBox picImage;
        private System.Windows.Forms.Timer tmGrab;
        private System.Windows.Forms.ToolStripStatusLabel stblblFPSLabel;
        private System.Windows.Forms.ToolStripStatusLabel stblblFPS;
        private Controls.CommandManager cltCommandManager;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ToolStripMenuItem mnuImage;
        private System.Windows.Forms.ToolStripMenuItem mnuHistogram;
        private System.Windows.Forms.ToolStripMenuItem mnuGrabImage;
        private System.Windows.Forms.ToolStripMenuItem mnuHistogramRGB;
        private System.Windows.Forms.ToolStripMenuItem mnuHistogramGrayscale;
        private System.Windows.Forms.ToolStripMenuItem mnuConfiguration;
        private System.Windows.Forms.ToolStripMenuItem mnuConfigVideoFormat;
        private System.Windows.Forms.ToolStripMenuItem mnuConfigVideoSource;
        private System.Windows.Forms.ToolStripMenuItem mnuConfigVideoDisplay;
        private System.Windows.Forms.ToolStripMenuItem mnuConfigVideoCompression;
        private System.Windows.Forms.ToolStripMenuItem mnuHoughGraph;
        private System.Windows.Forms.ToolStripMenuItem mnuVideo;
        private System.Windows.Forms.ToolStripMenuItem mnuStartVideo;
        private System.Windows.Forms.ToolStripMenuItem mnuDevices;
        private System.Windows.Forms.ToolStripStatusLabel lblBufferDesc;
        private System.Windows.Forms.ToolStripStatusLabel lblBuffer;
        private System.Windows.Forms.ToolStripStatusLabel stbFPSCurrentDesc;
        private System.Windows.Forms.ToolStripStatusLabel stblblFfpCurrent;
    }
}

