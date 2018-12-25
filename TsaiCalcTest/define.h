
#ifndef _DEFINE_
#define _DEFINE_



#include <stdio.h>
#include <time.h>
#include <stdlib.h>
#include <conio.h>
#include <math.h>

#include "define.h"

#include "EigenValue.h"		// Get operations for eigenvalores to KLT algorithm
#include "tsai.h"			// Calibration operation for camera



// Variable for calibration of camera
#define IMAGE_W 320
#define IMAGE_H 240

// Threshold of Corners Detector
float WThreshold = 9000.0F;


// Variable for map texture image 
int gHeight, gWidth, gSize;

// Define Image with corners detected 
unsigned char * CornersImage = NULL; // Image with only corners


// Components of matrix Cstr of the image gradient
float q1,q2,q3,q4;
// Store value of the positive small landa 
float *v; 


// 3x3 Median mask.
int SobelMaskX[9] = {1,0,-1,2,0,-2,1,0,-1};
int SobelMaskY[9] = {1,2,1,0,0,0,-1,-2,-1};



static int np;		// number of base points for calibration
static double *xw, *yw, *xf, *yf;	// pointers to arrays of calibration base points
static double mtsaiopengl_modelview[16];  // matrixs for calibration of camera  
static double mtsaiopengl_projection[16]; // matrixs for calibration of camera  
static double focal, T[3], r[9];		  // matrixs for calibration of camera  


unsigned char * gDataAux = NULL;	    // Image in Color obtein the camera 

// variables for detect corners in image 
int Flag_Points = 0;
int **VectorPoints , **VectorPointsNew, **VectorPointsOld;


// 3x3 Median mask.
int GaussianMask[9] = {1,2,1,2,4,2,1,2,1};

float * GradientX= NULL;	// Image gradient in X (convolution with Gauss 1D )
float * GradientY= NULL;	// Image gradient in Y (convolution with Gauss 1D )




// Initiation of Vector Auxiliars for processing of images 
void myVariablesInit() 
{
  int i;
  //gData = (unsigned char *)malloc(sizeof(char)*gSize*3);
  gDataAux = (unsigned char *)malloc(sizeof(char)*gSize*3);

  //Temporal = (unsigned char *)malloc(sizeof(char)*gSize);
  //Image_Grey = (float *) malloc(sizeof(float)*gSize); 
  //Image_Grey_In = (float *) malloc(sizeof(float)*gSize); 
  //Image_Grey_Out = (float *) malloc(sizeof(float)*gSize); 

  
  GradientX =  (float *) malloc(sizeof(float)*gSize); 
  GradientY =  (float *) malloc(sizeof(float)*gSize); 


  CornersImage = (unsigned char *)malloc(sizeof(char)*gSize*3);
  v = (float*)malloc(sizeof(float)* 1);
  // Variables of array where store the find points for Calibration
 VectorPoints =(int **)malloc(sizeof(int)*8);
 VectorPointsNew =(int **)malloc(sizeof(int)*8);
 VectorPointsOld =(int **)malloc(sizeof(int)*8);

 for(i=0;i<8;i++){
	VectorPoints[i] = (int *)malloc(sizeof(int)*2);
	VectorPointsNew[i] = (int *)malloc(sizeof(int)*2);
	VectorPointsOld[i] = (int *)malloc(sizeof(int)*2);
 };

	xw =(double*)malloc(sizeof(double)*8);
	yw =(double*)malloc(sizeof(double)*8);
	xf =(double*)malloc(sizeof(double)*8);
	yf =(double*)malloc(sizeof(double)*8);
}

// Functions Give Position 1D inside Vector 2D
int SearchPosicaoGeneral(int LineX, int ColumnY, int Width)
{
	return ((LineX * Width) +  ColumnY);
}

