using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Reflector.Disassembler
{
    public interface IAssemblyProvider
    {
        // Methods
        Assembly[] GetAssemblies();
        Assembly Load(string assemblyName);
    }
}
