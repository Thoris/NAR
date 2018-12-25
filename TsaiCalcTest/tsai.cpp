#include <math.h>
#include "tsai.h"

#define EPSILON		1.0E-8
#define MAX_POINTS	128
#define MAXDIM 9+MAX_POINTS

#define ABS(a)          (((a) > 0) ? (a) : -(a))
#define SIGNBIT(a)      (((a) > 0) ? 0 : 1)
#define SQR(a)          ((a) * (a))

static double U[5];

/*
 * Biblioteca Matemática
 */
#define SWAP(a,b) {double temp=(a);(a)=(b);(b)=temp;}

#define TOL 1e-10

static int Gauss (int n,                     /* [in] Número de equações                         */
                  double a[MAXDIM][MAXDIM],  /* [in] Matriz dos coeficientes                    */
                  double *b)                 /* [in] Vetor das constantes                       */
                                             /* [out] Vetor solução                             */
                                             /* Valor de retorno :  (-1) => sem solução         */
                                             /*                     ( 0) => solução unica       */
                                             /*                     ( 1) => infinitas soluções  */
{
	int    i, j, k;
	int    imax;            /* Linha pivot */
	double amax, rmax;      /* Auxiliares  */

	for (j=0; j<n-1; j++) 
	{  
		/* Loop nas colunas de a */
		rmax = 0.0;
		imax = j;
		for (i=j; i<n; i++)
		{   
			/* Loop para determinar maior relação a[i][j]/a[i][k] */
			amax = 0.0;
			for (k=j; k<n; k++)    /* Loop para determinar maior elemento da linha i     */
				if (fabs(a[i][k]) > amax)
					amax = fabs(a[i][k]);
			if (amax < TOL)        /* Verifica se os elementos da linha são nulos        */
				return -1;            /* Finaliza subrotina pois o sistema não tem solução  */
			else if ((fabs(a[i][j]) > (rmax*amax)) && (fabs(a[i][j]) >= TOL)) 
			{
				/* Testa relação da linha corrente */
				rmax = fabs(a[i][j]) / amax;
				imax = i;             /* Encontra linha de maior relação - linha pivot      */
			}
		}
		if (fabs(a[imax][j])<TOL) 
		{
			/* Verifica se o pivot é nulo              */
			for (k=imax+1; k<n; k++)          /* Procura linha com pivot nao nulo        */
				if (fabs(a[k][j]) < TOL)
					imax = k;                       /* Troca linha j por linha k               */
			if (fabs(a[imax][j]) < TOL)
				return 1;                        /* não existe solução única para o sistema */
		}
		if (imax != j) 
		{                   
			/* Troca linha j por linha imax            */
			for (k=j; k<n; k++)
				SWAP(a[imax][k], a[j][k]);
			SWAP(b[imax], b[j]);
		}
		for (i=j+1; i<n; i++) 
		{            /* Anula elementos abaixo da diagonal      */
			double aux = a[i][j] / a[j][j];
			for (k=j+1; k<n; k++)             /* Transforma os demais elementos da linha */
				a[i][k] -= aux * a[j][k];
				b[i] -= aux * b[j];
		}
	}
	if (fabs(a[n-1][n-1]) <= TOL)       /* Verifica unicidade da solução           */
		return -1;                         /* Sistema sem solução                     */
	else 
	{
		b[n-1] /= a[n-1][n-1];             /* Calcula última coordenada               */
		for (i=n-2; i>=0; i--) 
		{           /* Inicia retro-substituição               */
			for (j=i+1; j<n; j++)
				b[i] -= a[i][j] * b[j];
			b[i] /= a[i][i];
		}
	}
	return 0;                           /* Finaliza a subrotina com solução única  */
}

static int MinQuad(int nl, int nc, double A[MAX_POINTS][5], double B[MAX_POINTS])
{
 int i, j, k;
 double X[5];
 double M[MAXDIM][MAXDIM];

 for (i=0; i<nc; i++)
  for (j=0; j<nc; j++) {
   M[i][j] = A[0][j]*A[0][i];
   for (k=1; k<nl; k++)
    M[i][j] += A[k][j]*A[k][i];
  }

 for (i=0; i<nc; i++) {
  X[i] = A[0][i]*B[0];
  for (k=1; k<nl; k++)
   X[i] += A[k][i]*B[k];
 }

 if (Gauss(nc, M, X) != 0) return 0;

 for (i=0; i<nc; i++) 
  B[i] = X[i];
 return 1;
}

/*
 * Funcoes auxiliares
 */

