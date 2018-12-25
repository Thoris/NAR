using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vil.Core
{
    internal class FilterStruct
    {
        // Fields
        internal int location;
        internal AvailableMetrics metric;
        internal string name;

        // Methods
        internal FilterStruct(string name, int location, AvailableMetrics metric)
        {
            this.name = name;
            this.location = location;
            this.metric = metric;
        }
    }

 

}
