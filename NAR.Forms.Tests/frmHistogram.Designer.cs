namespace NAR.Forms.Tests
{
    partial class frmHistogram
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chtHistogram = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chtHistogram)).BeginInit();
            this.SuspendLayout();
            // 
            // chtHistogram
            // 
            chartArea1.Name = "ChartArea1";
            this.chtHistogram.ChartAreas.Add(chartArea1);
            this.chtHistogram.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chtHistogram.Legends.Add(legend1);
            this.chtHistogram.Location = new System.Drawing.Point(0, 0);
            this.chtHistogram.Name = "chtHistogram";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chtHistogram.Series.Add(series1);
            this.chtHistogram.Size = new System.Drawing.Size(534, 286);
            this.chtHistogram.TabIndex = 0;
            this.chtHistogram.Text = "chart1";
            // 
            // frmHistogram
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 286);
            this.Controls.Add(this.chtHistogram);
            this.Name = "frmHistogram";
            this.Text = "frmHistogram";
            ((System.ComponentModel.ISupportInitialize)(this.chtHistogram)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chtHistogram;
    }
}