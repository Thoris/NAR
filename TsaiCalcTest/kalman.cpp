#include <stdlib.h>

#include "kalman.h"

#define NO_ERR 0
#define NULLPTR_ERR 1
#define BADSIZE_ERR 2
#define BADCOEF_ERR 3
typedef int Status;

#define min(x,y) (x<y?x:y)

void DotProduct (float *op1, float *op2, int dim, float *result)
{
    int i;
    *result = 0.0;
    for (i=0; i<dim; i++ ) 
      *result += op1[i] * op2[i];
}

float * CreateMatrix(int m, int n) {
   return (float *) malloc(m*n*sizeof(float));
}

float * CreateVector(int n) {
   return (float *) malloc(n*sizeof(float));
}

void DeleteMatrix(float *m) {
  free(m);
}

void DeleteVector(float *v) {
  free(v);
}


void   CopyMatrix (float *srcMatr, int m, int n,float *destMatr)
{
    int i;
    for (i=0; i<m*n; i++) destMatr[i] = srcMatr[i];
}

void   CopyVector (float *srcVec, int n,float *destVec)
{
    int i;
    for (i=0; i<n; i++) destVec[i] = srcVec[i];
}


void  SetIdentity(float * matr, int m, int n)
{
    int i, k;
    for (i=0; i<m*n; i++)  matr[i] = 0.0;
    k = (m<n ? m : n);
    for (i=0; i<k; i++) matr[i*n+i] = 1.0;
}

void ScaleVector( float *src, float *dest, int n, float k) {
  int i;
  for (i=0; i<n; i++) dest[i]=src[i]*k;
}

void SubVector( float *src1, float *src2, float *dest, int n) {
  int i;
   for (i=0; i<n; i++) dest[i]=src1[i] - src2[i];
}

void AddVector( float *src1, float *src2, float *dest, int n) {
  int i;
   for (i=0; i<n; i++) dest[i]=src1[i] + src2[i];
}





Status AddMatrix ( float * srcMatr1,
                           float * srcMatr2,
                           float * destMatr,
                           int matrHeight,
                           int matrWidth )
{
    int step, i, j; 
    float* src1;
    float* src2;
    float* dst;
	    
    /* check bad arguments */
    if ( srcMatr1 == NULL ) return NULLPTR_ERR;
    if ( destMatr == NULL ) return NULLPTR_ERR;
    if ( srcMatr2 == NULL ) return NULLPTR_ERR;
    if ( matrWidth <= 0 ) return BADSIZE_ERR;
    if ( matrHeight <= 0 ) return BADSIZE_ERR;
    
    step = matrWidth;
    
    src1 = srcMatr1;
    src2 = srcMatr2;
    dst = destMatr;
    
    for ( i = 0; i < matrHeight; i++ )
    {
        for ( j = 0; j < matrWidth; j++ )
            dst[j] = src1[j] + src2[j];
        dst+=step;
        src2 += step;
        src1+=step;
    }
    return NO_ERR;
}

/*F///////////////////////////////////////////////////////////////////////////////////////
//    Name:       SubMatrix   
//    Purpose:    Difference of 2 matrix
//    Context:   
//    Parameters: srcMatr1 - first matrix
//                srcMatr2 - second matrix(subtrahend)
//                destMatr - destination matrix
//                matrWidth
//                matrHeight - sizes of matrix       
//               
//    Returns:   
//    Notes:     
//F*/

Status SubMatrix( float * srcMatr1,
                           float * srcMatr2,   
                           float * destMatr,
                           int matrHeight,
                           int matrWidth )
{
    int step, i, j; 
    float* src1;
    float* src2;
    float* dst;
    
    /* check bad arguments */
    if ( srcMatr1 == NULL ) return NULLPTR_ERR;
    if ( destMatr == NULL ) return NULLPTR_ERR;
    if ( srcMatr2 == NULL ) return NULLPTR_ERR;
    if ( matrWidth <= 0 ) return BADSIZE_ERR;
    if ( matrHeight <= 0 ) return BADSIZE_ERR;
    
    step = matrWidth;
    
    src1 = srcMatr1;
    src2 = srcMatr2;
    dst = destMatr;
    
    for ( i = 0; i < matrHeight; i++ )
    {
        for ( j = 0; j < matrWidth; j++ )
        {   
            dst[j] = src1[j] - src2[j];
        }
        dst+=step;
        src2 += step;
        src1+=step;
    }
    return NO_ERR;
}

