using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Threading;
using System.IO;

namespace Vil.Core
{
    public class AssemblyGiver
    {
        // Fields
        private Assembly assembly;
        internal static Hashtable assHash = new Hashtable();

        // Methods
        public AssemblyGiver()
        {
            ResolveEventHandler handler = new ResolveEventHandler(this.AssemblyResolver);
            Thread.GetDomain().AssemblyResolve += handler;
        }

        public Assembly AssemblyResolver(object sender, ResolveEventArgs args)
        {
            string str = this.assembly.Location.Substring(0, this.assembly.Location.LastIndexOf(Path.DirectorySeparatorChar.ToString()) + 1);
            string name = args.Name;
            if ((name.IndexOf(",") != -1) && (name.IndexOf("Culture") != -1))
            {
                name = name.Substring(0, name.IndexOf(","));
            }
            if (assHash.ContainsKey(str + name + ".dll"))
            {
                return (Assembly)assHash[str + name + ".dll"];
            }
            Assembly assembly = Assembly.LoadFrom(str + name + ".dll");
            assHash[str + name + ".dll"] = assembly;
            return assembly;
        }

        public Assembly LoadAssembly(string assemblyName)
        {
            if (assHash.ContainsKey(assemblyName))
            {
                return (Assembly)assHash[assemblyName];
            }
            Assembly assembly = Assembly.LoadFrom(assemblyName);
            assHash[assemblyName] = assembly;
            return assembly;
        }
    }

}
