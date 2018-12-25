using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vil.Core
{
    public class EvaluatorItem
    {
        // Fields
        public string Expression;
        public string Name;
        public Type ReturnType;

        // Methods
        public EvaluatorItem(Type returnType, string expression, string name)
        {
            this.ReturnType = returnType;
            this.Expression = expression;
            this.Name = name;
        }
    }

 

}