static int cc_compute_U(int np, double *xw, double *yw, double *xf, double *yf)
{
 int i;

 double A[MAX_POINTS][5];
 double B[MAX_POINTS];

 for (i=0; i<np; i++) {
  A[i][0] = yf[i] * xw[i];
  A[i][1] = yf[i] * yw[i];
  A[i][2] = yf[i];
  A[i][3] = -xf[i] * xw[i];
  A[i][4] = -xf[i] * yw[i];
  B[i] = xf[i];
 }

 if (!MinQuad(np, 5, A, B)) return 0;

 U[0] = B[0];
 U[1] = B[1];
 U[2] = B[2];
 U[3] = B[3];
 U[4] = B[4];

 return 1;
}


static void cc_compute_Tx_and_Ty(int np, double *xw, double *yw, double *xf, double *yf, double T[3])
{
 double Ty_squared, Sr, distance, far_distance;
 double Tx, Ty, r1, r2, r4, r5, x, y;
 int i, far_point;

 double r1p = U[0],
        r2p = U[1],
        r4p = U[3],
        r5p = U[4];

 if ((fabs (r1p) < EPSILON) && (fabs (r2p) < EPSILON))
  Ty_squared = 1 / (SQR(r4p) + SQR(r5p));
 else if ((fabs (r4p) < EPSILON) && (fabs (r5p) < EPSILON))
  Ty_squared = 1 / (SQR(r1p) + SQR(r2p));
 else if ((fabs (r1p) < EPSILON) && (fabs (r4p) < EPSILON))
  Ty_squared = 1 / (SQR(r2p) + SQR(r5p));
 else if ((fabs (r2p) < EPSILON) && (fabs (r5p) < EPSILON))
  Ty_squared = 1 / (SQR(r1p) + SQR(r4p));
 else {
  Sr = SQR(r1p) + SQR(r2p) + SQR(r4p) + SQR(r5p);
  Ty_squared = (Sr - sqrt (SQR(Sr) - 4 * SQR(r1p * r5p - r4p * r2p))) /
               (2 * SQR(r1p * r5p - r4p * r2p));
 }

 far_distance = 0;
 far_point = 0;
 for (i=0; i<np; i++)
  if ((distance = SQR(xf[i]) + SQR(yf[i])) > far_distance) {
   far_point = i;
   far_distance = distance;
  }

 Ty = sqrt (Ty_squared);
 r1 = U[0] * Ty;
 r2 = U[1] * Ty;
 Tx = U[2] * Ty;
 r4 = U[3] * Ty;
 r5 = U[4] * Ty;
 x = r1 * xw[far_point] + r2 * yw[far_point] + Tx;
 y = r4 * xw[far_point] + r5 * yw[far_point] + Ty;

 if ((SIGNBIT (x) != SIGNBIT (xf[far_point])) || (SIGNBIT (y) != SIGNBIT (yf[far_point])))
  Ty = -Ty;

 T[0] = U[2] * Ty;
 T[1] = Ty;
}

static void cc_compute_R(double T[3], double r[9])
{
 r[0] = U[0] * T[1];
 r[1] = U[1] * T[1];
 r[2] = sqrt (1 - SQR(r[0]) - SQR(r[1]));

 r[3] = U[3] * T[1];
 r[4] = U[4] * T[1];
 r[5] = sqrt (1 - SQR(r[3]) - SQR(r[4]));
 if (!SIGNBIT (r[0] * r[3] + r[1] * r[4]))
  r[5] = -r[5];

 r[6] = r[1] * r[5] - r[2] * r[4];
 r[7] = r[2] * r[3] - r[0] * r[5];
 r[8] = r[0] * r[4] - r[1] * r[3];
}

static int cc_compute_f_and_Tz(int np, double *xw, double *yw, double *xf, double *yf,
                               double *f, double T[3], double r[9])
{
 double A[MAX_POINTS][5];
 double B[MAX_POINTS];
 int i;

 for (i=0; i<np; i++) {
  A[i][0] = r[0] * xw[i] + r[1] * yw[i] + T[0];
  A[i][1] = -xf[i];
  B[i] = (r[6] * xw[i] + r[7] * yw[i]) * xf[i];
  A[i+np][0] = r[3] * xw[i] + r[4] * yw[i] + T[1];
  A[i+np][1] = -yf[i];
  B[i+np] = (r[6] * xw[i] + r[7] * yw[i]) * yf[i];
 }

 if (!MinQuad(2*np, 2, A, B)) return 0;

 *f = B[0];
 T[2] = B[1];

 return 1;
}

