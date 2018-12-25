

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace NAR.ARKit
{
    


    public class ARToolKit
    {

        public class ARVec
        {
            public double []v;
            public int    clm;
        } ;

        public class ARMat
        {
	        public double []m;
	        public int row;
	        public int clm;
        };

        public class ARMarkerInfo
        {
            public int area;
            public int id;
            public int dir;
            public double cf;
            public double[] pos;
            public double[,] line;
            public double[,] vertex;

            public ARMarkerInfo()
            {
                pos = new double[2];
                line = new double[4, 3];
                vertex = new double[4, 2];
            }
        } ;

  
        public class arPrevInfo
        {
            public ARMarkerInfo  marker;
            public int     count;

            public arPrevInfo()
            {
                marker = new ARMarkerInfo();
            }
        };

        public class ARMarkerInfo2
        {
            public int area;
            public double[] pos;//= new double [2];
            public int coord_num;
            public int[] x_coord;// = new int [10000];
            public int[] y_coord;//= new int [10000];
            public int[] vertex;// = new int [5];

            public ARMarkerInfo2()
            {
                pos = new double[2];
                x_coord = new int[10000];
                y_coord = new int[10000];
                vertex = new int[5];
            }
        };

        #region Constants/Enumerations

        private const int ArPattSampleNum = 64;
        private const int PdLoop=   3;
        private const int MaxWidth = 640;
        private const int MaxHeight = 480;
        private const int WorkSize = 1024 * 32;
        private const int ArSquareMax = 30;
        private const int ArChainMax = 10000;

        private const int ArAreaMax = 100000;
        private const int ArAreaMin = 70;

        private const float VZERO           = 1;// 1e-16;

        private const int EVEC_MAX = 10;

        #endregion

        #region Variables

        private int[] l_image ;
        private int[] work ;
        private int[] work2 ;
        private int[] warea ;
        private int[] wclip ;
        private double[] wpos ;
        private ARMarkerInfo2[] marker_info2;
        private ARMarkerInfo[] wmarker_info;


        private double [] dist_factor;
        private ARMarkerInfo    [] marker_infoL;
        private int prev_num = 0;

        private arPrevInfo  []           prev_info;

        #endregion

        #region Constructors/Destructors

        public ARToolKit()
        {

            l_image = new int[MaxHeight * MaxWidth];
            work = new int[WorkSize];
            work2 = new int[WorkSize * 7];
            warea = new int[WorkSize];
            wclip = new int[WorkSize * 4];
            wpos = new double[WorkSize * 2];

            marker_info2 = new ARMarkerInfo2[ArSquareMax];
            marker_infoL = new ARMarkerInfo[ArSquareMax];

            prev_info = new arPrevInfo[ArSquareMax];

            dist_factor = new double[4];
        }
        
        #endregion

        #region Methods

        private int EX(ARMat input, ARVec mean)
        {
            double[] m, v;
            int row, clm;
            int i, j;

            row = input.row;
            clm = input.clm;
            if (row <= 0 || clm <= 0)
                return (-1);
            if (mean.clm != clm)
                return (-1);

            for (i = 0; i < clm; i++)
                mean.v[i] = 0.0;


            int index = 0;
            m = input.m;
            for (i = 0; i < row; i++)
            {
                v = mean.v;
                for (j = 0; j < clm; j++)
                {
                    v[index] = m[index];
                    index++;

                }

            }

            for (i = 0; i < clm; i++)
                mean.v[i] /= row;

            return (0);
        }
        private int CENTER(ARMat inout, ARVec mean)
        {
            double[] m, v;
            int row, clm;
            int i, j;

            row = inout.row;
            clm = inout.clm;
            if (mean.clm != clm) return (-1);


            int index = 0;
            m = inout.m;
            for (i = 0; i < row; i++)
            {
                v = mean.v;
                for (j = 0; j < clm; j++)
                {
                    m[index] -= v[index];
                    index++;
                }
            }

            return (0);
        }
        private double ARELEM0(ref ARMat mat, int r, int c)
        {
            return mat.m[r * mat.clm + c];
        }
        private int x_by_xt(ARMat input, ARMat output)
        {
            int in1, in2;
            int outIndex;
            int row, clm;
            int i, j, k;

            row = input.row;
            clm = input.clm;
            if (output.row != row || output.clm != row)
                return (-1);

            outIndex = 0;
            for (i = 0; i < row; i++)
            {
                for (j = 0; j < row; j++)
                {
                    if (j < i)
                    {
                        output.m[outIndex] = output.m[j * row + i];
                    }
                    else
                    {
                        in1 = clm * i;
                        in2 = clm * j;
                        output.m[outIndex] = 0.0;
                        for (k = 0; k < clm; k++)
                        {
                            output.m[outIndex] += input.m[in1++] * input.m[in2++];
                        }
                    }
                    outIndex++;
                }
            }

            return (0);
        }
        private int xt_by_x(ARMat input, ARMat output)
        {
            int in1, in2, outIndex;
            int row, clm;
            int i, j, k;

            row = input.row;
            clm = input.clm;
            if (output.row != clm || output.clm != clm)
                return (-1);

            outIndex = 0;
            for (i = 0; i < clm; i++)
            {
                for (j = 0; j < clm; j++)
                {
                    if (j < i)
                    {
                        output.m[outIndex] = output.m[j * clm + i];
                    }
                    else
                    {
                        in1 = i;
                        in2 = j;
                        output.m[outIndex] = 0.0;
                        for (k = 0; k < row; k++)
                        {
                            output.m[outIndex] += input.m[in1] * input.m[in2];
                            in1 += clm;
                            in2 += clm;
                        }
                    }
                    outIndex++;
                }
            }

            return (0);
        }
        private int EV_create(ARMat input, ARMat u, ARMat output, ARVec ev)
        {
            int m, m1, m2;
            double sum, work;
            int row, clm;
            int i, j, k;

            row = input.row;
            clm = input.clm;
            if (row <= 0 || clm <= 0)
                return (-1);
            if (u.row != row || u.clm != row)
                return (-1);
            if (output.row != row || output.clm != clm)
                return (-1);
            if (ev.clm != row)
                return (-1);

            m = 0;
            for (i = 0; i < row; i++)
            {
                if (ev.v[i] < VZERO) break;
                work = 1 / Math.Sqrt(Math.Abs(ev.v[i]));
                for (j = 0; j < clm; j++)
                {
                    sum = 0.0;
                    m1 = i * row;
                    m2 = j;
                    for (k = 0; k < row; k++)
                    {
                        sum += u.m[m1] * input.m[m2];
                        m1++;
                        m2 += clm;
                    }
                    output.m[m++] = sum * work;
                }
            }
            for (; i < row; i++)
            {
                ev.v[i] = 0.0;
                for (j = 0; j < clm; j++)
                    output.m[m++] = 0.0;
            }

            return (0);
        }
        private int PCA(ARMat input, ARMat output, ARVec ev)
        {
            ARMat u;
            int m1, m2;
            int row, clm, min;
            int i, j;

            row = input.row;
            clm = input.clm;
            min = (clm < row) ? clm : row;
            if (row < 2 || clm < 2)
                return (-1);
            if (output.clm != input.clm)
                return (-1);
            if (output.row != min)
                return (-1);
            if (ev.clm != min)
                return (-1);

            u = new ARMat();
            u.row = min;
            u.clm = min;
            u.m = new double[min * min];


            if (u.row != min || u.clm != min)
                return (-1);
            if (row < clm)
            {
                if (x_by_xt(input, u) < 0)
                {
                    //arMatrixFree(u); 
                    return (-1);
                }
            }
            else
            {
                if (xt_by_x(input, u) < 0)
                {
                    //arMatrixFree(u); 
                    return (-1);
                }
            }

            //if( QRM( u, ev ) < 0 ) { arMatrixFree(u); return(-1); }

            if (row < clm)
            {
                if (EV_create(input, u, output, ev) < 0)
                {
                    //arMatrixFree(u);
                    return (-1);
                }
            }
            else
            {
                m1 = 0;
                m2 = 0;
                for (i = 0; i < min; i++)
                {
                    if (ev.v[i] < VZERO)
                        break;
                    for (j = 0; j < min; j++)
                    {
                        output.m[m2++] = u.m[m1++];
                    }
                }
                for (; i < min; i++)
                {
                    ev.v[i] = 0.0;
                    for (j = 0; j < min; j++)
                        output.m[m2++] = 0.0;
                }
            }

            //arMatrixFree(u);

            return (0);
        }
        /// <summary>
        /// 
        ///Input:
        ///  ---- clm (Data dimention)--->
        ///  [ 10  20  30 ] ^
        ///  [ 20  10  15 ] |
        ///  [ 12  23  13 ] row
        ///  [ 20  10  15 ] |(Sample number)
        ///  [ 13  14  15 ] v
        ///
        ///Evec:
        ///  ---- clm (Eigen vector)--->
        ///  [ 10  20  30 ] ^
        ///  [ 20  10  15 ] |
        ///  [ 12  23  13 ] row
        ///  [ 20  10  15 ] |(Number of egen vec)
        ///  [ 13  14  15 ] v
        ///
        ///Ev:
        ///  ---- clm (Number of eigen vector)---
        ///  [ 10  20  30 ] eigen value
        ///
        ///Mean:
        ///  ---- clm (Data dimention)---
        ///  [ 10  20  30 ] mean value
        /// </summary>
        /// <param name="input"></param>
        /// <param name="evec"></param>
        /// <param name="ev"></param>
        /// <param name="mean"></param>
        /// <returns></returns>
        private int arMatrixPCA(ARMat input, ARMat evec, ARVec ev, ARVec mean)
        {
            ARMat work;
            double srow, sum;
            int row, clm;
            int check, rval;
            int i;

            row = input.row;
            clm = input.clm;
            check = (row < clm) ? row : clm;
            if (row < 2 || clm < 2)
                return (-1);
            if (evec.clm != input.clm || evec.row != check)
                return (-1);
            if (ev.clm != check)
                return (-1);
            if (mean.clm != input.clm)
                return (-1);



            work = new ARMat();
            work.m = new double[input.clm * input.row];
            work.clm = input.clm;
            work.row = input.row;

            for (int r = 0; r < input.row; r++)
            {
                for (int c = 0; c < input.clm; c++)
                {
                    //ARELEM0(work, r, c) = ARELEM0(input, r, c);
                }
            }



            srow = System.Math.Sqrt((double)row);
            if (EX(work, mean) < 0)
            {
                //    arMatrixFree( work );
                return (-1);
            }
            if (CENTER(work, mean) < 0)
            {
                //arMatrixFree( work );
                return (-1);
            }
            for (i = 0; i < row * clm; i++)
                work.m[i] /= srow;

            rval = PCA(work, evec, ev);
            //arMatrixFree( work );

            sum = 0.0;
            for (i = 0; i < ev.clm; i++) sum += ev.v[i];
            for (i = 0; i < ev.clm; i++) ev.v[i] /= sum;

            return (rval);
        }
        private int arParamObserv2Ideal(double[] dist_factor, double ox, double oy, ref double ix, ref double iy)
        {
            double z02, z0, p, q, z, px, py;
            int i;

            px = ox - dist_factor[0];
            py = oy - dist_factor[1];
            p = dist_factor[2] / 100000000.0;
            z02 = px * px + py * py;
            q = z0 = System.Math.Sqrt(px * px + py * py);

            for (i = 1; ; i++)
            {
                if (z0 != 0.0)
                {
                    z = z0 - ((1.0 - p * z02) * z0 - q) / (1.0 - 3.0 * p * z02);
                    px = px * z / z0;
                    py = py * z / z0;
                }
                else
                {
                    px = 0.0;
                    py = 0.0;
                    break;
                }
                if (i == PdLoop)
                    break;

                z02 = px * px + py * py;
                z0 = System.Math.Sqrt(px * px + py * py);
            }

            ix = px / dist_factor[3] + dist_factor[0];
            iy = py / dist_factor[3] + dist_factor[1];

            return (0);
        }
        private int arGetLine2(int[] x_coord, int[] y_coord, int coord_num, int[] vertex, double[,] line, double[,] v, double[] dist_factor)
        {
            ARMat input, evec;
            ARVec ev, mean;
            double w1;
            int st, ed, n;
            int i, j;

            ev = new ARVec();
            ev.v = new double[2];
            ev.clm = 2;


            mean = new ARVec();
            mean.v = new double[2];
            mean.clm = 2;

            evec = new ARMat();
            evec.m = new double[2 * 2];
            evec.clm = 2;
            evec.row = 2;


            for (i = 0; i < 4; i++)
            {
                w1 = (double)(vertex[i + 1] - vertex[i] + 1) * 0.05 + 0.5;
                st = (int)(vertex[i] + w1);
                ed = (int)(vertex[i + 1] - w1);
                n = ed - st + 1;

                input = new ARMat();
                input.m = new double[n * 2];
                input.row = n;
                input.clm = 2;



                for (j = 0; j < n; j++)
                {
                    arParamObserv2Ideal(
                        dist_factor,
                        x_coord[st + j],
                        y_coord[st + j],
                        ref input.m[j * 2 + 0],
                        ref input.m[j * 2 + 1]);
                }
                if (arMatrixPCA(input, evec, ev, mean) < 0)
                {
                    //arMatrixFree( input );
                    //arMatrixFree( evec );
                    //arVecFree( mean );
                    //arVecFree( ev );
                    return (-1);
                }
                line[i, 0] = evec.m[1];
                line[i, 1] = -evec.m[0];
                line[i, 2] = -(line[i, 0] * mean.v[0] + line[i, 1] * mean.v[1]);
                //arMatrixFree( input );
            }
            //arMatrixFree( evec );
            //arVecFree( mean );
            //arVecFree( ev );

            for (i = 0; i < 4; i++)
            {
                w1 = line[(i + 3) % 4, 0] * line[i, 1] - line[i, 0] * line[(i + 3) % 4, 1];
                if (w1 == 0.0) return (-1);
                v[i, 0] = (line[(i + 3) % 4, 1] * line[i, 2]
                           - line[i, 1] * line[(i + 3) % 4, 2]) / w1;
                v[i, 1] = (line[i, 0] * line[(i + 3) % 4, 2]
                           - line[(i + 3) % 4, 0] * line[i, 2]) / w1;
            }

            return (0);
        }
        private int arGetLine(int[] x_coord, int[] y_coord, int coord_num, int[] vertex, double[,] line, double[,] v)
        {
            return arGetLine2(x_coord, y_coord, coord_num, vertex, line, v, dist_factor);
        }
        private double minv(ref double[] ap, int dimen, int rowa)
        {
            double wap, wcp, wbp;/* work pointer                 */
            int i, j, n, ip, nwork;
            int[] nos = new int[50];
            double epsl;
            double p, pbuf, work;
            //double  fabs();
            int indexAp = 0;

            epsl = 1.0e-10;         /* Threshold value      */


            return 0;
            //switch (dimen)
            //{
            //    case (0):
            //        return (0);                 /* check size */
            //    case (1):
            //        ap[indexAp] = 1.0 / (ap[indexAp]);
            //        return (ap);                   /* 1 dimension */
            //}

            //for(n = 0; n < dimen ; n++)
            //        nos[n] = n;

            //for(n = 0; n < dimen ; n++) 
            //{
            //        wcp = ap + n * rowa;

            //        for(i = n, wap = wcp, p = 0.0; i < dimen ; i++, wap += rowa)
            //                if( p < ( pbuf = Math.Abs (wap)) ) 
            //                {
            //                        p = pbuf;
            //                        ip = i;
            //                }
            //        if (p <= epsl)
            //                return(0);

            //        nwork = nos[ip];
            //        nos[ip] = nos[n];
            //        nos[n] = nwork;

            //        for(j = 0, wap = ap + ip * rowa, wbp = wcp; j < dimen ; j++) 
            //        {
            //                work = wap;
            //                *wap++ = wbp;
            //                *wbp++ = work;
            //        }

            //        for(j = 1, wap = wcp, work = *wcp; j < dimen ; j++, wap++)
            //                *wap = *(wap + 1) / work;
            //        *wap = 1.0 / work;

            //        for(i = 0; i < dimen ; i++) {
            //                if(i != n) {
            //                        wap = ap + i * rowa;
            //                        for(j = 1, wbp = wcp, work = *wap;
            //                                        j < dimen ; j++, wap++, wbp++)
            //                                *wap = *(wap + 1) - work * (*wbp);
            //                        *wap = -work * (*wbp);
            //                }
            //        }
            //}

            //for(n = 0; n < dimen ; n++) {
            //        for(j = n; j < dimen ; j++)
            //                if( nos[j] == n) break;
            //        nos[j] = nos[n];
            //        for(i = 0, wap = ap + j, wbp = ap + n; i < dimen ;
            //                                i++, wap += rowa, wbp += rowa) {
            //                work = *wap;
            //                *wap = *wbp;
            //                *wbp = work;
            //        }
            //}
            //return(ap);
        }
        private int arMatrixMul(ARMat dest, ARMat a, ARMat b)
        {
            int r, c, i;

            if (a.clm != b.row || dest.row != a.row || dest.clm != b.clm) return -1;

            for (r = 0; r < dest.row; r++)
            {
                for (c = 0; c < dest.clm; c++)
                {
                    ARELEM0(ref dest, r, c);// = 0.0;
                    for (i = 0; i < a.clm; i++)
                    {
                        //ARELEM0(dest, r, c) += ARELEM0(a, r, i) * ARELEM0(b, i, c);
                    }
                }
            }

            return 0;
        }
        private void get_cpara(double[,] world, double[,] vertex, double[,] para)
        {
            ARMat a, b, c;
            int i;

            a = new ARMat();
            a.m = new double[8 * 8];
            a.clm = 8;
            a.row = 8;

            b = new ARMat();
            b.m = new double[8 * 1];
            b.clm = 1;
            b.row = 8;

            c = new ARMat();
            c.m = new double[8 * 1];
            c.clm = 1;
            c.row = 8;


            for (i = 0; i < 4; i++)
            {
                a.m[i * 16 + 0] = world[i, 0];
                a.m[i * 16 + 1] = world[i, 1];
                a.m[i * 16 + 2] = 1.0;
                a.m[i * 16 + 3] = 0.0;
                a.m[i * 16 + 4] = 0.0;
                a.m[i * 16 + 5] = 0.0;
                a.m[i * 16 + 6] = -world[i, 0] * vertex[i, 0];
                a.m[i * 16 + 7] = -world[i, 1] * vertex[i, 0];
                a.m[i * 16 + 8] = 0.0;
                a.m[i * 16 + 9] = 0.0;
                a.m[i * 16 + 10] = 0.0;
                a.m[i * 16 + 11] = world[i, 0];
                a.m[i * 16 + 12] = world[i, 1];
                a.m[i * 16 + 13] = 1.0;
                a.m[i * 16 + 14] = -world[i, 0] * vertex[i, 1];
                a.m[i * 16 + 15] = -world[i, 1] * vertex[i, 1];
                b.m[i * 2 + 0] = vertex[i, 0];
                b.m[i * 2 + 1] = vertex[i, 1];
            }

            minv(ref a.m, a.row, a.row);

            //arMatrixSelfInv( a );
            arMatrixMul(c, a, b);
            for (i = 0; i < 2; i++)
            {
                para[i, 0] = c.m[i * 3 + 0];
                para[i, 1] = c.m[i * 3 + 1];
                para[i, 2] = c.m[i * 3 + 2];
            }
            para[2, 0] = c.m[2 * 3 + 0];
            para[2, 1] = c.m[2 * 3 + 1];
            para[2, 2] = 1.0;
            //arMatrixFree( a );
            //arMatrixFree( b );
            //arMatrixFree( c );
        }
        private int arGetPatt(Model.IImage image, int[] x_coord, int[] y_coord, int[] vertex, ref int[, ,] ext_pat)
        {
            int[, ,] ext_pat2 = new int[16, 16, 3];
            double[,] world = new double[4, 2];
            double[,] local = new double[4, 2];
            double[,] para = new double[3, 3];
            double d, xw, yw;
            int xc, yc;
            int xdiv, ydiv;
            int xdiv2, ydiv2;
            int lx1, lx2, ly1, ly2;
            int i, j;
            // int       k1, k2, k3; // unreferenced
            double xdiv2_reciprocal; // [tp]
            double ydiv2_reciprocal; // [tp]
            int ext_pat2_x_index;
            int ext_pat2_y_index;
            int image_index;

            world[0, 0] = 100.0;
            world[0, 1] = 100.0;
            world[1, 0] = 100.0 + 10.0;
            world[1, 1] = 100.0;
            world[2, 0] = 100.0 + 10.0;
            world[2, 1] = 100.0 + 10.0;
            world[3, 0] = 100.0;
            world[3, 1] = 100.0 + 10.0;
            for (i = 0; i < 4; i++)
            {
                local[i, 0] = x_coord[vertex[i]];
                local[i, 1] = y_coord[vertex[i]];
            }
            get_cpara(world, local, para);

            lx1 = (int)((local[0, 0] - local[1, 0]) * (local[0, 0] - local[1, 0])
                + (local[0, 1] - local[1, 1]) * (local[0, 1] - local[1, 1]));
            lx2 = (int)((local[2, 0] - local[3, 0]) * (local[2, 0] - local[3, 0])
                + (local[2, 1] - local[3, 1]) * (local[2, 1] - local[3, 1]));
            ly1 = (int)((local[1, 0] - local[2, 0]) * (local[1, 0] - local[2, 0])
                + (local[1, 1] - local[2, 1]) * (local[1, 1] - local[2, 1]));
            ly2 = (int)((local[3, 0] - local[0, 0]) * (local[3, 0] - local[0, 0])
                + (local[3, 1] - local[0, 1]) * (local[3, 1] - local[0, 1]));
            if (lx2 > lx1) lx1 = lx2;
            if (ly2 > ly1) ly1 = ly2;
            xdiv2 = 16;
            ydiv2 = 16;
            while (xdiv2 * xdiv2 < lx1 / 4) xdiv2 *= 2;
            while (ydiv2 * ydiv2 < ly1 / 4) ydiv2 *= 2;

            if (xdiv2 > ArPattSampleNum) xdiv2 = ArPattSampleNum;
            if (ydiv2 > ArPattSampleNum) ydiv2 = ArPattSampleNum;

            xdiv = xdiv2 / 16;
            ydiv = ydiv2 / 16;


            xdiv2_reciprocal = 1.0 / xdiv2;
            ydiv2_reciprocal = 1.0 / ydiv2;

            //for (int c = 0; c < 16 * 16 * 3; c++)
            //    ext_pat2[c] = 0;

            for (j = 0; j < ydiv2; j++)
            {
                yw = 102.5 + 5.0 * (j + 0.5) * ydiv2_reciprocal;
                for (i = 0; i < xdiv2; i++)
                {
                    xw = 102.5 + 5.0 * (i + 0.5) * xdiv2_reciprocal;
                    d = para[2, 0] * xw + para[2, 1] * yw + para[2, 2];
                    if (d == 0)
                        return (-1);
                    xc = (int)((para[0, 0] * xw + para[0, 1] * yw + para[0, 2]) / d);
                    yc = (int)((para[1, 0] * xw + para[1, 1] * yw + para[1, 2]) / d);

                    if (xc >= 0 && xc < image.Image.Width && yc >= 0 && yc < image.Image.Height)
                    {
                        ext_pat2_y_index = j / ydiv;
                        ext_pat2_x_index = i / xdiv;
                        image_index = (yc * image.Image.Width + xc) * 3;
                        ext_pat2[ext_pat2_y_index, ext_pat2_x_index, 0] += image.Bytes[image_index + 2];
                        ext_pat2[ext_pat2_y_index, ext_pat2_x_index, 1] += image.Bytes[image_index + 1];
                        ext_pat2[ext_pat2_y_index, ext_pat2_x_index, 2] += image.Bytes[image_index + 0];
                    }
                }
            }

            for (j = 0; j < 16; j++)
            {
                for (i = 0; i < 16; i++)
                {				// PRL 2006-06-08.
                    ext_pat[j, i, 0] = ext_pat2[j, i, 0] / (xdiv * ydiv);
                    ext_pat[j, i, 1] = ext_pat2[j, i, 1] / (xdiv * ydiv);
                    ext_pat[j, i, 2] = ext_pat2[j, i, 2] / (xdiv * ydiv);
                }
            }

            return (0);
        }
        private int pattern_match(Model.IImage data, ref int code, ref int dir, ref double cf)
        {
            double[] invec = new double[EVEC_MAX];
            int[] input = new int[16 * 16 * 3];
            int i = 0, j, l;
            int k = 0; // fix VC7 compiler warning: uninitialized variable
            int ave, sum, res, res2;
            double datapow, sum2, min;
            double max = 0.0; // fix VC7 compiler warning: uninitialized variable

            sum = ave = 0;
            for (i = 0; i < 16 * 16 * 3; i++)
            {
                ave += (255 - data.Bytes[i]);
            }
            ave /= (16 * 16 * 3);

            if (true) //Template matching color
            {
                for (i = 0; i < 16 * 16 * 3; i++)
                {
                    input[i] = (255 - data.Bytes[i]) - ave;
                    sum += input[i] * input[i];
                }
            }
            else
            {
                for (i = 0; i < 16 * 16; i++)
                {
                    input[i] = ((255 - data.Bytes[i * 3 + 0]) + (255 - data.Bytes[i * 3 + 1]) + (255 - data.Bytes[i * 3 + 02])) / 3 - ave;
                    sum += input[i] * input[i];
                }
            }

            datapow = Math.Sqrt((double)sum);
            if (datapow == 0.0)
            {
                code = 0;
                dir = 0;
                cf = -1.0;
                return -1;
            }

            res = res2 = -1;
            //if (true)  //template matching color
            //{
            //    //if( true && evecf ) //without pca
            //    if (true)
            //    {

            //        for (i = 0; i < evec_dim; i++)
            //        {
            //            invec[i] = 0.0;
            //            for (j = 0; j < 16 * 16 * 3; j++)
            //            {
            //                invec[i] += evec[i, j] * input[j];
            //            }
            //            invec[i] /= datapow;
            //        }

            //        min = 10000.0;
            //        k = -1;
            //        for (l = 0; l < pattern_num; l++)
            //        {
            //            k++;
            //            while (patf[k] == 0)
            //                k++;
            //            if (patf[k] == 2)
            //                continue;

            //            for (j = 0; j < 4; j++)
            //            {
            //                sum2 = 0;
            //                for (i = 0; i < evec_dim; i++)
            //                {
            //                    sum2 += (invec[i] - epat[k, j, i]) * (invec[i] - epat[k, j, i]);
            //                }

            //                if (sum2 < min)
            //                {
            //                    min = sum2;
            //                    res = j;
            //                    res2 = k;
            //                }
            //            }

            //        }
            //        sum = 0;
            //        for (i = 0; i < 16 * 16 * 3; i++)
            //            sum += input[i] * pat[res2, res, i];
            //        max = sum / patpow[res2, res] / datapow;
            //    }
            //    else
            //    {
            //        k = -1;
            //        max = 0.0;
            //        for (l = 0; l < pattern_num; l++)
            //        {
            //            k++;
            //            while (patf[k] == 0)
            //                k++;
            //            if (patf[k] == 2)
            //                continue;
            //            for (j = 0; j < 4; j++)
            //            {
            //                sum = 0;
            //                for (i = 0; i < 16 * 16 * 3; i++)
            //                    sum += input[i] * pat[k, j, i];
            //                sum2 = sum / patpow[k, j] / datapow;
            //                if (sum2 > max)
            //                {
            //                    max = sum2;
            //                    res = j;
            //                    res2 = k;
            //                }
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    for (l = 0; l < pattern_num; l++)
            //    {
            //        k++;
            //        while (patf[k] == 0)
            //            k++;
            //        if (patf[k] == 2)
            //            continue;
            //        for (j = 0; j < 4; j++)
            //        {
            //            sum = 0;
            //            for (i = 0; i < 16 * 16; i++)
            //                sum += input[i] * patBW[k, j, i];
            //            sum2 = sum / patpowBW[k, j] / datapow;
            //            if (sum2 > max)
            //            {
            //                max = sum2;
            //                res = j;
            //                res2 = k;
            //            }
            //        }
            //    }
            //}

            code = res2;
            dir = res;
            cf = max;

            return 0;
        }
        private int arGetCode(Model.IImage image, int[] x_coord, int[] y_coord, int[] vertex, int code, int dir, double cf)
        {

            int[, ,] ext_pat = new int[16, 16, 33];


            arGetPatt(image, x_coord, y_coord, vertex, ref ext_pat);


            pattern_match(image, ref code, ref dir, ref cf);


            return (0);
        }
        private ARMarkerInfo[] arGetMarkerInfo(Model.IImage image, ARMarkerInfo2[] marker_info2, ref int marker_num)
        {
            int id = 0, dir = 0;
            double cf = 0;
            int i, j;

            for (i = j = 0; i < marker_num; i++)
            {
                marker_infoL[j].area = marker_info2[i].area;
                marker_infoL[j].pos[0] = marker_info2[i].pos[0];
                marker_infoL[j].pos[1] = marker_info2[i].pos[1];

                if (arGetLine(marker_info2[i].x_coord, marker_info2[i].y_coord,
                              marker_info2[i].coord_num, marker_info2[i].vertex,
                              marker_infoL[j].line, marker_infoL[j].vertex) < 0) continue;

                arGetCode(image,
                          marker_info2[i].x_coord, marker_info2[i].y_coord,
                          marker_info2[i].vertex, id, dir, cf);

                marker_infoL[j].id = id;
                marker_infoL[j].dir = dir;
                marker_infoL[j].cf = cf;

                j++;
            }
            marker_num = j;

            return (marker_infoL);
        }
        public int arDetectMarker(Model.IImage dataPtr, int thresh, ref ARMarkerInfo[] marker_info, ref int marker_num)
        {
            int wmarker_num = 0;
            int label_num = 0;
            int[] area = null;
            int[] clip = null;
            int[] label_ref = null;
            double[] pos = null;
            double rarea, rlen, rlenmin;
            double diff, diffmin;
            int cid, cdir;
            int i, j, k;

            marker_num = 0;

            l_image = labeling2(dataPtr, thresh, ref label_num, ref area, ref pos, ref clip, ref label_ref);
            if (l_image == null) return -1;

            marker_info2 = arDetectMarker2(dataPtr, label_num, label_ref, area, pos, clip, ArAreaMax, ArAreaMin, 1.0, ref wmarker_num);
            if (marker_info2 == null) return -1;

            wmarker_info = arGetMarkerInfo(dataPtr, marker_info2, ref wmarker_num);
            if (wmarker_info == null) return -1;

            for (i = 0; i < prev_num; i++)
            {
                rlenmin = 10.0;
                cid = -1;
                for (j = 0; j < wmarker_num; j++)
                {
                    rarea = (double)prev_info[i].marker.area / (double)wmarker_info[j].area;
                    if (rarea < 0.7 || rarea > 1.43) continue;
                    rlen = ((wmarker_info[j].pos[0] - prev_info[i].marker.pos[0])
                           * (wmarker_info[j].pos[0] - prev_info[i].marker.pos[0])
                           + (wmarker_info[j].pos[1] - prev_info[i].marker.pos[1])
                           * (wmarker_info[j].pos[1] - prev_info[i].marker.pos[1])) / wmarker_info[j].area;
                    if (rlen < 0.5 && rlen < rlenmin)
                    {
                        rlenmin = rlen;
                        cid = j;
                    }
                }
                if (cid >= 0 && wmarker_info[cid].cf < prev_info[i].marker.cf)
                {
                    wmarker_info[cid].cf = prev_info[i].marker.cf;
                    wmarker_info[cid].id = prev_info[i].marker.id;
                    diffmin = 10000.0 * 10000.0;
                    cdir = -1;
                    for (j = 0; j < 4; j++)
                    {
                        diff = 0;
                        for (k = 0; k < 4; k++)
                        {
                            diff += (prev_info[i].marker.vertex[k, 0] - wmarker_info[cid].vertex[(j + k) % 4, 0])
                                  * (prev_info[i].marker.vertex[k, 0] - wmarker_info[cid].vertex[(j + k) % 4, 0])
                                  + (prev_info[i].marker.vertex[k, 1] - wmarker_info[cid].vertex[(j + k) % 4, 1])
                                  * (prev_info[i].marker.vertex[k, 1] - wmarker_info[cid].vertex[(j + k) % 4, 1]);
                        }
                        if (diff < diffmin)
                        {
                            diffmin = diff;
                            cdir = (prev_info[i].marker.dir - j + 4) % 4;
                        }
                    }
                    wmarker_info[cid].dir = cdir;
                }
            }

            for (i = 0; i < wmarker_num; i++)
            {
                /*
                    printf("cf = %g\n", wmarker_info[i].cf);
                */
                if (wmarker_info[i].cf < 0.5) wmarker_info[i].id = -1;
            }


            /*------------------------------------------------------------*/

            for (i = j = 0; i < prev_num; i++)
            {
                prev_info[i].count++;
                if (prev_info[i].count < 4)
                {
                    prev_info[j] = prev_info[i];
                    j++;
                }
            }
            prev_num = j;

            for (i = 0; i < wmarker_num; i++)
            {
                if (wmarker_info[i].id < 0) continue;

                for (j = 0; j < prev_num; j++)
                {
                    if (prev_info[j].marker.id == wmarker_info[i].id) break;
                }
                prev_info[j].marker = wmarker_info[i];
                prev_info[j].count = 1;
                if (j == prev_num) prev_num++;
            }

            for (i = 0; i < prev_num; i++)
            {
                for (j = 0; j < wmarker_num; j++)
                {
                    rarea = (double)prev_info[i].marker.area / (double)wmarker_info[j].area;
                    if (rarea < 0.7 || rarea > 1.43) continue;
                    rlen = ((wmarker_info[j].pos[0] - prev_info[i].marker.pos[0])
                           * (wmarker_info[j].pos[0] - prev_info[i].marker.pos[0])
                           + (wmarker_info[j].pos[1] - prev_info[i].marker.pos[1])
                           * (wmarker_info[j].pos[1] - prev_info[i].marker.pos[1])) / wmarker_info[j].area;
                    if (rlen < 0.5) break;
                }
                if (j == wmarker_num)
                {
                    wmarker_info[wmarker_num] = prev_info[i].marker;
                    wmarker_num++;
                }
            }


            marker_num = wmarker_num;
            marker_info = wmarker_info;

            return 0;
        }
        public Model.IImage CreateLabelImage(int width, int height)
        {
            int bytesPerPixel = 3;
            int size = width * height * bytesPerPixel;

            byte[] res = new byte[size];

            int pos = 0;
            int index = 0;


            for (int l = 0; l < height; l++)
            {
                for (int c = 0; c < width; c++)
                {

                    res[index + 0] = (byte)l_image[pos];
                    res[index + 1] = (byte)l_image[pos];
                    res[index + 2] = (byte)l_image[pos];


                    pos++;
                    index += bytesPerPixel;
                }
            }

            return new Model.ImageBitmap(width, height, res);
        }
        private int [] labeling2(Model.IImage image, int thresh, ref int label_num, ref int [] area, ref double [] pos, ref int [] clip, ref int [] label_ref )
        {
            


            int width = image.Image.Width;
            int height = image.Image.Height;

            int wlabel_num;

            int wk;
            int wk_max;
            int m = 0;
            int n = 0;                     
    
            int poff;    
            int i, j, k;
            int bytesPerPixel = 3;
            int lxsize = width, lysize = height;
            int size = lxsize * lysize * bytesPerPixel;
            byte[] newImage = new byte[size];

            Array.Copy(image.Bytes, newImage, size);

            int thresht3 = thresh * 3;

            lxsize = width;
            lysize = height;


            int indexPnt1 = 0; // Leftmost pixel of top row of image.
            int indexPnt2 = (lysize - 1) * lxsize; // Leftmost pixel of bottom row of image.
            int indexPnt = 0;

            for (i = 0; i < lxsize - (lxsize % 4); i += 4)
            {
                l_image[indexPnt1++] = l_image[indexPnt2++] = 0;
                l_image[indexPnt1++] = l_image[indexPnt2++] = 0;
                l_image[indexPnt1++] = l_image[indexPnt2++] = 0;
                l_image[indexPnt1++] = l_image[indexPnt2++] = 0;
                
            }

            indexPnt1 = 0;// Leftmost pixel of top row of image.
            indexPnt2 = (lxsize - 1);// Rightmost pixel of top row of image.


            // 4x loop unrolling
            for (i = 0; i < lysize - (lysize % 4); i += 4)
            {
                l_image[indexPnt1] = l_image[indexPnt2] = 0;
                indexPnt1 += lxsize;
                indexPnt2 += lxsize;

                l_image[indexPnt1] = l_image[indexPnt2] = 0;
                indexPnt1 += lxsize;
                indexPnt2 += lxsize;

                l_image[indexPnt1] = l_image[indexPnt2] = 0;
                indexPnt1 += lxsize;
                indexPnt2 += lxsize;

                l_image[indexPnt1] = l_image[indexPnt2] = 0;
                indexPnt1 += lxsize;
                indexPnt2 += lxsize;
            }

            wk_max = 0;
            indexPnt2 = lxsize + 1;

            indexPnt =  (width + 1) * bytesPerPixel;
            poff = bytesPerPixel;

            for (j = 1; j < lysize - 1; j++, indexPnt += poff * 2, indexPnt2 += 2)
            {
                for (i = 1; i < lxsize - 1; i++, indexPnt += poff, indexPnt2++)
                {
                    if (newImage[indexPnt] + newImage[indexPnt + 1] + newImage[indexPnt2 + 2] <= thresht3)
                    {
                        indexPnt1 = indexPnt2 - lxsize;
                        if (l_image[indexPnt1] > 0)
                        {
                            l_image[indexPnt2] = l_image[indexPnt1];



                            work2[((l_image[indexPnt2]) - 1) * 7 + 0]++;
                            work2[((l_image[indexPnt2]) - 1) * 7 + 1] += i;
                            work2[((l_image[indexPnt2]) - 1) * 7 + 2] += j;
                            work2[((l_image[indexPnt2]) - 1) * 7 + 6] = j;
                        }
                        else if (l_image[indexPnt1 + 1] > 0)
                        {
                            if (l_image[indexPnt1 - 1] > 0)
                            {
                                m = work[l_image[indexPnt1 + 1] - 1];
                                n = work[l_image[indexPnt1 - 1] - 1];
                                if (m > n)
                                {
                                    l_image[indexPnt2] = (byte)n;
                                    wk = 0;
                                    for (k = 0; k < wk_max; k++)
                                    {
                                        if (work[wk] == m)
                                            work[wk] = n;
                                        wk++;
                                    }

                                }
                                else if (m < n)
                                {
                                    l_image[indexPnt2] = (byte)m;
                                    wk = 0;
                                    for (k = 0; k < wk_max; k++)
                                    {
                                        if (work[wk] == n)
                                            work[wk] = m;
                                        wk++;
                                    }
                                }
                                else
                                {
                                    l_image[indexPnt2] = (byte)m;
                                }

                                // ORIGINAL CODE
                                work2[((l_image[indexPnt2]) - 1) * 7 + 0]++;
                                work2[((l_image[indexPnt2]) - 1) * 7 + 1] += i;
                                work2[((l_image[indexPnt2]) - 1) * 7 + 2] += j;
                                work2[((l_image[indexPnt2]) - 1) * 7 + 6] = j;

                            }
                            else if (l_image[indexPnt2 - 1] > 0)
                            {
                                m = work[l_image[indexPnt1 + 1] - 1];
                                n = work[l_image[indexPnt2 - 1] - 1];

                                if (m > n)
                                {
                                    l_image[indexPnt2] = (byte)n;
                                    wk = 0;
                                    for (k = 0; k < wk_max; k++)
                                    {
                                        if (work[wk] == m)
                                            work[wk] = n;
                                        wk++;
                                    }


                                }
                                else
                                {
                                    l_image[indexPnt2] = (byte) m;
                                }

                                // ORIGINAL CODE
                                work2[((l_image[indexPnt2]) - 1) * 7 + 0]++;
                                work2[((l_image[indexPnt2]) - 1) * 7 + 1] += i;
                                work2[((l_image[indexPnt2]) - 1) * 7 + 2] += j;

                            }
                            else
                            {
                                l_image[indexPnt2] = l_image[indexPnt1 + 1];

                                // ORIGINAL CODE
                                work2[((l_image[indexPnt2]) - 1) * 7 + 0]++;
                                work2[((l_image[indexPnt2]) - 1) * 7 + 1] += i;
                                work2[((l_image[indexPnt2]) - 1) * 7 + 2] += j;
                                if (work2[((l_image[indexPnt2]) - 1) * 7 + 3] > i) work2[((l_image[indexPnt2]) - 1) * 7 + 3] = i;
                                work2[((l_image[indexPnt2]) - 1) * 7 + 6] = j;
                            }



                        }
                        else if (l_image[indexPnt1 - 1] > 0)
                        {
                            l_image[indexPnt2] = l_image[indexPnt1 - 1];

                            // ORIGINAL CODE
                            work2[((l_image[indexPnt2]) - 1) * 7 + 0]++;
                            work2[((l_image[indexPnt2]) - 1) * 7 + 1] += i;
                            work2[((l_image[indexPnt2]) - 1) * 7 + 2] += j;
                            if (work2[((l_image[indexPnt2]) - 1) * 7 + 4] < i) work2[((l_image[indexPnt2]) - 1) * 7 + 4] = i;
                            work2[((l_image[indexPnt2]) - 1) * 7 + 6] = j;
                        }
                        else if (l_image[indexPnt2 - 1] > 0)
                        {
                            l_image[indexPnt2] = l_image[indexPnt2 - 1];


                            // ORIGINAL CODE
                            work2[((l_image[indexPnt2]) - 1) * 7 + 0]++;
                            work2[((l_image[indexPnt2]) - 1) * 7 + 1] += i;
                            work2[((l_image[indexPnt2]) - 1) * 7 + 2] += j;
                            if (work2[((l_image[indexPnt2]) - 1) * 7 + 4] < i)
                                work2[((l_image[indexPnt2]) - 1) * 7 + 4] = i;

                        }
                        else
                        {
                            wk_max++;
                            if (wk_max > 1024 * 32)
                            {
                                return l_image;
                            }
                            work[wk_max - 1] = l_image[indexPnt2] = (byte)wk_max;
                            work2[(wk_max - 1) * 7 + 0] = 1;
                            work2[(wk_max - 1) * 7 + 1] = i;
                            work2[(wk_max - 1) * 7 + 2] = j;
                            work2[(wk_max - 1) * 7 + 3] = i;
                            work2[(wk_max - 1) * 7 + 4] = i;
                            work2[(wk_max - 1) * 7 + 5] = j;
                            work2[(wk_max - 1) * 7 + 6] = j;

                        }

                    }
                    else 
                    {
                        l_image[indexPnt2] = 0;
                    }

                    
                }
            }

            j = 1;
            wk = 0;
            for (i = 1; i <= wk_max; i++, wk++)
            {
                if (work[wk] == i)
                    j++;
                else
                    work[wk] = work[wk - 1];
            }

            label_num = wlabel_num = j - 1;
            if (label_num == 0)
                return null;


            for (i = 0; i < label_num; i++)
            {
                wclip[i * 4 + 0] = lxsize;
                wclip[i * 4 + 1] = 0;
                wclip[i * 4 + 2] = lxsize;
                wclip[i * 4 + 3] = 0;
            }

            for (i = 0; i < wk_max; i++)
            {
                j = work[i] - 1;
                warea[j] += work2[i * 7 + 0];
                wpos[j * 2 + 0] += work2[i * 7 + 1];
                wpos[j * 2 + 1] += work2[i * 7 + 2];
                if (wclip[j * 4 + 0] > work2[i * 7 + 3]) 
                    wclip[j * 4 + 0] = work2[i * 7 + 3];
                if (wclip[j * 4 + 1] < work2[i * 7 + 4]) 
                    wclip[j * 4 + 1] = work2[i * 7 + 4];
                if (wclip[j * 4 + 2] > work2[i * 7 + 5])
                    wclip[j * 4 + 2] = work2[i * 7 + 5];
                if (wclip[j * 4 + 3] < work2[i * 7 + 6])
                    wclip[j * 4 + 3] = work2[i * 7 + 6];
            }

            for (i = 0; i < label_num; i++)
            {
                wpos[i * 2 + 0] /= warea[i];
                wpos[i * 2 + 1] /= warea[i];
            }

            label_ref = work;
            area = warea;
            pos = wpos;
            clip = wclip;

            return l_image;

        }
        private int arGetContour(int [] limage, Model.IImage image, int [] label_ref, int label, int[] clip, int indexClip, ARMarkerInfo2 marker_info2)
        {
            int[] xdir = { 0, 1, 1, 1, 0, -1, -1, -1 };
            int[] ydir = { -1, -1, 0, 1, 1, 1, 0, -1 };
            int[] wx = new int[ArChainMax];
            int[] wy = new int[ArChainMax];
            int xsize, ysize;
            int sx = 0;
            int sy = 0;
            int dir;
            int dmax, d, v1 = 0;
            int i, j;


            int indexP1;

            xsize = image.Image.Width;
            ysize = image.Image.Height;

            j = clip[indexClip + 2];

            indexP1 = j * xsize + clip[indexClip + 0];
            for (i = clip[indexClip + 0]; i <= clip[indexClip + 1]; i++, indexP1++)
            {
                if( limage[indexP1] > 0 && label_ref[ limage[indexP1]-1] == label ) 
                {
                    sx = i; 
                    sy = j; 
                    break;
                }
            }
            if (i > clip[indexClip + 1]) 
            {
                //printf("??? 1\n"); 
                return(-1);
            }

            marker_info2.coord_num = 1;
            marker_info2.x_coord[0] = sx;
            marker_info2.y_coord[0] = sy;
            dir = 5;
            for (; ; )
            {
                indexP1 = marker_info2.y_coord[marker_info2.coord_num - 1] * xsize
                    + marker_info2.x_coord[marker_info2.coord_num - 1];

                dir = (dir + 5) % 8;
                
                for (i = 0; i < 8; i++)
                {
                    if (limage[indexP1+( ydir[dir] * xsize + xdir[dir])] > 0) 
                        break;
                    dir = (dir + 1) % 8;
                }
                if (i == 8)
                {
                    //printf("??? 2\n"); 
                    return (-1);
                }
                marker_info2.x_coord[marker_info2.coord_num] = marker_info2.x_coord[marker_info2.coord_num - 1] + xdir[dir];
                marker_info2.y_coord[marker_info2.coord_num] = marker_info2.y_coord[marker_info2.coord_num - 1] + ydir[dir];
                if (marker_info2.x_coord[marker_info2.coord_num] == sx && 
                    marker_info2.y_coord[marker_info2.coord_num] == sy) break;
                marker_info2.coord_num++;
                if (marker_info2.coord_num == ArChainMax - 1)
                {
                    //printf("??? 3\n"); 
                    return (-1);
                }
            }

            dmax = 0;
            for (i = 1; i < marker_info2.coord_num; i++)
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
            for (i = v1; i < marker_info2.coord_num; i++)
            {
                marker_info2.x_coord[i - v1] = marker_info2.x_coord[i];
                marker_info2.y_coord[i - v1] = marker_info2.y_coord[i];
            }
            for (i = 0; i < v1; i++)
            {
                marker_info2.x_coord[i - v1 + marker_info2.coord_num] = wx[i];
                marker_info2.y_coord[i - v1 + marker_info2.coord_num] = wy[i];
            }
            marker_info2.x_coord[marker_info2.coord_num] = marker_info2.x_coord[0];
            marker_info2.y_coord[marker_info2.coord_num] = marker_info2.y_coord[0];
            marker_info2.coord_num++;

            return 0;
        }
        private int get_vertex(int[] x_coord, int[] y_coord, int st, int ed, double thresh, int[] vertex, ref int vnum)
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
                if (get_vertex(x_coord, y_coord, st, v1, thresh, vertex, ref vnum) < 0)
                    return (-1);

                if ((vnum) > 5) return (-1);
                vertex[(vnum)] = v1;
                (vnum)++;

                if (get_vertex(x_coord, y_coord, v1, ed, thresh, vertex, ref vnum) < 0)
                    return (-1);
            }

            return (0);
        }
        private int check_square(int area, ARMarkerInfo2 marker_info2, double factor)
        {
            int sx, sy;
            int dmax, d, v1;
            int[] vertex = new int[10];
            int vnum = 0;
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
            for (i = 1; i < marker_info2.coord_num - 1; i++)
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
            if (get_vertex(marker_info2.x_coord, marker_info2.y_coord, 0, v1,
                           thresh, wv1, ref wvnum1) < 0)
            {
                return (-1);
            }
            if (get_vertex(marker_info2.x_coord, marker_info2.y_coord,
                           v1, marker_info2.coord_num - 1, thresh, wv2, ref wvnum2) < 0)
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
                if (get_vertex(marker_info2.x_coord, marker_info2.y_coord,
                               0, v2, thresh, wv1, ref wvnum1) < 0)
                {
                    return (-1);
                }
                if (get_vertex(marker_info2.x_coord, marker_info2.y_coord,
                               v2, v1, thresh, wv2, ref wvnum2) < 0)
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
                v2 = (v1 + marker_info2.coord_num - 1) / 2;
                wvnum1 = wvnum2 = 0;
                if (get_vertex(marker_info2.x_coord, marker_info2.y_coord,
                           v1, v2, thresh, wv1, ref wvnum1) < 0)
                {
                    return (-1);
                }
                if (get_vertex(marker_info2.x_coord, marker_info2.y_coord,
                           v2, marker_info2.coord_num - 1, thresh, wv2, ref wvnum2) < 0)
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

            marker_info2.vertex[0] = vertex[0];
            marker_info2.vertex[1] = vertex[1];
            marker_info2.vertex[2] = vertex[2];
            marker_info2.vertex[3] = vertex[3];
            marker_info2.vertex[4] = marker_info2.coord_num - 1;

            return (0);
        }
        private ARMarkerInfo2 [] arDetectMarker2(Model.IImage image, int label_num, int[] label_ref, int[] warea, double[] wpos, int[] wclip, int area_max, int area_min, double factor, ref int marker_num)
        {
            int xsize, ysize;
            int marker_num2;
            int i, j, ret = 0;
            double d;
            

            xsize = image.Image.Width;
            ysize = image.Image.Height;

            marker_num2 = 0;
            for (i = 0; i < label_num; i++)
            {
                if (warea[i] < area_min || warea[i] > area_max)
                    continue;
                if (wclip[i * 4 + 0] == 1 || wclip[i * 4 + 1] == xsize - 2) 
                    continue;
                if (wclip[i * 4 + 2] == 1 || wclip[i * 4 + 3] == ysize - 2) 
                    continue;

                if (marker_info2[marker_num] == null)
                    marker_info2[marker_num] = new ARMarkerInfo2();

                ret = arGetContour(l_image, image, label_ref, i + 1,
                                    wclip, i* 4, marker_info2[marker_num2]);
                if (ret < 0) 
                    continue;

                ret = check_square(warea[i], (marker_info2[marker_num2]), factor);
                if (ret < 0) 
                    continue;

                marker_info2[marker_num2].area = warea[i];
                marker_info2[marker_num2].pos[0] = wpos[i * 2 + 0];
                marker_info2[marker_num2].pos[1] = wpos[i * 2 + 1];
                marker_num2++;
                if (marker_num2 == ArSquareMax) 
                    break;
            }

            for (i = 0; i < marker_num2; i++)
            {
                for (j = i + 1; j < marker_num2; j++)
                {
                    d = (marker_info2[i].pos[0] - marker_info2[j].pos[0])
                      * (marker_info2[i].pos[0] - marker_info2[j].pos[0])
                      + (marker_info2[i].pos[1] - marker_info2[j].pos[1])
                      * (marker_info2[i].pos[1] - marker_info2[j].pos[1]);
                    if (marker_info2[i].area > marker_info2[j].area)
                    {
                        if (d < marker_info2[i].area / 4)
                        {
                            marker_info2[j].area = 0;
                        }
                    }
                    else
                    {
                        if (d < marker_info2[j].area / 4)
                        {
                            marker_info2[i].area = 0;
                        }
                    }
                }
            }
            for (i = 0; i < marker_num2; i++)
            {
                if (marker_info2[i].area == 0.0)
                {
                    for (j = i + 1; j < marker_num2; j++)
                    {
                        marker_info2[j - 1] = marker_info2[j];
                    }
                    marker_num2--;
                }
            }


            marker_num = marker_num2;
            return (marker_info2);

        }
        #endregion
    }
}