/*F///////////////////////////////////////////////////////////////////////////////////////
//    Name:       cvScaleMatrix   
//    Purpose:    Multiplies the Matrix by a value
//    Context:   
//    Parameters: srcMatr -  source  matrix
//                destMatr - destination matrix
//                matrWidth
//                matrHeight - sizes of matrix
//                value - factor       
//               
//    Returns:   
//    Notes:     
//F*/
Status ScaleMatrix( float * srcMatr,
                             float * destMatr,
                             int matrHeight,
                             int matrWidth,
                             float value )
{
    int step, i, j; 
    float* src;
    float* dst;
    
    /* check bad arguments */
    if ( srcMatr == NULL ) return NULLPTR_ERR;
    if ( destMatr == NULL ) return NULLPTR_ERR;
    if ( matrWidth <= 0 ) return BADSIZE_ERR;
    if ( matrHeight <= 0 ) return BADSIZE_ERR;
    
    step = matrWidth;
    
    src = srcMatr;
    dst = destMatr;
    
    for ( i = 0; i < matrHeight; i++ )
    {
        for ( j = 0; j < matrWidth; j++ )
        {   
            dst[j] = value * src[j];
        }
        dst+=step;
        src += step;
    }
    return NO_ERR;
}

/*F///////////////////////////////////////////////////////////////////////////////////////
//    Name:       cvMulMatrix   
//    Purpose:    Product of 2 matrix
//    Context:   
//    Parameters: srcMatr1 - first matrix
//                matrWidth1
//                matrHeight1 - sizes of srcMatr1
//                srcMatr2 - second matrix
//                matrWidth2
//                matrHeight2 - sizes of srcMatr2       
//
//                destMatr - destination matrix
//               
//    Returns:
//    NO_ERROR or error code     
//    Notes:
//    matrWidth1 mus be equal to matrHeight2      
//F*/
Status MulMatrix (  float * srcMatr1,
                             int matrHeight1,
                             int matrWidth1,
                             float * srcMatr2,
                             int matrHeight2,
                             int matrWidth2,
                             float * destMatr )
{
    int i,j,k;
    float sum = 0;
    float* first = srcMatr1;
    float* second = srcMatr2;
    float* dest = destMatr;
    
    /* check bad arguments */
    if ( srcMatr1 == NULL ) return NULLPTR_ERR;
    if ( destMatr == NULL ) return NULLPTR_ERR;
    if ( srcMatr2 == NULL ) return NULLPTR_ERR;
    if ( matrWidth1 <= 0 ) return BADSIZE_ERR;
    if ( matrHeight1 <= 0 ) return BADSIZE_ERR;
    if ( matrWidth2 <= 0 ) return BADSIZE_ERR;
    if ( matrHeight2 <= 0 ) return BADSIZE_ERR;
    if ( matrHeight2 != matrWidth1 ) return BADSIZE_ERR;
    for(j = 0; j < matrHeight1; j++)
    {
        for(i = 0; i < matrWidth2; i++)
        {
            sum = 0;
            second = srcMatr2 + i;
            for(k = 0; k < matrWidth1; k++)
            {
                sum += first[k]*(*second);
                second+=matrWidth2;
            }
            dest[i] = sum;
        }
        first += matrWidth1;
        dest += matrWidth2;
    }
    return NO_ERR;
}

void TransformVector (float *mat, float *src, float*dest, int m, int n){
  MulMatrix(mat, m, n, src, n, 1, dest);
}

/*F///////////////////////////////////////////////////////////////////////////////////////
//    Name:       ippmMulTransposed   
//    Purpose:    Product of 2 matrix
//    Context:   
//    Parameters: srcMatr1 - first matrix
//                matrWidth1
//                matrHeight1 - sizes of srcMatr1
//                srcMatr2 - second matrix
//                matrWidth2
//                matrHeight2 - sizes of srcMatr2       
//
//                destMatr - destination matrix
//               
//    Returns:
//    IPP_NO_ERROR or error code     
//    Notes:
//    matrWidth1 mus be equal to matrHeight2      
//F*/
Status MulTransposed ( float * srcMatr1,
								int matrHeight1,
								int matrWidth1,
								float * srcMatr2,
								int matrHeight2,
								int matrWidth2,
								float * destMatr
                             )
{
    int i,j;
    for(i = 0; i < matrHeight1; i++)
	{
		for(j = 0; j < matrHeight2; j++)
		{
			DotProduct(srcMatr1+i*matrWidth1,
							   srcMatr2+j*matrWidth2,
							   matrWidth1,
							   destMatr+i*matrHeight2+j);
		}
	}
    return NO_ERR;
}


