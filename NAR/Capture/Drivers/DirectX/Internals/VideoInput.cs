using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Capture.Drivers.DirectX.Internals
{
    /// <summary>
    /// Video input of a capture board.
    /// </summary>
    /// 
    /// <remarks><para>The class is used to describe video input of devices like video capture boards,
    /// which usually provide several inputs.</para>
    /// </remarks>
    /// 
    public class VideoInput
    {
        #region Variables

        /// <summary>
        /// Index of the video input.
        /// </summary>
        public readonly int Index;

        /// <summary>
        /// Type of the video input.
        /// </summary>
        public readonly PhysicalConnectorType Type;

        #endregion

        #region Properties

        /// <summary>
        /// Default video input. Used to specify that it should not be changed.
        /// </summary>
        public static VideoInput Default
        {
            get { return new VideoInput(-1, PhysicalConnectorType.Default); }
        }

        #endregion

        #region Constructors/Destructors

        internal VideoInput(int index, PhysicalConnectorType type)
        {
            Index = index;
            Type = type;
        }

        #endregion

    }
}
