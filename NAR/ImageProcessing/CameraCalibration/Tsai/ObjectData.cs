using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ImageProcessing.CameraCalibration.Tsai
{
    public class ObjectData
    {
        char[] name;//[256];
        int id;
        int visible;
        double[,] marker_coord; //[4][2];
        double[,] trans;//[3][4];
        int vrml_id;
        int vrml_id_orig;
        double[] marker_width;//[2];
        double[] deslocamento;//[3];
        double[] marker_center;//[2];
        double[] tam_caixa;//[3];
    }
}
