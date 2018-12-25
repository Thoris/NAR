#include "kalman_camera.h"
#include <stdlib.h>
 


void      rotation_to_angles (double *R, double *w)
{
    double    sg,
              cg;

    w[2] = atan2 (R[3], R[0]);
    SINCOS (w[2], sg, cg);
    w[1] = atan2 (-R[6], R[0] * cg + R[3] * sg);
    w[0] = atan2 (R[2] * sg - R[5] * cg, R[4] * cg - R[1] * sg);
}

/***********************************************************************\
* This routine simply takes the roll, pitch and yaw angles and fills in	*
* the rotation matrix R.					*
\***********************************************************************/
void      angles_to_rotation (double *w, double *R)
{
    double    sa,
              ca,
              sb,
              cb,
              sg,
              cg;

    SINCOS (w[0], sa, ca);
    SINCOS (w[1], sb, cb);
    SINCOS (w[2], sg, cg);

    R[0] = cb * cg;
    R[1] = cg * sa * sb - ca * sg;
    R[2] = sa * sg + ca * cg * sb;
    R[3] = cb * sg;
    R[4] = sa * sb * sg + ca * cg;
    R[5] = ca * sb * sg - cg * sa;
    R[6] = -sb;
    R[7] = cb * sa;
    R[8] = ca * cb;
}

void SetDiagonal(float *matr, int m, int n, float value) {
  int k, i;
  k = (m<n ? m : n);
  for (i=0; i<m*n; i++) matr[i] = 0.0;
  for (i=0; i<k; i++) matr[i*n + i] = value;
}

void Transpose (double R[9], double S[9]) {
  int i, j;
  for (i=0; i<3; i++)
   for (j=0; j<3; j++) 
    S[3*i+j] = R[3*j+i];
}

void Mult (double R[9], double S[9], double T[9]) {
  int i, j;
  for (i=0; i<3; i++)
   for (j=0; j<3; j++) 
    T[3*i+j] = R[3*i+0]*S[0*3+j] + R[3*i+1]*S[1*3+j]+ R[3*i+2]*S[2*3+j];
}

KalmanStruct * init_kf_points(int npts, double *xf, double *yf, 
                             float sigmaR) {
  float *Phi, *H, *Q, *R, *x, *P;
  KalmanStruct *kf;
  int i;

  Phi = (float *) malloc(16*npts*npts*sizeof(float));
  H = (float *) malloc(8*npts*npts*sizeof(float));
  Q = (float *) malloc(16*npts*npts*sizeof(float));
  R = (float *) malloc(4*npts*npts*sizeof(float));
  P = (float *) malloc(16*npts*npts*sizeof(float));
  x = (float *) malloc(4*npts*sizeof(float));

  SetDiagonal(Phi, 4*npts, 4*npts, 1.0);
  for (i=0; i<2*npts; i++) Phi[4*npts*i + 2*npts + i] = 1;
  SetDiagonal(H, 2*npts, 4*npts, 1.0);
  SetDiagonal(Q, 4*npts, 4*npts, 0.0);
  for (i=2*npts; i<4*npts; i++) {Q[i*4*npts + i] = 1.0;}
  SetDiagonal(R, 2*npts, 2*npts, sigmaR);
  SetDiagonal(P, 4*npts, 4*npts, 1e5*sigmaR);
  for (i=0; i<npts; i++) {x[i] = xf[i]; x[npts+i] = yf[i]; 
                          x[2*npts+ 2*i] = 0.0;  x[2*npts + 2*i + 1] = 0.0;}
  kf = CreateKalman(4*npts, 2*npts);
  kf->Phi = Phi;
  kf->Q = Q;
  kf->R = R;
  kf->H = H;
  kf->x = x;
  kf->P = P;
  return (kf);

}

