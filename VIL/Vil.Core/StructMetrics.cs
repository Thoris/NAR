using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vil.Core
{

    internal class StructMetrics : TypeMetrics
    {
        // Methods
        internal StructMetrics(Type type)
        {
            base.type = type;
            base.codeElement = CodeElement.Structure;
        }
    }

 

}
