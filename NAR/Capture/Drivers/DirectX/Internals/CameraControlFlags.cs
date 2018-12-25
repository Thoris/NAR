using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Capture.Drivers.DirectX.Internals
{
    /// <summary>
    /// From CameraControlFlags
    /// </summary>
    [Flags]
    public enum CameraControlFlags
    {
        /// <summary>
        /// No Control Flag.
        /// </summary>
        None = 0x0,
        /// <summary>
        /// Auto Control Flag.
        /// </summary>
        Auto = 0x0001,
        /// <summary>
        /// Manual Control Flag.
        /// </summary>
        Manual = 0x0002
    }
}
