using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Capture.Drivers.DirectX
{
    public enum PIXELFORMAT
    {
        PIXELFORMAT_UNKNOWN = 0,
        PIXELFORMAT_UYVY = 1,
        PIXELFORMAT_YUY2 = 2,
        PIXELFORMAT_RGB565 = 3,
        PIXELFORMAT_RGB555 = 4,
        PIXELFORMAT_RGB24 = 5,
        PIXELFORMAT_RGB32 = 6,
        PIXELFORMAT_INVALID = 7,
        PIXELFORMAT_QUERY = 8,
        PIXELFORMAT_ENUM_MAX = 9
    } ;
}
