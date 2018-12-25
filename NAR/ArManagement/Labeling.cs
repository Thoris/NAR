#define USE_OPTIMIZATIONS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ArManagement
{
    public class Labeling
    {
        //public int Label(Model.IImage image, int thresh, out int label_num, out int area, out double pos, out int clip, out int label_ref, int LorR)
        //{
        //    if (Config.ARConfiguration.Current.Debug)
        //    {
        //        //return DebugLabel();
        //    }
        //    else
        //    {
        //        //return (1);
        //    }

        //    return 1; 

        //}

        private int Label(Model.IImage image, int thresh, int label_num, int area, double pos, int clip, int label_ref, int LorR)
        {
            Model.IImage pnt;
            Model.IImage pnt1, pnt2;
            Model.IImage l_image;
            int       wk;                      /*  pointer for work    */
            int       wk_max;                   /*  work                */
            int       m,n;                      /*  work                */
            int       i,j,k;                    /*  for loop            */
            int       lxsize, lysize;
            int       poff;
            int       work, work2;
            int       wlabel_num;
            int       warea;
            int       wclip;
            double    wpos;

//#if USE_OPTIMIZATIONS
//    int		  pnt2_index;   // [tp]
//#endif
//    int		  thresht3 = thresh * 3;

//    if (LorR) {
//        l_image = &l_imageL[0];
//        work    = &workL[0];
//        work2   = &work2L[0];
//        wlabel_num = &wlabel_numL;
//        warea   = &wareaL[0];
//        wclip   = &wclipL[0];
//        wpos    = &wposL[0];
//    } else {
//        l_image = &l_imageR[0];
//        work    = &workR[0];
//        work2   = &work2R[0];
//        wlabel_num = &wlabel_numR;
//        warea   = &wareaR[0];
//        wclip   = &wclipR[0];
//        wpos    = &wposR[0];
//    }

//    if (arImageProcMode == AR_IMAGE_PROC_IN_HALF) {
//        lxsize = arImXsize / 2;
//        lysize = arImYsize / 2;
//    } else {
//        lxsize = arImXsize;
//        lysize = arImYsize;
//    }

//    pnt1 = &l_image[0]; // Leftmost pixel of top row of image.
//    pnt2 = &l_image[(lysize - 1)*lxsize]; // Leftmost pixel of bottom row of image.

//#ifndef USE_OPTIMIZATIONS
//    for(i = 0; i < lxsize; i++) {
//        *(pnt1++) = *(pnt2++) = 0;
//    }
//#else
//// 4x loop unrolling
//    for (i = 0; i < lxsize - (lxsize%4); i += 4) {
//        *(pnt1++) = *(pnt2++) = 0;
//        *(pnt1++) = *(pnt2++) = 0;
//        *(pnt1++) = *(pnt2++) = 0;
//        *(pnt1++) = *(pnt2++) = 0;
//    }
//#endif
//    pnt1 = &l_image[0]; // Leftmost pixel of top row of image.
//    pnt2 = &l_image[lxsize - 1]; // Rightmost pixel of top row of image.

//#ifndef USE_OPTIMIZATIONS
//    for(i = 0; i < lysize; i++) {
//        *pnt1 = *pnt2 = 0;
//        pnt1 += lxsize;
//        pnt2 += lxsize;
//    }
//#else
//// 4x loop unrolling
//    for (i = 0; i < lysize - (lysize%4); i += 4) {
//        *pnt1 = *pnt2 = 0;
//        pnt1 += lxsize;
//        pnt2 += lxsize;

//        *pnt1 = *pnt2 = 0;
//        pnt1 += lxsize;
//        pnt2 += lxsize;

//        *pnt1 = *pnt2 = 0;
//        pnt1 += lxsize;
//        pnt2 += lxsize;

//        *pnt1 = *pnt2 = 0;
//        pnt1 += lxsize;
//        pnt2 += lxsize;
//    }
//#endif

//    wk_max = 0;
//    pnt2 = &(l_image[lxsize+1]);
//    if (arImageProcMode == AR_IMAGE_PROC_IN_HALF) {
//        pnt = &(image[(arImXsize*2+2)*AR_PIX_SIZE_DEFAULT]);
//        poff = AR_PIX_SIZE_DEFAULT*2;
//    } else {
//        pnt = &(image[(arImXsize+1)*AR_PIX_SIZE_DEFAULT]);
//        poff = AR_PIX_SIZE_DEFAULT;
//    }
//    for (j = 1; j < lysize - 1; j++, pnt += poff*2, pnt2 += 2) {
//        for(i = 1; i < lxsize-1; i++, pnt+=poff, pnt2++) {
//#if (AR_DEFAULT_PIXEL_FORMAT == AR_PIXEL_FORMAT_ARGB)
//            if( *(pnt+1) + *(pnt+2) + *(pnt+3) <= thresht3 )
//#elif (AR_DEFAULT_PIXEL_FORMAT == AR_PIXEL_FORMAT_ABGR)
//            if( *(pnt+1) + *(pnt+2) + *(pnt+3) <= thresht3 )
//#elif (AR_DEFAULT_PIXEL_FORMAT == AR_PIXEL_FORMAT_BGRA)
//            if( *(pnt+0) + *(pnt+1) + *(pnt+2) <= thresht3 )
//#elif (AR_DEFAULT_PIXEL_FORMAT == AR_PIXEL_FORMAT_BGR)
//            if( *(pnt+0) + *(pnt+1) + *(pnt+2) <= thresht3 )
//#elif (AR_DEFAULT_PIXEL_FORMAT == AR_PIXEL_FORMAT_RGBA)
//            if( *(pnt+0) + *(pnt+1) + *(pnt+2) <= thresht3 )
//#elif (AR_DEFAULT_PIXEL_FORMAT == AR_PIXEL_FORMAT_RGB)
//            if( *(pnt+0) + *(pnt+1) + *(pnt+2) <= thresht3 )
//#elif (AR_DEFAULT_PIXEL_FORMAT == AR_PIXEL_FORMAT_MONO)
//            if( *(pnt) <= thresh )
//#elif (AR_DEFAULT_PIXEL_FORMAT == AR_PIXEL_FORMAT_2vuy)
//            if( *(pnt+1) <= thresh )
//#elif (AR_DEFAULT_PIXEL_FORMAT == AR_PIXEL_FORMAT_yuvs)
//            if( *(pnt+0) <= thresh )
//#else
//#  error Unknown default pixel format defined in config.h
//#endif
//            {
//                pnt1 = &(pnt2[-lxsize]);
//                if( *pnt1 > 0 ) {
//                    *pnt2 = *pnt1;

//#ifndef USE_OPTIMIZATIONS
//                    // ORIGINAL CODE
//                    work2[((*pnt2)-1)*7+0] ++;
//                    work2[((*pnt2)-1)*7+1] += i;
//                    work2[((*pnt2)-1)*7+2] += j;
//                    work2[((*pnt2)-1)*7+6] = j;
//#else
//                    // OPTIMIZED CODE [tp]
//                    // ((*pnt2)-1)*7 should be treated as constant, since
//                    //  work2[n] (n=0..xsize*ysize) cannot overwrite (*pnt2)
//                    pnt2_index = ((*pnt2)-1) * 7;
//                    work2[pnt2_index+0]++;
//                    work2[pnt2_index+1]+= i;
//                    work2[pnt2_index+2]+= j;
//                    work2[pnt2_index+6] = j;
//                    // --------------------------------
//#endif
//                }
//                else if( *(pnt1+1) > 0 ) {
//                    if( *(pnt1-1) > 0 ) {
//                        m = work[*(pnt1+1)-1];
//                        n = work[*(pnt1-1)-1];
//                        if( m > n ) {
//                            *pnt2 = n;
//                            wk = &(work[0]);
//                            for(k = 0; k < wk_max; k++) {
//                                if( *wk == m ) *wk = n;
//                                wk++;
//                            }
//                        }
//                        else if( m < n ) {
//                            *pnt2 = m;
//                            wk = &(work[0]);
//                            for(k = 0; k < wk_max; k++) {
//                                if( *wk == n ) *wk = m;
//                                wk++;
//                            }
//                        }
//                        else *pnt2 = m;

//#ifndef USE_OPTIMIZATIONS
//                        // ORIGINAL CODE
//                        work2[((*pnt2)-1)*7+0] ++;
//                        work2[((*pnt2)-1)*7+1] += i;
//                        work2[((*pnt2)-1)*7+2] += j;
//                        work2[((*pnt2)-1)*7+6] = j;
//#else
//                        // PERFORMANCE OPTIMIZATION:
//                        pnt2_index = ((*pnt2)-1) * 7;
//                        work2[pnt2_index+0]++;
//                        work2[pnt2_index+1]+= i;
//                        work2[pnt2_index+2]+= j;
//                        work2[pnt2_index+6] = j;
//#endif

//                    }
//                    else if( *(pnt2-1) > 0 ) {
//                        m = work[*(pnt1+1)-1];
//                        n = work[*(pnt2-1)-1];
//                        if( m > n ) {
//                            *pnt2 = n;
//                            wk = &(work[0]);
//                            for(k = 0; k < wk_max; k++) {
//                                if( *wk == m ) *wk = n;
//                                wk++;
//                            }
//                        }
//                        else if( m < n ) {
//                            *pnt2 = m;
//                            wk = &(work[0]);
//                            for(k = 0; k < wk_max; k++) {
//                                if( *wk == n ) *wk = m;
//                                wk++;
//                            }
//                        }
//                        else *pnt2 = m;

//#ifndef USE_OPTIMIZATIONS
//                        // ORIGINAL CODE
//                        work2[((*pnt2)-1)*7+0] ++;
//                        work2[((*pnt2)-1)*7+1] += i;
//                        work2[((*pnt2)-1)*7+2] += j;
//#else
//                        // PERFORMANCE OPTIMIZATION:
//                        pnt2_index = ((*pnt2)-1) * 7;
//                        work2[pnt2_index+0]++;
//                        work2[pnt2_index+1]+= i;
//                        work2[pnt2_index+2]+= j;
//#endif

//                    }
//                    else {
//                        *pnt2 = *(pnt1+1);

//#ifndef USE_OPTIMIZATIONS
//                        // ORIGINAL CODE
//                        work2[((*pnt2)-1)*7+0] ++;
//                        work2[((*pnt2)-1)*7+1] += i;
//                        work2[((*pnt2)-1)*7+2] += j;
//                        if( work2[((*pnt2)-1)*7+3] > i ) work2[((*pnt2)-1)*7+3] = i;
//                        work2[((*pnt2)-1)*7+6] = j;
//#else
//                        // PERFORMANCE OPTIMIZATION:
//                        pnt2_index = ((*pnt2)-1) * 7;
//                        work2[pnt2_index+0]++;
//                        work2[pnt2_index+1]+= i;
//                        work2[pnt2_index+2]+= j;
//                        if( work2[pnt2_index+3] > i ) work2[pnt2_index+3] = i;
//                        work2[pnt2_index+6] = j;
//#endif
//                    }
//                }
//                else if( *(pnt1-1) > 0 ) {
//                    *pnt2 = *(pnt1-1);

//#ifndef USE_OPTIMIZATIONS
//                        // ORIGINAL CODE
//                    work2[((*pnt2)-1)*7+0] ++;
//                    work2[((*pnt2)-1)*7+1] += i;
//                    work2[((*pnt2)-1)*7+2] += j;
//                    if( work2[((*pnt2)-1)*7+4] < i ) work2[((*pnt2)-1)*7+4] = i;
//                    work2[((*pnt2)-1)*7+6] = j;
//#else
//                    // PERFORMANCE OPTIMIZATION:
//                    pnt2_index = ((*pnt2)-1) * 7;
//                    work2[pnt2_index+0]++;
//                    work2[pnt2_index+1]+= i;
//                    work2[pnt2_index+2]+= j;
//                    if( work2[pnt2_index+4] < i ) work2[pnt2_index+4] = i;
//                    work2[pnt2_index+6] = j;
//#endif
//                }
//                else if( *(pnt2-1) > 0) {
//                    *pnt2 = *(pnt2-1);

//#ifndef USE_OPTIMIZATIONS
//                        // ORIGINAL CODE
//                    work2[((*pnt2)-1)*7+0] ++;
//                    work2[((*pnt2)-1)*7+1] += i;
//                    work2[((*pnt2)-1)*7+2] += j;
//                    if( work2[((*pnt2)-1)*7+4] < i ) work2[((*pnt2)-1)*7+4] = i;
//#else
//                    // PERFORMANCE OPTIMIZATION:
//                    pnt2_index = ((*pnt2)-1) * 7;
//                    work2[pnt2_index+0]++;
//                    work2[pnt2_index+1]+= i;
//                    work2[pnt2_index+2]+= j;
//                    if( work2[pnt2_index+4] < i ) work2[pnt2_index+4] = i;
//#endif
//                }
//                else {
//                    wk_max++;
//                    if( wk_max > WORK_SIZE ) {
//                        return(0);
//                    }
//                    work[wk_max-1] = *pnt2 = wk_max;
//                    work2[(wk_max-1)*7+0] = 1;
//                    work2[(wk_max-1)*7+1] = i;
//                    work2[(wk_max-1)*7+2] = j;
//                    work2[(wk_max-1)*7+3] = i;
//                    work2[(wk_max-1)*7+4] = i;
//                    work2[(wk_max-1)*7+5] = j;
//                    work2[(wk_max-1)*7+6] = j;
//                }
//            }
//            else {
//                *pnt2 = 0;
//            }
//        }
//        if (arImageProcMode == AR_IMAGE_PROC_IN_HALF) pnt += arImXsize*AR_PIX_SIZE_DEFAULT;
//    }

//    j = 1;
//    wk = &(work[0]);
//    for(i = 1; i <= wk_max; i++, wk++) {
//        *wk = (*wk==i)? j++: work[(*wk)-1];
//    }
//    *label_num = *wlabel_num = j - 1;
//    if( *label_num == 0 ) {
//        return( l_image );
//    }

//    put_zero( (ARUint8 *)warea, *label_num *     sizeof(int) );
//    put_zero( (ARUint8 *)wpos,  *label_num * 2 * sizeof(double) );
//    for(i = 0; i < *label_num; i++) {
//        wclip[i*4+0] = lxsize;
//        wclip[i*4+1] = 0;
//        wclip[i*4+2] = lysize;
//        wclip[i*4+3] = 0;
//    }
//    for(i = 0; i < wk_max; i++) {
//        j = work[i] - 1;
//        warea[j]    += work2[i*7+0];
//        wpos[j*2+0] += work2[i*7+1];
//        wpos[j*2+1] += work2[i*7+2];
//        if( wclip[j*4+0] > work2[i*7+3] ) wclip[j*4+0] = work2[i*7+3];
//        if( wclip[j*4+1] < work2[i*7+4] ) wclip[j*4+1] = work2[i*7+4];
//        if( wclip[j*4+2] > work2[i*7+5] ) wclip[j*4+2] = work2[i*7+5];
//        if( wclip[j*4+3] < work2[i*7+6] ) wclip[j*4+3] = work2[i*7+6];
//    }

//    for( i = 0; i < *label_num; i++ ) {
//        wpos[i*2+0] /= warea[i];
//        wpos[i*2+1] /= warea[i];
//    }

//    *label_ref = work;
//    *area      = warea;
//    *pos       = wpos;
//    *clip      = wclip;
//    return (l_image);



            return 0;
        }
        private int DebugLabel()
        {
            return 0;
        }

    }
}
