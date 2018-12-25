using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Vil.Core
{
    internal class FieldMetrics : MetricProvider
    {
        // Fields
        internal FieldInfo fieldInfo;

        // Methods
        internal FieldMetrics(FieldInfo fieldInfo)
        {
            this.fieldInfo = fieldInfo;
        }

        internal void ProvideMetrics(MetricRequest request, MetricResponse response)
        {
            base.computedMetrics = new float[request.neededMetrics.Length];
            for (int i = 0; i < request.neededMetrics.Length; i++)
            {
                AvailableMetrics metrics1 = request.neededMetrics[i];
                base.computedMetrics[i] = -1f;
            }
            base.codeElement = CodeElement.Field;
            base.returnType = this.fieldInfo.FieldType;
            base.name = this.fieldInfo.DeclaringType.FullName + "::" + this.fieldInfo.Name;
            if (this.fieldInfo.IsStatic)
            {
                base.location = Location.Static;
            }
            else
            {
                base.location = Location.Instance;
            }
            if (this.fieldInfo.IsPublic)
            {
                base.visability = Visability.Public;
            }
            else if (this.fieldInfo.IsPrivate)
            {
                base.visability = Visability.Private;
            }
            else if (this.fieldInfo.IsFamilyAndAssembly)
            {
                base.visability = Visability.ProtectedAndInternal;
            }
            else if (this.fieldInfo.IsFamilyOrAssembly)
            {
                base.visability = Visability.ProtectedOrInternal;
            }
            else if (this.fieldInfo.IsFamily)
            {
                base.visability = Visability.Protected;
            }
            else if (this.fieldInfo.IsAssembly)
            {
                base.visability = Visability.Internal;
            }
            response.resultSet.Add(this);
        }
    }


}