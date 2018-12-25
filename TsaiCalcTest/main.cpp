#include "stdio.h"
#include "stdlib.h"
#include "conio.h"
#include "tsai.h"
#include "kalman.h"
#include "define.h"


//#define TAB_SIZE_X 8
//#define TAB_SIZE_Y 6
//#define SIZE TAB_SIZE_X * TAB_SIZE_Y
//
//#define TOTAL_POINTS  8
//
//
//void initWorldCoords(double * _wx, double * _wy)
//{
//	int i, j ;
//
//#define SCALE 1.65
//
//	for ( i = 0 ; i < TAB_SIZE_Y ; i ++ )
//		for ( j = 0 ; j < TAB_SIZE_X ; j ++ )
//		{
//			_wx[TAB_SIZE_X * i + j] = SCALE * ( ((double) j+ 1) );//) - TAB_SIZE_X / 2.0 + 0.5); //
//			_wy[TAB_SIZE_X * i + j] = SCALE * ( ((double) i+ 1) );// ) - TAB_SIZE_Y / 2.0 + 0.5);//+ 1) );
//		}
//}
//
//
//void main_back()
//{
//
//	int count = SIZE;
//	KalmanStruct *kfc, *kf;
//	double rotation[9], translation[3], f, dt = 1.0 ;
//
//	double wx [SIZE] = {-30, -30, 30, 30, -15, -15, 15, 15};
//	double wy [SIZE] = {-30, -30, 30, 30, -15, -15, 15, 15};
//
//	//////////////////////////////
//	//   *                  *   //
//	//      *            *      //
//	//                          //
//	//                          //
//	//      *            *      //
//	//   *                  *   //
//	//////////////////////////////
//
//	bool useKalman = false;
//	bool initKalman = true;
//
//	initWorldCoords(wx, wy);
//
//
//	printf("Testing the Calc of Tsai\n");
//	printf("------------------------\n");
//
//
//	for (int c=0 ; c < TAB_SIZE_Y * TAB_SIZE_X; c++)
//	{
//		printf ("Points [x,y] = {%+000.0lf,%+000.0lf}\n", wx[c], wy[c]);		
//	}
//
//	printf("------------------------\n");
//	
//
//
//	double ix [SIZE] = {-30, -30, 30, 30, -15, -15, 15, 15};
//	double iy [SIZE] = {-30, -30, 30, 30, -15, -15, 15, 15};
//
//	initWorldCoords(ix, iy);
//
//
//	for( int i = 0 ; i < count ; i ++ )
//	{
//			ix[i] = ix[i] - 640 / 2 ;
//			iy[i] = iy[i] - 320 / 2 ;
//	}
//
//
//	if ( !initKalman )
//	{
//
//		//if(useKalman)
//		//	step_kf_points( TOTAL_POINTS, ix, iy, 0.1,  dt, kf);
//
//		tsaiCalibration2D_Tr( SIZE, wx, wy, ix, iy, f, translation, rotation ) ; 
//
//		//if(useKalman)
//		//	step_kf_camera( 0.1, 0.1, 1.0, rotation, translation, &f, dt, kfc);
//
//	}
//	else
//	{
//		//if(useKalman)
//		//	kf = init_kf_points( count, ix, iy, 0.1);
//
//		tsaiCalibration2D( SIZE, wx, wy, ix, iy, &f, translation, rotation ) ; 
//
//		//if(useKalman)
//		//	kfc = init_kf_camera( 0.1, 0.1, 1.0, rotation, translation, &f);
//
//		initKalman = !initKalman;
//
//	}
//
//
//	getch();
//
//}

//------------------------------------------------------------------------------------------------------------------------------------







// Variable for calibration of camera
#define WIDTH 320
#define HEIGHT 240
#define EPSILON 1e-10

//static double s_spotsize = 0.25;
static double s_spotsize = 1;



//typedef float point[2];
///* a*x + b*y + c = 0 */
//typedef struct {
//    double a, b, c;
//} line;
//
///* p1 and p2 */
//typedef point segment[2];
//
//typedef struct
//{
//	segment hlines[9];
//	segment vlines[9];
//} Interpretation;
//



