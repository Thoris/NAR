using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ArManagement.Objects
{

    /** \struct ARParam
    * \brief camera intrinsic parameters.
    * 
    * This structure contains the main parameters for
    * the intrinsic parameters of the camera
    * representation. The camera used is a pinhole
    * camera with standard parameters. User should
    * consult a computer vision reference for more
    * information. (e.g. Three-Dimensional Computer Vision 
    * (Artificial Intelligence) by Olivier Faugeras).
    * \param xsize length of the image (in pixels).
    * \param ysize height of the image (in pixels).
    * \param mat perspective matrix (K).
    * \param dist_factor radial distortions factor
    *          dist_factor[0]=x center of distortion
    *          dist_factor[1]=y center of distortion
    *          dist_factor[2]=distortion factor
    *          dist_factor[3]=scale factor
    */


    public class ARParam
    {
        public int xsize, ysize;
        public double[,] mat = new double[3, 4];
        public double[] dist_factor = new double[4];
    }
}
