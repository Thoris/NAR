using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace NAR.Capture.Drivers.Echo.Items
{
    public class RotateBase : TranslationBase, IDataItem
    {
        #region Variables

        private Point[] _points = new Point[4];
        private bool _initialized = false;

        #endregion

        #region Properties

        public Point[] Points
        {
            get { return _points; }
            set { _points = value; }
        }

        #endregion

        #region Constructors/Destructors
        public RotateBase()
            : base ()
        {
            

        }
        #endregion

        #region IDataItem Members

        public Bitmap Draw(Bitmap source)
        {
            return source;
        }

        public void Move()
        {
            if (!_initialized)
            {
                _initialized = true;

                _points[0].X = base.CurrentPos.X;
                _points[0].Y = base.CurrentPos.Y;

                _points[1].X = base.Size.X;
                _points[1].Y = base.CurrentPos.Y;

                _points[2].X = base.CurrentPos.X;
                _points[2].Y = base.Size.Y;

                _points[3].X = base.Size.X;
                _points[3].Y = base.Size.Y;

            }
        }

        #endregion
    }
}
