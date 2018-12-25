using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Vil.Core
{
    internal class InterfaceMetrics : TypeMetrics
    {
        // Methods
        internal InterfaceMetrics(Type type)
        {
            base.type = type;
        }

        internal void ProvideMetrics(MetricRequest request, MetricResponse response)
        {
            int num2;
            base.computedMetrics = new float[request.neededMetrics.Length];
            for (int i = 0; i < request.neededMetrics.Length; i++)
            {
                switch (request.neededMetrics[i])
                {
                    case AvailableMetrics.METHODS:
                        base.computedMetrics[i] = base.type.GetMethods().Length;
                        break;

                    case AvailableMetrics.FIELDS:
                        base.computedMetrics[i] = base.type.GetFields().Length;
                        break;

                    case AvailableMetrics.EVENTS:
                        base.computedMetrics[i] = base.type.GetEvents().Length;
                        break;

                    case AvailableMetrics.IMPLEMENTEDINTERFACES:
                        base.computedMetrics[i] = base.type.GetInterfaces().Length;
                        break;

                    default:
                        base.computedMetrics[i] = -1f;
                        break;
                }
            }
            base.codeElement = CodeElement.Interface;
            base.returnType = null;
            base.name = base.type.FullName;
            base.visability = Visability.Public;
            base.location = Location.NA;
            response.resultSet.Add(this);
            response.resultSetRollUp.Add(this);
            FieldInfo[] fields = base.type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            for (num2 = 0; num2 < fields.Length; num2++)
            {
                FieldInfo fieldInfo = fields[num2];
                new FieldMetrics(fieldInfo).ProvideMetrics(request, response);
            }
            MethodInfo[] methods = base.type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            for (num2 = 0; num2 < methods.Length; num2++)
            {
                MethodBase methodBase = methods[num2];
                new MethodMetrics(methodBase).ProvideMetrics(request, response);
            }
            foreach (EventInfo info2 in base.type.GetEvents(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                new EventMetrics(info2).ProvideMetrics(request, response);
            }
        }
    }


}