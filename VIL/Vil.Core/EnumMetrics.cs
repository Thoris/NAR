using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Vil.Core
{
    internal class EnumMetrics : TypeMetrics
    {
        // Methods
        internal EnumMetrics(Type type)
        {
            base.type = type;
        }

        internal void ProvideMetrics(MetricRequest request, MetricResponse response)
        {
            base.computedMetrics = new float[request.neededMetrics.Length];
            for (int i = 0; i < request.neededMetrics.Length; i++)
            {
                AvailableMetrics metrics = request.neededMetrics[i];
                if (metrics == AvailableMetrics.FIELDS)
                {
                    base.computedMetrics[i] = base.type.GetFields().Length;
                }
                else
                {
                    base.computedMetrics[i] = -1f;
                }
            }
            base.codeElement = CodeElement.Enumeration;
            base.returnType = null;
            base.name = base.type.FullName;
            base.visability = Visability.Public;
            base.location = Location.NA;
            response.resultSet.Add(this);
            response.resultSetRollUp.Add(this);
            foreach (FieldInfo info in base.type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                new FieldMetrics(info).ProvideMetrics(request, response);
            }
        }
    }



}