// Functions for obtein gradient with convolution image
// with mask the Sobel 
void Get_Gradient_Sobel(float *Data, int DataWidth, int DataHeight)
{
	int i,j,k, x,y,Position, PositionAux,Half;
	float tempX,tempY;
	Half = 1;
	Position = 0;
	for(x=0 ; x<DataHeight ; x++){
		for(y=0 ; y<DataWidth ; y++){	
			k=0; tempX=0.0F;tempY=0.0F;
			for(i=-Half; i<=Half; i++){
				for(j=-Half; j<=Half;j++){
					PositionAux = Position;
					if(( x+i > -1)&&(y+j > -1) &&(x+i < DataHeight) && (y+j< DataWidth))		
						PositionAux = SearchPosicaoGeneral(x+i,y+j,DataWidth);
					 tempX+= Data[PositionAux]* SobelMaskX[k];
					 tempY+= Data[PositionAux]* SobelMaskY[k];
					k++;					
				}
			}
			GradientX[Position] = tempX;   // Component Gradient in X
			GradientY[Position] = tempY;   // Component Gradient in Y 			
			Position++;			
		}
	}
}

// Functions for check bounds in relation with points in pattern
int Check_Range_X_Y(int x ,int y, int ** Points){
	int i,Check = 0;
	for(i=0;i < 8;i++){
		if(abs(Points[i][0] - x)< 30 && abs(Points[i][1]- y)< 30){
			Check = 1; break;
		}		
	};
	return Check;
}



// Functions for Order vector of 8 points
void Order_Square(int **Vector, int **NewVector){
	int i,Pointx,PointX,Pointy,PointY;
	int Xmax =-1, Xmin =240 , Ymax=-1, Ymin =320;
	int Check[8];
	for(i=0; i<8; i++)
	{	NewVector[i][0]=NewVector[i][1]= Check[i] = 0;}

	for(i=0; i<8; i++)
	{if( Vector[i][0] <= Xmin && Check[i] == 0)
			{Xmin = Vector[i][0]; 
			 Pointx = i;} 
	}
	Check[Pointx]=1;

	for(i=0; i<8; i++)	
	{if ( Vector[i][0] >= Xmax && Check[i] == 0)
		{Xmax = Vector[i][0]; PointX = i;}
	}
	Check[PointX]=1;
	
	for(i=0; i<8; i++){
		if ( Vector[i][1] <= Ymin && Check[i]==0 )
		{Ymin = Vector[i][1]; Pointy = i;}
	}
	Check[Pointy]=1;

	for(i=0; i<8; i++){
		if( Vector[i][1] >= Ymax && Check[i]==0 )
			{Ymax = Vector[i][1]; PointY = i;}
	}
	Check[PointY] = 1;

	NewVector[0][0]= Vector[Pointx][0]; NewVector[0][1]= Vector[Pointx][1];
	NewVector[1][0]= Vector[PointX][0]; NewVector[1][1]= Vector[PointX][1];
	NewVector[2][0]= Vector[Pointy][0]; NewVector[2][1]= Vector[Pointy][1]; 
	NewVector[3][0]= Vector[PointY][0]; NewVector[3][1]= Vector[PointY][1];

	Xmax =-1, Xmin =240 , Ymax=-1, Ymin =320;
	for(i=0; i<8; i++){
		if ( Vector[i][0] <= Xmin && Check[i] == 0)
		{Xmin = Vector[i][0]; Pointx = i;  }
	}
	Check[Pointx]=1;

	for(i=0; i<8; i++){
		if ( Vector[i][0] >= Xmax && Check[i] == 0)
		{Xmax = Vector[i][0]; PointX = i;}
	}
	Check[PointX]=1;
	
	for(i=0; i<8; i++){
		if ( Vector[i][1] <= Ymin && Check[i]==0 )
			{Ymin = Vector[i][1]; Pointy = i;}
	}
	Check[Pointy]=1;

	for(i=0; i<8; i++){
		if( Vector[i][1] >= Ymax && Check[i]==0 )
			{		Ymax = Vector[i][1]; PointY = i;}
	}
	Check[PointY] = 1;
	
	NewVector[4][0]= Vector[Pointx][0]; NewVector[4][1]= Vector[Pointx][1];
	NewVector[5][0]= Vector[PointX][0]; NewVector[5][1]= Vector[PointX][1];
	NewVector[6][0]= Vector[Pointy][0]; NewVector[6][1]= Vector[Pointy][1]; 
	NewVector[7][0]= Vector[PointY][0]; NewVector[7][1]= Vector[PointY][1];
}

