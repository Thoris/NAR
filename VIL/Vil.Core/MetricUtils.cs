using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Vil.Core
{
    public class MetricUtils
    {
        // Fields
        public static Hashtable metricLookup = new Hashtable();

        // Methods
        static MetricUtils()
        {
            metricLookup["cc"] = AvailableMetrics.CC;
            metricLookup["lcom"] = AvailableMetrics.LCOM;
            metricLookup["wmc"] = AvailableMetrics.WMC;
            metricLookup["loc"] = AvailableMetrics.LOC;
            metricLookup["cbo"] = AvailableMetrics.CBO;
            metricLookup["dit"] = AvailableMetrics.DIT;
            metricLookup["noc"] = AvailableMetrics.NOC;
            metricLookup["rfc"] = AvailableMetrics.RFC;
            metricLookup["dead"] = AvailableMetrics.DEAD;
            metricLookup["params"] = AvailableMetrics.PARAMS;
            metricLookup["maxstack"] = AvailableMetrics.MAXSTACK;
            metricLookup["dup"] = AvailableMetrics.DUP;
            metricLookup["locals"] = AvailableMetrics.LOCALS;
            metricLookup["tryblocks"] = AvailableMetrics.TRYBLOCKS;
            metricLookup["methods"] = AvailableMetrics.METHODS;
            metricLookup["fields"] = AvailableMetrics.FIELDS;
            metricLookup["constructors"] = AvailableMetrics.CONSTRUCTORS;
            metricLookup["classes"] = AvailableMetrics.CLASSES;
            metricLookup["interfaces"] = AvailableMetrics.INTERFACES;
            metricLookup["enumerations"] = AvailableMetrics.ENUMERATIONS;
            metricLookup["structs"] = AvailableMetrics.STRUCTS;
            metricLookup["events"] = AvailableMetrics.EVENTS;
            metricLookup["modules"] = AvailableMetrics.MODULES;
            metricLookup["types"] = AvailableMetrics.TYPES;
            metricLookup["imptypes"] = AvailableMetrics.IMPTYPES;
            metricLookup["imps"] = AvailableMetrics.IMPS;
            metricLookup["impinterfaces"] = AvailableMetrics.IMPLEMENTEDINTERFACES;
            metricLookup["acouplings"] = AvailableMetrics.ACOUPLINGS;
            metricLookup["ecouplings"] = AvailableMetrics.ECOUPLINGS;
            metricLookup["properties"] = AvailableMetrics.PROPERTIES;
            metricLookup["instability"] = AvailableMetrics.INSTABILITY;
            metricLookup["abstractness"] = AvailableMetrics.ABSTRACTNESS;
            metricLookup["distance"] = AvailableMetrics.DISTANCE;
            metricLookup["abstracts"] = AvailableMetrics.ABSTRACTS;
        }
    }




}