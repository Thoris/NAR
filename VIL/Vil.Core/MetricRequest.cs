using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;

namespace Vil.Core
{
    public class MetricRequest
    {
        // Fields
        public int abstractnessLoc = -1;
        public int abstractsLoc = -1;
        public int acouplingsLoc = -1;
        public int cboLoc = -1;
        public int ccLoc = -1;
        public int classesLoc = -1;
        public bool codeElement = false;
        public int constructorsLoc = -1;
        public int curCounter = 0;
        public int deadLoc = -1;
        public int distanceLoc = -1;
        public int ditLoc = -1;
        public int dupLoc = -1;
        public int ecouplingsLoc = -1;
        public int enumerationsLoc = -1;
        public int eventsLoc = -1;
        public int fieldsLoc = -1;
        public bool hasFilter = false;
        public bool ignoreTestFixtures = true;
        public int impinterfacesLoc = -1;
        public int impsLoc = -1;
        public int imptypesLoc = -1;
        public int instabilityLoc = -1;
        public int interfacesLoc = -1;
        public int lcomLoc = -1;
        public int localsLoc = -1;
        public bool location = false;
        public int locLoc = -1;
        public int maxstackLoc = -1;
        public int methodsLoc = -1;
        public int modulesLoc = -1;
        public AvailableMetrics[] neededMetrics;
        internal Hashtable NOCHash = new Hashtable();
        public int nocLoc = -1;
        public int numRequested = 0;
        public int paramsLoc = -1;
        public IProgress progressHandler;
        public int propertiesLoc = -1;
        public bool returnType = false;
        public int rfcLoc = -1;
        public RequestScope scope = RequestScope.Default;
        public bool shortType = false;
        public bool showModule = false;
        public int structsLoc = -1;
        public bool trim = false;
        public int tryblocksLoc = -1;
        public int typesLoc = -1;
        public bool visability = false;
        public int wmcLoc = -1;

        // Methods
        internal bool CanUseType(Type type)
        {
            if (this.ignoreTestFixtures && Metrics.IsTestFixture(type))
            {
                return false;
            }
            return true;
        }

        public void InitLocs()
        {
            this.ccLoc = -1;
            this.lcomLoc = -1;
            this.wmcLoc = -1;
            this.locLoc = -1;
            this.cboLoc = -1;
            this.ditLoc = -1;
            this.rfcLoc = -1;
            this.nocLoc = -1;
            this.deadLoc = -1;
            this.paramsLoc = -1;
            this.maxstackLoc = -1;
            this.dupLoc = -1;
            this.localsLoc = -1;
            this.tryblocksLoc = -1;
            this.methodsLoc = -1;
            this.fieldsLoc = -1;
            this.constructorsLoc = -1;
            this.classesLoc = -1;
            this.interfacesLoc = -1;
            this.enumerationsLoc = -1;
            this.structsLoc = -1;
            this.eventsLoc = -1;
            this.modulesLoc = -1;
            this.impinterfacesLoc = -1;
            this.typesLoc = -1;
            this.imptypesLoc = -1;
            this.impsLoc = -1;
            this.propertiesLoc = -1;
            this.acouplingsLoc = -1;
            this.ecouplingsLoc = -1;
            this.instabilityLoc = -1;
            this.abstractnessLoc = -1;
            this.distanceLoc = -1;
            this.abstractsLoc = -1;
        }

