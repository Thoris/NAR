using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Vil.Core
{
    internal class EventMetrics : MetricProvider
    {
        // Fields
        internal EventInfo eventInfo;

        // Methods
        internal EventMetrics(EventInfo eventInfo)
        {
            this.eventInfo = eventInfo;
        }

        internal void ProvideMetrics(MetricRequest request, MetricResponse response)
        {
            base.computedMetrics = new float[request.neededMetrics.Length];
            for (int i = 0; i < request.neededMetrics.Length; i++)
            {
                AvailableMetrics metrics1 = request.neededMetrics[i];
                base.computedMetrics[i] = -1f;
            }
            base.codeElement = CodeElement.Event;
            base.returnType = null;
            base.name = this.eventInfo.DeclaringType.FullName + "::" + this.eventInfo.Name;
            base.location = Location.NA;
            base.visability = Visability.NA;
            response.resultSet.Add(this);
        }
    }

 

}
