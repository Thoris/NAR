using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Capture.Drivers.DirectX
{
    public class DS_MEDIA_FORMAT
    {
        public VIDEO_INPUT_DEVICE inputDevice;
        public int biWidth;
        public int biHeight;
        public double frameRate;
        public Guid subtype;
        public bool isInterlaced;
        public string sourceFilterName;   // (inputDevice == WDM_VIDEO_CAPTURE_FILTER): WDM capture filter's friendly/device name
        // (inputDevice == ASYNC_FILE_INPUT_FILTER):  input file name
        public bool isDeviceName;         // false: friendly name, true: device name

        public int inputFlags;          // (inputDevice == WDM_VIDEO_CAPTURE_FILTER): combination of ASYNC_INPUT_FLAGS
        // (inputDevice == ASYNC_FILE_INPUT_FILTER):  combination of VIDEO_INPUT_FLAGS
        // NOTE: don't forget to set (defaultInputFlags = false) if you use custom flags!
        public bool defaultInputFlags;
        public PIXELFORMAT pixel_format;  // default: PIXELFORMAT_RGB32
        public string ieee1394_id;         // unique 64-bit camera identifier (IEEE 1394 devices only)
        // will be ignored/set to zero for non-IEEE 1394 cameras

        public bool flipH; // applies to PIXELFORMAT_RGB32 only
        public bool flipV; // applies to PIXELFORMAT_RGB32 only
    }
}
