using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ImageProcessing.Segmentation
{
    class WatershedPixel
    {
        public int X;
        public int Y;
        public int Height;
        // labels the pixel as belonging to a unique basin or as a part of the watershed line
        public int Label;
        // Distance is a work image of distances
        public int Distance;

        public WatershedPixel()
        {
            this.X = -1;
            this.Y = -1;
            this.Height = -1000;
            this.Label = -1000;
            this.Distance = -1000;
        }

        public WatershedPixel(int x, int y)
        {
            this.X = x;
            this.Y = y;
            this.Height = -1000;
            //this.Label = WatershedCommon.INIT;
            this.Distance = 0;
        }

        public WatershedPixel(int x, int y, int height)
        {
            this.X = x;
            this.Y = y;
            this.Height = height;
            //this.Label = WatershedCommon.INIT;
            this.Distance = 0;
        }

        public override bool Equals(Object obj)
        {
            WatershedPixel p = (WatershedPixel)obj;
            return (X == p.X && X == p.Y);
        }

        public override int GetHashCode()
        {
            return X;
        }
        public override string ToString()
        {
            return "Height = " + Height + "; X = " + X.ToString() + ", Y = " + Y.ToString() +
                   "; Label = " + Label.ToString() + "; Distance = " + Distance.ToString();
        }
    }
}
