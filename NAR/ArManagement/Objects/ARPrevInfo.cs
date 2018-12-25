using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ArManagement.Objects
{

    /** \struct arPrevInfo
    * \brief structure for temporal continuity of tracking
    *
    * History structure for arDetectMarkerLite and arGetTransMatCont
    */
    class ARPrevInfo
    {
        ARMarkerInfo marker;
        int count;
    }
}
