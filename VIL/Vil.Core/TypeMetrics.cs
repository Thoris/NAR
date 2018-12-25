using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Vil.Core
{

    public class TypeMetrics : MetricProvider
    {
        // Fields
        internal Type type;

        // Methods
        internal int GetDIT()
        {
            Type baseType = this.type;
            if (this.type == typeof(object))
            {
                return 0;
            }
            int num = 1;
            while (baseType.BaseType != typeof(object))
            {
                num++;
                baseType = baseType.BaseType;
            }
            return num;
        }

        public Type GetTheType()
        {
            return this.type;
        }

        internal void ProvideMetrics(MetricRequest request, MetricResponse response)
        {
            int num6;
            if (((request.scope != RequestScope.Method) && (request.scope != RequestScope.Constructor)) && ((request.scope != RequestScope.Imp) && (request.scope != RequestScope.Enumeration)))
            {
                if ((request.nocLoc != -1) && (this.type != typeof(object)))
                {
                    if (request.NOCHash.ContainsKey(this.type.BaseType.FullName))
                    {
                        request.NOCHash[this.type.BaseType.FullName] = ((int)request.NOCHash[this.type.BaseType.FullName]) + 1;
                    }
                    else
                    {
                        request.NOCHash[this.type.BaseType.FullName] = 1;
                    }
                }
                if (this.type.IsPublic)
                {
                    base.visability = Visability.Public;
                }
                else
                {
                    base.visability = Visability.Private;
                }
                base.location = Location.NA;
                base.returnType = null;
                base.name = this.type.FullName;
                response.resultSet.Add(this);
                response.resultSetRollUp.Add(this);
            }
            int count = response.resultSet.Count;
            if (request.scope == RequestScope.All)
            {
                FieldInfo[] fields = this.type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                for (num6 = 0; num6 < fields.Length; num6++)
                {
                    FieldInfo fieldInfo = fields[num6];
                    new FieldMetrics(fieldInfo).ProvideMetrics(request, response);
                }
                EventInfo[] events = this.type.GetEvents(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                for (num6 = 0; num6 < events.Length; num6++)
                {
                    EventInfo eventInfo = events[num6];
                    new EventMetrics(eventInfo).ProvideMetrics(request, response);
                }
            }
            if (request.scope != RequestScope.Constructor)
            {
                MethodInfo[] methods = this.type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                for (num6 = 0; num6 < methods.Length; num6++)
                {
                    MethodBase methodBase = methods[num6];
                    new MethodMetrics(methodBase).ProvideMetrics(request, response);
                }
            }
            if (request.scope != RequestScope.Method)
            {
                ConstructorInfo[] constructors = this.type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                num6 = 0;
                while (num6 < constructors.Length)
                {
                    MethodBase base3 = constructors[num6];
                    new ConstructorMetrics(base3).ProvideMetrics(request, response);
                    num6++;
                }
            }
            if (((request.scope != RequestScope.Method) && (request.scope != RequestScope.Constructor)) && (request.scope != RequestScope.Imp))
            {
                base.computedMetrics = new float[request.neededMetrics.Length];
                for (int i = 0; i < request.neededMetrics.Length; i++)
                {
                    int num3;
                    Type type;
                    int num4;
                    Type type2;
                    int num5;
                    Type type3;
                    Type[] typeArray4;
                    switch (request.neededMetrics[i])
                    {
                        case AvailableMetrics.LCOM:
                            base.computedMetrics[i] = Metrics.LackOfCohesionOfMethods(this.type);
                            goto Label_05B3;

                        case AvailableMetrics.WMC:
                            base.computedMetrics[i] = Metrics.SumSubordinates(request.ccLoc, count, response.resultSet);
                            goto Label_05B3;

                        case AvailableMetrics.LOC:
                            base.computedMetrics[i] = Metrics.SumSubordinates(i, count, response.resultSet);
                            goto Label_05B3;

                        case AvailableMetrics.CBO:
                            base.computedMetrics[i] = Metrics.CouplingBetweenObjects(this.type);
                            goto Label_05B3;

                        case AvailableMetrics.DIT:
                            if (base.codeElement != CodeElement.Class)
                            {
                                goto Label_0567;
                            }
                            base.computedMetrics[i] = this.GetDIT();
                            goto Label_05B3;

                        case AvailableMetrics.RFC:
                            base.computedMetrics[i] = Metrics.ResponseForClass(this.type);
                            goto Label_05B3;

                        case AvailableMetrics.METHODS:
                            base.computedMetrics[i] = this.type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly).Length;
                            goto Label_05B3;

                        case AvailableMetrics.FIELDS:
                            base.computedMetrics[i] = this.type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly).Length;
                            goto Label_05B3;

                        case AvailableMetrics.CONSTRUCTORS:
                            base.computedMetrics[i] = this.type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly).Length;
                            goto Label_05B3;

                        case AvailableMetrics.CLASSES:
                            {
                                Type[] nestedTypes = this.type.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                                num3 = 0;
                                typeArray4 = nestedTypes;
                                num6 = 0;
                                goto Label_048E;
                            }
                        case AvailableMetrics.ENUMERATIONS:
                            {
                                Type[] typeArray2 = this.type.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                                num4 = 0;
                                typeArray4 = typeArray2;
                                num6 = 0;
                                goto Label_04DE;
                            }
                        case AvailableMetrics.STRUCTS:
                            {
                                Type[] typeArray3 = this.type.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                                num5 = 0;
                                typeArray4 = typeArray3;
                                num6 = 0;
                                goto Label_0537;
                            }
                        case AvailableMetrics.EVENTS:
                            base.computedMetrics[i] = this.type.GetEvents(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly).Length;
                            goto Label_05B3;

                        case AvailableMetrics.TYPES:
                            base.computedMetrics[i] = this.type.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly).Length;
                            goto Label_05B3;

                        case AvailableMetrics.IMPLEMENTEDINTERFACES:
                            base.computedMetrics[i] = this.type.GetInterfaces().Length;
                            goto Label_05B3;

                        case AvailableMetrics.IMPS:
                            base.computedMetrics[i] = this.type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly).Length + this.type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly).Length;
                            goto Label_05B3;

                        case AvailableMetrics.PROPERTIES:
                            base.computedMetrics[i] = this.type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly).Length;
                            goto Label_05B3;

                        default:
                            base.computedMetrics[i] = -1f;
                            goto Label_05B3;
                    }
                Label_0469:
                    type = typeArray4[num6];
                    if (type.IsClass && !type.IsValueType)
                    {
                        num3++;
                    }
                    num6++;
                Label_048E:
                    if (num6 < typeArray4.Length)
                    {
                        goto Label_0469;
                    }
                    base.computedMetrics[i] = num3;
                    goto Label_05B3;
                Label_04C2:
                    type2 = typeArray4[num6];
                    if (type2.IsEnum)
                    {
                        num4++;
                    }
                    num6++;
                Label_04DE:
                    if (num6 < typeArray4.Length)
                    {
                        goto Label_04C2;
                    }
                    base.computedMetrics[i] = num4;
                    goto Label_05B3;
                Label_0512:
                    type3 = typeArray4[num6];
                    if (!type3.IsEnum && type3.IsValueType)
                    {
                        num5++;
                    }
                    num6++;
                Label_0537:
                    if (num6 < typeArray4.Length)
                    {
                        goto Label_0512;
                    }
                    base.computedMetrics[i] = num5;
                    goto Label_05B3;
                Label_0567:
                    base.computedMetrics[i] = -1f;
                Label_05B3: ;
                }
            }
        }
    }


}