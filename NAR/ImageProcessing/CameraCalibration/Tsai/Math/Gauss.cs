using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ImageProcessing.CameraCalibration.Tsai.Math
{
    public class Gauss
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n">[in] Número de equações</param>
        /// <param name="a">[in] Matriz dos coeficientes</param>
        /// <param name="b">[in] Vetor das constantes</param>
        /// <returns>
        /// Vetor solução:
        /// (-1) => sem solução
        /// ( 0) => solução única
        /// ( 1) => infinitas soluções
        /// </returns>
        public static int Calcule(int n, double[,] a, ref double[] b)
        {
            int i, j, k;
            int imax;               /* Linha pivot */
            double amax, rmax;      /* Auxiliares  */

            for (j = 0; j < n - 1; j++)
            {
                /* Loop nas colunas de a */
                rmax = 0.0;
                imax = j;
                for (i = j; i < n; i++)
                {
                    /* Loop para determinar maior relação a[i][j]/a[i][k] */
                    amax = 0.0;
                    for (k = j; k < n; k++)
                        /* Loop para determinar maior elemento da linha i     */
                        if (System.Math.Abs(a[i, k]) > amax)
                            amax = System.Math.Abs(a[i, k]);

                    if (amax < Util.TOL)         /* Verifica se os elementos da linha são nulos        */
                        return -1;          /* Finaliza subrotina pois o sistema não tem solução  */
                    else if ((System.Math.Abs(a[i, j]) > (rmax * amax)) && (System.Math.Abs(a[i, j]) >= Util.TOL))
                    {
                        /* Testa relação da linha corrente */
                        rmax = System.Math.Abs(a[i, j]) / amax;
                        imax = i;             /* Encontra linha de maior relação - linha pivot      */
                    }
                }
                if (System.Math.Abs(a[imax, j]) < Util.TOL)
                {
                    /* Verifica se o pivot é nulo              */
                    /* Procura linha com pivot nao nulo        */
                    for (k = imax + 1; k < n; k++)          
                        if (System.Math.Abs(a[k, j]) < Util.TOL)
                            imax = k;                       /* Troca linha j por linha k               */
                    if (System.Math.Abs(a[imax, j]) < Util.TOL)
                        return 1;                        /* não existe solução única para o sistema */
                }
                if (imax != j)
                {                   /* Troca linha j por linha imax            */
                    for (k = j; k < n; k++)
                        Util.SWAP(ref a[imax, k], ref a[j, k]);
                    Util.SWAP(ref b[imax], ref b[j]);
                }
                /* Anula elementos abaixo da diagonal      */
                for (i = j + 1; i < n; i++)
                {            
                    double aux = a[i, j] / a[j, j];

                    /* Transforma os demais elementos da linha */
                    for (k = j + 1; k < n; k++)             
                        a[i, k] -= aux * a[j, k];
                    b[i] -= aux * b[j];
                }
            }
            /* Verifica unicidade da solução           */
            if (System.Math.Abs(a[n - 1, n - 1]) <= Util.TOL)       
                return -1;                         /* Sistema sem solução                     */
            else
            {
                b[n - 1] /= a[n - 1, n - 1];             
                
                /* Calcula última coordenada               */
                for (i = n - 2; i >= 0; i--)
                {           
                    /* Inicia retro-substituição               */
                    for (j = i + 1; j < n; j++)
                        b[i] -= a[i, j] * b[j];
                    b[i] /= a[i, i];
                }
            }
            return 0;                           /* Finaliza a subrotina com solução única  */
        }


        #endregion
    }
}
