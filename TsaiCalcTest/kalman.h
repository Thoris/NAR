typedef struct KalmanStruct
{
    int m;                     /* number of measurement vector dimensions */
    int n;                     /* number of state vector dimensions */
    float* x	;         /* =state_post->data.fl */
    float* PriorState;          /* =state_post->data.fl */
    float* Phi;           /* =transition_matrix->data.fl */
    float* H;     /* =measurement_matrix->data.fl */
    float* R;        /* =measurement_noise_cov->data.fl */
    float* Q;        /* =process_noise_cov->data.fl */
    float* K;        /* =gain->data.fl */
    float* PriorErrorCovariance;/* =error_cov_pre->data.fl */
    float* P;/* =error_cov_post->data.fl */
    float* Temp1;               /* temp1->data.fl */
    float* Temp2;               /* temp2->data.fl */
	float* MeasErrorCovariance;  /* cov. de zk - xk-1 */
	float chi_square;				/* confidence score */


}

KalmanStruct;

KalmanStruct* CreateKalman(int n, int m);

void ReleaseKalman(KalmanStruct* Kalm);

void KalmanPredict(KalmanStruct* Kalman);

void KalmanCorrect(KalmanStruct* Kalman, float* Measure);


