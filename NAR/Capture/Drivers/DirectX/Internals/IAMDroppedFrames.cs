using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace NAR.Capture.Drivers.DirectX.Internals
{
     [ComImport,
    Guid("29840822-5B84-11D0-BD3B-00A0C911CE86"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMDroppedFrames
    {
         /// <summary>
         /// Retrieves the total number of frames that the pin dropped since it last started streaming.
         /// </summary>
         /// <param name="plDropped">Pointer to the total number of dropped frames</param>
         /// <returns>S_OK = Success, E_FAIL = Failure, E_INVALIDARG = Invalid argument</returns>
         int GetNumDropped([Out] out long plDropped);

         /// <summary>
         /// Retrieves the total number of frames that the pin delivered downstream (did not drop).
         /// </summary>
         /// <param name="plNotDropped">Pointer to the total number of frames that were not dropped</param>
         /// <returns>S_OK = Success, E_FAIL = Failure, E_INVALIDARG = Invalid argument</returns>
         int GetNumNotDropped([Out] out long plNotDropped);

         /// <summary>
         /// Retrieves the average size of frames that the pin dropped.
         /// </summary>
         /// <param name="plAverageSize">Pointer to the average size of frames sent out by the pin since the pin started streaming, in bytes</param>
         /// <returns>S_OK = Success, E_FAIL = Failure, E_INVALIDARG = Invalid argument</returns>
         int GetAverageFrameSize([Out] out long plAverageSize);

    }
}
