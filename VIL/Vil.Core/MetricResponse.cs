using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Collections;
using System.Globalization;

namespace Vil.Core
{
    public class MetricResponse
    {
        // Fields
        public static Hashtable metricMap = new Hashtable();
        public static Hashtable metricMapShort = new Hashtable();
        internal MetricRequest request;
        public ArrayList resultSet;
        internal ArrayList resultSetRollUp;

        // Methods
        static MetricResponse()
        {
            metricMap[AvailableMetrics.CC] = "Cyclomatic<br>Complexity";
            metricMap[AvailableMetrics.LCOM] = "Lack of<br>Cohesion<br>of Methods";
            metricMap[AvailableMetrics.WMC] = "Weighted<br>Methods<br>per Class";
            metricMap[AvailableMetrics.LOC] = "Lines of<br>Code";
            metricMap[AvailableMetrics.CBO] = "Coupling<br>Between<br>Objects";
            metricMap[AvailableMetrics.DIT] = "Depth in<br>Tree";
            metricMap[AvailableMetrics.RFC] = "Response<br>for Class";
            metricMap[AvailableMetrics.NOC] = "Number of<br>Children";
            metricMap[AvailableMetrics.DEAD] = "Dead Code";
            metricMap[AvailableMetrics.PARAMS] = "Parameters";
            metricMap[AvailableMetrics.MAXSTACK] = "Maximum<br>Stack Depth";
            metricMap[AvailableMetrics.DUP] = "Duplicate<br>Code";
            metricMap[AvailableMetrics.LOCALS] = "Local<br>Variables";
            metricMap[AvailableMetrics.TRYBLOCKS] = "Try/Catch<br>Blocks";
            metricMap[AvailableMetrics.METHODS] = "Methods";
            metricMap[AvailableMetrics.FIELDS] = "Fields";
            metricMap[AvailableMetrics.CONSTRUCTORS] = "Constructors";
            metricMap[AvailableMetrics.CLASSES] = "Classes";
            metricMap[AvailableMetrics.INTERFACES] = "Interfaces";
            metricMap[AvailableMetrics.ENUMERATIONS] = "Enumerations";
            metricMap[AvailableMetrics.STRUCTS] = "Structs";
            metricMap[AvailableMetrics.EVENTS] = "Events";
            metricMap[AvailableMetrics.MODULES] = "Modules";
            metricMap[AvailableMetrics.TYPES] = "Types";
            metricMap[AvailableMetrics.IMPLEMENTEDINTERFACES] = "Implemented<br>Interfaces";
            metricMap[AvailableMetrics.IMPS] = "Implementations";
            metricMap[AvailableMetrics.IMPTYPES] = "Implementing<br>Types";
            metricMap[AvailableMetrics.ECOUPLINGS] = "Efferent<br>Couplings";
            metricMap[AvailableMetrics.ACOUPLINGS] = "Afferent<br>Couplings";
            metricMap[AvailableMetrics.INSTABILITY] = "Instability";
            metricMap[AvailableMetrics.ABSTRACTNESS] = "Abstractness";
            metricMap[AvailableMetrics.DISTANCE] = "Distance";
            metricMap[AvailableMetrics.PROPERTIES] = "Properties";
            metricMap[AvailableMetrics.ABSTRACTS] = "Abstract<br>Types";
            metricMapShort[AvailableMetrics.CC] = "CC";
            metricMapShort[AvailableMetrics.LCOM] = "LCOM";
            metricMapShort[AvailableMetrics.WMC] = "WMC";
            metricMapShort[AvailableMetrics.LOC] = "LOC";
            metricMapShort[AvailableMetrics.CBO] = "CBO";
            metricMapShort[AvailableMetrics.DIT] = "DIT";
            metricMapShort[AvailableMetrics.RFC] = "RFC";
            metricMapShort[AvailableMetrics.NOC] = "NOC";
            metricMapShort[AvailableMetrics.DEAD] = "Dead";
            metricMapShort[AvailableMetrics.PARAMS] = "Params";
            metricMapShort[AvailableMetrics.MAXSTACK] = "MaxStck";
            metricMapShort[AvailableMetrics.DUP] = "DupCode";
            metricMapShort[AvailableMetrics.LOCALS] = "Locals";
            metricMapShort[AvailableMetrics.TRYBLOCKS] = "TryBlks";
            metricMapShort[AvailableMetrics.METHODS] = "Methods";
            metricMapShort[AvailableMetrics.FIELDS] = "Fields";
            metricMapShort[AvailableMetrics.CONSTRUCTORS] = "Cnstrct";
            metricMapShort[AvailableMetrics.CLASSES] = "Classes";
            metricMapShort[AvailableMetrics.INTERFACES] = "Intrfcs";
            metricMapShort[AvailableMetrics.ENUMERATIONS] = "Enums";
            metricMapShort[AvailableMetrics.STRUCTS] = "Structs";
            metricMapShort[AvailableMetrics.EVENTS] = "Events";
            metricMapShort[AvailableMetrics.MODULES] = "Modules";
            metricMapShort[AvailableMetrics.TYPES] = "Types";
            metricMapShort[AvailableMetrics.IMPLEMENTEDINTERFACES] = "ImpInt";
            metricMapShort[AvailableMetrics.IMPS] = "Imps";
            metricMapShort[AvailableMetrics.IMPTYPES] = "ImpTyps";
            metricMapShort[AvailableMetrics.ECOUPLINGS] = "EffCoup";
            metricMapShort[AvailableMetrics.ACOUPLINGS] = "AffCoup";
            metricMapShort[AvailableMetrics.INSTABILITY] = "Instabl";
            metricMapShort[AvailableMetrics.ABSTRACTNESS] = "Abstnss";
            metricMapShort[AvailableMetrics.DISTANCE] = "Distnce";
            metricMapShort[AvailableMetrics.PROPERTIES] = "Propert";
            metricMapShort[AvailableMetrics.ABSTRACTS] = "Abstrct";
        }

