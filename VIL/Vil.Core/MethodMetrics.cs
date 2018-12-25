using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Vil.Core
{

    internal class MethodMetrics : MethodCoreMetrics
    {
        // Methods
        internal MethodMetrics(MethodBase methodBase)
        {
            base.methodBase = methodBase;
            base.codeElement = CodeElement.Method;
            base.returnType = ((MethodInfo)methodBase).ReturnType;
        }
    }

 

}
