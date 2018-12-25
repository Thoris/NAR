using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Capture.Drivers.DirectX
{
    public enum ASYNC_INPUT_FLAGS
    {
        ASYNC_INPUT_DO_NOT_USE_CLOCK = 0x0100, // call IMediaFilter::SetSyncSource(NULL) on async file source filter
        // this will effectively prevent samples from being dropped, but
        // may result in 100% CPU usage on single-processor systems
        ASYNC_LOOP_VIDEO = 0x0200, // continuously loops through an input file
        ASYNC_RENDER_SECONDARY_STREAMS = 0x0400  // The Async File Source Filter is always used in combination with an
        // AVI Splitter Filter (CLSID_AviSplitter), where Stream 0 is assumed
        // to contain video data (MEDIATYPE_Video). This flag instructs DSVL_GraphManager
        // to call IGraphBuilder->Render(IPin*) on Streams 1..n
        // Set this flag if your AVI file contains an audio stream AND you want it
        // rendered by DirectSound (usually through CLSID_DSoundRender).
        // DO NOT SET this flag if your AVI file contains more than one video stream
        // not yet implemented:
        //	ASYNC_SHOW_DECODER_DIALOG      = 0x0800, // displays either the video decoder's property pages


    } ;
}
