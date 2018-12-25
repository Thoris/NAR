using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace NAR.Capture.Drivers.Echo.Items
{
    public abstract class TranslationBase : BaseData
    {
        #region Variables

        private int _maxPixelTranslate = 4;
        private int _nextPosX = 1;
        private int _nextPosY = 1;
        private Point _minimumPos;
        private Point _maximumPos;
        private Point _currentPos;
        private Random _random = new Random();

        #endregion

        #region Properties

        public Point MinimumPos
        {
            get { return _minimumPos; }
            set { _minimumPos = value; }
        }
        public Point MaximumPos
        {
            get { return _maximumPos; }
            set { _maximumPos = value; }
        }
        public Point CurrentPos
        {
            get { return _currentPos; }
            set { _currentPos = value; }
        }
        
        #endregion

        #region Methods

        protected void Translate()
        {
            int x = _currentPos.X + _nextPosX;
            int y = _currentPos.Y + _nextPosY;

            if (x < _minimumPos.X)
            {
                _nextPosX = _random.Next(_maxPixelTranslate);
            }
            if (y < _minimumPos.Y)
            {
                _nextPosY = _random.Next(_maxPixelTranslate);
            }
            if (x > _maximumPos.X - base.Size.X)
            {
                _nextPosX = _random.Next(_maxPixelTranslate) * (-1);
            }
            if (y > _maximumPos.Y - base.Size.Y)
            {
                _nextPosY = _random.Next(_maxPixelTranslate) * (-1);
            }


            _currentPos = new Point(x, y);
        }

        #endregion
    }
}
