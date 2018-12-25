// ------------------------------------------------------------------
// DirectX.Capture
//
// History:
//	2003-Jan-24		BL		- created
//
// Copyright (c) 2003 Brian Low
// ------------------------------------------------------------------

using System;

namespace NAR.Capture.Drivers.DirectShow.DSCapture
{
	/// <summary>
	///  Exception thrown when the device cannot be rendered or started.
	/// </summary>
	public class DeviceInUseException : SystemException
    {
        #region Constructors/Destructors
        /// <summary>
        /// Initializes a new instance with the specified HRESULT
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="hResult"></param>
		public DeviceInUseException(string deviceName, int hResult) : base( deviceName + " is in use or cannot be rendered. (" + hResult + ")" )
		{
        }
        #endregion
    }
}
