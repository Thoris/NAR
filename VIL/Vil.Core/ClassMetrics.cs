using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vil.Core
{
    internal class ClassMetrics : TypeMetrics
    {
        // Methods
        internal ClassMetrics(Type type)
        {
            base.type = type;
            base.codeElement = CodeElement.Class;
            int length = type.GetCustomAttributes(true).Length;
        }
    }

 

}