// Function draw corners in order
void DrawInOrder(unsigned char * Data, int **Vector){

int Position;
Position = SearchPosicaoGeneral(Vector[0][0],Vector[0][1],320)*3;
	Data[Position] = 255;	Data[Position+1] = 0;	Data[Position+2] = 255;
	
Position = SearchPosicaoGeneral(Vector[1][0],Vector[1][1],320)*3;
	Data[Position] = 0;	Data[Position+1] = 255;	Data[Position+2] = 0;

Position = SearchPosicaoGeneral(Vector[2][0],Vector[2][1],320)*3;
	Data[Position] = 0;	Data[Position+1] = 0;	Data[Position+2] = 255;

Position = SearchPosicaoGeneral(Vector[3][0],Vector[3][1],320)*3;
	Data[Position] = 255;	Data[Position+1] = 255;	Data[Position+2] = 0;

Position = SearchPosicaoGeneral(Vector[4][0],Vector[4][1],320)*3;
	Data[Position] = 255;	Data[Position+1] = 0;	Data[Position+2] = 0;
	
Position = SearchPosicaoGeneral(Vector[5][0],Vector[5][1],320)*3;
	Data[Position] = 0;	Data[Position+1] = 255;	Data[Position+2] = 0;

Position = SearchPosicaoGeneral(Vector[6][0],Vector[6][1],320)*3;
	Data[Position] = 0;	Data[Position+1] = 0;	Data[Position+2] = 255;

Position = SearchPosicaoGeneral(Vector[7][0],Vector[7][1],320)*3;
	Data[Position] = 255;	Data[Position+1] = 255;	Data[Position+2] = 0;
}

