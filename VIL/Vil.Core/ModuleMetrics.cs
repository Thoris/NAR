using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Reflector.Disassembler;
using System.Reflection;

namespace Vil.Core
{

    internal class ModuleMetrics : MetricProvider
    {
        // Fields
        internal Module module;
        internal static ModuleReader reader;
        internal static Hashtable readermap = new Hashtable();
        private string root = "";

        // Methods
        internal ModuleMetrics(Module module, string root)
        {
            this.module = module;
            this.root = root;
        }

        internal void ProvideMetrics(MetricRequest request, MetricResponse response)
        {
            AssemblyProvider assemblyProvider = new AssemblyProvider();
            assemblyProvider.SetRoot(this.root);
            reader = new ModuleReader(this.module, assemblyProvider);
            base.visability = Visability.Public;
            base.location = Location.NA;
            base.codeElement = CodeElement.Module;
            base.returnType = null;
            base.name = this.module.Name;
            int count = response.resultSet.Count;
            response.resultSet.Add(this);
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            int num6 = 0;
            foreach (Type type in this.module.GetTypes())
            {
                request.progressHandler.Increment();
                if (type.IsClass)
                {
                    if (request.CanUseType(type))
                    {
                        readermap[type] = reader;
                        new ClassMetrics(type).ProvideMetrics(request, response);
                        if (type.IsAbstract)
                        {
                            num6++;
                        }
                        num2++;
                    }
                }
                else if (type.IsEnum)
                {
                    readermap[type] = reader;
                    new EnumMetrics(type).ProvideMetrics(request, response);
                    num4++;
                }
                else if (type.IsInterface)
                {
                    readermap[type] = reader;
                    new InterfaceMetrics(type).ProvideMetrics(request, response);
                    num6++;
                    num3++;
                }
                else if (type.IsValueType)
                {
                    readermap[type] = reader;
                    new StructMetrics(type).ProvideMetrics(request, response);
                    num5++;
                }
            }
            for (int i = count; i < response.resultSet.Count; i++)
            {
                ((MetricProvider)response.resultSet[i]).myModule = this.module;
            }
            if ((((request.scope != RequestScope.Method) && (request.scope != RequestScope.Constructor)) && ((request.scope != RequestScope.Imp) && (request.scope != RequestScope.Type))) && (((request.scope != RequestScope.ImpType) && (request.scope != RequestScope.Interface)) && (((request.scope != RequestScope.Class) && (request.scope != RequestScope.Struct)) && (request.scope != RequestScope.Enumeration))))
            {
                AvailableMetrics metrics;
                base.computedMetrics = new float[request.neededMetrics.Length];
                for (int j = 0; j < request.neededMetrics.Length; j++)
                {
                    metrics = request.neededMetrics[j];
                    switch (metrics)
                    {
                        case AvailableMetrics.CLASSES:
                            base.computedMetrics[j] = num2;
                            break;

                        case AvailableMetrics.INTERFACES:
                            base.computedMetrics[j] = num3;
                            break;

                        case AvailableMetrics.ENUMERATIONS:
                            base.computedMetrics[j] = num4;
                            break;

                        case AvailableMetrics.STRUCTS:
                            base.computedMetrics[j] = num5;
                            break;

                        case AvailableMetrics.TYPES:
                            base.computedMetrics[j] = ((num2 + num4) + num3) + num5;
                            break;

                        case AvailableMetrics.IMPTYPES:
                            base.computedMetrics[j] = num5 + num2;
                            break;

                        case AvailableMetrics.ECOUPLINGS:
                            {
                                ArrayList modules = new ArrayList();
                                modules.Add(this.module);
                                base.computedMetrics[j] = Metrics.GetEfferentCoupling(modules, request);
                                break;
                            }
                        case AvailableMetrics.ACOUPLINGS:
                            {
                                ArrayList list2 = new ArrayList();
                                list2.Add(this.module);
                                base.computedMetrics[j] = Metrics.GetAfferentCoupling(list2);
                                break;
                            }
                        case AvailableMetrics.ABSTRACTS:
                            base.computedMetrics[j] = num6;
                            break;

                        case AvailableMetrics.LOC:
                            base.computedMetrics[j] = Metrics.SumSubordinates(j, 0, response.resultSetRollUp);
                            break;

                        default:
                            base.computedMetrics[j] = -1f;
                            break;
                    }
                }
                for (int k = 0; k < request.neededMetrics.Length; k++)
                {
                    switch (request.neededMetrics[k])
                    {
                        case AvailableMetrics.INSTABILITY:
                            if ((base.computedMetrics[request.ecouplingsLoc] != 0f) || (base.computedMetrics[request.acouplingsLoc] != 0f))
                            {
                                break;
                            }
                            base.computedMetrics[k] = 0f;
                            goto Label_0481;

                        case AvailableMetrics.ABSTRACTNESS:
                            base.computedMetrics[k] = ((float)num6) / ((float)(((num2 + num4) + num3) + num5));
                            base.computedMetrics[k] = (float)Math.Round((double)base.computedMetrics[k], 2);
                            goto Label_0481;

                        default:
                            goto Label_0481;
                    }
                    base.computedMetrics[k] = base.computedMetrics[request.ecouplingsLoc] / (base.computedMetrics[request.ecouplingsLoc] + base.computedMetrics[request.acouplingsLoc]);
                    base.computedMetrics[k] = (float)Math.Round((double)base.computedMetrics[k], 2);
                Label_0481: ;
                }
                for (int m = 0; m < request.neededMetrics.Length; m++)
                {
                    metrics = request.neededMetrics[m];
                    if (metrics == AvailableMetrics.DISTANCE)
                    {
                        base.computedMetrics[m] = Math.Abs((float)(((base.computedMetrics[request.abstractnessLoc] + base.computedMetrics[request.instabilityLoc]) - 1f) / 2f));
                        base.computedMetrics[m] = (float)Math.Round((double)base.computedMetrics[m], 2);
                    }
                }
            }
        }
    }



}