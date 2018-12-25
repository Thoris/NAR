using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace NAR.Capture.Drivers.DirectX.Internals
{
    [ComVisible(true), ComImport,
     Guid("89c31040-846b-11ce-97d3-00aa0055595a"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumMediaTypes
    {
        [PreserveSig]
        int Next([In] int cMediaTypes,
           [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] AMMediaType[] ppMediaTypes,
           [Out] out int pcFetched);

        [PreserveSig]
        int Skip([In] int cMediaTypes);
        int Reset();
        int Clone([Out] out IEnumMediaTypes ppEnum);
    }
}