int step_kf_points(int npts, double *xf, double *yf, float sigmaQ, double dt, KalmanStruct *kf)   {
  float *z;
  int i;

  z = (float *) malloc(2*npts*sizeof(float));
  for (i=0; i<npts; i++) {z[i] = xf[i]; z[npts+i] = yf[i];}
  for (i=2*npts; i<4*npts; i++) kf->Q[i*4*npts + i] = sigmaQ *dt;
  for (i=0; i<2*npts; i++) kf->Phi[4*npts*i + 2*npts + i] = dt;
  KalmanPredict(kf);
  KalmanCorrect(kf, z);
  free(z);
  for (i=0; i<2*npts; i++) kf->Phi[4*npts*i + 2*npts + i] = dt;
  for (i=0; i<npts; i++) {xf[i] = kf->x[i]; yf[i]= kf->x[npts+i] ;}
  if (kf->chi_square < 50 ) {
     for (i=0; i<npts; i++) {xf[i] = kf->x[i]; yf[i]= kf->x[npts+i] ;}
     return 1;
  }
  else {
//    for (i=0; i<4*npts; i++) kf->x[i] = kf->PriorState[i];
//    for (i=0; i<16*npts*npts; i++) kf->P[i] = kf->PriorErrorCovariance[i];
    for (i=0; i<npts; i++) {xf[i] = kf->x[i]; yf[i]= kf->x[npts+i] ;}
    return 0;
  }
}

KalmanStruct * init_kf_camera(double sigmaT, double sigmaR, double sigmaf, double *r, double *T, double *f) {
  float *Phi, *H, *Q, *R, *x, *P;
  KalmanStruct *kf;
  int i; 
  double w[3];

  Phi = (float *) malloc(14*14*sizeof(float));
  H = (float *) malloc(14*7*sizeof(float));
  Q = (float *) malloc(14*14*sizeof(float));
  R = (float *) malloc(7*7*sizeof(float));
  P = (float *) malloc(14*14*sizeof(float));
  x = (float *) malloc(14*sizeof(float));

  SetDiagonal(Phi, 14, 14, 1.0);
  for (i=0; i<14; i++) Phi[14*i + 7 + i] = 1;
  SetDiagonal(H, 7, 14, 1.0);
  SetDiagonal(Q, 14, 14, 0.0);
  for (i=7; i<14; i++) {Q[14*i + i] = 1;}
  SetDiagonal(R, 7, 7, sigmaT);
  for (i=0; i<3; i++) R[7*i+i] = sigmaR;
  R[6*7+6] =  sigmaf;
  SetDiagonal(P, 14, 14, 1e5);
  rotation_to_angles(r, w);
  x[0] = w[0]; x[1] = w[1]; x[2] = w[2];
  x[3] = T[0]; x[4] = T[1]; x[5] = T[2];
  x[6] = *f;
  for (i=7; i<14; i++) x[i] = 0.0;
  kf = CreateKalman(14, 7);
  kf->Phi = Phi;
  kf->Q = Q;
  kf->R = R;
  kf->H = H;
  kf->x = x;
  kf->P = P;
  return (kf);

}

int step_kf_camera(double sigmaT, double sigmar, double sigmaf, 
                   double *r, double *T, double *f, double dt, KalmanStruct *kf) {

  float z[7];
  int i;
  double w[3],  w0[3],  R0[9],  Rincr[9];
  
  w0[0] = kf->x[0]; w0[1] = kf->x[1]; w0[2] = kf->x[2];
  angles_to_rotation(w0, R0);
  rotation_to_angles(r, w);
  kf->x[0] = kf->x[1] = kf->x[2] = 0.0;
  z[0] = w[0] - w0[0];
  z[1] = w[1] - w0[1];
  z[2] = w[2] - w0[2];
  z[3] = T[0]; z[4] = T[1]; z[5] = T[2];
  z[6] = *f;  
  for (i=7; i<10; i++) kf->Q[i*14 + i] = sigmar *dt;
  for (i=10; i<13; i++) kf->Q[i*14 + i] = sigmaT *dt;
  kf->Q[13*14 + 13] = sigmaf*dt;
  for (i=0; i<7; i++) kf->Phi[14*i + 7 + i] = dt;
  KalmanPredict(kf);
  KalmanCorrect(kf, z);
  w[0] = kf->x[0];  
  w[1] = kf->x[1];  
  w[2] = kf->x[2];
  angles_to_rotation(w, Rincr);
  Mult(R0, Rincr, r);
  T[0] = kf->x[3]; T[1] = kf->x[4]; T[2] = kf->x[5];
  *f = kf->x[6];
  return (1);

}


