using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAR.Capture.Drivers.DirectX.Internals;

namespace NAR.Capture.Drivers.DirectX
{
    public class MemoryBufferEntry
    {
        public int use_count;
	    public TimeSpan timestamp;
	    public IMediaSample media_sample;
        public byte[] ptr;
    }
}
