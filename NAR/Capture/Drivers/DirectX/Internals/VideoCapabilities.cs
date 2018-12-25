using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace NAR.Capture.Drivers.DirectX.Internals
{
    /// <summary>
    /// Capabilities of video device such as frame size and frame rate.
    /// </summary>
    public class VideoCapabilities
    {
        #region Variables

        /// <summary>
        /// Frame size supported by video device.
        /// </summary>
        public readonly Size FrameSize;
        /// <summary>
        /// Frame rate supported by video device for corresponding <see cref="FrameSize">frame size</see>.
        /// </summary>
        public readonly int FrameRate;

        #endregion

        #region Constructors/Destructors

        internal VideoCapabilities() { }
        // Retrieve capabilities of a video device
        internal VideoCapabilities(IAMStreamConfig videoStreamConfig, int index)
        {
            AMMediaType mediaType = null;
            VideoStreamConfigCaps caps = new VideoStreamConfigCaps();

            try
            {
                // retrieve capabilities struct at the specified index
                int hr = videoStreamConfig.GetStreamCaps(index, out mediaType, caps);

                if (hr != 0)
                    Marshal.ThrowExceptionForHR(hr);

                // extract info
                FrameSize = caps.InputSize;
                FrameRate = (int)(10000000 / caps.MinFrameInterval);
            }
            finally
            {
                if (mediaType != null)
                    mediaType.Dispose();
            }
        }

        #endregion

        #region Methods

        // Retrieve capabilities of a video device
        static internal VideoCapabilities[] FromStreamConfig(IAMStreamConfig videoStreamConfig)
        {
            if (videoStreamConfig == null)
                throw new ArgumentNullException("videoStreamConfig");

            // ensure this device reports capabilities
            int count, size;
            int hr = videoStreamConfig.GetNumberOfCapabilities(out count, out size);

            if (hr != 0)
                Marshal.ThrowExceptionForHR(hr);

            if (count <= 0)
                throw new NotSupportedException("This video device does not report capabilities.");

            if (size > Marshal.SizeOf(typeof(VideoStreamConfigCaps)))
                throw new NotSupportedException("Unable to retrieve video device capabilities. This video device requires a larger VideoStreamConfigCaps structure.");

            // group capabilities with similar parameters
            Dictionary<ulong, VideoCapabilities> videocapsList = new Dictionary<ulong, VideoCapabilities>();

            for (int i = 0; i < count; i++)
            {
                try
                {
                    VideoCapabilities vc = new VideoCapabilities(videoStreamConfig, i);

                    ulong key = (((ulong)vc.FrameSize.Height) << 32) |
                                (((ulong)vc.FrameSize.Width) << 16) |
                                  ((ulong)(uint)vc.FrameRate);

                    if (!videocapsList.ContainsKey(key))
                    {
                        videocapsList.Add(key, vc);
                    }
                }
                catch
                {
                }
            }

            VideoCapabilities[] videocaps = new VideoCapabilities[videocapsList.Count];
            videocapsList.Values.CopyTo(videocaps, 0);

            return videocaps;
        }

        #endregion

    }
}
