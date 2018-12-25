using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Capture.Drivers.DirectX
{
    public class AM_MEDIA_TYPE
    {
        public Guid majortype;
        public Guid subtype;
        public bool bFixedSizeSamples;
        public bool bTemporalCompression;
        public ulong lSampleSize;
        public Guid formattype;
        public IntPtr pUnk;
        public ulong cbFormat;
        public byte pbFormat;
    } 	;
}
