using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.IO;
using Vil.Core;
using Reflector.Disassembler;

namespace VIL
{
internal class CommandLineEntry
{
    // Fields
    private static string cmess = "Vil - version 1.1 Copyright (C) www.1bot.com 2003, 2004. All rights reserved.";
    private static int err = 10;
    private static string thever = "1.1";
    private static string verdate = "632237052028526608";

    // Methods
    private static Assembly[] GetAssembly(Arguments commandLine)
    {
        Assembly assembly = null;
        string str = "";
        if (commandLine["assembly"] != null)
        {
            str = commandLine["assembly"];
        }
        else if (commandLine["a"] != null)
        {
            str = commandLine["a"];
        }
        string[] strArray = str.Split(",".ToCharArray());
        int num = 0;
        ArrayList list = new ArrayList();
        foreach (string str2 in strArray)
        {
            string str3 = "";
            bool flag = false;
            bool flag2 = false;
            if (str2.ToLower().EndsWith("*.dll"))
            {
                flag2 = true;
            }
            if (str2.ToLower().EndsWith("*.exe"))
            {
                flag = true;
            }
            if (str2.ToLower().EndsWith("*.net"))
            {
                flag2 = true;
                flag = true;
            }
            string path = str2;
            ArrayList list2 = new ArrayList();
            if (!Path.IsPathRooted(path))
            {
                string currentDirectory = Directory.GetCurrentDirectory();
                if (path.StartsWith("~"))
                {
                    string environmentVariable = Environment.GetEnvironmentVariable("HOME");
                    if (environmentVariable == null)
                    {
                        Console.Error.WriteLine("Warning : " + path + " was not resolvable.  Not loaded.");
                        goto Label_056B;
                    }
                    currentDirectory = environmentVariable;
                    path = path.Substring(1);
                }
                if (path.StartsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    path = path.Substring(1);
                }
                if (!currentDirectory.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    currentDirectory = currentDirectory + Path.DirectorySeparatorChar.ToString();
                }
                int num2 = path.LastIndexOf(Path.DirectorySeparatorChar.ToString());
                if (num2 > -1)
                {
                    currentDirectory = Path.Combine(currentDirectory, path.Substring(0, num2 + 1));
                    path = path.Substring(num2 + 1);
                }
                int length = path.LastIndexOf("*");
                if (length > 0)
                {
                    str3 = path.Substring(0, length);
                }
                if (flag2)
                {
                    foreach (string str7 in Directory.GetFiles(currentDirectory, "*.dll"))
                    {
                        if (str3.Length > 0)
                        {
                            if (str7.Substring(currentDirectory.Length).StartsWith(str3))
                            {
                                list2.Add(str7);
                            }
                        }
                        else
                        {
                            list2.Add(str7);
                        }
                    }
                }
                if (flag)
                {
                    foreach (string str8 in Directory.GetFiles(currentDirectory, "*.exe"))
                    {
                        if (str3.Length > 0)
                        {
                            if (str8.Substring(currentDirectory.Length).StartsWith(str3))
                            {
                                list2.Add(str8);
                            }
                        }
                        else
                        {
                            list2.Add(str8);
                        }
                    }
                }
                if (!flag && !flag2)
                {
                    string str9 = Path.Combine(currentDirectory, path);
                    list2.Add(str9);
                }
            }
            else
            {
                string str10 = str2.Substring(0, str2.LastIndexOf(Path.DirectorySeparatorChar.ToString()) + 1);
                if (str10.StartsWith(@"\"))
                {
                    str10 = Path.GetPathRoot(Directory.GetCurrentDirectory()) + str10.Substring(1);
                }
                if (!str10.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    str10 = str10 + Path.DirectorySeparatorChar.ToString();
                }
                string str11 = str2.Substring(str2.LastIndexOf(Path.DirectorySeparatorChar.ToString()) + 1);
                int num4 = str11.LastIndexOf("*");
                if (num4 > 0)
                {
                    str3 = str11.Substring(0, num4);
                }
                if (flag2)
                {
                    foreach (string str12 in Directory.GetFiles(str10, "*.dll"))
                    {
                        if (str3.Length > 0)
                        {
                            if (str12.Substring(str10.Length).StartsWith(str3))
                            {
                                list2.Add(str12);
                            }
                        }
                        else
                        {
                            list2.Add(str12);
                        }
                    }
                }
                if (flag)
                {
                    foreach (string str13 in Directory.GetFiles(str10, "*.exe"))
                    {
                        if (str3.Length > 0)
                        {
                            if (str13.Substring(str10.Length).StartsWith(str3))
                            {
                                list2.Add(str13);
                            }
                        }
                        else
                        {
                            list2.Add(str13);
                        }
                    }
                }
                if (!flag && !flag2)
                {
                    path = Path.Combine(str10, str11);
                    list2.Add(path);
                }
            }
            foreach (object obj2 in list2)
            {
                path = (string) obj2;
                try
                {
                    assembly = new AssemblyGiver().LoadAssembly(path);
                    if (assembly.GetCustomAttributes(typeof(CommonIterator), false).Length > 0)
                    {
                        Console.Error.WriteLine("Not Loaded: " + path + " is part of vil, and vil is not able to look at itself.");
                    }
                    else
                    {
                        list.Add(assembly);
                        num++;
                    }
                    continue;
                }
                catch (BadImageFormatException)
                {
                    Console.Error.WriteLine("Not Loaded: " + path + " is not a valid .Net assembly");
                    continue;
                }
                catch (FileNotFoundException)
                {
                    Console.Error.WriteLine("ERROR: The assembly ( " + path + " ) cannot be found!");
                    continue;
                }
                catch (FileLoadException)
                {
                    Console.Error.WriteLine("Warning: The assembly ( " + path + " ) could not be loaded as an assembly!");
                    continue;
                }
                catch (Exception exception)
                {
                    Console.Error.WriteLine(exception.ToString());
                    continue;
                }
            }
        Label_056B:;
        }
        Assembly[] assemblyArray = new Assembly[list.Count];
        int num5 = 0;
        foreach (object obj3 in list)
        {
            assemblyArray[num5++] = (Assembly) obj3;
        }
        return assemblyArray;
    }

    private static bool il()
    {
        //return true;


        string location = Assembly.GetEntryAssembly().Location;
        location = location.Substring(0, 1 + location.LastIndexOf(Path.DirectorySeparatorChar.ToString())) + "vil.lic";
        string str2 = null;
        if (File.Exists(location))
        {
            try
            {
                StreamReader reader = File.OpenText(location);
                //
                // str4 = s1.1
                //
                string str4 = reader.ReadLine();
                reader.Close();

                //str4 = "v1.1 MY3nO9+ThIU5cTfnSYhaHRT5ioH6phUEi5X50kF2z04=";

                //
                // strArray = 
                //
                string[] strArray = str4.Split(" ".ToCharArray(), 2);
                //
                // str2 = v1.1
                //
                str2 = strArray[0].Trim();
                //
                // str5 =  opened the door, and everything changed - forever!
                //

                //string test = EncDec.Encrypt("1000 s1.1", str2 + " opened the door, and everything changed - forever!");

                string str5 = EncDec.Decrypt(strArray[1].Trim(), str2 + " opened the door, and everything changed - forever!");
                //
                // s = 1000
                //
                string s = str5.Split(" ".ToCharArray(), 2)[0].Trim();
                //
                // str7 = s1.1
                //
                string str7 = str5.Split(" ".ToCharArray(), 2)[1].Trim();
                long ticks = long.Parse(s);
                DateTime time = new DateTime(ticks);
                if (!str2.StartsWith("v"))
                {
                    return false;
                }
                if (str7.StartsWith("t"))
                {
                    if (DateTime.Now.Ticks > time.AddDays(15.0).Ticks)
                    {
                        return false;
                    }
                    long num2 = (time.AddDays(15.0).Ticks - DateTime.Now.Ticks) / 0xc92a69c000L;
                    if (num2 > 0L)
                    {
                        Console.Error.WriteLine("!! The license for use with 100 or more types will expire in " + num2 + " days.");
                    }
                    else
                    {
                        Console.Error.WriteLine("!! The license for use with 100 or more types will expire in less than 1 day.");
                    }
                    return true;
                }
                if (str7.StartsWith("m"))
                {
                    return (time.AddYears(int.Parse(str7.Substring(1, 1))).Ticks > long.Parse(verdate));
                }
                return (str7.StartsWith("s") && (str7.Substring(1) == thever));
            }
            catch (Exception)
            {
                return false;
            }
        }
        return false;
    }

    public static void Main(string[] args)
    {
        string str = "win";
        if (Environment.OSVersion.ToString().ToLower().IndexOf("win") == -1)
        {
            str = "nix";
        }
        try
        {
            bool flag2;
            bool flag3;
            Assembly[] assemblyArray;
            string[] strArray;
            AvailableMetrics[] metricsArray;
            MetricRequest request;
            object obj14;
            bool flag = false;
            string str2 = "";
            string str3 = "";
            string str4 = "";
            string str5 = "";
            Arguments commandLine = new Arguments(args);
            if ((commandLine["nix"] != null) && (commandLine["nojudge"] != null))
            {
                Console.Error.WriteLine("ERROR: A request cannot have both /nix and /nojudge specified.");
            }
            else if ((commandLine["nix"] != null) && (str == "nix"))
            {
                Console.Error.WriteLine("WARNING: The /nix option is not needed when running on a *nix platform.");
            }
            else
            {
                if ((commandLine["outhtml"] != null) || (commandLine["outxml"] != null))
                {
                    flag = true;
                }
                if ((((args.Length == 0) || ((commandLine["assembly"] == null) && (commandLine["a"] == null))) || ((commandLine["assembly"] != null) && (commandLine["assembly"].ToString() == "true"))) || ((commandLine["a"] != null) && (commandLine["a"].ToString() == "true")))
                {
                    Console.Error.WriteLine("ERROR: No assembly was specified!\n");
                }
                else if ((commandLine["assembly"] != null) && (commandLine["a"] != null))
                {
                    Console.Error.WriteLine("ERROR: A request cannot have multiple assembly declarations!");
                }
                else
                {
                    flag2 = false;
                    flag3 = false;
                    assemblyArray = GetAssembly(commandLine);
                    if (assemblyArray.Length == 0)
                    {
                        Console.Error.WriteLine("ERROR: No valid assemblies were specified!\n");
                    }
                    else
                    {
                        if (flag)
                        {
                            foreach (Assembly assembly in assemblyArray)
                            {
                                str3 = str3 + assembly.Location + "<br>";
                            }
                        }
                        if (commandLine["dup"] != null)
                        {
                            Duplication duplication = new Duplication();
                            duplication.ProcessAssembly(assemblyArray[0]);
                            duplication.RenderAsText();
                        }
                        else if (commandLine["il"] != null)
                        {
                            new InstructionDumper().ProcessAssembly(assemblyArray[0]);
                        }
                        else if (commandLine["stack"] != null)
                        {
                            new InstructionDumper().ProcessAssemblyStacks(assemblyArray[0]);
                        }
                        else if ((commandLine["metrics"] != null) && (commandLine["m"] != null))
                        {
                            Console.Error.WriteLine("ERROR: A request cannot have multiple metrics declarations!");
                        }
                        else
                        {
                            string str6 = null;
                            if (commandLine["metrics"] != null)
                            {
                                str6 = commandLine["metrics"];
                            }
                            if (commandLine["m"] != null)
                            {
                                str6 = commandLine["m"];
                            }
                            if (str6 != null)
                            {
                                strArray = str6.Split(",".ToCharArray());
                            }
                            else
                            {
                                strArray = new string[0];
                            }
                            metricsArray = new AvailableMetrics[strArray.Length];
                            request = new MetricRequest();
                            if (commandLine["test"] != null)
                            {
                                request.ignoreTestFixtures = false;
                                if (flag)
                                {
                                    str4 = str4 + "omit unit tests<br>";
                                }
                            }
                            if ((commandLine["scope"] != null) && (commandLine["sc"] != null))
                            {
                                Console.Error.WriteLine("Error: A request cannot have multiple scope declarations.");
                            }
                            else
                            {
                                string str7 = null;
                                if (commandLine["scope"] != null)
                                {
                                    str7 = commandLine["scope"];
                                }
                                if (commandLine["sc"] != null)
                                {
                                    str7 = commandLine["sc"];
                                }
                                if (str7 == null)
                                {
                                    goto Label_0593;
                                }
                                switch (str7.ToLower())
                                {
                                    case "all":
                                        request.scope = RequestScope.All;
                                        goto Label_0553;

                                    case "type":
                                        request.scope = RequestScope.Type;
                                        goto Label_0553;

                                    case "method":
                                        request.scope = RequestScope.Method;
                                        goto Label_0553;

                                    case "assembly":
                                        request.scope = RequestScope.Assembly;
                                        goto Label_0553;

                                    case "imptype":
                                        request.scope = RequestScope.ImpType;
                                        goto Label_0553;

                                    case "imp":
                                        request.scope = RequestScope.Imp;
                                        goto Label_0553;

                                    case "class":
                                        request.scope = RequestScope.Class;
                                        goto Label_0553;

                                    case "interface":
                                        request.scope = RequestScope.Interface;
                                        goto Label_0553;

                                    case "struct":
                                        request.scope = RequestScope.Struct;
                                        goto Label_0553;

                                    case "enumeration":
                                        request.scope = RequestScope.Enumeration;
                                        goto Label_0553;

                                    case "constructor":
                                        request.scope = RequestScope.Constructor;
                                        goto Label_0553;
                                }
                                Console.Error.WriteLine("Invalid Scope Declaration: scopes are 'all','assembly','type','method', 'imp', 'imptype', 'class', 'enumeration', 'interface', 'struct', 'constructor', and 'default'!");
                            }
                        }
                    }
                }
            }
            return;
        Label_0553:
            if (flag)
            {
                obj14 = str4;
                str4 = string.Concat(new object[] { obj14, "scope = ", request.scope, "<br>" });
            }
        Label_0593:
            request.neededMetrics = metricsArray;
            bool flag4 = false;
            int index = 0;
            foreach (string str8 in strArray)
            {
                if (MetricUtils.metricLookup.ContainsKey(str8.ToLower()))
                {
                    AvailableMetrics metrics = (AvailableMetrics) MetricUtils.metricLookup[str8.ToLower()];
                    request.neededMetrics[index] = metrics;
                    request.SetLocs(index);
                    if (flag)
                    {
                        str5 = str5 + ((string) MetricResponse.metricMap[metrics]).Replace("<br>", " ") + "<br>";
                    }
                }
                else
                {
                    Console.Error.WriteLine("Error: Invalid Metrics Declaration - '" + str8 + "' is not recognized!");
                    return;
                }
                index++;
                flag4 = true;
            }
            request.numRequested = request.neededMetrics.Length;
            request.curCounter = request.numRequested;
            if ((commandLine["module"] != null) || (commandLine["mo"] != null))
            {
                request.showModule = true;
                if (flag)
                {
                    str4 = str4 + "show module<br>";
                }
            }
            if (commandLine["visibility"] != null)
            {
                request.visability = true;
                if (flag)
                {
                    str4 = str4 + "show visibility<br>";
                }
            }
            if (commandLine["static"] != null)
            {
                request.location = true;
                if (flag)
                {
                    str4 = str4 + "show static/instance<br>";
                }
            }
            if (commandLine["return"] != null)
            {
                request.returnType = true;
                if (flag)
                {
                    str4 = str4 + "show return type<br>";
                }
            }
            if ((commandLine["element"] != null) || (commandLine["e"] != null))
            {
                request.codeElement = true;
                if (flag)
                {
                    str4 = str4 + "show code element<br>";
                }
            }
            if (commandLine["notrim"] != null)
            {
                request.trim = false;
                if (flag)
                {
                    str4 = str4 + "don't trim text for console<br>";
                }
            }
            else
            {
                request.trim = true;
            }
            if (commandLine["long"] != null)
            {
                request.shortType = false;
                if (flag)
                {
                    str4 = str4 + "show long type names<br>";
                }
            }
            else
            {
                request.shortType = true;
            }
            if (commandLine["sorta"] != null)
            {
                request.SetSortColumn(commandLine["sorta"]);
                if (flag)
                {
                    str4 = str4 + "sort ascending by ";
                }
                flag2 = true;
            }
            if ((commandLine["sort"] != null) && (commandLine["s"] != null))
            {
                Console.Error.WriteLine("Error: A request cannot have multiple sort declarations.");
            }
            else if ((commandLine["sortd"] != null) && (commandLine["s"] != null))
            {
                Console.Error.WriteLine("Error: A request cannot have multiple sort declarations.");
            }
            else if ((commandLine["sort"] != null) && (commandLine["sortd"] != null))
            {
                Console.Error.WriteLine("Error: A request cannot have multiple sort declarations.");
            }
            else
            {
                if (commandLine["sort"] != null)
                {
                    request.SetSortColumn(commandLine["sort"]);
                    if (flag)
                    {
                        str4 = str4 + "sort descending by ";
                    }
                    flag3 = true;
                }
                if (commandLine["s"] != null)
                {
                    request.SetSortColumn(commandLine["s"]);
                    if (flag)
                    {
                        str4 = str4 + "sort descending by ";
                    }
                    flag3 = true;
                }
                if (commandLine["sortd"] != null)
                {
                    request.SetSortColumn(commandLine["sortd"]);
                    if (flag)
                    {
                        str4 = str4 + "sort descending by ";
                    }
                    flag3 = true;
                }
                if (MetricProvider.sortColumn == -1)
                {
                    Console.Error.WriteLine("Invalid Metric in Sort Declaration!");
                }
                else if (flag2 && flag3)
                {
                    Console.Error.WriteLine("ERROR: A request cannot have 2 sort declarations!");
                }
                else if ((flag2 || flag3) && !flag4)
                {
                    Console.Error.WriteLine("ERROR: A request cannot have sort declarations without a metrics declaration!");
                }
                else
                {
                    if ((flag2 || flag3) && flag)
                    {
                        str4 = str4 + ((string) MetricResponse.metricMap[request.neededMetrics[MetricProvider.sortColumn]]) + "<br>";
                    }
                    bool flag5 = false;
                    bool flag6 = false;
                    int num2 = 15;
                    int num3 = 15;
                    try
                    {
                        if ((commandLine["head"] != null) && (commandLine["h"] != null))
                        {
                            Console.Error.WriteLine("ERROR: A request cannot have multiple head declarations!");
                            return;
                        }
                        if (commandLine["head"] != null)
                        {
                            string s = commandLine["head"].Trim();
                            if ((s.Length != 0) && (s != "true"))
                            {
                                num2 = int.Parse(s);
                                if (num2 < 1)
                                {
                                    Console.Error.WriteLine("ERROR: Invalid \"Head\" declaration!");
                                    return;
                                }
                            }
                            obj14 = str4;
                            str4 = string.Concat(new object[] { obj14, "show first ", num2, " items<br>" });
                            flag5 = true;
                        }
                        if (commandLine["h"] != null)
                        {
                            string str10 = commandLine["h"].Trim();
                            if ((str10.Length != 0) && (str10 != "true"))
                            {
                                num2 = int.Parse(str10);
                                if (num2 < 1)
                                {
                                    Console.Error.WriteLine("ERROR: Invalid \"Head\" declaration!");
                                    return;
                                }
                            }
                            obj14 = str4;
                            str4 = string.Concat(new object[] { obj14, "show first ", num2, " items<br>" });
                            flag5 = true;
                        }
                    }
                    catch (Exception)
                    {
                        Console.Error.WriteLine("ERROR: Invalid \"Head\" declaration!");
                        return;
                    }
                    try
                    {
                        if (commandLine["tail"] != null)
                        {
                            string str11 = commandLine["tail"].Trim();
                            if ((str11.Length != 0) && (str11 != "true"))
                            {
                                num3 = int.Parse(str11);
                                if (num3 < 1)
                                {
                                    Console.Error.WriteLine("ERROR: Invalid \"Tail\" declaration!");
                                    return;
                                }
                            }
                            obj14 = str4;
                            str4 = string.Concat(new object[] { obj14, "show last ", num3, " items<br>" });
                            flag6 = true;
                        }
                    }
                    catch (Exception)
                    {
                        Console.Error.WriteLine("ERROR: Invalid \"Tail\" declaration!");
                        return;
                    }
                    if (flag5 && flag6)
                    {
                        Console.Error.WriteLine("ERROR: A request cannot have both \"head\" and \"tail\" declarations!");
                    }
                    else
                    {
                        string expression = "";
                        string str13 = "";
                        if ((commandLine["filter"] != null) && (commandLine["f"] != null))
                        {
                            Console.Error.WriteLine("ERROR: A request cannot have multiple filter declarations!");
                        }
                        else
                        {
                            if (commandLine["filter"] != null)
                            {
                                str13 = commandLine["filter"];
                                expression = request.ProcessFilter(commandLine["filter"]);
                            }
                            if (commandLine["f"] != null)
                            {
                                str13 = commandLine["f"];
                                expression = request.ProcessFilter(commandLine["f"]);
                            }
                            if (request.hasFilter)
                            {
                                str2 = str13;
                            }
                            request.ProcessWMC();
                            MetricResponse response = new MetricResponse(request);
                            Hashtable judgeHash = new Hashtable();
                            bool flag7 = false;
                            if ((commandLine["nojudge"] == null) && ((str == "nix") || ((str == "win") && (commandLine["nix"] != null))))
                            {
                                flag7 = true;
                            }
                            if (commandLine["nojudge"] == null)
                            {
                                judgeHash = MetricResponse.GetJudgementals();
                                if (judgeHash.Count == 0)
                                {
                                    Console.Error.WriteLine("Warning: There was a problem reading 'judge.vjf'.  Judging will be disabled for this report!");
                                }
                                else if (flag7)
                                {
                                    judgeHash["canLinCon"] = 0x37;
                                }
                            }
                            int num4 = 0x4b;
                            MultiAssemblyMetrics metrics2 = new MultiAssemblyMetrics(assemblyArray);
                            if ((metrics2.NumberOfTypes() > ((num4 + err) + 15)) && !il())
                            {
                                Console.Error.WriteLine("!!! This instance of vil is not licensed to perform analysis on more \n!!! than 100 Types(classes, structs, enumerations, or interfaces) at a time.\n!!! Please go to www.1bot.com to acquire a valid license key.\n");
                            }
                            else
                            {
                                try
                                {
                                    metrics2.ProvideMetrics(request, response);
                                }
                                catch (FileNotFoundException exception)
                                {
                                    Console.Error.WriteLine("ERROR: A dependent assembly(\"" + exception.FileName + "\") could not be found.\nPlease make sure you are running Vil against assemblies that have access to their dependent assemblies!");
                                    return;
                                }
                                catch (ModuleReaderException exception2)
                                {
                                    Console.Error.WriteLine("ERROR: (\"" + exception2.Message + "\")");
                                    return;
                                }
                                if (request.scope == RequestScope.Assembly)
                                {
                                    ArrayList list = new ArrayList();
                                    foreach (object obj2 in response.resultSet)
                                    {
                                        if (((MetricProvider) obj2).codeElement == CodeElement.Assembly)
                                        {
                                            list.Add(obj2);
                                        }
                                    }
                                    response.resultSet = list;
                                }
                                else if (request.scope == RequestScope.Type)
                                {
                                    ArrayList list2 = new ArrayList();
                                    foreach (object obj3 in response.resultSet)
                                    {
                                        if (((((MetricProvider) obj3).codeElement == CodeElement.Structure) || (((MetricProvider) obj3).codeElement == CodeElement.Class)) || ((((MetricProvider) obj3).codeElement == CodeElement.Interface) || (((MetricProvider) obj3).codeElement == CodeElement.Enumeration)))
                                        {
                                            list2.Add(obj3);
                                        }
                                    }
                                    response.resultSet = list2;
                                }
                                else if (request.scope == RequestScope.Imp)
                                {
                                    ArrayList list3 = new ArrayList();
                                    foreach (object obj4 in response.resultSet)
                                    {
                                        if ((((MetricProvider) obj4).codeElement == CodeElement.Constructor) || (((MetricProvider) obj4).codeElement == CodeElement.Method))
                                        {
                                            list3.Add(obj4);
                                        }
                                    }
                                    response.resultSet = list3;
                                }
                                else if (request.scope == RequestScope.Imp)
                                {
                                    ArrayList list4 = new ArrayList();
                                    foreach (object obj5 in response.resultSet)
                                    {
                                        if ((((MetricProvider) obj5).codeElement == CodeElement.Constructor) || (((MetricProvider) obj5).codeElement == CodeElement.Method))
                                        {
                                            list4.Add(obj5);
                                        }
                                    }
                                    response.resultSet = list4;
                                }
                                else if (request.scope == RequestScope.Method)
                                {
                                    ArrayList list5 = new ArrayList();
                                    foreach (object obj6 in response.resultSet)
                                    {
                                        if (((MetricProvider) obj6).codeElement == CodeElement.Method)
                                        {
                                            list5.Add(obj6);
                                        }
                                    }
                                    response.resultSet = list5;
                                }
                                else if (request.scope == RequestScope.Constructor)
                                {
                                    ArrayList list6 = new ArrayList();
                                    foreach (object obj7 in response.resultSet)
                                    {
                                        if (((MetricProvider) obj7).codeElement == CodeElement.Constructor)
                                        {
                                            list6.Add(obj7);
                                        }
                                    }
                                    response.resultSet = list6;
                                }
                                else if (request.scope == RequestScope.Class)
                                {
                                    ArrayList list7 = new ArrayList();
                                    foreach (object obj8 in response.resultSet)
                                    {
                                        if (((MetricProvider) obj8).codeElement == CodeElement.Class)
                                        {
                                            list7.Add(obj8);
                                        }
                                    }
                                    response.resultSet = list7;
                                }
                                else if (request.scope == RequestScope.Struct)
                                {
                                    ArrayList list8 = new ArrayList();
                                    foreach (object obj9 in response.resultSet)
                                    {
                                        if (((MetricProvider) obj9).codeElement == CodeElement.Structure)
                                        {
                                            list8.Add(obj9);
                                        }
                                    }
                                    response.resultSet = list8;
                                }
                                else if (request.scope == RequestScope.Interface)
                                {
                                    ArrayList list9 = new ArrayList();
                                    foreach (object obj10 in response.resultSet)
                                    {
                                        if (((MetricProvider) obj10).codeElement == CodeElement.Interface)
                                        {
                                            list9.Add(obj10);
                                        }
                                    }
                                    response.resultSet = list9;
                                }
                                else if (request.scope == RequestScope.Enumeration)
                                {
                                    ArrayList list10 = new ArrayList();
                                    foreach (object obj11 in response.resultSet)
                                    {
                                        if (((MetricProvider) obj11).codeElement == CodeElement.Enumeration)
                                        {
                                            list10.Add(obj11);
                                        }
                                    }
                                    response.resultSet = list10;
                                }
                                else if (request.scope == RequestScope.ImpType)
                                {
                                    ArrayList list11 = new ArrayList();
                                    foreach (object obj12 in response.resultSet)
                                    {
                                        if ((((MetricProvider) obj12).codeElement == CodeElement.Class) || (((MetricProvider) obj12).codeElement == CodeElement.Structure))
                                        {
                                            list11.Add(obj12);
                                        }
                                    }
                                    response.resultSet = list11;
                                }
                                if (request.hasFilter)
                                {
                                    try
                                    {
                                        EvaluatorItem[] items = new EvaluatorItem[] { new EvaluatorItem(typeof(bool), expression, "GetNewBool") };
                                        Evaluator evaluator = new Evaluator(items);
                                        ArrayList list12 = new ArrayList();
                                        foreach (object obj13 in response.resultSet)
                                        {
                                            if (evaluator.EvaluateMyBool("GetNewBool", (MetricProvider) obj13))
                                            {
                                                list12.Add(obj13);
                                            }
                                        }
                                        response.resultSet = list12;
                                    }
                                    catch (Exception)
                                    {
                                        Console.Error.WriteLine("ERROR:  There is a problem with the filter declaration.");
                                        return;
                                    }
                                }
                                if (flag2 || flag3)
                                {
                                    response.resultSet.Sort();
                                }
                                if (flag3)
                                {
                                    response.resultSet.Reverse();
                                }
                                if (flag5 && (num2 < response.resultSet.Count))
                                {
                                    response.resultSet.RemoveRange(num2, response.resultSet.Count - num2);
                                }
                                if (flag6 & (num3 < response.resultSet.Count))
                                {
                                    response.resultSet.RemoveRange(0, response.resultSet.Count - num3);
                                }
                                bool flag8 = false;
                                string str14 = "Vil Report";
                                if ((commandLine["title"] != null) && (commandLine["title"].Length > 0))
                                {
                                    str14 = commandLine["title"];
                                }
                                if (commandLine["out"] != null)
                                {
                                    StreamWriter writer;
                                    flag8 = true;
                                    string path = makeFileName(commandLine["out"]);
                                    if (path == "")
                                    {
                                        return;
                                    }
                                    if (File.Exists(path))
                                    {
                                        File.Delete(path);
                                        writer = File.CreateText(path);
                                    }
                                    else
                                    {
                                        writer = File.CreateText(path);
                                    }
                                    VARG varg5 = new VARG();
                                    writer.Write(response.ToConsoleText("console", varg5, judgeHash));
                                    writer.Close();
                                }
                                if (commandLine["outhtml"] != null)
                                {
                                    StreamWriter writer2;
                                    flag8 = true;
                                    string str16 = makeFileName(commandLine["outhtml"]);
                                    if (str16 == "")
                                    {
                                        return;
                                    }
                                    if (File.Exists(str16))
                                    {
                                        File.Delete(str16);
                                        writer2 = File.CreateText(str16);
                                    }
                                    else
                                    {
                                        writer2 = File.CreateText(str16);
                                    }
                                    VARG varg = new VARG();
                                    varg.filter = str2;
                                    varg.metrics = str5;
                                    varg.assemblies = str3;
                                    varg.options = str4;
                                    varg.reportName = str14;
                                    if (commandLine["nologo"] == null)
                                    {
                                        varg.copymessage = cmess;
                                    }
                                    else
                                    {
                                        varg.copymessage = "";
                                    }
                                    writer2.Write(response.ToConsoleText("html", varg, judgeHash));
                                    writer2.Close();
                                }
                                if (commandLine["outhtmlshort"] != null)
                                {
                                    StreamWriter writer3;
                                    flag8 = true;
                                    string str17 = makeFileName(commandLine["outhtmlshort"]);
                                    if (str17 == "")
                                    {
                                        return;
                                    }
                                    if (File.Exists(str17))
                                    {
                                        File.Delete(str17);
                                        writer3 = File.CreateText(str17);
                                    }
                                    else
                                    {
                                        writer3 = File.CreateText(str17);
                                    }
                                    VARG varg2 = new VARG();
                                    if (commandLine["nologo"] == null)
                                    {
                                        varg2.copymessage = cmess;
                                    }
                                    else
                                    {
                                        varg2.copymessage = "";
                                    }
                                    varg2.reportName = str14;
                                    writer3.Write(response.ToConsoleText("htmlshort", varg2, judgeHash));
                                    writer3.Close();
                                }
                                if (commandLine["outhtmltable"] != null)
                                {
                                    StreamWriter writer4;
                                    flag8 = true;
                                    string str18 = makeFileName(commandLine["outhtmltable"]);
                                    if (str18 == "")
                                    {
                                        return;
                                    }
                                    VARG varg3 = new VARG();
                                    varg3.reportName = str14;
                                    if (File.Exists(str18))
                                    {
                                        File.Delete(str18);
                                        writer4 = File.CreateText(str18);
                                    }
                                    else
                                    {
                                        writer4 = File.CreateText(str18);
                                    }
                                    writer4.Write(response.ToConsoleText("htmltable", varg3, judgeHash));
                                    writer4.Close();
                                }
                                if (commandLine["outxml"] != null)
                                {
                                    StreamWriter writer5;
                                    flag8 = true;
                                    string str19 = makeFileName(commandLine["outxml"]);
                                    if (str19 == "")
                                    {
                                        return;
                                    }
                                    VARG varg4 = new VARG();
                                    varg4.filter = str2;
                                    varg4.metrics = str5;
                                    varg4.assemblies = str3;
                                    varg4.options = str4;
                                    if (commandLine["nologo"] == null)
                                    {
                                        varg4.copymessage = cmess;
                                    }
                                    else
                                    {
                                        varg4.copymessage = "";
                                    }
                                    varg4.reportName = str14;
                                    if (File.Exists(str19))
                                    {
                                        File.Delete(str19);
                                        writer5 = File.CreateText(str19);
                                    }
                                    else
                                    {
                                        writer5 = File.CreateText(str19);
                                    }
                                    writer5.Write(response.ToConsoleText("xml", varg4, judgeHash));
                                    writer5.Close();
                                }
                                if (!flag8)
                                {
                                    if (commandLine["nologo"] == null)
                                    {
                                        Console.Out.WriteLine(cmess + "\n");
                                    }
                                    Console.Out.WriteLine(response.ToConsoleText("console", new VARG(), judgeHash));
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
            Console.Error.WriteLine("ERROR:  There has been an unexpected problem!");
        }
    }

    internal static string makeFileName(string filename)
    {
        string path = filename;
        string currentDirectory = "";
        new ArrayList();
        if (!Path.IsPathRooted(path))
        {
            currentDirectory = Directory.GetCurrentDirectory();
            if (path.StartsWith("~"))
            {
                string environmentVariable = Environment.GetEnvironmentVariable("HOME");
                if (environmentVariable == null)
                {
                    Console.Error.WriteLine("Error : Output file (" + path + ") was not resolvable. ");
                    return "";
                }
                currentDirectory = environmentVariable;
                path = path.Substring(1);
            }
            if (path.StartsWith(Path.DirectorySeparatorChar.ToString()))
            {
                path = path.Substring(1);
            }
            if (!currentDirectory.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                currentDirectory = currentDirectory + Path.DirectorySeparatorChar.ToString();
            }
            int num = path.LastIndexOf(Path.DirectorySeparatorChar.ToString());
            if (num > -1)
            {
                currentDirectory = Path.Combine(currentDirectory, path.Substring(0, num + 1));
                path = path.Substring(num + 1);
            }
        }
        else
        {
            currentDirectory = path.Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar.ToString()) + 1);
            if (currentDirectory.StartsWith(@"\"))
            {
                currentDirectory = Path.GetPathRoot(Directory.GetCurrentDirectory()) + currentDirectory.Substring(1);
            }
            if (!currentDirectory.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                currentDirectory = currentDirectory + Path.DirectorySeparatorChar.ToString();
            }
            path = path.Substring(path.LastIndexOf(Path.DirectorySeparatorChar.ToString()) + 1);
        }
        return Path.Combine(currentDirectory, path);
    }
}
 

}
