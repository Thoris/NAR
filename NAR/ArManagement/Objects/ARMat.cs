using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ArManagement.Objects
{
    /** \struct ARMat
* \brief matrix structure.
* 
* Defined the structure of the matrix type based on a dynamic allocation.
* The matrix format is :<br>
*  <---- clm ---><br>
*  [ 10  20  30 ] ^<br>
*  [ 20  10  15 ] |<br>
*  [ 12  23  13 ] row<br>
*  [ 20  10  15 ] |<br>
*  [ 13  14  15 ] v<br>
* 
* \param m content of matrix 
* \param row number of lines in matrix
* \param clm number of column in matrix
*/


    public class ARMat
    {
        public double [] m;
        public int row;
        public int clm;
    }
}
