using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Vil.Core
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VARG
    {
        public string filter;
        public string assemblies;
        public string options;
        public string metrics;
        public string copymessage;
        public string reportName;
    }

 

 

}