        public string ProcessFilter(string rawFilter)
        {
            if (Assembly.GetCallingAssembly().GetCustomAttributes(typeof(CommonIterator), false).Length != 0)
            {
                if ((rawFilter != null) && (rawFilter.Trim().Length > 0))
                {
                    ArrayList list = new ArrayList();
                    list.Add(new FilterStruct("cc", this.ccLoc, AvailableMetrics.CC));
                    list.Add(new FilterStruct("dit", this.ditLoc, AvailableMetrics.DIT));
                    list.Add(new FilterStruct("lcom", this.lcomLoc, AvailableMetrics.LCOM));
                    list.Add(new FilterStruct("wmc", this.wmcLoc, AvailableMetrics.WMC));
                    list.Add(new FilterStruct("cbo", this.cboLoc, AvailableMetrics.CBO));
                    list.Add(new FilterStruct("rfc", this.rfcLoc, AvailableMetrics.RFC));
                    list.Add(new FilterStruct("noc", this.nocLoc, AvailableMetrics.NOC));
                    list.Add(new FilterStruct("dead", this.deadLoc, AvailableMetrics.DEAD));
                    list.Add(new FilterStruct("params", this.paramsLoc, AvailableMetrics.PARAMS));
                    list.Add(new FilterStruct("maxstack", this.maxstackLoc, AvailableMetrics.MAXSTACK));
                    list.Add(new FilterStruct("dup", this.dupLoc, AvailableMetrics.DUP));
                    list.Add(new FilterStruct("locals", this.localsLoc, AvailableMetrics.LOCALS));
                    list.Add(new FilterStruct("tryblocks", this.tryblocksLoc, AvailableMetrics.TRYBLOCKS));
                    list.Add(new FilterStruct("methods", this.methodsLoc, AvailableMetrics.METHODS));
                    list.Add(new FilterStruct("fields", this.fieldsLoc, AvailableMetrics.FIELDS));
                    list.Add(new FilterStruct("constructors", this.constructorsLoc, AvailableMetrics.CONSTRUCTORS));
                    list.Add(new FilterStruct("classes", this.classesLoc, AvailableMetrics.CLASSES));
                    list.Add(new FilterStruct("enumerations", this.enumerationsLoc, AvailableMetrics.ENUMERATIONS));
                    list.Add(new FilterStruct("structs", this.structsLoc, AvailableMetrics.STRUCTS));
                    list.Add(new FilterStruct("events", this.eventsLoc, AvailableMetrics.EVENTS));
                    list.Add(new FilterStruct("modules", this.modulesLoc, AvailableMetrics.MODULES));
                    list.Add(new FilterStruct("impinterfaces", this.impinterfacesLoc, AvailableMetrics.IMPLEMENTEDINTERFACES));
                    list.Add(new FilterStruct("interfaces", this.interfacesLoc, AvailableMetrics.INTERFACES));
                    list.Add(new FilterStruct("imptypes", this.imptypesLoc, AvailableMetrics.IMPTYPES));
                    list.Add(new FilterStruct("types", this.typesLoc, AvailableMetrics.TYPES));
                    list.Add(new FilterStruct("imps", this.impsLoc, AvailableMetrics.IMPS));
                    list.Add(new FilterStruct("loc", this.locLoc, AvailableMetrics.LOC));
                    list.Add(new FilterStruct("properties", this.propertiesLoc, AvailableMetrics.PROPERTIES));
                    list.Add(new FilterStruct("ecouplings", this.ecouplingsLoc, AvailableMetrics.ECOUPLINGS));
                    list.Add(new FilterStruct("acouplings", this.acouplingsLoc, AvailableMetrics.ACOUPLINGS));
                    list.Add(new FilterStruct("instability", this.instabilityLoc, AvailableMetrics.INSTABILITY));
                    list.Add(new FilterStruct("abstractness", this.abstractnessLoc, AvailableMetrics.ABSTRACTNESS));
                    list.Add(new FilterStruct("distance", this.distanceLoc, AvailableMetrics.DISTANCE));
                    list.Add(new FilterStruct("abstracts", this.abstractsLoc, AvailableMetrics.ABSTRACTS));
                    this.hasFilter = true;
                    rawFilter = "(" + rawFilter.ToLower() + ")";
                    StringBuilder builder = new StringBuilder();
                    foreach (object obj2 in list)
                    {
                        FilterStruct struct2 = (FilterStruct)obj2;
                        if (rawFilter.IndexOf(struct2.name) == -1)
                        {
                            continue;
                        }
                        if (struct2.location != -1)
                        {
                            goto Label_04BC;
                        }
                        AvailableMetrics metric = struct2.metric;
                        if (metric != AvailableMetrics.WMC)
                        {
                            switch (metric)
                            {
                                case AvailableMetrics.ACOUPLINGS:
                                    this.acouplingsLoc = this.curCounter;
                                    goto Label_0467;

                                case AvailableMetrics.INSTABILITY:
                                    this.instabilityLoc = this.curCounter;
                                    goto Label_0467;

                                case AvailableMetrics.ABSTRACTNESS:
                                    this.abstractnessLoc = this.curCounter;
                                    goto Label_0467;

                                case AvailableMetrics.DISTANCE:
                                    this.distanceLoc = this.curCounter;
                                    goto Label_0467;

                                case AvailableMetrics.NOC:
                                    goto Label_045B;
                            }
                        }
                        else
                        {
                            this.wmcLoc = this.curCounter;
                        }
                        goto Label_0467;
                    Label_045B:
                        this.nocLoc = this.curCounter;
                    Label_0467:
                        struct2.location = this.curCounter;
                        this.curCounter++;
                        AvailableMetrics[] destinationArray = new AvailableMetrics[this.curCounter];
                        Array.Copy(this.neededMetrics, 0, destinationArray, 0, this.curCounter - 1);
                        destinationArray[this.curCounter - 1] = struct2.metric;
                        this.neededMetrics = destinationArray;
                    Label_04BC:
                        rawFilter = rawFilter.Replace(struct2.name, " me.computedMetrics[" + struct2.location + "] ");
                        builder.Append(" &&  me.computedMetrics[" + struct2.location + "] != -1 ");
                    }
                    rawFilter = rawFilter + " && ( true " + builder.ToString() + ")";
                    return rawFilter;
                }
                this.hasFilter = false;
            }
            return "";
        }

