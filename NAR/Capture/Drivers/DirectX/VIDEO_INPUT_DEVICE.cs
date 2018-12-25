using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Capture.Drivers.DirectX
{
    public enum VIDEO_INPUT_DEVICE
    {
        WDM_VIDEO_CAPTURE_FILTER = 0, // select video source from among WDM Streaming Capture Device filters in 
        // class CLSID_VideoInputDeviceCategory.
        ASYNC_FILE_INPUT_FILTER = 1, // read video from an AVI file (through an asynchronous file input filter)
        INVALID_INPUT_FILTER = 2     // do not use
    }
}