        public MetricResponse(MetricRequest request)
        {
            this.request = request;
            this.resultSet = new ArrayList();
            this.resultSetRollUp = new ArrayList();
        }

        public static string ClipNameSpace(string name)
        {
            if (name.IndexOf("::") == -1)
            {
                if (name.LastIndexOf(".") == -1)
                {
                    return name;
                }
                return name.Substring(name.LastIndexOf(".") + 1);
            }
            string str = name.Substring(0, name.IndexOf("::"));
            if (str.LastIndexOf(".") == -1)
            {
                return name;
            }
            return name.Substring(str.LastIndexOf(".") + 1);
        }

        private static string GetCSS()
        {
            string location = Assembly.GetEntryAssembly().Location;
            location = location.Substring(0, 1 + location.LastIndexOf(Path.DirectorySeparatorChar.ToString())) + "vil.css";
            if (File.Exists(location))
            {
                try
                {
                    StreamReader reader = File.OpenText(location);
                    string str2 = reader.ReadToEnd();
                    reader.Close();
                    return ("<Style type='text/css' >" + str2 + "</style>");
                }
                catch (Exception)
                {
                    return "";
                }
            }
            return "";
        }

        public static Hashtable GetJudgementals()
        {
            string location = Assembly.GetEntryAssembly().Location;
            location = location.Substring(0, 1 + location.LastIndexOf(Path.DirectorySeparatorChar.ToString())) + "judge.vjf";
            if (File.Exists(location))
            {
                try
                {
                    NumberFormatInfo numberFormat = new CultureInfo("en-US", false).NumberFormat;
                    Hashtable hashtable = new Hashtable();
                    StreamReader reader = File.OpenText(location);
                    while (reader.Peek() >= 0)
                    {
                        string[] strArray = reader.ReadLine().Split(" \t".ToCharArray(), 5);
                        if (strArray[0].ToLower() == "consoleveryhigh")
                        {
                            hashtable["consoleveryhigh"] = int.Parse(strArray[1]);
                        }
                        else
                        {
                            if (strArray[0].ToLower() == "consolehigh")
                            {
                                hashtable["consolehigh"] = int.Parse(strArray[1]);
                                continue;
                            }
                            if (strArray[0].ToLower() == "imp")
                            {
                                judger judger = new judger();
                                judger.veryhigh = float.Parse(strArray[2], numberFormat);
                                if ((strArray.Length > 3) && (strArray[3] != null))
                                {
                                    judger.high = float.Parse(strArray[3], numberFormat);
                                }
                                else
                                {
                                    judger.high = 1E+07f;
                                }
                                hashtable[strArray[0].ToLower() + strArray[1].ToLower()] = judger;
                                continue;
                            }
                            if (strArray[0].ToLower() == "type")
                            {
                                judger judger2 = new judger();
                                judger2.veryhigh = float.Parse(strArray[2], numberFormat);
                                if ((strArray.Length > 3) && (strArray[3] != null))
                                {
                                    judger2.high = float.Parse(strArray[3], numberFormat);
                                }
                                else
                                {
                                    judger2.high = 1E+07f;
                                }
                                hashtable[strArray[0].ToLower() + strArray[1].ToLower()] = judger2;
                                continue;
                            }
                            if (strArray[0].ToLower() == "module")
                            {
                                judger judger3 = new judger();
                                judger3.veryhigh = float.Parse(strArray[2].Replace(",", "."), numberFormat);
                                if ((strArray.Length > 3) && (strArray[3] != null))
                                {
                                    judger3.high = float.Parse(strArray[3].Replace(",", "."), numberFormat);
                                }
                                else
                                {
                                    judger3.high = 1E+07f;
                                }
                                hashtable[strArray[0].ToLower() + strArray[1].ToLower()] = judger3;
                            }
                        }
                    }
                    reader.Close();
                    return hashtable;
                }
                catch (Exception)
                {
                    return new Hashtable();
                }
            }
            return new Hashtable();
        }