/*F///////////////////////////////////////////////////////////////////////////////////////
//    Name:       cvMulTransMatrixL, cvMulTransMatrixR
//    Purpose:    Multiply a matrix by transposed itself
//    Context:   
//    Parameters: srcMatr (matrHeight, matrWidth) - source matrix
//                                (matrHeight - rows number, matrWidth - columns number)
//                destMatr(matrHeight, matrWidth) - destination matrix
//    Returns:
//    NO_ERROR or error code     
//    Notes: 1) The function cvMulTransMatrixL calculates product A At,
//              where A - source matrix, At - its transposition.
//              The function cvMulTransMatrixR calculates product At A.
//           2) Performs more quickly then ordinary multiplication.
//F*/
Status MulTransMatrixL ( float * srcMatr,
                                  int      matrWidth,
                                  int      matrHeight,
                                  float * destMatr )
{
        float tem;
        int i,j;
        if ( srcMatr== NULL || destMatr== NULL || srcMatr == destMatr  ) return NULLPTR_ERR;
        if ( matrHeight <= 0 || matrWidth <= 0 ) return BADSIZE_ERR;
        
        
        for(i = 0; i < matrHeight; i++)
        {
            for(j = 0; j <= i; j++)
            {
                DotProduct(srcMatr + j * matrWidth,
                    srcMatr+i*matrWidth,
                    matrWidth,
                    &tem);
                destMatr[i*matrHeight+j] =tem;
                destMatr[j*matrHeight+i] =tem;
            }
            
        }
        return NO_ERR;
}
/*--------------------------------------------------------------------------------------*/
Status MulTransMatrixR( float * srcMatr,
                                 int      matrWidth,
                                 int      matrHeight,
                                 float * destMatr )
{
    int i, j;
    float *C = destMatr, *col_i;
    
    if ( srcMatr == NULL || C == NULL || srcMatr == C  ) return NULLPTR_ERR;
    if ( matrHeight <= 0 || matrWidth <= 0 ) return BADSIZE_ERR;
    
    col_i = (float*)malloc(4*matrHeight);
    
    for(i = 0; i < matrWidth; i++, C += matrWidth)
    {
        for(j = 0; j<matrHeight; j++) col_i[j] = srcMatr[j*matrWidth+i];
        for(j = i; j < matrWidth; j++)
        {
            int  st = matrWidth, k = 0;
            float s = 0.0f, *A = srcMatr;
            for(; k < matrHeight; k++, A+=st) s += col_i[k]*A[j];
            C[j] = s;
        }
    }
    free((void**)&col_i);
    for(i = 1; i < matrWidth; i++)
        for(j = 0; j < i; j++)
            destMatr[j+i*matrWidth] = destMatr[i+j*matrWidth];
        return NO_ERR;
        
}

/*F///////////////////////////////////////////////////////////////////////////////////////
//    Name:       cvTransposeMatrix   
//    Purpose:    Transposes matrix
//    Context:   
//    Parameters: srcMatr - source matrix
//                matrWidth
//                matrHeight - sizes of srcMatr
//                destMatr - destination matrix
//               
//    Returns:
//    NO_ERROR or error code     
//    Notes:
//    matrWidth1 mus be equal to matrHeight2      
//F*/
Status TransposeMatrix(  float * srcMatr,
                                  int matrHeight,
                                  int matrWidth,
                                  float * destMatr )
{
    int i,j;
    float* dst = destMatr;
    float* src = srcMatr;
    int Decrem = matrWidth * matrHeight-1;
    if ( srcMatr == NULL ) return NULLPTR_ERR;
    if ( destMatr == NULL ) return NULLPTR_ERR;
    if ( matrWidth <= 0 ) return BADSIZE_ERR;
    if ( matrHeight <= 0 ) return BADSIZE_ERR;
    for(j = 0; j < matrHeight; j++)
    {
        for(i = 0; i < matrWidth; i++)
        {
            *dst = src[i];
            dst+=matrHeight;
        }
        dst-=Decrem;
        src+=matrWidth;
    }
    return NO_ERR;
}

