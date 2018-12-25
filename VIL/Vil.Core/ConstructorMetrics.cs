using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Vil.Core
{
    internal class ConstructorMetrics : MethodCoreMetrics
    {
        // Methods
        internal ConstructorMetrics(MethodBase methodBase)
        {
            base.methodBase = methodBase;
            base.codeElement = CodeElement.Constructor;
            base.returnType = null;
        }
    }

 

}
