using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ArManagement
{
    public class Marker
    {
        #region Variables
        
        private Objects.ARMarkerInfo2 [] marker_info2 = new Objects.ARMarkerInfo2[Config.ARConfiguration.SquareMax];

        #endregion

        #region Methods

        public bool SavePattern(Model.IImage image, Objects.ARMarkerInfo markerInfo, string fileName)
        {
            return true;
        }
        public bool Detect(IntPtr image, byte threshold, Objects.ARMarkerInfo markerInfo, int markerNum)
        {
            return true;
        }
        public bool DetectLite(IntPtr image, byte threshold, Objects.ARMarkerInfo markerInfo, int markerNum)
        {
            return true;
        }
        public bool Detect(IntPtr image, byte threshold, Objects.ARMarkerInfo markerInfo, int markerNum, int lorR)
        {
            return true;
        }
        public bool DetectLite(IntPtr image, byte threshold, Objects.ARMarkerInfo markerInfo, int markerNum, int lorR)
        {
            return true;
        }

        #endregion


        public int GetVertex(int[] x_coord, int[] y_coord, int st, int ed, double thresh, int[] vertex, int vnum)
        {

            double d, dmax;
            double a, b, c;
            int i, v1 = 0;

            a = y_coord[ed] - y_coord[st];
            b = x_coord[st] - x_coord[ed];
            c = x_coord[ed] * y_coord[st] - y_coord[ed] * x_coord[st];
            dmax = 0;
            for (i = st + 1; i < ed; i++)
            {
                d = a * x_coord[i] + b * y_coord[i] + c;
                if (d * d > dmax)
                {
                    dmax = d * d;
                    v1 = i;
                }
            }
            if (dmax / (a * a + b * b) > thresh)
            {
                if (GetVertex(x_coord, y_coord, st, v1, thresh, vertex, vnum) < 0)
                    return (-1);

                if ((vnum) > 5)
                    return (-1);

                vertex[(vnum)] = v1;
                (vnum)++;

                if (GetVertex(x_coord, y_coord, v1, ed, thresh, vertex, vnum) < 0)
                    return (-1);
            }

            return 0;
        }
        public int CheckSquare(int area, Objects.ARMarkerInfo2 marker_info2, double factor)
        {
            int sx, sy;
            int dmax, d, v1;
            int[] vertex = new int[10];
            int vnum;
            int[] wv1 = new int[10];
            int wvnum1;
            int[] wv2 = new int[10];
            int wvnum2, v2;
            double thresh;
            int i;



            dmax = 0;
            v1 = 0;
            sx = marker_info2.x_coord[0];
            sy = marker_info2.y_coord[0];
            for (i = 1; i < marker_info2.Coord_num - 1; i++)
            {
                d = (marker_info2.x_coord[i] - sx) * (marker_info2.x_coord[i] - sx)
                  + (marker_info2.y_coord[i] - sy) * (marker_info2.y_coord[i] - sy);
                if (d > dmax)
                {
                    dmax = d;
                    v1 = i;
                }
            }

            thresh = (area / 0.75) * 0.01 * factor;
            vnum = 1;
            vertex[0] = 0;
            wvnum1 = 0;
            wvnum2 = 0;
            if (GetVertex(marker_info2.x_coord, marker_info2.y_coord, 0, v1, thresh, wv1, wvnum1) < 0)
            {
                return (-1);
            }
            if (GetVertex(marker_info2.x_coord, marker_info2.y_coord, v1, marker_info2.Coord_num - 1, thresh, wv2, wvnum2) < 0)
            {
                return (-1);
            }

            if (wvnum1 == 1 && wvnum2 == 1)
            {
                vertex[1] = wv1[0];
                vertex[2] = v1;
                vertex[3] = wv2[0];
            }
            else if (wvnum1 > 1 && wvnum2 == 0)
            {
                v2 = v1 / 2;
                wvnum1 = wvnum2 = 0;
                if (GetVertex(marker_info2.x_coord, marker_info2.y_coord, 0, v2, thresh, wv1, wvnum1) < 0)
                {
                    return (-1);
                }
                if (GetVertex(marker_info2.x_coord, marker_info2.y_coord, v2, v1, thresh, wv2, wvnum2) < 0)
                {
                    return (-1);
                }
                if (wvnum1 == 1 && wvnum2 == 1)
                {
                    vertex[1] = wv1[0];
                    vertex[2] = wv2[0];
                    vertex[3] = v1;
                }
                else
                {
                    return (-1);
                }
            }
            else if (wvnum1 == 0 && wvnum2 > 1)
            {
                v2 = (v1 + marker_info2.Coord_num - 1) / 2;
                wvnum1 = wvnum2 = 0;
                if (GetVertex(marker_info2.x_coord, marker_info2.y_coord, v1, v2, thresh, wv1, wvnum1) < 0)
                {
                    return (-1);
                }
                if (GetVertex(marker_info2.x_coord, marker_info2.y_coord, v2, marker_info2.Coord_num - 1, thresh, wv2, wvnum2) < 0)
                {
                    return (-1);
                }
                if (wvnum1 == 1 && wvnum2 == 1)
                {
                    vertex[1] = v1;
                    vertex[2] = wv1[0];
                    vertex[3] = wv2[0];
                }
                else
                {
                    return (-1);
                }
            }
            else
            {
                return (-1);
            }

            marker_info2.Vertex[0] = vertex[0];
            marker_info2.Vertex[1] = vertex[1];
            marker_info2.Vertex[2] = vertex[2];
            marker_info2.Vertex[3] = vertex[3];
            marker_info2.Vertex[4] = marker_info2.Coord_num - 1;

            return (0);

        }
        public int GetContour(Model.IImage image, int [] label_ref, int label, int [] clip, Objects.ARMarkerInfo2 marker_info2)
        {

            int [] xdir = new int[] { 0, 1, 1, 1, 0,-1,-1,-1};
            int [] ydir = new int[] {-1,-1, 0, 1, 1, 1, 0,-1};
            int [] wx = new int[Objects.ARMarkerInfo2.AR_CHAIN_MAX];
            int [] wy = new int[Objects.ARMarkerInfo2.AR_CHAIN_MAX];
            int p1;
            int xsize, ysize;
            int sx = 0, sy = 0, dir;
            int dmax, d, v1 = 0;
            int i, j;

            if (Config.ARConfiguration.Current.ImageProcMode == Config.ARImageProcMode.Half)
            {
                xsize = image.Image.Width / 2;
                ysize = image.Image.Height / 2;
            }
            else
            {
                xsize = image.Image.Width;
                ysize = image.Image.Height;
            }
            j = clip[2];
            p1 = image.Bytes[j * xsize + clip[0]];
            for (i = clip[0]; i <= clip[1]; i++, p1++)
            {
                if (p1 > 0 && label_ref[(p1) - 1] == label)
                {
                    sx = i; sy = j; break;
                }
            }
            if (i > clip[1])
            {
                //printf("??? 1\n"); return (-1);
            }

            marker_info2.Coord_num = 1;
            marker_info2.x_coord[0] = sx;
            marker_info2.y_coord[0] = sy;
            dir = 5;
            for (; ; )
            {
                p1 = image.Bytes[marker_info2.y_coord[marker_info2.Coord_num - 1] * xsize
                            + marker_info2.x_coord[marker_info2.Coord_num - 1]];
                dir = (dir + 5) % 8;
                for (i = 0; i < 8; i++)
                {
                    //if (p1[ydir[dir] * xsize + xdir[dir]] > 0) break;
                    dir = (dir + 1) % 8;
                }
                if (i == 8)
                {
                    //printf("??? 2\n"); return (-1);
                }
                marker_info2.x_coord[marker_info2.Coord_num] = marker_info2.x_coord[marker_info2.Coord_num - 1] + xdir[dir];
                marker_info2.y_coord[marker_info2.Coord_num] = marker_info2.y_coord[marker_info2.Coord_num - 1] + ydir[dir];
                if (marker_info2.x_coord[marker_info2.Coord_num] == sx
                 && marker_info2.y_coord[marker_info2.Coord_num] == sy) break;
                marker_info2.Coord_num++;
                if (marker_info2.Coord_num == Objects.ARMarkerInfo2.AR_CHAIN_MAX - 1)
                {
                    //printf("??? 3\n"); return (-1);
                }
            }

            dmax = 0;
            for (i = 1; i < marker_info2.Coord_num; i++)
            {
                d = (marker_info2.x_coord[i] - sx) * (marker_info2.x_coord[i] - sx)
                  + (marker_info2.y_coord[i] - sy) * (marker_info2.y_coord[i] - sy);
                if (d > dmax)
                {
                    dmax = d;
                    v1 = i;
                }
            }

            for (i = 0; i < v1; i++)
            {
                wx[i] = marker_info2.x_coord[i];
                wy[i] = marker_info2.y_coord[i];
            }
            for (i = v1; i < marker_info2.Coord_num; i++)
            {
                marker_info2.x_coord[i - v1] = marker_info2.x_coord[i];
                marker_info2.y_coord[i - v1] = marker_info2.y_coord[i];
            }
            for (i = 0; i < v1; i++)
            {
                marker_info2.x_coord[i - v1 + marker_info2.Coord_num] = wx[i];
                marker_info2.y_coord[i - v1 + marker_info2.Coord_num] = wy[i];
            }
            marker_info2.x_coord[marker_info2.Coord_num] = marker_info2.x_coord[0];
            marker_info2.y_coord[marker_info2.Coord_num] = marker_info2.y_coord[0];
            marker_info2.Coord_num++;

            return 0;
        }
        public int Detect(Model.IImage dataPtr, int thresh, Objects.ARMarkerInfo marker_info, int marker_num)
        {
            Model.IImage limage;
            int label_num;
            int area, clip, label_ref;
            double pos;
            double rarea, rlen, rlenmin;
            double diff, diffmin;
            int cid, cdir;
            int i, j, k;

            //    marker_num = 0;

            //    limage = arLabeling( dataPtr, thresh, label_num, area, pos, clip, label_ref );
            //    if( limage == 0 )    return -1;

            //    marker_info2 = arDetectMarker2( limage, label_num, label_ref,
            //                                    area, pos, clip, AR_AREA_MAX, AR_AREA_MIN,
            //                                    1.0, &wmarker_num);
            //    if( marker_info2 == 0 ) return -1;

            //    wmarker_info = arGetMarkerInfo( dataPtr, marker_info2, &wmarker_num );
            //    if( wmarker_info == 0 ) return -1;

            //    for( i = 0; i < prev_num; i++ ) {
            //        rlenmin = 10.0;
            //        cid = -1;
            //        for( j = 0; j < wmarker_num; j++ ) {
            //            rarea = (double)prev_info[i].marker.area / (double)wmarker_info[j].area;
            //            if( rarea < 0.7 || rarea > 1.43 ) continue;
            //            rlen = ( (wmarker_info[j].pos[0] - prev_info[i].marker.pos[0])
            //                   * (wmarker_info[j].pos[0] - prev_info[i].marker.pos[0])
            //                   + (wmarker_info[j].pos[1] - prev_info[i].marker.pos[1])
            //                   * (wmarker_info[j].pos[1] - prev_info[i].marker.pos[1]) ) / wmarker_info[j].area;
            //            if( rlen < 0.5 && rlen < rlenmin ) {
            //                rlenmin = rlen;
            //                cid = j;
            //            }
            //        }
            //        if( cid >= 0 && wmarker_info[cid].cf < prev_info[i].marker.cf ) {
            //            wmarker_info[cid].cf = prev_info[i].marker.cf;
            //            wmarker_info[cid].id = prev_info[i].marker.id;
            //            diffmin = 10000.0 * 10000.0;
            //            cdir = -1;
            //            for( j = 0; j < 4; j++ ) {
            //                diff = 0;
            //                for( k = 0; k < 4; k++ ) {
            //                    diff += (prev_info[i].marker.vertex[k][0] - wmarker_info[cid].vertex[(j+k)%4][0])
            //                          * (prev_info[i].marker.vertex[k][0] - wmarker_info[cid].vertex[(j+k)%4][0])
            //                          + (prev_info[i].marker.vertex[k][1] - wmarker_info[cid].vertex[(j+k)%4][1])
            //                          * (prev_info[i].marker.vertex[k][1] - wmarker_info[cid].vertex[(j+k)%4][1]);
            //                }
            //                if( diff < diffmin ) {
            //                    diffmin = diff;
            //                    cdir = (prev_info[i].marker.dir - j + 4) % 4;
            //                }
            //            }
            //            wmarker_info[cid].dir = cdir;
            //        }
            //    }

            //    for( i = 0; i < wmarker_num; i++ ) {
            ///*
            //    printf("cf = %g\n", wmarker_info[i].cf);
            //*/
            //        if( wmarker_info[i].cf < 0.5 ) wmarker_info[i].id = -1;
            //   }


            ///*------------------------------------------------------------*/

            //    for( i = j = 0; i < prev_num; i++ ) {
            //        prev_info[i].count++;
            //        if( prev_info[i].count < 4 ) {
            //            prev_info[j] = prev_info[i];
            //            j++;
            //        }
            //    }
            //    prev_num = j;

            //    for( i = 0; i < wmarker_num; i++ ) {
            //        if( wmarker_info[i].id < 0 ) continue;

            //        for( j = 0; j < prev_num; j++ ) {
            //            if( prev_info[j].marker.id == wmarker_info[i].id ) break;
            //        }
            //        prev_info[j].marker = wmarker_info[i];
            //        prev_info[j].count  = 1;
            //        if( j == prev_num ) prev_num++;
            //    }

            //    for( i = 0; i < prev_num; i++ ) {
            //        for( j = 0; j < wmarker_num; j++ ) {
            //            rarea = (double)prev_info[i].marker.area / (double)wmarker_info[j].area;
            //            if( rarea < 0.7 || rarea > 1.43 ) continue;
            //            rlen = ( (wmarker_info[j].pos[0] - prev_info[i].marker.pos[0])
            //                   * (wmarker_info[j].pos[0] - prev_info[i].marker.pos[0])
            //                   + (wmarker_info[j].pos[1] - prev_info[i].marker.pos[1])
            //                   * (wmarker_info[j].pos[1] - prev_info[i].marker.pos[1]) ) / wmarker_info[j].area;
            //            if( rlen < 0.5 ) break;
            //        }
            //        if( j == wmarker_num ) {
            //            wmarker_info[wmarker_num] = prev_info[i].marker;
            //            wmarker_num++;
            //        }
            //    }


            //    *marker_num  = wmarker_num;
            //    *marker_info = wmarker_info;

            return 0;
        }
        private Objects.ARMarkerInfo2  Detect2(Model.IImage limage, int label_num, int [] label_ref, int [] warea, double [] wpos, int [] wclip, int area_max, int area_min, double factor, int marker_num)
        {

            Objects.ARMarkerInfo2 pm;
            int xsize, ysize;
            int marker_num2;
            int i, j, ret = 0;
            double d;

            if (Config.ARConfiguration.Current.ImageProcMode == Config.ARImageProcMode.Half)
            //if (arImageProcMode == AR_IMAGE_PROC_IN_HALF)
            {
                area_min /= 4;
                area_max /= 4;
                xsize = limage.Image.Width / 2;
                ysize = limage.Image.Height / 2;
                //xsize = arImXsize / 2;
                //ysize = arImYsize / 2;
            }
            else
            {
                xsize = limage.Image.Width;
                ysize = limage.Image.Height;
                //xsize = arImXsize;
                //ysize = arImYsize;
            }
            marker_num2 = 0;
            for (i = 0; i < label_num; i++)
            {
                if (warea[i] < area_min || warea[i] > area_max) continue;
                if (wclip[i * 4 + 0] == 1 || wclip[i * 4 + 1] == xsize - 2) continue;
                if (wclip[i * 4 + 2] == 1 || wclip[i * 4 + 3] == ysize - 2) continue;

                //ret = GetContour(limage, label_ref, i + 1, wclip[i * 4], marker_info2[marker_num2]);
                if (ret < 0) continue;

                ret = CheckSquare(warea[i], marker_info2[marker_num2], factor);
                if (ret < 0) continue;

                marker_info2[marker_num2].Area = warea[i];
                marker_info2[marker_num2].Pos[0] = wpos[i * 2 + 0];
                marker_info2[marker_num2].Pos[1] = wpos[i * 2 + 1];
                marker_num2++;
                if (marker_num2 == Config.ARConfiguration.SquareMax) break;
            }

            for (i = 0; i < marker_num2; i++)
            {
                for (j = i + 1; j < marker_num2; j++)
                {
                    d = (marker_info2[i].Pos[0] - marker_info2[j].Pos[0])
                      * (marker_info2[i].Pos[0] - marker_info2[j].Pos[0])
                      + (marker_info2[i].Pos[1] - marker_info2[j].Pos[1])
                      * (marker_info2[i].Pos[1] - marker_info2[j].Pos[1]);
                    if (marker_info2[i].Area > marker_info2[j].Area)
                    {
                        if (d < marker_info2[i].Area / 4)
                        {
                            marker_info2[j].Area = 0;
                        }
                    }
                    else
                    {
                        if (d < marker_info2[j].Area / 4)
                        {
                            marker_info2[i].Area = 0;
                        }
                    }
                }
            }
            for (i = 0; i < marker_num2; i++)
            {
                if (marker_info2[i].Area == 0.0)
                {
                    for (j = i + 1; j < marker_num2; j++)
                    {
                        marker_info2[j - 1] = marker_info2[j];
                    }
                    marker_num2--;
                }
            }

            if (Config.ARConfiguration.Current.ImageProcMode == Config.ARImageProcMode.Half)
            {
                pm = marker_info2[0];
                for (i = 0; i < marker_num2; i++)
                {
                    pm.Area *= 4;
                    pm.Pos[0] *= 2.0;
                    pm.Pos[1] *= 2.0;
                    for (j = 0; j < pm.Coord_num; j++)
                    {
                        pm.x_coord[j] *= 2;
                        pm.y_coord[j] *= 2;
                    }
                    //pm++;
                }
            }

            marker_num = marker_num2;
            return (marker_info2[0]);
        }

    }
}
