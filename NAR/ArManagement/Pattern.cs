using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NAR.ArManagement
{
    public class Pattern
    {
        public const int MaxNumberPatterns = 50;
        public const int SizeX = 16;
        public const int SizeY = 16;

        private int pattern_num = -1;
        private int[] patf;
        private int[, ,] pat = new int[MaxNumberPatterns, 4, SizeY * SizeX * 3];
        private int[, ,] patBW = new int[MaxNumberPatterns, 4, SizeY * SizeX * 3];
        

        //------------------------------------------
        double [,] patpow = new double[MaxNumberPatterns,4];
        double [,] patpowBW = new double[MaxNumberPatterns, 4];


        #region Methods
        public int Load(string filename)
        {
            int patno;
            int h, i, j, l, m;
            int i1, i2, i3;

            if (patf == null)
                patf = new int[MaxNumberPatterns];

            if (pattern_num == -1)
            {
                for (i = 0; i < MaxNumberPatterns; i++) 
                    patf[i] = 0;
                pattern_num = 0;
            }

            for (i = 0; i < MaxNumberPatterns; i++)
            {
                if (patf[i] == 0) 
                    break;
            }
            if (i == MaxNumberPatterns) 
                return -1;
            patno = i;

            StreamReader reader = new StreamReader(filename);

            try
            {
                for (h = 0; h < 4; h++)
                {
                    l = 0;
                    for (i3 = 0; i3 < 3; i3++)
                    {
                        for (i2 = 0; i2 < SizeY; i2++)
                        {
                            string line = reader.ReadLine();

                            if (!string.IsNullOrEmpty(line))
                            {

                                for (i1 = 0; i1 < SizeX; i1++)
                                {
                                    string valueLine = line.Substring(i1 * 3 + i1 + 1, 3);
                                    j = 255 - int.Parse(valueLine);

                                    pat[patno, h, (i2 * SizeX + i1) * 3 + i3] = j;
                                    if (i3 == 0)
                                        patBW[patno, h, i2 * SizeX + i1] = j;
                                    else
                                        patBW[patno, h, i2 * SizeX + i1] += j;
                                    if (i3 == 2)
                                        patBW[patno, h, i2 * SizeX + i1] /= 3;
                                    l += j;
                                }
                            }

                        }
                    }
                    l /= (SizeY * SizeX * 3);

                    m = 0;
                    for (i = 0; i < SizeY * SizeX * 3; i++)
                    {
                        pat[patno, h, i] -= l;
                        m += (pat[patno, h, i] * pat[patno, h, i]);
                    }
                    patpow[patno, h] = Math.Sqrt((double)m);
                    if (patpow[patno, h] == 0.0)
                        patpow[patno, h] = 0.0000001;

                    m = 0;
                    for (i = 0; i < SizeY * SizeX; i++)
                    {
                        patBW[patno, h, i] -= l;
                        m += (patBW[patno, h, i] * patBW[patno, h, i]);
                    }
                    patpowBW[patno, h] = Math.Sqrt((double)m);
                    if (patpowBW[patno, h] == 0.0)
                        patpowBW[patno, h] = 0.0000001;
                }

                patf[patno] = 1;
                pattern_num++;
            }
            finally
            {
                reader.Close();
            }
            
           
            return (patno);
        }
        #endregion
    }
}