// Functions Search Corners inside the image
void CornersOperator(int DataWidth, int DataHeight)
{  	
	int x,y;					// Row x ; Column y
	int Position , PositionAux; // Position of pixel in Array[320 x240]
	int i,j,k;                  // Row neighbord i ; Column neighbord j ; Position k
	int Half;					// If Amount of neighbords equal 3 , then evaluate since -(3/2) to (3/2)
	int Cont=0;					// Accountant of points of pattern
	int l;						// Account for corners



	gSize = DataHeight * DataWidth;
	myVariablesInit();




	// Lista for store a list of corners  
	Lista* CornersPoints, *CornersPointsOrd, *Aux; 

	// Initialization List of the corners "CornersPoints" 
	// and a copy "CornersPointsOrd "for order points
	CornersPoints = inicializa();
	CornersPointsOrd = inicializa();

	// Initialization of the image with corners detected
	for(i=0;i< gSize*3;i++)
		CornersImage[i] = (unsigned char)0;

	Half= 1;
	Position= 0;
	///////////////////////////////
	// For all pixel evaluate 
	// if is or isn´t possible corners 
	///////////////////////////////
	for(x=0 ; x<DataHeight ; x++)
	{
		for(y=0 ; y<DataWidth ; y++)
		{	
			// If pattern is activate only search new corners for the image in
			// neighbord of initial pattern points detected

			// if still capture of 8 points to the pattern evaluate 
			// for all points in the image execute the algoritm of Harris for detected corners
			
			if(Flag_Points == 0 )
			{
				k=0; q1=q2=q3=q4=0.0F;				

				///////////////////////////////////
				// Get the Matrix C might create
				// with Gradient Ex and Ey 
				// of neighbords the actual pixel 
				//	      |ExEx(q1)		ExEy(q3)|
				//  Cij = |						|
				//		  |ExEy(q2)		EyEy(q4)|
				///////////////////////////////////
			
				// Loop for get and evaluate neighbord of (x,y) pixel
				for(i=-Half; i<=Half; i++)
				{
					for(j=-Half; j<=Half;j++)
					{
						PositionAux = Position;
						if(( x+i > -1)&&(y+j > -1) &&(x+i < DataHeight) && (y+j< DataWidth))		
							PositionAux = SearchPosicaoGeneral(x+i,y+j,DataWidth);
						q1 += GaussianMask[k]*(GradientX[PositionAux]*GradientX[PositionAux]);
						q2 += GaussianMask[k]*(GradientX[PositionAux]*GradientY[PositionAux]);
						q4 += GaussianMask[k]*(GradientY[PositionAux]*GradientY[PositionAux]);								
						k++;
					}
				}
				
				q1 = (float)q1/16; q2 = (float)q2/16; q4 = (float)q4/16; q3 = q2;
			
				//////////////////////////////////
				// Obtein the EigenValues of Cij 
				// and get the Less Landa
				//////////////////////////////////
				eigen(q1, q2, q3, q4, v); 
			
				/////////////////////////////////////////////
				// Implement of threshold of possible
				// Corners points with Landa v[0] > somevalue
				// Insert inside List of corners in decreasing order
				// all possibles corners points, 
				// and their position(x(Row),y(Column))
				//////////////////////////////////////
				if(v[0] > (float)WThreshold)
				{			
					CornersPoints = insere_ordenado( CornersPoints,v[0],x,y);				   
				}
			 
			}
			// if already capture of 8 points to the pattern evaluate 
			// only execute the corners detector around of a neighbordhood of corners points
			else if(Flag_Points == 1)
			{
				if(Check_Range_X_Y(x,y,VectorPoints)!=0)
				{
					k=0; q1=q2=q3=q4=0.0F;				
					
					// Loop for get and evaluate neighbord of (x,y) pixel
					for(i=-Half; i<=Half; i++)
					{
						for(j=-Half; j<=Half;j++)
						{
							PositionAux = Position;
							if(( x+i > -1)&&(y+j > -1) &&(x+i < DataHeight) && (y+j< DataWidth))		
								PositionAux = SearchPosicaoGeneral(x+i,y+j,DataWidth);
							q1 += GaussianMask[k]*(GradientX[PositionAux]*GradientX[PositionAux]);
							q2 += GaussianMask[k]*(GradientX[PositionAux]*GradientY[PositionAux]);
							q4 += GaussianMask[k]*(GradientY[PositionAux]*GradientY[PositionAux]);								
							k++;
						}
					}
					q1 = (float)q1/16; q2 = (float)q2/16; q4 = (float)q4/16; q3 = q2;
					eigen(q1, q2, q3, q4, v); 
					
					if(v[0] > (float)WThreshold)
					{
						CornersPoints = insere_ordenado( CornersPoints,v[0],x,y);			
					}
				};
			};
			Position++;			
		};
	};

	////////////////////////////////////////////////////
	// Quit the List of corners possible points corners 
	// neighbords (the same x ) and proximos
	///////////////////////////////////////////////////
	retira_x_iguais(CornersPoints);
	retira_x_Prox(CornersPoints);
		
	Aux = CornersPoints; Cont=0;
	// Copy List of corners to image corners
	while(CornersPoints!=NULL)
	{   	
		i= CornersPoints->x;
		j= CornersPoints->y;
		Position = SearchPosicaoGeneral(i, j, gWidth) * 3;
		/////////////////////////////////////////
		// Create Possible image with 
		// only corners find
		///////////////////////////////////////
		// Image in Black and White
		CornersImage[Position] = (unsigned char)255;
		CornersImage[Position+1] = (unsigned char)255;
		CornersImage[Position+2] = (unsigned char)255;

		// Image in Color mixed with image original
			
		gDataAux[Position] = (unsigned char)255;
		gDataAux[Position+1] = (unsigned char)0;
		gDataAux[Position+2] = (unsigned char)0;
			
		CornersPoints = CornersPoints->prox;	
		Cont++;					
	};		

	// Save 8 points of the pattern 
	if(Cont == 8)
	{
		l=0;
		while(Aux!=NULL)
		{
			CornersPointsOrd = insere_ordenado_X(CornersPointsOrd,Aux->info,Aux->x,Aux->y); 
			Aux = Aux ->prox;
		}
		while(CornersPointsOrd != NULL)
		{
			if(Flag_Points == 0)
			{
				VectorPointsOld[l][0] =VectorPointsNew[l][0] = CornersPointsOrd->x ; 
				VectorPointsOld[l][1] =VectorPointsNew[l][1] = CornersPointsOrd->y;
			}
			else
			{
				VectorPointsNew[l][0] = CornersPointsOrd->x ; 
				VectorPointsNew[l][1] = CornersPointsOrd->y;
			};
			CornersPointsOrd = CornersPointsOrd->prox; 
			l++;					
		};
			
		Order_Square(VectorPointsNew, VectorPoints);
			


		for(l=0;l<8;l++)
		{
			printf("\n Vector %i , em x = %i , y = %i",l, VectorPoints[l][1],VectorPoints[l][0]);
		}

		DrawInOrder(gDataAux, VectorPoints);
		Flag_Points = 1;
	}
	else 
		Flag_Points =0;

	if(Cont == 8)
	{
		// Number of points np = 8;
		np =8;
		xw[0] = 29.5 ; xw[1] = -30.5 ; xw[2] = -30.5 ; xw[3] = 29.5 ; xw[4] = 10.5 ; xw[5] = -19.5 ; xw[6] = -19.5 ; xw[7] = 10.5 ;
		yw[0] = -34.5 ; yw[1] = 35.5 ; yw[2] = -34.5 ; yw[3] = 35.5 ; yw[4] = -22.5 ; yw[5] = 12.5 ; yw[6] = -22.5 ; yw[7] = 12.5 ;
		
		// xf yf points of world
		xf[0] = (double)VectorPoints[0][1]- 160; xf[1] = (double)VectorPoints[1][1]- 160;
		yf[0] = (double)VectorPoints[0][0]- 120; yf[1] = (double)VectorPoints[1][0]- 120;
		xf[2] = (double)VectorPoints[2][1]- 160; xf[3] = (double)VectorPoints[3][1]- 160;
		yf[2] = (double)VectorPoints[2][0]- 120; yf[3] = (double)VectorPoints[3][0]- 120;
		xf[4] = (double)VectorPoints[4][1]- 160; xf[5] = (double)VectorPoints[5][1]- 160;
		yf[4] = (double)VectorPoints[4][0]- 120; yf[5] = (double)VectorPoints[5][0]- 120;
		xf[6] = (double)VectorPoints[6][1]- 160; xf[7] = (double)VectorPoints[7][1]- 160;
		yf[6] = (double)VectorPoints[6][0]- 120; yf[7] = (double)VectorPoints[7][0]- 120;

		
		tsaiCalibration2D(np, xw, yw, xf, yf, &focal, T, r);
		tsaiOGLModelViewMatrix(T, r, mtsaiopengl_modelview);
		tsaiOGLProjectionMatrix(focal, 1.0F, 1000.0F, IMAGE_W, IMAGE_H, mtsaiopengl_projection);
	};


	printf("\n ----> Acertados  %i",Cont);
	libera(CornersPoints);
	libera(CornersPointsOrd);
	libera(Aux);
	return;
}

#endif