static int cc_compute_Tz(int np, double *xw, double *yw, double *xf, double *yf,
                         double f, double T[3], double r[9])
{
 double A[MAX_POINTS][5];
 double B[MAX_POINTS];
 int i;

 for (i=0; i<np; i++) {
  A[i][0] = -xf[i];
  B[i] = (r[6] * xw[i] + r[7] * yw[i]) * xf[i] - f*(r[0] * xw[i] + r[1] * yw[i] + T[0]);
  A[i+np][0] = -yf[i];
  B[i+np] = (r[6] * xw[i] + r[7] * yw[i]) * yf[i] - f*(r[3] * xw[i] + r[4] * yw[i] + T[1]);
 }

 if (!MinQuad(2*np, 1, A, B)) return 0;

 T[2] = B[0];

 return 1;
}

static int EncontraHomografia(double *H, int np, double *xw, double *yw, double *xf, double *yf)
{
 int i, j, k;
 double A[3*MAX_POINTS+3][MAXDIM];
 double M[MAXDIM][MAXDIM];
 double Q[MAXDIM];
 int n=np-1;

 for (i=0; i<3*n+3; i++)
  for (j=0; j<9+n; j++)
   A[i][j] = 0.0f;

 for (i=0; i<n; i++) {
  A[i+2*n+2][6] = A[i+n+1][3] = A[i][0] = xw[i],
  A[i+2*n+2][7] = A[i+n+1][4] = A[i][1] = yw[i],
  A[i+2*n+2][8] = A[i+n+1][5] = A[i][2] = 1.0f,
  A[i][9+i] = -xf[i],
  A[i+n+1][9+i] = -yf[i],
  A[i+2*n+2][9+i] = -1.0f;
 }
 A[3*n+2][6] = A[2*n+1][3] = A[n][0] = xw[n],
 A[3*n+2][7] = A[2*n+1][4] = A[n][1] = yw[n],
 A[3*n+2][8] = A[2*n+1][5] = A[n][2] = 1.0f;

 for (i=0; i<9+n; i++)
  for (j=0; j<9+n; j++) {
   M[i][j] = A[0][j]*A[0][i];
   for (k=1; k<3*n+3; k++)
    M[i][j] += A[k][j]*A[k][i];
  }

 Q[6] = xw[n],
 Q[7] = yw[n],
 Q[2] = xf[n],
 Q[0] = Q[6]*Q[2],
 Q[1] = Q[7]*Q[2],
 Q[5] = yf[n],
 Q[3] = Q[6]*Q[5],
 Q[4] = Q[7]*Q[5],
 Q[8] = 1.0f;
 for (i=9; i<9+n; Q[i++] = 0);

 if (Gauss(9+n, M, Q) != 0) return 0;

 for (i=0; i<9; i++)
  H[i] = Q[i];

 return 1;
}

int tsaiCalibration2D(int np, double *xw, double *yw, double *xf, double *yf,
                      double *f, double T[3], double r[9])
{
 if (!cc_compute_U(np, xw, yw, xf, yf)) return 0;
 cc_compute_Tx_and_Ty(np, xw, yw, xf, yf, T);
 cc_compute_R(T, r);
 if (!cc_compute_f_and_Tz(np, xw, yw, xf, yf, f, T, r)) return 0;
 if (*f < 0) {
  r[2] = -r[2];
  r[5] = -r[5];
  r[6] = -r[6];
  r[7] = -r[7];
  cc_compute_f_and_Tz(np, xw, yw, xf, yf, f, T, r);
 }
 return 1;
}

int tsaiCalibration2D_Tr(int np, double *xw, double *yw, double *xf, double *yf, double f,
                         double T[3], double r[9])
{
 double f2;
 if (!cc_compute_U(np, xw, yw, xf, yf)) return 0;
 cc_compute_Tx_and_Ty(np, xw, yw, xf, yf, T);
 cc_compute_R(T, r);
 if (!cc_compute_f_and_Tz(np, xw, yw, xf, yf, &f2, T, r)) return 0;
 if (f2 < 0) {
  r[2] = -r[2];
  r[5] = -r[5];
  r[6] = -r[6];
  r[7] = -r[7];
 }
 if (!cc_compute_Tz(np, xw, yw, xf, yf, f, T, r)) return 0;
 return 1;
}

void tsaiProject(double xw, double yw, double zw, double *xf, double *yf,
                 double f, double T[3], double r[9])
{
 double xc = r[0] * xw + r[1] * yw + r[2] * zw + T[0];
 double yc = r[3] * xw + r[4] * yw + r[5] * zw + T[1];
 double zc = r[6] * xw + r[7] * yw + r[8] * zw + T[2];

 *xf = f * xc / zc;
 *yf = f * yc / zc;
}