        private void MakeMeta(string charStart, StringBuilder stringBuilder, MetricProvider metricProvider, string sepchar, string otype)
        {
            if (this.request.visability)
            {
                stringBuilder.Append(charStart);
                if (otype == "xml")
                {
                    stringBuilder.Append("visibility=\"");
                }
                switch (metricProvider.visability)
                {
                    case Visability.Public:
                        stringBuilder.Append("public");
                        break;

                    case Visability.Private:
                        stringBuilder.Append("private");
                        break;

                    case Visability.Internal:
                        if (!this.request.trim)
                        {
                            stringBuilder.Append("internal");
                            break;
                        }
                        stringBuilder.Append("internl");
                        break;

                    case Visability.Protected:
                        if (!this.request.trim)
                        {
                            stringBuilder.Append("protected");
                            break;
                        }
                        stringBuilder.Append("protect");
                        break;

                    case Visability.ProtectedAndInternal:
                        if (!this.request.trim)
                        {
                            stringBuilder.Append("protected&internal");
                            break;
                        }
                        stringBuilder.Append("pro&int");
                        break;

                    case Visability.ProtectedOrInternal:
                        if (!this.request.trim)
                        {
                            stringBuilder.Append("protected|internal");
                            break;
                        }
                        stringBuilder.Append("pro|int");
                        break;

                    case Visability.NA:
                        stringBuilder.Append("-");
                        break;
                }
                stringBuilder.Append(sepchar);
            }
            if (this.request.location)
            {
                stringBuilder.Append(charStart);
                if (otype == "xml")
                {
                    stringBuilder.Append("static=\"");
                }
                switch (metricProvider.location)
                {
                    case Location.Instance:
                        if (!this.request.trim)
                        {
                            stringBuilder.Append("instance");
                            break;
                        }
                        stringBuilder.Append("instnce");
                        break;

                    case Location.Static:
                        stringBuilder.Append("static");
                        break;

                    case Location.NA:
                        stringBuilder.Append("-");
                        break;
                }
                stringBuilder.Append(sepchar);
            }
            if (this.request.codeElement)
            {
                stringBuilder.Append(charStart);
                if (otype == "xml")
                {
                    stringBuilder.Append("element=\"");
                }
                switch (metricProvider.codeElement)
                {
                    case CodeElement.Class:
                        stringBuilder.Append("class");
                        break;

                    case CodeElement.Interface:
                        if (!this.request.trim)
                        {
                            stringBuilder.Append("interface");
                            break;
                        }
                        stringBuilder.Append("intrfac");
                        break;

                    case CodeElement.Structure:
                        stringBuilder.Append("struct");
                        break;

                    case CodeElement.Enumeration:
                        stringBuilder.Append("enum");
                        break;

                    case CodeElement.Event:
                        stringBuilder.Append("event");
                        break;

                    case CodeElement.Field:
                        stringBuilder.Append("field");
                        break;

                    case CodeElement.Method:
                        stringBuilder.Append("method");
                        break;

                    case CodeElement.Assembly:
                        if (!this.request.trim)
                        {
                            stringBuilder.Append("assembly");
                            break;
                        }
                        stringBuilder.Append("assmbly");
                        break;

                    case CodeElement.Module:
                        stringBuilder.Append("module");
                        break;

                    case CodeElement.Constructor:
                        if (!this.request.trim)
                        {
                            stringBuilder.Append("constructor");
                            break;
                        }
                        stringBuilder.Append("constru");
                        break;
                }
                stringBuilder.Append(sepchar);
            }
            if (this.request.returnType)
            {
                stringBuilder.Append(charStart);
                if (otype == "xml")
                {
                    stringBuilder.Append("return=\"");
                }
                if (metricProvider.returnType != null)
                {
                    if (this.request.shortType)
                    {
                        stringBuilder.Append(metricProvider.returnType.Name);
                    }
                    else
                    {
                        stringBuilder.Append(metricProvider.returnType.FullName);
                    }
                }
                else
                {
                    stringBuilder.Append("-");
                }
                stringBuilder.Append(sepchar);
            }
            if (this.request.showModule)
            {
                stringBuilder.Append(charStart);
                if (otype == "xml")
                {
                    stringBuilder.Append("module=\"");
                }
                if (metricProvider.myModule != null)
                {
                    stringBuilder.Append(metricProvider.myModule.Name);
                }
                else
                {
                    stringBuilder.Append("-");
                }
                stringBuilder.Append(sepchar);
            }
            stringBuilder.Append(charStart);
            if (otype == "xml")
            {
                stringBuilder.Append("name=\"");
            }
            if ((otype.StartsWith("html") || (otype == "xml")) && (metricProvider.name.IndexOf("<") >= 0))
            {
                string name;
                stringBuilder.Append("");
                if ((this.request.shortType && (metricProvider.codeElement != CodeElement.Assembly)) && (metricProvider.codeElement != CodeElement.Module))
                {
                    name = ClipNameSpace(metricProvider.name);
                }
                else
                {
                    name = metricProvider.name;
                }
                name = name.Replace("<", "&lt;").Replace(">", "&gt;");
                stringBuilder.Append(name);
            }
            else if ((this.request.shortType && (metricProvider.codeElement != CodeElement.Assembly)) && (metricProvider.codeElement != CodeElement.Module))
            {
                stringBuilder.Append(ClipNameSpace(metricProvider.name));
            }
            else
            {
                stringBuilder.Append(metricProvider.name);
            }
            if (metricProvider.isImplementation)
            {
                stringBuilder.Append("(");
                string str2 = "";
                foreach (Type type in metricProvider.parameters)
                {
                    string fullName;
                    if (this.request.shortType)
                    {
                        fullName = type.Name;
                    }
                    else
                    {
                        fullName = type.FullName;
                    }
                    if (otype == "xml")
                    {
                        fullName = fullName.Replace("&", "&amp;");
                    }
                    stringBuilder.Append(str2 + fullName);
                    str2 = ",";
                }
                stringBuilder.Append(")");
            }
        }