void InitCalibration(int np, double* xw, double* yw, double* xf, double* yf)
{
	int m_width, m_height;
	double m_znear, m_zfar, m_proj[16], m_view[16];
	double T[3], R[9], m_f;


	for (int c=0; c < 3; c++)
		T[c] = 0;
	for (int c=0; c < 9; c++)
		R[c] = 0;

	//assert(m_width != -1 && m_height != -1);
	tsaiCalibration2D(np, xw, yw, xf, yf, &m_f, T, R);
#ifdef USEKALMAN
	// init kalman filters
  if (m_kf)
		ReleaseKalman(m_kf);
  if (m_kfc)
		ReleaseKalman(m_kfc);
  m_kf = init_kf_points(np, xf, yf, 0.5);
  m_kfc = init_kf_camera(2, 2, R, T);
#endif
	// get projection and view matrices
  m_width = WIDTH;
  m_height = HEIGHT;
  m_znear = 1;
  m_zfar = 100;
  tsaiOGLProjectionMatrix(m_f, m_znear, m_zfar, m_width, m_height, m_proj);
  tsaiOGLModelViewMatrix(T, R, m_view);
}


void StepCalibration(int np, double* xw, double* yw, double* xf, double* yf)
{
	double T[3], R[9];
	double m_f;
#ifdef USEKALMAN
  // step the kalman filter for the points
	assert(m_kf && m_kfc);
  int ok = step_kf_points(np, xf, yf, 0.1f, 1, m_kf);
#endif
	// calibrate points
  tsaiCalibration2D_Tr(np, xw, yw, xf, yf, m_f, T, R);
#ifdef USEKALMAN
	// step camera kalman filter
  ok = step_kf_camera(0.008, 0.008, R, T, 1, m_kfc);
#endif
	// get projection and view matrices
  //tsaiOGLProjectionMatrix(m_f, m_znear, m_zfar, m_width, m_height, m_proj);
  //tsaiOGLModelViewMatrix(T, R, m_view);
}




///* determina a reta passando pelos pontos p1 e p2 */
//void GEOM_line_throughpoints(line *l, point p1, point p2)
//{
//    l->a = p1[1] - p2[1];
//    l->b = p2[0] - p1[0];
//    l->c = p1[0]*p2[1] - p2[0]*p1[1];
//}
//
//void init()
//{
//	static Interpretation s_interpret;
//
//	int x, y, count;
//	point p;
//	line lh[9], lv[9];
//	int whichx[81], whichy[81];
//	double xi[81], yi[81];
//	
//	// calculate the lines present in the chess board interpretation
//	for (x = 0; x < 9; x++)
//		GEOM_line_throughpoints(&lv[x], s_interpret.vlines[x][0], s_interpret.vlines[x][1]);
//	for (y = 0; y < 9; y++)
//		GEOM_line_throughpoints(&lh[y], s_interpret.hlines[y][0], s_interpret.hlines[y][1]);
//
//
//
//
//}



///* cálculo do(s) ponto(s) de interseção entre duas retas */
//void GEOM_line_intersection(point p, line *l1, line *l2)
//{
//	double D  = l1->a*l2->b - l1->b*l2->a,
//	Dx = l1->b*l2->c - l1->c*l2->b,
//	Dy = l1->c*l2->a - l1->a*l2->c;
//
//	if (fabs(D) > EPSILON) 
//	{
//		p[0] = Dx/D;
//		p[1] = Dy/D;
//	}
//	else
//	{
//  
//		if (fabs(Dx) > EPSILON || fabs(Dy) > EPSILON) 
//		{
//			/* retas paralelas */
//			//            fprintf(stderr, "Retas paralelas\n");
//		}
//		else
//		{
//			/* retas coicidentes */
//			//            fprintf(stderr, "Retas coincidentes\n");
//		}
//	}
//}



void SCENE_calibrate_camera(int np, double ximage[], double yimage[], int whichx[], int whichy[])
{
 	int i;
	double xf[81], yf[81];
	double xw[81], yw[81];
	// correct coordinates for tsai (origin in the center of the image, y up
	for (i=0; i<np; i++)
	{
		//xf[i] = ximage[i]-WIDTH/2.0;
		//yf[i] = HEIGHT/2.0-yimage[i];
		//xw[i] = (whichx[i])*s_spotsize;
		//yw[i] = (whichy[i])*s_spotsize;

		xf[i] = ximage[i];
		yf[i] = yimage[i];
		xw[i] = (whichx[i])*s_spotsize;
		yw[i] = (whichy[i])*s_spotsize;


//#ifdef DEBUG
		printf("DEBUG: tsai point %d: image=(%.1f,%.1f), world=(%.3f,%.3f)\n",i,xf[i],yf[i],xw[i],yw[i]);
//#endif
	}
	// calibrate camera using given points
	static int firsttime = 1;
	if (firsttime)
	{
		InitCalibration(np,xw,yw,xf,yf);
		firsttime = 0;
	}
	else
		StepCalibration(np,xw,yw,xf,yf);
	
}


