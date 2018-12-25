using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vil.Core
{
    internal class NullProgressProvider : IProgress
    {
        // Methods
        public void Increment()
        {
        }
    }


}
