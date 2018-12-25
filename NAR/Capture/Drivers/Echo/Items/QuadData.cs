using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace NAR.Capture.Drivers.Echo.Items
{
    public class QuadData : TranslationBase, IDataItem
    {

        #region Constructors/Destructors

        public QuadData()
        {
        }

        #endregion

        #region IDataItem Members

        public System.Drawing.Bitmap Draw(System.Drawing.Bitmap source)
        {
            Bitmap img = (Bitmap)source.Clone();

            Graphics g = Graphics.FromImage(img);

            Pen myPen = new Pen(base.Forecolor, base.LineLenght);
            Brush myBrush = new SolidBrush(base.FillColor);

            if (base.Fill)            
                g.FillRectangle(myBrush, base.CurrentPos.X, base.CurrentPos.Y, base.Size.X, base.Size.Y);
            
            
            g.DrawRectangle(myPen, base.CurrentPos.X, base.CurrentPos.Y, base.Size.X, base.Size.Y);

            myPen.Dispose();

            g.Dispose();

            return img;
        }

        public void Move()
        {
            base.Translate();
        }

        #endregion
    }
}