void init(double *xi, double *yi, int *whichx, int *whichy)
{
	int count = 8;



	///////////////////////////////////////////////////////////////////////////////////////////////////
	//
	//           * -10,+10 (5)                                                    * +10,+10 (6)
	//
	//
	//                        * -5,+5 (1)                         * +5,+5 (2)
	//
	//
	//
	//                        * -5,-5 (3)                         * +5,-5 (4)
	//
	//
	//           * -10,-10 (7)                                                    * +10,-10 (8)
	//
	///////////////////////////////////////////////////////////////////////////////////////////////////


		
	whichx[0] = -5 ; whichx[1] = 5 ; whichx[2] = -5 ; whichx[3] = 5 ; whichx[4] = -10 ; whichx[5] = 10 ; whichx[6] = -10 ; whichx[7] = 10 ;
	whichy[0] = 5 ; whichy[1] = 5 ; whichy[2] = -5 ; whichy[3] = -5 ; whichy[4] = 10; whichy[5] = 10 ; whichy[6] = -10 ; whichy[7] = -10 ;




/*
	whichx[0] = 29 ; whichx[1] = -30 ; whichx[2] = -30 ; whichx[3] = 29 ; whichx[4] = 10 ; whichx[5] = -19 ; whichx[6] = -19 ; whichx[7] = 10 ;
	whichy[0] = -34 ; whichy[1] = 35 ; whichy[2] = -34 ; whichy[3] = 35 ; whichy[4] = -22; whichy[5] = 12 ; whichy[6] = -22 ; whichy[7] = 12 ;
*/

	//xi[0] = 29 ; xi[1] = -30 ; xi[2] = -30 ; xi[3] = 29 ; xi[4] = 10 ; xi[5] = -19 ; xi[6] = -19 ; xi[7] = 11 ;
	//yi[0] = -34 ; yi[1] = 35 ; yi[2] = -34 ; yi[3] = 35 ; yi[4] = -22 ; yi[5] = 12 ; yi[6] = -22 ; yi[7] = 13 ;
	


	//World
	xi[0] = -5 ; xi[1] = 5 ; xi[2] = -5 ; xi[3] = 5 ; xi[4] = -10 ; xi[5] = 10 ; xi[6] = -10 ; xi[7] = 10 ;
	yi[0] = 5 ; yi[1] = 5 ; yi[2] = -5 ; yi[3] = -5 ; yi[4] = 10 ; yi[5] = 10 ; yi[6] = -10 ; yi[7] = -10 ;
	
	//Test1
	xi[0] = -3 ; xi[1] = 6 ; xi[2] = -3 ; xi[3] = 6 ; xi[4] = -8 ; xi[5] = 10 ; xi[6] = -8 ; xi[7] = 10 ;
	yi[0] = 3 ; yi[1] = 10 ; yi[2] = -3 ; yi[3] = -10 ; yi[4] = 4 ; yi[5] = 18 ; yi[6] = -4 ; yi[7] = -18 ;
	
	////Test2
	//xi[0] = 3 ; xi[1] = -6 ; xi[2] = 3 ; xi[3] = -6 ; xi[4] = 8 ; xi[5] = -10 ; xi[6] = 8 ; xi[7] = -10 ;
	//yi[0] = 3 ; yi[1] = -10 ; yi[2] = -3 ; yi[3] = 10 ; yi[4] = -4 ; yi[5] = -18 ; yi[6] = 4 ; yi[7] = 18 ;
	//
	////Test3
	//xi[0] = -2 ; xi[1] = 2 ; xi[2] = -3 ; xi[3] = 3 ; xi[4] = -2 ; xi[5] = 2 ; xi[6] = -5 ; xi[7] = 5 ;
	//yi[0] = 7 ; yi[1] = 7 ; yi[2] = -5 ; yi[3] = -5 ; yi[4] = 15 ; yi[5] = 15 ; yi[6] = -15 ; yi[7] = -15 ;
	//


	// calibrate camera and render scene
    SCENE_calibrate_camera(count,xi,yi,whichx,whichy);
    //SCENE_render();

}




void main()
{
	printf("Testando...\n");

	double xi[10], yi[10];
	int whichx[10], whichy[10];

	init(xi, yi, whichx, whichy);

	//CornersOperator(320, 240);
	
	getch();

}
