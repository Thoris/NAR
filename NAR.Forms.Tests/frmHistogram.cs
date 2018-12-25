using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace NAR.Forms.Tests
{
    public partial class frmHistogram : Form
    {
        #region Constants

        public const string Grayscale = "Grayscale";
        public const string Red = "Red";
        public const string Green = "Green";
        public const string Blue = "Blue";

        #endregion

        #region Constructors/Destructors
        public frmHistogram()
        {
            InitializeComponent();

            this.chtHistogram.Series.Clear();
        }
        #endregion

        #region Methods
        private void ConfigureGrayscale(byte[] grayscale)
        {
            if (this.chtHistogram != null && this.chtHistogram.Series.Count == 0)
            {
                this.chtHistogram.Series.Add(Grayscale);
                

                Series graySeries = this.chtHistogram.Series[Grayscale];
                graySeries.Color = Color.Gray;

                for (int c = 0; c < grayscale.Length; c++)
                {
                    graySeries.Points.Add(new DataPoint());
                }
            }
        }
        public void SetGrayScale(byte[] grayscale)
        {
            ConfigureGrayscale(grayscale);

            Series series = this.chtHistogram.Series[Grayscale];
                
            for (int c = 0; c < grayscale.Length; c++)
            {
                series.Points[c].SetValueY (grayscale[c]);
            }

            this.chtHistogram.Invalidate();

        }
        private void ConfigureRGB(byte[] red, byte[] green, byte[] blue)
        {
            if (this.chtHistogram != null && this.chtHistogram.Series.Count == 0)
            {
                this.chtHistogram.Series.Add(Red);
                this.chtHistogram.Series.Add(Green);
                this.chtHistogram.Series.Add(Blue);

                Series redSeries = this.chtHistogram.Series[Red];
                redSeries.Color = Color.Red;
                Series greenSeries = this.chtHistogram.Series[Green];
                greenSeries.Color = Color.Green;
                Series blueSeries = this.chtHistogram.Series[Blue];
                blueSeries.Color = Color.Blue;

                for (int c = 0; c < red.Length; c++)
                {
                    redSeries.Points.Add(new DataPoint());
                    greenSeries.Points.Add(new DataPoint());
                    blueSeries.Points.Add(new DataPoint());
                }
            }
        }
        public void SetRGB(byte[] red, byte[] green, byte[] blue)
        {

            ConfigureRGB(red, green, blue);

            Series redSeries = this.chtHistogram.Series[Red];
            Series greenSeries = this.chtHistogram.Series[Green];
            Series blueSeries = this.chtHistogram.Series[Blue];

            for (int c = 0; c < red.Length; c++)
            {
                redSeries.Points[c].SetValueY(red[c]);
                greenSeries.Points[c].SetValueY(green[c]);
                blueSeries.Points[c].SetValueY(blue[c]);            
            }

            this.chtHistogram.Invalidate();

        }
        #endregion
    }
}
