#include <math.h> 
#include "kalman.h"

#ifdef _SINCOS_
void sincos();
#define SINCOS(x,s,c)   sincos(x,&s,&c)
#else
double sin(), cos();
#define SINCOS(x,s,c)   s=sin(x);c=cos(x)
#endif

void      rotation_to_angles (double *R, double *w);
void      angles_to_rotation (double *w, double *R);

KalmanStruct * init_kf_points(int npts, double *xf, double *yf, 
                             float sigmaR);
int step_kf_points(int npts, double *xf, double *yf, float sigmaQ, double dt, KalmanStruct *kf);
KalmanStruct * init_kf_camera(double sigmaT, double sigmaR, double sigmaf, double *r, double *T, double *f);
int step_kf_camera(double sigmaT, double sigmar, double sigmaf, 
                   double *r, double *T, double *f, double dt, KalmanStruct *kf);