        internal void RenderConsoleTextElement(int row, MetricProvider metricProvider, StringBuilder stringBuilder, string lineStart, string lineEnd, string sepchar, string charStart, string otype, Hashtable judgeHash)
        {
            bool flag = false;
            if (judgeHash.Count > 2)
            {
                flag = true;
            }
            if (otype == "console")
            {
                stringBuilder.Append(lineStart);
            }
            else if (otype == "xml")
            {
                stringBuilder.Append(lineStart);
                this.MakeMeta(" ", stringBuilder, metricProvider, "\"", otype);
                stringBuilder.Append("\">");
            }
            else if (row == 0)
            {
                stringBuilder.Append("<tr class='evenrow'>");
            }
            else
            {
                stringBuilder.Append("<tr class='oddrow'>");
            }
            float num = 0f;
            string str = "";
            if ((metricProvider.codeElement == CodeElement.Method) || (metricProvider.codeElement == CodeElement.Constructor))
            {
                str = "imp";
            }
            else if ((metricProvider.codeElement == CodeElement.Class) || (metricProvider.codeElement == CodeElement.Structure))
            {
                str = "type";
            }
            else if ((metricProvider.codeElement == CodeElement.Module) || (metricProvider.codeElement == CodeElement.Assembly))
            {
                str = "module";
            }
            else
            {
                flag = false;
            }
            if (flag && (((otype == "console") && judgeHash.ContainsKey("canLinCon")) || (otype.StartsWith("html") || (otype == "xml"))))
            {
                if (otype == "console")
                {
                    for (int i = 0; i < this.request.numRequested; i++)
                    {
                        num = metricProvider.computedMetrics[i];
                        string str2 = "";
                        if (judgeHash.ContainsKey(str + this.request.neededMetrics[i].ToString().ToLower()))
                        {
                            judger judger = (judger)judgeHash[str + this.request.neededMetrics[i].ToString().ToLower()];
                            if (num >= judger.veryhigh)
                            {
                                stringBuilder.Append("\x001b[" + ((int)judgeHash["consoleveryhigh"]) + "m");
                                str2 = "\x001b[m";
                            }
                            else if (num >= judger.high)
                            {
                                stringBuilder.Append("\x001b[" + ((int)judgeHash["consolehigh"]) + "m");
                                str2 = "\x001b[m";
                            }
                            else
                            {
                                stringBuilder.Append(charStart);
                            }
                        }
                        else
                        {
                            stringBuilder.Append(charStart);
                        }
                        if (num != -1f)
                        {
                            stringBuilder.Append(num.ToString() + str2 + sepchar);
                        }
                        else
                        {
                            stringBuilder.Append("-" + sepchar);
                        }
                    }
                }
                else if (otype == "xml")
                {
                    for (int j = 0; j < this.request.numRequested; j++)
                    {
                        num = metricProvider.computedMetrics[j];
                        if (judgeHash.ContainsKey(str + this.request.neededMetrics[j].ToString().ToLower()))
                        {
                            judger judger2 = (judger)judgeHash[str + this.request.neededMetrics[j].ToString().ToLower()];
                            if (num >= judger2.veryhigh)
                            {
                                stringBuilder.Append("<Metric level=\"veryhigh\"");
                            }
                            else if (num >= judger2.high)
                            {
                                stringBuilder.Append("<Metric level=\"high\"");
                            }
                            else
                            {
                                stringBuilder.Append(charStart);
                            }
                        }
                        else
                        {
                            stringBuilder.Append(charStart);
                        }
                        stringBuilder.Append(" name=\"" + this.request.neededMetrics[j].ToString() + "\"");
                        stringBuilder.Append(" label=\"" + ((string)metricMap[this.request.neededMetrics[j]]).Replace("<br>", " ") + "\">");
                        if (num != -1f)
                        {
                            stringBuilder.Append(num.ToString() + sepchar);
                        }
                        else
                        {
                            stringBuilder.Append("-" + sepchar);
                        }
                    }
                }
                else
                {
                    for (int k = 0; k < this.request.numRequested; k++)
                    {
                        num = metricProvider.computedMetrics[k];
                        if (judgeHash.ContainsKey(str + this.request.neededMetrics[k].ToString().ToLower()))
                        {
                            judger judger3 = (judger)judgeHash[str + this.request.neededMetrics[k].ToString().ToLower()];
                            if (num >= judger3.veryhigh)
                            {
                                stringBuilder.Append("<td class=veryhigh>");
                            }
                            else if (num >= judger3.high)
                            {
                                stringBuilder.Append("<td class=high>");
                            }
                            else
                            {
                                stringBuilder.Append(charStart);
                            }
                        }
                        else
                        {
                            stringBuilder.Append(charStart);
                        }
                        if (num != -1f)
                        {
                            stringBuilder.Append(num.ToString() + sepchar);
                        }
                        else
                        {
                            stringBuilder.Append("-" + sepchar);
                        }
                    }
                }
            }
            else
            {
                for (int m = 0; m < this.request.numRequested; m++)
                {
                    num = metricProvider.computedMetrics[m];
                    stringBuilder.Append(charStart);
                    if (otype == "xml")
                    {
                        stringBuilder.Append(" name=\"" + this.request.neededMetrics[m].ToString() + "\"");
                        stringBuilder.Append(" label=\"" + ((string)metricMap[this.request.neededMetrics[m]]).Replace("<br>", " ") + "\">");
                    }
                    if (num != -1f)
                    {
                        stringBuilder.Append(num.ToString() + sepchar);
                    }
                    else
                    {
                        stringBuilder.Append("-" + sepchar);
                    }
                }
            }
            if (otype != "xml")
            {
                this.MakeMeta(charStart, stringBuilder, metricProvider, sepchar, otype);
            }
            if (otype != "xml")
            {
                stringBuilder.Append(sepchar + lineEnd);
            }
            else
            {
                stringBuilder.Append(lineEnd + "\n");
            }
        }

