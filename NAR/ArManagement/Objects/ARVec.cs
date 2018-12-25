using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ArManagement.Objects
{
    /** \struct ARVec
* \brief vector structure.
*
* The vector format is :<br>
*  <---- clm ---><br>
*  [ 10  20  30 ]<br>
* Defined the structure of the vector type based on a dynamic allocation.
* \param m content of vector
* \param clm number of column in matrix
*/
    class ARVec
    {
        public double v;
        public int clm;
    }
}