void tsaiUnproject(double xf, double yf, double *xw, double *yw, double zw,
                   double f, double T[3], double r[9])
{
 double common_denominator;

 common_denominator = ((r[0] * r[7] - r[1] * r[6]) * yf + (r[4] * r[6] - r[3] * r[7]) * xf -  f * r[0] * r[4] + f * r[1] * r[3]);

 *xw = (((r[1] * r[8] - r[2] * r[7]) * yf + (r[5] * r[7] - r[4] * r[8]) * xf - f * r[1] * r[5] + f * r[2] * r[4]) * zw +
        (r[1] * T[2] - r[7] * T[0]) * yf + (r[7] * T[1] - r[4] * T[2]) * xf - f * r[1] * T[1] + f * r[4] * T[0]) / common_denominator;

 *yw = -(((r[0] * r[8] - r[2] * r[6]) * yf + (r[5] * r[6] - r[3] * r[8]) * xf - f * r[0] * r[5] + f * r[2] * r[3]) * zw +
         (r[0] * T[2] - r[6] * T[0]) * yf + (r[6] * T[1] - r[3] * T[2]) * xf - f * r[0] * T[1] + f * r[3] * T[0]) / common_denominator;
}

void tsaiCameraPosition(double *x, double *y, double *z,
                        double T[3], double r[9])
{
 double X[3];
 double M[MAXDIM][MAXDIM];

 M[0][0] = r[0]; M[0][1] = r[1]; M[0][2] = r[2];
 M[1][0] = r[3]; M[1][1] = r[4]; M[1][2] = r[5];
 M[2][0] = r[6]; M[2][1] = r[7]; M[2][2] = r[8];

 X[0] = -T[0];
 X[1] = -T[1];
 X[2] = -T[2];

 Gauss(3, M, X);

 *x = X[0];
 *y = X[1];
 *z = -X[2];
}

void tsaiOGLModelViewMatrix(double T[3], double r[9], double mv[16])
{
 mv[0] =  r[0]; mv[4] =  r[1]; mv[8]  = -r[2]; mv[12] =  T[0];
 mv[1] =  r[3]; mv[5] =  r[4]; mv[9]  = -r[5]; mv[13] =  T[1];
 mv[2] = -r[6]; mv[6] = -r[7]; mv[10] =  r[8]; mv[14] = -T[2];
 mv[3] = 0.000; mv[7] = 0.000; mv[11] = 0.000; mv[15] = 1.000;
}

void tsaiOGLProjectionMatrix(double f, double np, double fp, int width, int height, double pr[16])
{
 double b, t, l, r;
 t = np*height/2.0/f;
 b = -t;
 r = np*width/2.0/f;
 l = -r;

 pr[0] = 2*np/(r-l); pr[4] =        0.0; pr[8]  =      (r+l)/(r-l); pr[12] =              0.0;
 pr[1] =        0.0; pr[5] = 2*np/(t-b); pr[9]  =      (t+b)/(t-b); pr[13] =              0.0;
 pr[2] =        0.0; pr[6] =        0.0; pr[10] = -(fp+np)/(fp-np); pr[14] = -2*fp*np/(fp-np);
 pr[3] =        0.0; pr[7] =        0.0; pr[11] =             -1.0; pr[15] =              0.0;
}

int tsaiBuildNewData(int np, double *xw, double *yw, double *xf, double *yf,
                     int npnx, int npny, double *xwn, double *ywn, double *xfn, double *yfn)
{
 int i, j, k;
 double minx, miny, maxx, maxy;
 double x, y;
 double dx, dy;
 double H[9];
 if (!EncontraHomografia(H, np, xw, yw, xf, yf)) return 0;
 minx = maxx = xw[0];
 miny = maxy = yw[0];
 for (i=1; i<np; i++) {
  if      (xw[i] > maxx) maxx = xw[i];
  else if (xw[i] < minx) minx = xw[i];
  if      (yw[i] > maxy) maxy = yw[i];
  else if (yw[i] < miny) miny = yw[i];
 }
 dx = (maxx - minx)/(npnx - 1);
 dy = (maxy - miny)/(npny - 1);
 x = minx;
 for (i=0, k=0; i<npnx; i++) {
  y = miny;
  for (j=0; j<npny; j++, k++) {
   double xf0 = H[0]*x + H[1]*y + H[2];
   double yf0 = H[3]*x + H[4]*y + H[5];
   double zf0 = H[6]*x + H[7]*y + H[8];
   xwn[k] = x;
   ywn[k] = y;
   xfn[k] = xf0/zf0;
   yfn[k] = yf0/zf0;
   y += dy;
  }
  x += dx;
 }
 return 1;
}