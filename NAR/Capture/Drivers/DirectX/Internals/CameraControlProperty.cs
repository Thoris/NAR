using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Capture.Drivers.DirectX.Internals
{
    /// <summary>
    /// From CameraControlProperty
    /// </summary>
    public enum CameraControlProperty
    {
        /// <summary>
        /// Pan Control.
        /// </summary>
        Pan = 0,
        /// <summary>
        /// Tilt Control.
        /// </summary>
        Tilt,
        /// <summary>
        /// Roll Control.
        /// </summary>
        Roll,
        /// <summary>
        /// Zoom Control.
        /// </summary>
        Zoom,
        /// <summary>
        /// Exposure Control.
        /// </summary>
        Exposure,
        /// <summary>
        /// Iris Control.
        /// </summary>
        Iris,
        /// <summary>
        /// Focus Control.
        /// </summary>
        Focus
    }
}
