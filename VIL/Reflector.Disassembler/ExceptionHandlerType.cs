using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reflector.Disassembler
{
    public enum ExceptionHandlerType
    {
        Fault,
        Catch,
        Finally,
        Filter
    }
}
