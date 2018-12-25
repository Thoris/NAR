#ifndef _TSAI_H_
#define _TSAI_H_

/* Método TSAI - Calibração de Cameras - versão coplanar */

/************************************************************************************************/
/* Calibra a camera dados np pares de pontos.                                                   */
/*  Entrada:                                                                                    */
/*   np - número de pares de pontos - [ (xw,yw) , (xf, yf) ].                                   */
/*   (xw, yw) - ponto do mundo.                                                                 */
/*   (xf, yf) - ponto em coodernadas da tela em pixel (origem no centro da imagem e eixo ox da  */
/*                                                     esquerda para direita e eixo oy de baixo */
/*                                                     para cima).                              */
/*  Saída:                                                                                      */
/*   f - distância focal.                                                                       */
/*   T - translação (origem do mundo em sistema de coordenadas da câmera).                      */
/*   r - rotação.                                                                               */
/************************************************************************************************/
int tsaiCalibration2D(int np, double *xw, double *yw, double *xf, double *yf,
                      double *f, double T[3], double r[9]);

/************************************************************************************************/
/* Calibra a camera dados np pares de pontos, exceto distância focal.                           */
/*  Entrada:                                                                                    */
/*   np - número de pares de pontos - [ (xw,yw) , (xf, yf) ].                                   */
/*   (xw, yw) - ponto do mundo.                                                                 */
/*   (xf, yf) - ponto em coodernadas da tela em pixel (origem no centro da imagem e eixo ox da  */
/*                                                     esquerda para direita e eixo oy de baixo */
/*                                                     para cima).                              */
/*   f - distância focal.                                                                       */
/*  Saída:                                                                                      */
/*   T - translação (origem do mundo em sistema de coordenadas da câmera).                      */
/*   r - rotação.                                                                               */
/************************************************************************************************/
int tsaiCalibration2D_Tr(int np, double *xw, double *yw, double *xf, double *yf, double f,
                         double T[3], double r[9]);

/************************************************************************************************/
/* Projeta o ponto (xw, yw, zw) na tela.                                                        */
/*  Entrada:                                                                                    */
/*   (xw, yw, zw) - ponto a ser projetado.                                                      */
/*   f - distância focal.                                                                       */
/*   T - translação (origem do mundo em sistema de coordenadas da câmera).                      */
/*   r - rotação.                                                                               */
/*  Saída:                                                                                      */
/*   (xf, yf) - ponto projetado.                                                                */
/************************************************************************************************/
void tsaiProject(double xw, double yw, double zw, double *xf, double *yf,
                 double f, double T[3], double r[9]);

/************************************************************************************************/
/* Inverte a projeção do ponto da tela.                                                         */
/*  Entrada:                                                                                    */
/*   (xf, yf) - ponto projetado da tela.                                                        */
/*   f - distância focal.                                                                       */
/*   T - translação (origem do mundo em sistema de coordenadas da câmera).                      */
/*   r - rotação.                                                                               */
/*   zw - coordenada z do ponto do mundo.                                                       */
/*  Saída:                                                                                      */
/*   (xw, yw, zw) - ponto do mundo. Note que zw deve ser dado de entrada.                       */
/************************************************************************************************/
void tsaiUnproject(double xf, double yf, double *xw, double *yw, double zw,
                   double f, double T[3], double r[9]);

/************************************************************************************************/
/* Encontra a posição da câmera no sistema de coordenadas do mundo                              */
/*  Entrada:                                                                                    */
/*   T - translação (origem do mundo em sistema de coordenadas da câmera).                      */
/*   r - rotação.                                                                               */
/*  Saída:                                                                                      */
/*   (x, y, z) - coordenadas da posição da câmera.                                              */
/************************************************************************************************/
void tsaiCameraPosition(double *x, double *y, double *z,
                        double T[3], double r[9]);

/************************************************************************************************/
/* Retorna a matriz Model View a ser utilizado no OpenGL.                                       */
/*  Entrada:                                                                                    */
/*   T - translação (origem do mundo em sistema de coordenadas da câmera).                      */
/*   r - rotação.                                                                               */
/*  Saída:                                                                                      */
/*   mv - matriz Model View.                                                                    */
/************************************************************************************************/
void tsaiOGLModelViewMatrix(double T[3], double r[9], double mv[16]);

/************************************************************************************************/
/* Retorna a matriz de projeção a ser utilizado no OpenGL.                                      */
/*  Entrada:                                                                                    */
/*   f - distância focal.                                                                       */
/*   nearplane - plano near do frustum de visão.                                                */
/*   farplane - plano far do frustum de visão.                                                  */
/*   width - largura do frustum de visão.                                                       */
/*   height - altura do frustum de visão.                                                       */
/*  Saída:                                                                                      */
/*   pr - matriz de projeção.                                                                   */
/************************************************************************************************/
void tsaiOGLProjectionMatrix(double f, double nearplane, double farplane, int width, int height, double pr[16]);

/************************************************************************************************/
/* Reamostra os pares de pontos segundo pares de pontos originais.                              */
/*  Entrada:                                                                                    */
/*   np - número de pares de pontos - [ (xw,yw) , (xf, yf) ].                                   */
/*   (xw, yw) - ponto do mundo.                                                                 */
/*   (xf, yf) - ponto em coodernadas da tela em pixel (origem no centro da imagem e eixo ox da  */
/*                                                     esquerda para direita e eixo oy de baixo */
/*                                                     para cima).                              */
/*   npnx - número de pares de pontos novos na direção x do modelo. (Max. 10)                   */
/*   npny - número de pares de pontos novos na direção y do modelo. (Max. 10)                   */
/*  Saída:                                                                                      */
/*   (xwn, ywn) - ponto novo do mundo.                                                          */
/*   (xfn, yfn) - ponto novo em coodernadas da tela em pixel.                                   */
/************************************************************************************************/
int tsaiBuildNewData(int np, double *xw, double *yw, double *xf, double *yf,
                     int npnx, int npny, double *xwn, double *ywn, double *xfn, double *yfn);

#endif