        public string ToConsoleText(string otype, VARG varg, Hashtable judgeHash)
        {
            int num3;
            string sepchar = "";
            string str2 = "";
            string lineEnd = "";
            string charStart = "";
            string str5 = "";
            string str6 = "";
            string str7 = "";
            if (otype.StartsWith("html") && (judgeHash.Count > 2))
            {
                str7 = "<br><font size=2>(<span class=veryhigh>**</span> = very high number for this metric ; <span class=high>**</span> = high number for this metric)</font>";
            }
            if (otype == "console")
            {
                sepchar = "\t";
                lineEnd = "\n";
            }
            else if (otype == "html")
            {
                this.request.trim = false;
                sepchar = "</td>";
                lineEnd = "</tr>";
                str2 = "<tr>";
                charStart = "<td>";
                string str8 = "";
                if (varg.filter.Length > 0)
                {
                    str8 = "<br>&nbsp;&nbsp;&nbsp;&nbsp;Filtered by:  " + varg.filter;
                }
                string str9 = string.Concat(new object[] { "<table class='summary'><caption>", varg.reportName, " - ", DateTime.Now, str8, "</caption><tr><th>Assemblies</th><th>Metrics</th><th>Options</th></tr><tr><td class=sumcell>", varg.assemblies, "</td><td class=sumcell>", varg.metrics, "</td><td class=sumcell>", varg.options, "</td></tr></table><br><br>" });
                str5 = string.Concat(new object[] { "<html><head>", GetCSS(), "</head><body>", str9, "<table ><caption>", varg.reportName, " - ", DateTime.Now, str7, "</caption>" });
                string str10 = "";
                if (varg.copymessage.Length > 0)
                {
                    str10 = "<br><span class=copyright>" + varg.copymessage + "</span>";
                }
                str6 = "</table>" + str10 + "<body></html>";
            }
            else if (otype == "htmlshort")
            {
                this.request.trim = false;
                sepchar = "</td>";
                lineEnd = "</tr>";
                str2 = "<tr>";
                charStart = "<td>";
                str5 = string.Concat(new object[] { "<html><head>", GetCSS(), "</head><body><table ><caption>", varg.reportName, " - ", DateTime.Now, str7, "</caption>" });
                string str11 = "";
                if (varg.copymessage.Length > 0)
                {
                    str11 = "<br><span class=copyright>" + varg.copymessage + "</span>";
                }
                str6 = "</table>" + str11 + "<body></html>";
            }
            else if (otype == "htmltable")
            {
                this.request.trim = false;
                sepchar = "</td>";
                lineEnd = "</tr>";
                str2 = "<tr>";
                charStart = "<td>";
                str5 = string.Concat(new object[] { "<table ><caption>", varg.reportName, " - ", DateTime.Now, str7, "</caption>" });
                str6 = "</table>";
            }
            else if (otype == "xml")
            {
                this.request.trim = false;
                sepchar = "</Metric>";
                lineEnd = "</CodeElement>";
                str2 = "<CodeElement ";
                charStart = "<Metric ";
                string str12 = "";
                if (varg.copymessage.Length > 0)
                {
                    str12 = " copyright=\"" + varg.copymessage + "\"";
                }
                string str13 = "";
                if (varg.filter.Length > 0)
                {
                    str13 = " filter =\"" + varg.filter.Replace("&", "&amp;") + "\"";
                }
                StringBuilder builder = new StringBuilder();
                builder.Append("<Specification><Assemblies>");
                string[] strArray = varg.assemblies.Replace("<br>", "`").Split("`".ToCharArray());
                for (num3 = 0; num3 < strArray.Length; num3++)
                {
                    string str14 = strArray[num3];
                    if (str14.Trim().Length > 0)
                    {
                        builder.Append("<Assembly>" + str14 + "</Assembly>");
                    }
                }
                builder.Append("</Assemblies>");
                builder.Append("<RequestedMetrics>");
                strArray = varg.metrics.Replace("<br>", "`").Split("`".ToCharArray());
                for (num3 = 0; num3 < strArray.Length; num3++)
                {
                    string str15 = strArray[num3];
                    if (str15.Trim().Length > 0)
                    {
                        builder.Append("<RequestedMetric>" + str15 + "</RequestedMetric>");
                    }
                }
                builder.Append("</RequestedMetrics>");
                builder.Append("<Options>");
                strArray = varg.options.Replace("<br>", "`").Split("`".ToCharArray());
                for (num3 = 0; num3 < strArray.Length; num3++)
                {
                    string str16 = strArray[num3];
                    if (str16.Trim().Length > 0)
                    {
                        builder.Append("<Option>" + str16 + "</Option>");
                    }
                }
                builder.Append("</Options>");
                builder.Append("</Specification>");
                str5 = string.Concat(new object[] { "<MetricReport name=\"", varg.reportName, "\" date=\"", DateTime.Now, "\"", str13, str12, ">", builder.ToString(), "<Results>" });
                str6 = "</Results></MetricReport>";
            }
            if (Assembly.GetCallingAssembly().GetCustomAttributes(typeof(CommonIterator), false).Length == 0)
            {
                return "";
            }
            StringBuilder stringBuilder = new StringBuilder();
            int num = 0;
            stringBuilder.Append(str5);
            if (otype != "xml")
            {
                stringBuilder.Append(str2);
            }
            AvailableMetrics[] neededMetrics = this.request.neededMetrics;
            for (num3 = 0; num3 < neededMetrics.Length; num3++)
            {
                object obj2 = neededMetrics[num3];
                AvailableMetrics metrics = (AvailableMetrics)obj2;
                if (otype == "console")
                {
                    if (this.request.trim)
                    {
                        stringBuilder.Append(charStart + ((string)metricMapShort[metrics]) + sepchar);
                    }
                    else
                    {
                        stringBuilder.Append(charStart + metrics.ToString() + sepchar);
                    }
                }
                else if (!(otype == "xml"))
                {
                    stringBuilder.Append("<th>" + ((string)metricMap[metrics]) + "</th>");
                }
                num++;
                if (num == this.request.numRequested)
                {
                    break;
                }
            }
            if (otype == "console")
            {
                if (this.request.visability)
                {
                    stringBuilder.Append(charStart + "VISIBLE" + sepchar);
                }
                if (this.request.location)
                {
                    stringBuilder.Append(charStart + "STATIC" + sepchar);
                }
                if (this.request.codeElement)
                {
                    stringBuilder.Append(charStart + "ELEMENT" + sepchar);
                }
                if (this.request.returnType)
                {
                    stringBuilder.Append(charStart + "RETURN" + sepchar);
                }
                if (this.request.showModule)
                {
                    stringBuilder.Append(charStart + "MODULE" + sepchar);
                }
                stringBuilder.Append(charStart + "NAME" + sepchar + lineEnd);
            }
            else if (otype.StartsWith("html"))
            {
                if (this.request.visability)
                {
                    stringBuilder.Append("<th>Visibility</th>");
                }
                if (this.request.location)
                {
                    stringBuilder.Append("<th>Static</th>");
                }
                if (this.request.codeElement)
                {
                    stringBuilder.Append("<th>Element</th>");
                }
                if (this.request.returnType)
                {
                    stringBuilder.Append("<th>Return</th>");
                }
                if (this.request.showModule)
                {
                    stringBuilder.Append("<th>Module</th>");
                }
                stringBuilder.Append("<th>Name</th>" + lineEnd);
            }
            int num2 = 0;
            foreach (object obj3 in this.resultSet)
            {
                num2++;
                this.RenderConsoleTextElement(num2 % 2, (MetricProvider)obj3, stringBuilder, str2, lineEnd, sepchar, charStart, otype, judgeHash);
            }
            stringBuilder.Append(str6);
            return stringBuilder.ToString();
        }

        internal string ToHTML()
        {
            return "<html/>";
        }

        internal string ToXML()
        {
            return "<xml/>";
        }
    }


}