        public void ProcessWMC()
        {
            if (Assembly.GetCallingAssembly().GetCustomAttributes(typeof(CommonIterator), false).Length != 0)
            {
                AvailableMetrics[] metricsArray;
                if ((this.wmcLoc != -1) && (this.ccLoc == -1))
                {
                    this.ccLoc = this.curCounter;
                    this.curCounter++;
                    metricsArray = new AvailableMetrics[this.curCounter];
                    Array.Copy(this.neededMetrics, 0, metricsArray, 0, this.curCounter - 1);
                    metricsArray[this.curCounter - 1] = AvailableMetrics.CC;
                    this.neededMetrics = metricsArray;
                }
                if ((this.distanceLoc != -1) && (this.abstractnessLoc == -1))
                {
                    this.abstractnessLoc = this.curCounter;
                    this.curCounter++;
                    metricsArray = new AvailableMetrics[this.curCounter];
                    Array.Copy(this.neededMetrics, 0, metricsArray, 0, this.curCounter - 1);
                    metricsArray[this.curCounter - 1] = AvailableMetrics.ABSTRACTNESS;
                    this.neededMetrics = metricsArray;
                }
                if ((this.distanceLoc != -1) && (this.instabilityLoc == -1))
                {
                    this.instabilityLoc = this.curCounter;
                    this.curCounter++;
                    metricsArray = new AvailableMetrics[this.curCounter];
                    Array.Copy(this.neededMetrics, 0, metricsArray, 0, this.curCounter - 1);
                    metricsArray[this.curCounter - 1] = AvailableMetrics.INSTABILITY;
                    this.neededMetrics = metricsArray;
                }
                if ((this.instabilityLoc != -1) && (this.ecouplingsLoc == -1))
                {
                    this.ecouplingsLoc = this.curCounter;
                    this.curCounter++;
                    metricsArray = new AvailableMetrics[this.curCounter];
                    Array.Copy(this.neededMetrics, 0, metricsArray, 0, this.curCounter - 1);
                    metricsArray[this.curCounter - 1] = AvailableMetrics.ECOUPLINGS;
                    this.neededMetrics = metricsArray;
                }
                if ((this.instabilityLoc != -1) && (this.acouplingsLoc == -1))
                {
                    this.acouplingsLoc = this.curCounter;
                    this.curCounter++;
                    metricsArray = new AvailableMetrics[this.curCounter];
                    Array.Copy(this.neededMetrics, 0, metricsArray, 0, this.curCounter - 1);
                    metricsArray[this.curCounter - 1] = AvailableMetrics.ACOUPLINGS;
                    this.neededMetrics = metricsArray;
                }
                if ((this.abstractnessLoc != -1) && (this.abstractsLoc == -1))
                {
                    this.abstractsLoc = this.curCounter;
                    this.curCounter++;
                    metricsArray = new AvailableMetrics[this.curCounter];
                    Array.Copy(this.neededMetrics, 0, metricsArray, 0, this.curCounter - 1);
                    metricsArray[this.curCounter - 1] = AvailableMetrics.ABSTRACTS;
                    this.neededMetrics = metricsArray;
                }
                if ((this.abstractnessLoc != -1) && (this.typesLoc == -1))
                {
                    this.typesLoc = this.curCounter;
                    this.curCounter++;
                    metricsArray = new AvailableMetrics[this.curCounter];
                    Array.Copy(this.neededMetrics, 0, metricsArray, 0, this.curCounter - 1);
                    metricsArray[this.curCounter - 1] = AvailableMetrics.TYPES;
                    this.neededMetrics = metricsArray;
                }
            }
        }