/*F///////////////////////////////////////////////////////////////////////////////////////
//    Name:       cvInvertMatrix   
//    Purpose:    Invertes matrix
//    Context:   
//    Parameters: srcMatr - source matrix
//                matrSize - sizes of srcMatr
//                destMatr - destination matrix
//               
//    Returns:
//    NO_ERROR or error code     
//    Notes:
//F*/
Status InvertMatrix( float * srcMatr,
                              int matrSize,
                              float * destMatr )

{
    float *    Tem;
    float *    tmVect;
    float *    tmLineVect;
    float *    tmDstLineVect;
    int         i,j;
    float       Mult;
    int         change,st;
    int         change_sign;

    change_sign = 0;
    Tem           = CreateMatrix(matrSize,matrSize);
    tmVect        = CreateVector(matrSize);
    tmLineVect    = CreateVector(matrSize);
    tmDstLineVect = CreateVector(matrSize);

    CopyMatrix (srcMatr, matrSize, matrSize,Tem);
    SetIdentity(destMatr,matrSize, matrSize);

    for (i = 0; i < matrSize; i++) {

        /* Sort line for non zero element */

        change = i;


        /* !!! In change when test !=0 if all values == 0 then DET=0 and INV not exist */

        for (st = i; st < matrSize; st++) {

            if (Tem[st * matrSize + i] != 0) {

                change = st;
                break;
            }
        }

        if (change != i ) {
            CopyVector (    &Tem[(i) * matrSize ],
                                    matrSize,
                                    tmVect);

            CopyVector (    &Tem[st * matrSize ],
                                    matrSize,
                                    &Tem[(i) * matrSize ]);
            
            CopyVector (    tmVect,
                                    matrSize,
                                    &Tem[st * matrSize ]);

            CopyVector (    &destMatr[(i) * matrSize ],
                                    matrSize,
                                    tmVect);

            CopyVector (    &destMatr[st * matrSize ],
                                    matrSize,
                                    &destMatr[(i) * matrSize ]);
            
            CopyVector (    tmVect,
                                    matrSize,
                                    &destMatr[st * matrSize ]);
            change_sign++;
        
        } else {
            if (Tem[i * matrSize + i] == 0) {
                /* Inv matrix does not exist det == 0 */
                return BADCOEF_ERR;
            }
        }

        Mult = 1.0f / Tem[i * matrSize + i];

        Tem[i * matrSize + i] = 1.0f;
        
        /* Test !!!*/
        
        ScaleVector(    &Tem[i * matrSize + i + 1],
                                &Tem[i * matrSize + i + 1],
                                matrSize - i - 1,Mult);

        ScaleVector(    &destMatr[i * matrSize  ],
                                &destMatr[i * matrSize  ],
                                matrSize ,Mult);

        /* Subtructing */

        CopyVector (    &Tem[i * matrSize + i + 1],
                                matrSize - i - 1,
                                tmLineVect);

        CopyVector (    &destMatr[i * matrSize],
                                matrSize,
                                tmDstLineVect);

        for (j = i + 1; j < matrSize; j++) {
            
            Mult = Tem[j * matrSize + i];

            Tem[j * matrSize + i] = 0.0f;

            ScaleVector(    tmLineVect,
                                    tmVect,
                                    matrSize - i - 1, Mult);

            SubVector(      &Tem[j * matrSize + i + 1],
                                    tmVect,
                                    &Tem[j * matrSize + i + 1],
                                    matrSize - i - 1);


            ScaleVector(    tmDstLineVect,
                                    tmVect,
                                    matrSize, Mult);

            SubVector(      &destMatr[j * matrSize],
                                    tmVect,
                                    &destMatr[j * matrSize],
                                    matrSize);

        }
    }

    /* Down to top direct */

    for (i = matrSize - 1; i > 0; i--) {

        /* Subtructing */

        CopyVector (    &Tem[i * matrSize + i + 1],
                                matrSize - i - 1,
                                tmLineVect);

        CopyVector (    &destMatr[i * matrSize],
                                matrSize,
                                tmDstLineVect);

        for (j = i - 1; j >= 0 ; j--) {

            Mult = Tem[j * matrSize + i];

            Tem[j * matrSize + i] = 0.0f;

            ScaleVector(    tmLineVect,
                                    tmVect,
                                    matrSize - i - 1, Mult);

            SubVector(      &Tem[j * matrSize + i + 1],
                                    tmVect,
                                    &Tem[j * matrSize + i + 1],
                                    matrSize - i - 1);


            ScaleVector(    tmDstLineVect,
                                    tmVect,
                                    matrSize, Mult);

            SubVector(      &destMatr[j * matrSize],
                                    tmVect,
                                    &destMatr[j * matrSize],
                                    matrSize);


        }
    }

    if (change_sign & 1 == 1) {
        ScaleMatrix(destMatr,destMatr,matrSize,matrSize,-1.0f);
    }


    DeleteMatrix(Tem          );
    DeleteVector(tmVect       );
    DeleteVector(tmLineVect   );
    DeleteVector(tmDstLineVect);

    return NO_ERR;
}/* InvertMatrix */

