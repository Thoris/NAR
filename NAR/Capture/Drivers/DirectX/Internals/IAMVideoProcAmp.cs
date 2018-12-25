using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace NAR.Capture.Drivers.DirectX.Internals
{
    [ComVisible(true), ComImport,
    Guid("C6E13360-30AC-11d0-A18C-00A0C9118956"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMVideoProcAmp
    {
        [PreserveSig]
        int GetRange(VideoProcAmpProperty property, [Out] out int min, [Out] out int max, [Out] out int steppingDelta, [Out] out int defaultValue, out int capFlags);
        //int GetRange( int prop, [Out] out int min, [Out] out int max, [Out] out int steppingDelta, [Out] out int defaultValue, out int capFlags);

        [PreserveSig]
        int Set(VideoProcAmpProperty property, int value, int flags);

        [PreserveSig]
        int Get(VideoProcAmpProperty property, out int value, out int flags);

    }
}
