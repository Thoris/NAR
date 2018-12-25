using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace NAR.Capture.Drivers.Echo.Items
{
    public class CircleData : TranslationBase, IDataItem
    {
        #region Variables

        #endregion

        #region Properties
        #endregion

        #region Constructors/Destructors

        public CircleData()
        {
            
        }

        #endregion

        #region Methods

        public void Move()
        {
            base.Translate();
            
        }
        public Bitmap Draw(Bitmap source)
        {
            Bitmap img = (Bitmap)source.Clone();

            Graphics g = Graphics.FromImage(img);

            Pen myPen = new Pen(base.Forecolor, base.LineLenght);

            Brush myBrush = new SolidBrush(base.FillColor);

            if (base.Fill)
                g.FillEllipse(myBrush, base.CurrentPos.X, base.CurrentPos.Y, base.Size.X, base.Size.Y);
            
            g.DrawEllipse(myPen, base.CurrentPos.X, base.CurrentPos.Y, base.Size.X, base.Size.Y);

            myPen.Dispose();

            g.Dispose();

            return img;

        }

        #endregion
        
    }
}