/*F///////////////////////////////////////////////////////////////////////////////////////
//    Name:       DetMatrix   
//    Purpose:    Comute determinate of matrix
//    Context:   
//    Parameters: matr - source matrix
//                matrSize - sizes of srcMatr
//                det - result
//               
//    Returns:
//    NO_ERROR or error code     
//    Notes:
//F*/
Status DetMatrix( float *    matr,
                           int         matrSize,
                           float*      det )
{
    float *    matrWork;
    float *    tmLine;
    int         change;
    int         i,j,t;
    int         change_sign;
    float       coef;
    float       mul;

    matrWork = CreateMatrix(matrSize, matrSize);
    tmLine  = CreateVector(matrSize);

    change_sign = 0;
    *det = 0;

    CopyMatrix(matr,matrSize,matrSize,matrWork);

    /* Make matrix is up triangular */
    for (i = 0; i < matrSize; i++) {

        /* Test first element for 0 */

        if (matrWork[i * matrSize + i] == 0) {
            /* try to find a non zero element */
            change = i;
            for (t = i; t < matrSize; t++) {
                if (matrWork[t * matrSize + i] != 0) {
                    change = t;
                    break;
                }
            }

            if (change != i) {/* change lines */
                CopyVector( matrWork + change * matrSize,
                                    matrSize,
                                    tmLine);

                CopyVector( matrWork + i * matrSize,
                                    matrSize,
                                    matrWork + change * matrSize);

                CopyVector( tmLine,
                                    matrSize,
                                    matrWork + i * matrSize);
                change_sign++;

            } else {/* Element != 0 not found */
                DeleteMatrix(matrWork);
                DeleteVector(tmLine);
                return NO_ERR;/* det == 0 */
            }
        }

        /* Virtual set elements to zero */

        coef = 1.0f / matrWork[i*matrSize+i];

        for (j = i + 1; j < matrSize; j++) {

            mul = coef * matrWork[j * matrSize + i];

            ScaleVector(    matrWork + i * matrSize + i + 1,
                                    tmLine,
                                    matrSize - i - 1,
                                    mul);

            SubVector(  matrWork + j * matrSize + i + 1,
                                tmLine,
                                matrWork + j * matrSize + i + 1,
                                matrSize - i - 1);

        }
    }

    if ((change_sign&1)==1) {
        *det = -1;

    } else {
        *det = 1;
    }

    for (t=0;t<matrSize;t++) {
        *det *= matrWork[t*matrSize+t];
    }

    return NO_ERR;
}/* DetMatrix */

/*F///////////////////////////////////////////////////////////////////////////////////////
//    Name:       Trace   
//    Purpose:    Comute the trace of matrix, sum of diagonal elements
//    Context:   
//    Parameters: matr - source matrix
//                width
//                height - sizes of matr
//                trace - result
//               
//    Returns:
//    NO_ERROR or error code     
//    Notes:
//F*/
Status Trace( float *    matr,
                       int         width,
                       int         height,
                       float*      trace )
{
    int t;
    int size;
    float tr;

    size = min(width,height);
    tr = 0;
    for (t = 0; t < size; t++) {
        tr += matr[t*width+t];
    }
    *trace = tr;

    return NO_ERR;
}/* Trace */


/* End of file */





