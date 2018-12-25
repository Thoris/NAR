using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using Reflector.Disassembler;
using System.IO;

namespace Vil.Core
{
    public class MultiAssemblyMetrics : MetricProvider
    {
        // Fields
        private Assembly[] assemblies;
        private IProgress progress;
        internal static Hashtable tmap;

        // Methods
        public MultiAssemblyMetrics(Assembly[] assemblies)
        {
            this.assemblies = assemblies;
            this.progress = new NullProgressProvider();
        }

        public MultiAssemblyMetrics(Assembly[] assemblies, IProgress progress)
        {
            this.assemblies = assemblies;
            this.progress = progress;
        }

        public int NumberOfTypes()
        {
            if (Assembly.GetCallingAssembly().GetCustomAttributes(typeof(CommonIterator), false).Length == 0)
            {
                return 0x5f5e100;
            }
            int num = 0;
            foreach (Assembly assembly in this.assemblies)
            {
                num += assembly.GetTypes().Length;
            }
            return num;
        }

        public void ProvideMetrics(MetricRequest request, MetricResponse response)
        {
            if (Assembly.GetCallingAssembly().GetCustomAttributes(typeof(CommonIterator), false).Length != 0)
            {
                request.progressHandler = this.progress;
                Hashtable hashtable = new Hashtable();
                if (request.acouplingsLoc > -1)
                {
                    foreach (Assembly assembly in this.assemblies)
                    {
                        AssemblyProvider assemblyProvider = new AssemblyProvider();
                        assemblyProvider.SetRoot(assembly.Location.Substring(0, assembly.Location.LastIndexOf(Path.DirectorySeparatorChar.ToString()) + 1));
                        foreach (Module module in assembly.GetModules())
                        {
                            ModuleReader reader = new ModuleReader(module, assemblyProvider);
                            foreach (Type type in module.GetTypes())
                            {
                                hashtable[type] = reader;
                            }
                        }
                    }
                }
                tmap = hashtable;
                foreach (Assembly assembly2 in this.assemblies)
                {
                    new AssemblyMetrics(assembly2).ProvideMetrics(request, response);
                }
                if (((request.scope != RequestScope.Method) && (request.scope != RequestScope.Constructor)) && ((request.scope != RequestScope.Imp) && (request.scope != RequestScope.Enumeration)))
                {
                    for (int i = 0; i < request.neededMetrics.Length; i++)
                    {
                        AvailableMetrics metrics2 = request.neededMetrics[i];
                        if (metrics2 == AvailableMetrics.NOC)
                        {
                            foreach (object obj2 in response.resultSet)
                            {
                                MetricProvider provider2 = (MetricProvider)obj2;
                                if (provider2.codeElement == CodeElement.Class)
                                {
                                    if (request.NOCHash.ContainsKey(((TypeMetrics)obj2).type.FullName))
                                    {
                                        provider2.computedMetrics[i] = (int)request.NOCHash[((TypeMetrics)obj2).type.FullName];
                                    }
                                    else
                                    {
                                        provider2.computedMetrics[i] = 0f;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }


}