        public void SetLocs(int counter)
        {
            if (Assembly.GetCallingAssembly().GetCustomAttributes(typeof(CommonIterator), false).Length != 0)
            {
                switch (this.neededMetrics[counter])
                {
                    case AvailableMetrics.CC:
                        this.ccLoc = counter;
                        return;

                    case AvailableMetrics.LCOM:
                        this.lcomLoc = counter;
                        return;

                    case AvailableMetrics.WMC:
                        this.wmcLoc = counter;
                        return;

                    case AvailableMetrics.LOC:
                        this.locLoc = counter;
                        return;

                    case AvailableMetrics.CBO:
                        this.cboLoc = counter;
                        return;

                    case AvailableMetrics.DIT:
                        this.ditLoc = counter;
                        return;

                    case AvailableMetrics.RFC:
                        this.rfcLoc = counter;
                        return;

                    case AvailableMetrics.NOC:
                        this.nocLoc = counter;
                        return;

                    case AvailableMetrics.DEAD:
                        this.deadLoc = counter;
                        return;

                    case AvailableMetrics.PARAMS:
                        this.paramsLoc = counter;
                        return;

                    case AvailableMetrics.MAXSTACK:
                        this.maxstackLoc = counter;
                        return;

                    case AvailableMetrics.DUP:
                        this.dupLoc = counter;
                        return;

                    case AvailableMetrics.LOCALS:
                        this.localsLoc = counter;
                        return;

                    case AvailableMetrics.TRYBLOCKS:
                        this.tryblocksLoc = counter;
                        return;

                    case AvailableMetrics.METHODS:
                        this.methodsLoc = counter;
                        return;

                    case AvailableMetrics.FIELDS:
                        this.fieldsLoc = counter;
                        return;

                    case AvailableMetrics.CONSTRUCTORS:
                        this.constructorsLoc = counter;
                        return;

                    case AvailableMetrics.CLASSES:
                        this.classesLoc = counter;
                        return;

                    case AvailableMetrics.INTERFACES:
                        this.interfacesLoc = counter;
                        return;

                    case AvailableMetrics.ENUMERATIONS:
                        this.enumerationsLoc = counter;
                        return;

                    case AvailableMetrics.STRUCTS:
                        this.structsLoc = counter;
                        return;

                    case AvailableMetrics.EVENTS:
                        this.eventsLoc = counter;
                        return;

                    case AvailableMetrics.MODULES:
                        this.modulesLoc = counter;
                        return;

                    case AvailableMetrics.TYPES:
                        this.typesLoc = counter;
                        return;

                    case AvailableMetrics.IMPLEMENTEDINTERFACES:
                        this.impinterfacesLoc = counter;
                        return;

                    case AvailableMetrics.IMPS:
                        this.impsLoc = counter;
                        return;

                    case AvailableMetrics.IMPTYPES:
                        this.imptypesLoc = counter;
                        return;

                    case AvailableMetrics.ECOUPLINGS:
                        this.ecouplingsLoc = counter;
                        return;

                    case AvailableMetrics.ACOUPLINGS:
                        this.acouplingsLoc = counter;
                        return;

                    case AvailableMetrics.INSTABILITY:
                        this.instabilityLoc = counter;
                        return;

                    case AvailableMetrics.ABSTRACTNESS:
                        this.abstractnessLoc = counter;
                        return;

                    case AvailableMetrics.DISTANCE:
                        this.distanceLoc = counter;
                        return;

                    case AvailableMetrics.PROPERTIES:
                        this.propertiesLoc = counter;
                        return;

                    case AvailableMetrics.ABSTRACTS:
                        this.abstractsLoc = counter;
                        return;
                }
            }
        }

        public void SetSortColumn(string sortMetric)
        {
            if (Assembly.GetCallingAssembly().GetCustomAttributes(typeof(CommonIterator), false).Length != 0)
            {
                if (MetricUtils.metricLookup.ContainsKey(sortMetric.ToLower()))
                {
                    AvailableMetrics metrics = (AvailableMetrics)MetricUtils.metricLookup[sortMetric.ToLower()];
                    for (int i = 0; i < this.neededMetrics.Length; i++)
                    {
                        if (this.neededMetrics[i] == metrics)
                        {
                            MetricProvider.sortColumn = i;
                            return;
                        }
                    }
                    this.curCounter++;
                    AvailableMetrics[] destinationArray = new AvailableMetrics[this.curCounter];
                    Array.Copy(this.neededMetrics, 0, destinationArray, 0, this.curCounter - 1);
                    destinationArray[this.curCounter - 1] = metrics;
                    this.neededMetrics = destinationArray;
                    this.SetLocs(this.curCounter - 1);
                    MetricProvider.sortColumn = this.curCounter - 1;
                }
                else
                {
                    MetricProvider.sortColumn = -1;
                }
            }
        }
    }


}