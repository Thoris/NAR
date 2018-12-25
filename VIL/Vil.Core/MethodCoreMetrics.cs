using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Reflector.Disassembler;

namespace Vil.Core
{
    internal class MethodCoreMetrics : MetricProvider
    {
        // Fields
        internal MethodBase methodBase;

        // Methods
        internal void ProvideMetrics(MetricRequest request, MetricResponse response)
        {
            Reflector.Disassembler.MethodBody methodBody;
            try
            {
                methodBody = ModuleMetrics.reader.GetMethodBody(this.methodBase);
            }
            catch (ModuleReaderException)
            {
                methodBody = null;
            }
            base.computedMetrics = new float[request.neededMetrics.Length];
            for (int i = 0; i < request.neededMetrics.Length; i++)
            {
                InstructionDumper dumper;
                int[] numArray;
                int num2;
                if (methodBody == null)
                {
                    goto Label_01CA;
                }
                switch (request.neededMetrics[i])
                {
                    case AvailableMetrics.LOC:
                        {
                            base.computedMetrics[i] = Metrics.MethodSize(methodBody);
                            continue;
                        }
                    case AvailableMetrics.DEAD:
                        dumper = new InstructionDumper();
                        if (base.codeElement != CodeElement.Method)
                        {
                            break;
                        }
                        numArray = dumper.ComputeStack(methodBody, this.methodBase, false);
                        goto Label_00D8;

                    case AvailableMetrics.PARAMS:
                        {
                            base.computedMetrics[i] = this.methodBase.GetParameters().Length;
                            continue;
                        }
                    case AvailableMetrics.MAXSTACK:
                        {
                            InstructionDumper dumper2 = new InstructionDumper();
                            int num4 = 0;
                            try
                            {
                                int[] numArray2;
                                if (base.codeElement == CodeElement.Method)
                                {
                                    numArray2 = dumper2.ComputeStack(methodBody, this.methodBase, false);
                                }
                                else
                                {
                                    numArray2 = dumper2.ComputeStack(methodBody, this.methodBase, true);
                                }
                                for (int m = 0; m < numArray2.Length; m++)
                                {
                                    if (numArray2[m] > num4)
                                    {
                                        num4 = numArray2[m];
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                num4 = -1;
                            }
                            base.computedMetrics[i] = num4;
                            continue;
                        }
                    case AvailableMetrics.LOCALS:
                        {
                            base.computedMetrics[i] = methodBody.GetLocals().Length;
                            continue;
                        }
                    case AvailableMetrics.TRYBLOCKS:
                        {
                            base.computedMetrics[i] = methodBody.GetExceptions().Length;
                            continue;
                        }
                    case AvailableMetrics.CC:
                        {
                            base.computedMetrics[i] = Metrics.CyclomaticComplexity(methodBody);
                            continue;
                        }
                    default:
                        goto Label_01BB;
                }
                numArray = dumper.ComputeStack(methodBody, this.methodBase, true);
            Label_00D8:
                num2 = 0;
                for (int k = 0; k < numArray.Length; k++)
                {
                    if (numArray[k] < -10)
                    {
                        num2++;
                    }
                }
                base.computedMetrics[i] = num2;
                continue;
            Label_01BB:
                base.computedMetrics[i] = -1f;
                continue;
            Label_01CA:
                base.computedMetrics[i] = -1f;
            }
            base.isImplementation = true;
            base.name = this.methodBase.DeclaringType.FullName + "::" + this.methodBase.Name;
            ParameterInfo[] parameters = this.methodBase.GetParameters();
            int length = parameters.Length;
            Type[] typeArray = new Type[length];
            for (int j = 0; j < length; j++)
            {
                typeArray[j] = parameters[j].ParameterType;
            }
            base.parameters = typeArray;
            if (this.methodBase.IsStatic)
            {
                base.location = Location.Static;
            }
            else
            {
                base.location = Location.Instance;
            }
            if (this.methodBase.IsPublic)
            {
                base.visability = Visability.Public;
            }
            else if (this.methodBase.IsPrivate)
            {
                base.visability = Visability.Private;
            }
            else if (this.methodBase.IsFamilyAndAssembly)
            {
                base.visability = Visability.ProtectedAndInternal;
            }
            else if (this.methodBase.IsFamilyOrAssembly)
            {
                base.visability = Visability.ProtectedOrInternal;
            }
            else if (this.methodBase.IsFamily)
            {
                base.visability = Visability.Protected;
            }
            else if (this.methodBase.IsAssembly)
            {
                base.visability = Visability.Internal;
            }
            response.resultSet.Add(this);
        }
    }


}
