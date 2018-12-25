using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Vil.Core
{
    public class MetricProvider : IComparable
    {
        // Fields
        public Type backingType;
        public CodeElement codeElement;
        public float[] computedMetrics;
        public bool isImplementation = false;
        public Location location;
        public Assembly myAssembly;
        public Module myModule;
        public string name;
        public Type[] parameters;
        public Type returnType;
        public static int sortColumn = 0;
        public Type[] thrownTypes;
        public Visability visability;

        // Methods
        public int CompareTo(object obj)
        {
            MetricProvider provider = (MetricProvider)obj;
            return this.computedMetrics[sortColumn].CompareTo(provider.computedMetrics[sortColumn]);
        }
    }

 

}
