using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace NAR.Capture.Drivers.DirectX.Internals
{
    [StructLayout(LayoutKind.Sequential), ComVisible(false)]
    public class DsOptInt64
    {
        public DsOptInt64(long Value)
        {
            this.Value = Value;
        }
        public long Value;
    }
}