KalmanStruct* CreateKalman(int n, int m)
{
   KalmanStruct* Kalm;
   Kalm = (KalmanStruct*)malloc(sizeof(KalmanStruct));
   Kalm->n = n;
   Kalm->m = m;
//   Kalm->Phi = (float*)malloc(sizeof(float)*n*n);
//   Kalm->H = (float*)malloc(sizeof(float)*m*n);
//   Kalm->R =(float*)malloc(sizeof(float)*m*m);
//   Kalm->Q =(float*)malloc(sizeof(float)*n*n);
   Kalm->PriorState =(float*)malloc(sizeof(float)*n);
//   Kalm->x =(float*)malloc(sizeof(float)*n);
//   Kalm->P = (float*)malloc(sizeof(float)*n*n);
   Kalm->PriorErrorCovariance  = (float*)malloc(sizeof(float)*n*n);
   Kalm->K = (float*)malloc(sizeof(float)*n*m);
   Kalm->Temp1 = (float*)malloc(sizeof(float)*n*n);
   Kalm->Temp2 = (float*)malloc(sizeof(float)*n*n);
   Kalm->MeasErrorCovariance = (float*)malloc(sizeof(float)*m*m);
   return (Kalm);
}

 void ReleaseKalman(KalmanStruct* Kalm)
{
  free(&Kalm->PriorState);
	free(&Kalm->x);
	free(&Kalm->Phi);
	free(&Kalm->H);
	free(&Kalm->Q);
	free(&Kalm->R);
	free(&Kalm->P);
	free(&Kalm->PriorErrorCovariance);
	free(&Kalm->K);
	free(Kalm);
}

void KalmanPredict(KalmanStruct* Kalman)
{
    //Updating the state
    TransformVector(Kalman->Phi,
							Kalman->x,
							Kalman->PriorState,
							Kalman->n,
							Kalman->n);
    //Updating the Error Covariances
    MulTransposed(Kalman->P,
						  Kalman->n,
						  Kalman->n,
						  Kalman->Phi,
						  Kalman->n,
						  Kalman->n,
						  Kalman->Temp1);
    MulMatrix(Kalman->Phi,
					  Kalman->n,
					  Kalman->n,
					  Kalman->Temp1,
					  Kalman->n,
					  Kalman->n,
					  Kalman->PriorErrorCovariance);
    AddMatrix(Kalman->PriorErrorCovariance,
					  Kalman->Q,
					  Kalman->PriorErrorCovariance,
					  Kalman->n,
					  Kalman->n);

}



void KalmanCorrect(KalmanStruct* Kalman, float* Measure)
{

    /* Calculating Kalman Gain */
	MulTransposed(Kalman->PriorErrorCovariance,
					   Kalman->n,
					   Kalman->n, 
					   Kalman->H,
					   Kalman->m,
					   Kalman->n,
					   Kalman->Temp1);
	MulMatrix(Kalman->H,
				   Kalman->m,
				   Kalman->n,
				   Kalman->Temp1,
				   Kalman->n,
				   Kalman->m,
				   Kalman->K);
	AddMatrix(Kalman->K,
				   Kalman->R,
				   Kalman->K,
				   Kalman->m,
				   Kalman->m);
	InvertMatrix(Kalman->K,
					  Kalman->m,
					  Kalman->MeasErrorCovariance);
	MulMatrix(Kalman->Temp1,
				   Kalman->n,
				   Kalman->m,
				   Kalman->MeasErrorCovariance,
				   Kalman->m,
				   Kalman->m,
				   Kalman->K);
	/* Update the estimation */
	TransformVector(Kalman->H,
						 Kalman->PriorState,
						 Kalman->Temp1,
						 Kalman->m,
						 Kalman->n);
	SubVector(Measure,
				   Kalman->Temp1,
				   Kalman->Temp2,
				   Kalman->m);

 /* Tem2 = Hx_k - z_k */

	TransformVector(Kalman->K,
						 Kalman->Temp2,
						 Kalman->Temp1,
						 Kalman->n,
						 Kalman->m);
	AddVector(Kalman->PriorState,
				   Kalman->Temp1,
				   Kalman->x,
				   Kalman->n);

    /* Comute the chi-square statistics */
	TransformVector(Kalman->MeasErrorCovariance,
						 Kalman->Temp2,
						 Kalman->Temp1,
						 Kalman->m,
						 Kalman->m);
   
	DotProduct(Kalman->Temp2, Kalman->Temp1, Kalman->m, &Kalman->chi_square);
	/* Update the Error Covariance */
	MulMatrix(Kalman->K,
				   Kalman->n,
				   Kalman->m,
				   Kalman->H,
				   Kalman->m,
				   Kalman->n,
				   Kalman->P);
	SetIdentity(Kalman->Temp1,
					 Kalman->n,
					 Kalman->n);
	SubMatrix(Kalman->Temp1,
				   Kalman->P,
				   Kalman->Temp1,
				   Kalman->n,
				   Kalman->n);
	MulMatrix(Kalman->Temp1,
				   Kalman->n,
				   Kalman->n,
				   Kalman->PriorErrorCovariance,
				   Kalman->n,
				   Kalman->n,
				   Kalman->P);

}


