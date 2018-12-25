using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.IO;

namespace Vil.Core
{
    internal class AssemblyMetrics : MetricProvider
    {
        // Fields
        private Assembly assembly;

        // Methods
        internal AssemblyMetrics(Assembly assembly)
        {
            this.assembly = assembly;
        }

        internal void ProvideMetrics(MetricRequest request, MetricResponse response)
        {
            base.visability = Visability.Public;
            base.location = Location.NA;
            base.codeElement = CodeElement.Assembly;
            base.returnType = null;
            base.name = this.assembly.ToString();
            response.resultSet.Add(this);
            response.resultSetRollUp = new ArrayList();
            int count = response.resultSet.Count;
            int num2 = 0;
            ArrayList modules = new ArrayList();
            foreach (Module module in this.assembly.GetModules())
            {
                modules.Add(module);
                new ModuleMetrics(module, this.assembly.Location.Substring(0, this.assembly.Location.LastIndexOf(Path.DirectorySeparatorChar.ToString()) + 1)).ProvideMetrics(request, response);
                num2++;
            }
            for (int i = count; i < response.resultSet.Count; i++)
            {
                ((MetricProvider)response.resultSet[i]).myAssembly = this.assembly;
            }
            if ((((request.scope != RequestScope.Method) && (request.scope != RequestScope.Constructor)) && ((request.scope != RequestScope.Imp) && (request.scope != RequestScope.Type))) && (((request.scope != RequestScope.ImpType) && (request.scope != RequestScope.Interface)) && (((request.scope != RequestScope.Class) && (request.scope != RequestScope.Struct)) && (request.scope != RequestScope.Enumeration))))
            {
                AvailableMetrics metrics2;
                base.computedMetrics = new float[request.neededMetrics.Length];
                for (int j = 0; j < request.neededMetrics.Length; j++)
                {
                    metrics2 = request.neededMetrics[j];
                    if (metrics2 != AvailableMetrics.LOC)
                    {
                        switch (metrics2)
                        {
                            case AvailableMetrics.CLASSES:
                            case AvailableMetrics.INTERFACES:
                            case AvailableMetrics.ENUMERATIONS:
                            case AvailableMetrics.STRUCTS:
                            case AvailableMetrics.TYPES:
                            case AvailableMetrics.IMPTYPES:
                                base.computedMetrics[j] = Metrics.SumSubordinates(j, count, response.resultSet, CodeElement.Module);
                                goto Label_027E;

                            case AvailableMetrics.EVENTS:
                            case AvailableMetrics.IMPLEMENTEDINTERFACES:
                            case AvailableMetrics.IMPS:
                            case AvailableMetrics.INSTABILITY:
                            case AvailableMetrics.ABSTRACTNESS:
                            case AvailableMetrics.DISTANCE:
                            case AvailableMetrics.PROPERTIES:
                                goto Label_0270;

                            case AvailableMetrics.MODULES:
                                base.computedMetrics[j] = num2;
                                goto Label_027E;

                            case AvailableMetrics.ECOUPLINGS:
                                base.computedMetrics[j] = Metrics.GetEfferentCoupling(modules, request);
                                goto Label_027E;

                            case AvailableMetrics.ACOUPLINGS:
                                base.computedMetrics[j] = Metrics.GetAfferentCoupling(modules);
                                goto Label_027E;

                            case AvailableMetrics.ABSTRACTS:
                                base.computedMetrics[j] = Metrics.SumSubordinates(j, count, response.resultSet, CodeElement.Module);
                                goto Label_027E;

                            case AvailableMetrics.NOC:
                                goto Label_0223;
                        }
                        goto Label_0270;
                    }
                    base.computedMetrics[j] = Metrics.SumSubordinates(j, 0, response.resultSetRollUp);
                    goto Label_027E;
                Label_0223:
                    base.computedMetrics[j] = -1f;
                    goto Label_027E;
                Label_0270:
                    base.computedMetrics[j] = -1f;
                Label_027E: ;
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
                            goto Label_0385;

                        case AvailableMetrics.ABSTRACTNESS:
                            base.computedMetrics[k] = base.computedMetrics[request.abstractsLoc] / base.computedMetrics[request.typesLoc];
                            base.computedMetrics[k] = (float)Math.Round((double)base.computedMetrics[k], 2);
                            goto Label_0385;

                        default:
                            goto Label_0385;
                    }
                    base.computedMetrics[k] = base.computedMetrics[request.ecouplingsLoc] / (base.computedMetrics[request.ecouplingsLoc] + base.computedMetrics[request.acouplingsLoc]);
                    base.computedMetrics[k] = (float)Math.Round((double)base.computedMetrics[k], 2);
                Label_0385: ;
                }
                for (int m = 0; m < request.neededMetrics.Length; m++)
                {
                    metrics2 = request.neededMetrics[m];
                    if (metrics2 == AvailableMetrics.DISTANCE)
                    {
                        base.computedMetrics[m] = Math.Abs((float)(((base.computedMetrics[request.abstractnessLoc] + base.computedMetrics[request.instabilityLoc]) - 1f) / 2f));
                        base.computedMetrics[m] = (float)Math.Round((double)base.computedMetrics[m], 2);
                    }
                }
            }
        }
    }


}
