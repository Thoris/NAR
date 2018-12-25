using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Capture.Drivers.DirectX
{
    public enum VIDEO_INPUT_FLAGS // can be combined, i.e. (WDM_SHOW_DIALOG|WDM_MATCH_FILTER_NAME)
    {
        WDM_MATCH_FORMAT = 0x0001, // first media type matching the a given set of format requirements
        WDM_SHOW_FORMAT_DIALOG = 0x0002, // displays either the capture pin's property pages (non-DV cameras) or
        // the DV-decoder filter's format dialog before connecting anything
        WDM_SHOW_CONTROL_DIALOG = 0x0004, // displays the source filter's property pages before connecting anything
        WDM_MATCH_FILTER_NAME = 0x0008, // tries find a match based on a filter name substring, i.e. "QuickCam" or "Sony"
        WDM_MATCH_IEEE1394_ID = 0x0010, // match filter based on a unique 64-bit IEEE 1394 device ID (MSDV driver only)
        // this technique allows to explicitly choose between different cameras connected
        // to the same IEEE 1394 bus. Setting this flag usually requires use of the WDM
        // MSDV driver(qdv.dll)
        // >> use "/bin/IEEE1394_id.exe" to determine your camera's ID.
        WDM_USE_MAX_FRAMERATE = 0x0020  // uses the max. available frame rate for the format requested

    } ;
}
