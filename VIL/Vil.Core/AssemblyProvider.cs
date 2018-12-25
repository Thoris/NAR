using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Reflector.Disassembler;
using System.Diagnostics;
using System.IO;

namespace Vil.Core
{
    internal sealed class AssemblyProvider : IAssemblyProvider
    {
        // Fields
        private string froot = "";

        // Methods
        public Assembly[] GetAssemblies()
        {
            throw new NotImplementedException();
        }

        public Assembly Load(string assemblyName)
        {
            Assembly assembly5;
            try
            {
                if (AssemblyGiver.assHash.ContainsKey(assemblyName))
                {
                    return (Assembly)AssemblyGiver.assHash[assemblyName];
                }
                Assembly assembly = Assembly.Load(assemblyName);
                if (!assembly.GlobalAssemblyCache)
                {
                    AssemblyGiver.assHash[assembly.Location] = assembly;
                }
                assembly5 = assembly;
            }
            catch (Exception)
            {
                string str = assemblyName;
                if ((str.IndexOf(",") != -1) && (str.IndexOf("Culture") != -1))
                {
                    str = str.Substring(0, str.IndexOf(","));
                }
                try
                {
                    if (AssemblyGiver.assHash.ContainsKey(this.froot + str + ".dll"))
                    {
                        return (Assembly)AssemblyGiver.assHash[this.froot + str + ".dll"];
                    }
                    Assembly assembly2 = Assembly.LoadFrom(this.froot + str + ".dll");
                    AssemblyGiver.assHash[this.froot + str + ".dll"] = assembly2;
                    assembly5 = assembly2;
                }
                catch (Exception)
                {
                    if (Process.GetCurrentProcess().ProcessName == "mono")
                    {
                        string environmentVariable = Environment.GetEnvironmentVariable("MONO_PATH");
                        string str3 = Path.DirectorySeparatorChar.ToString();
                        if (!environmentVariable.EndsWith(str3))
                        {
                            environmentVariable = environmentVariable + str3;
                        }
                        if (AssemblyGiver.assHash.ContainsKey(environmentVariable + str + ".dll"))
                        {
                            return (Assembly)AssemblyGiver.assHash[environmentVariable + str + ".dll"];
                        }
                        Assembly assembly3 = Assembly.LoadFrom(environmentVariable + str + ".dll");
                        AssemblyGiver.assHash[environmentVariable + str + ".dll"] = assembly3;
                        return assembly3;
                    }
                    if (AssemblyGiver.assHash.ContainsKey(this.froot + str + ".dll"))
                    {
                        return (Assembly)AssemblyGiver.assHash[this.froot + str + ".dll"];
                    }
                    Assembly assembly4 = Assembly.LoadFrom(this.froot + str + ".dll");
                    AssemblyGiver.assHash[this.froot + str + ".dll"] = assembly4;
                    assembly5 = assembly4;
                }
            }
            return assembly5;
        }

        public void SetRoot(string root)
        {
            this.froot = root;
        }
    }


}
