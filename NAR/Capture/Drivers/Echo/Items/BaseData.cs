using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace NAR.Capture.Drivers.Echo.Items
{
    public abstract class BaseData
    {
        #region Variables

        private Point _size = new Point(50, 50);
        private Color _foreColor = Color.Blue;
        private int _lineLen = 2;
        private bool _fill;
        private Color _fillColor = Color.White;

        #endregion

        #region Properties

        public Point Size
        {
            get { return _size; }
            set { _size = value; }
        }
        public Color Forecolor
        {
            get { return _foreColor; }
            set { _foreColor = value; }
        }
        public int LineLenght
        {
            get { return _lineLen; }
            set { _lineLen = value; }
        }
        public bool Fill
        {
            get { return _fill; }
            set { _fill = value; }
        }
        public Color FillColor
        {
            get { return _fillColor; }
            set { _fillColor = value; }
        }

        #endregion

    }
}
