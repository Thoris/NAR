using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ArManagement.Objects
{

    /// <summary>
    /// Internal structure use for marker detection.
    /// Store information after contour detection (in observed screen coordinate, before distorsion correction).
    /// <remarks>
    /// The first vertex is stored again as the 5th entry in the array – for convenience of drawing a line-strip easier.
    /// </remarks>
    /// </summary>
    public class ARMarkerInfo2
    {
        public const int AR_CHAIN_MAX = 10000;

        /// <summary>
        /// Number of pixels in the labeled region
        /// </summary>
        public int Area;
        /// <summary>
        /// Position of the center of the marker (in observed screen coordinates)
        /// </summary>
        public double[] Pos = new double[2];
        /// <summary>
        /// Number of pixels in the contour.
        /// </summary>
        public int Coord_num;
        /// <summary>
        /// X coordinate of the pixels of contours (size limited by AR_CHAIN_MAX).
        /// </summary>
        public int[] x_coord = new int[AR_CHAIN_MAX];
        /// <summary>
        /// Y coordinate of the pixels of contours (size limited by AR_CHAIN_MAX).
        /// </summary>
        public int[] y_coord = new int[AR_CHAIN_MAX];
        /// <summary>
        /// Position of the vertices of the marker. (in observed screen coordinates)
        /// </summary>
        public int[] Vertex = new int[5];
    }

}
