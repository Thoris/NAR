using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace NAR.Capture.Drivers.Echo.Items
{
    public interface IDataItem
    {
        bool Fill { get; set; }
        Color FillColor { get; set; }
        Point CurrentPos { get; set; }
        Color Forecolor { get; set; }
        Point MinimumPos { get; set; }
        Point MaximumPos { get; set; }
        int LineLenght { get; set; }
        
        Bitmap Draw(Bitmap source);
        void Move();

    }
}
