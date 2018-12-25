using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ArManagement.Objects
{
    /// <summary>
    /// Brief main structure for detected marker.
    /// Store information after contour detection (in idea screen coordinate, after distorsion compensated).
    /// <remarks>
    /// lines are represented by 3 values a,b,c for ax+by+c=0
    /// </remarks>
    /// </summary>
    public class ARMarkerInfo
    {
        /// <summary>
        /// Number of pixels in the labeled region
        /// </summary>
        public int          Area;
        /// <summary>
        /// Marker identitied number
        /// </summary>
        public int          Id;
        /// <summary>
        /// Direction that tells about the rotation about the marker (possible values are 0, 1, 2 or 3). 
        /// This parameter makes it possible to tell about the line order of the detected marker 
        /// (so which line is the first one) and so find the first vertex. 
        /// This is important to compute the transformation matrix in arGetTransMat().
        /// </summary>
        public int          Dir;
        /// <summary>
        /// Confidence value (probability to be a marker)
        /// </summary>
        public double       Cf;
        /// <summary>
        /// Center of marker (in ideal screen coordinates)
        /// </summary>
        public double[]     Pos = new double[2];
        /// <summary>
        /// Line equations for four side of the marker (in ideal screen coordinates)
        /// </summary>
        public double[,]    Line = new double[4, 3];
        /// <summary>
        /// Edge points of the marker (in ideal screen coordinates)
        /// </summary>
        public double[,]    Vertex = new double[4, 2];
    }



}
