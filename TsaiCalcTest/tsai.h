#ifndef _TSAI_H_
#define _TSAI_H_

/* M�todo TSAI - Calibra��o de Cameras - vers�o coplanar */

/************************************************************************************************/
/* Calibra a camera dados np pares de pontos.                                                   */
/*  Entrada:                                                                                    */
/*   np - n�mero de pares de pontos - [ (xw,yw) , (xf, yf) ].                                   */
/*   (xw, yw) - ponto do mundo.                                                                 */
/*   (xf, yf) - ponto em coodernadas da tela em pixel (origem no centro da imagem e eixo ox da  */
/*                                                     esquerda para direita e eixo oy de baixo */
/*                                                     para cima).                              */
/*  Sa�da:                                                                                      */
/*   f - dist�ncia focal.                                                                       */
/*   T - transla��o (origem do mundo em sistema de coordenadas da c�mera).                      */
/*   r - rota��o.                                                                               */
/************************************************************************************************/
int tsaiCalibration2D(int np, double *xw, double *yw, double *xf, double *yf,
                      double *f, double T[3], double r[9]);

/************************************************************************************************/
/* Calibra a camera dados np pares de pontos, exceto dist�ncia focal.                           */
/*  Entrada:                                                                                    */
/*   np - n�mero de pares de pontos - [ (xw,yw) , (xf, yf) ].                                   */
/*   (xw, yw) - ponto do mundo.                                                                 */
/*   (xf, yf) - ponto em coodernadas da tela em pixel (origem no centro da imagem e eixo ox da  */
/*                                                     esquerda para direita e eixo oy de baixo */
/*                                                     para cima).                              */
/*   f - dist�ncia focal.                                                                       */
/*  Sa�da:                                                                                      */
/*   T - transla��o (origem do mundo em sistema de coordenadas da c�mera).                      */
/*   r - rota��o.                                                                               */
/************************************************************************************************/
int tsaiCalibration2D_Tr(int np, double *xw, double *yw, double *xf, double *yf, double f,
                         double T[3], double r[9]);

/************************************************************************************************/
/* Projeta o ponto (xw, yw, zw) na tela.                                                        */
/*  Entrada:                                                                                    */
/*   (xw, yw, zw) - ponto a ser projetado.                                                      */
/*   f - dist�ncia focal.                                                                       */
/*   T - transla��o (origem do mundo em sistema de coordenadas da c�mera).                      */
/*   r - rota��o.                                                                               */
/*  Sa�da:                                                                                      */
/*   (xf, yf) - ponto projetado.                                                                */
/************************************************************************************************/
void tsaiProject(double xw, double yw, double zw, double *xf, double *yf,
                 double f, double T[3], double r[9]);

/************************************************************************************************/
/* Inverte a proje��o do ponto da tela.                                                         */
/*  Entrada:                                                                                    */
/*   (xf, yf) - ponto projetado da tela.                                                        */
/*   f - dist�ncia focal.                                                                       */
/*   T - transla��o (origem do mundo em sistema de coordenadas da c�mera).                      */
/*   r - rota��o.                                                                               */
/*   zw - coordenada z do ponto do mundo.                                                       */
/*  Sa�da:                                                                                      */
/*   (xw, yw, zw) - ponto do mundo. Note que zw deve ser dado de entrada.                       */
/************************************************************************************************/
void tsaiUnproject(double xf, double yf, double *xw, double *yw, double zw,
                   double f, double T[3], double r[9]);

/************************************************************************************************/
/* Encontra a posi��o da c�mera no sistema de coordenadas do mundo                              */
/*  Entrada:                                                                                    */
/*   T - transla��o (origem do mundo em sistema de coordenadas da c�mera).                      */
/*   r - rota��o.                                                                               */
/*  Sa�da:                                                                                      */
/*   (x, y, z) - coordenadas da posi��o da c�mera.                                              */
/************************************************************************************************/
void tsaiCameraPosition(double *x, double *y, double *z,
                        double T[3], double r[9]);

/************************************************************************************************/
/* Retorna a matriz Model View a ser utilizado no OpenGL.                                       */
/*  Entrada:                                                                                    */
/*   T - transla��o (origem do mundo em sistema de coordenadas da c�mera).                      */
/*   r - rota��o.                                                                               */
/*  Sa�da:                                                                                      */
/*   mv - matriz Model View.                                                                    */
/************************************************************************************************/
void tsaiOGLModelViewMatrix(double T[3], double r[9], double mv[16]);

/************************************************************************************************/
/* Retorna a matriz de proje��o a ser utilizado no OpenGL.                                      */
/*  Entrada:                                                                                    */
/*   f - dist�ncia focal.                                                                       */
/*   nearplane - plano near do frustum de vis�o.                                                */
/*   farplane - plano far do frustum de vis�o.                                                  */
/*   width - largura do frustum de vis�o.                                                       */
/*   height - altura do frustum de vis�o.                                                       */
/*  Sa�da:                                                                                      */
/*   pr - matriz de proje��o.                                                                   */
/************************************************************************************************/
void tsaiOGLProjectionMatrix(double f, double nearplane, double farplane, int width, int height, double pr[16]);

/************************************************************************************************/
/* Reamostra os pares de pontos segundo pares de pontos originais.                              */
/*  Entrada:                                                                                    */
/*   np - n�mero de pares de pontos - [ (xw,yw) , (xf, yf) ].                                   */
/*   (xw, yw) - ponto do mundo.                                                                 */
/*   (xf, yf) - ponto em coodernadas da tela em pixel (origem no centro da imagem e eixo ox da  */
/*                                                     esquerda para direita e eixo oy de baixo */
/*                                                     para cima).                              */
/*   npnx - n�mero de pares de pontos novos na dire��o x do modelo. (Max. 10)                   */
/*   npny - n�mero de pares de pontos novos na dire��o y do modelo. (Max. 10)                   */
/*  Sa�da:                                                                                      */
/*   (xwn, ywn) - ponto novo do mundo.                                                          */
/*   (xfn, yfn) - ponto novo em coodernadas da tela em pixel.                                   */
/************************************************************************************************/
int tsaiBuildNewData(int np, double *xw, double *yw, double *xf, double *yf,
                     int npnx, int npny, double *xwn, double *ywn, double *xfn, double *yfn);

#endif