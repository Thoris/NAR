using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Collections;

using System.Reflection;
using Reflector.Disassembler;

namespace Vil.Core
{
    public class Metrics
    {
        // Fields
        private static StringCollection typeReferences = new StringCollection();

        // Methods
        static Metrics()
        {
            typeReferences.AddRange(new string[] { "box", "castclass", "cpobj", "initobj", "isinst", "ldelema", "ldobj", "mkrefany", "newarr", "refanyval", "sizeof", "stobj", "unbox" });
        }

        internal static void AddTypes(StringCollection typeNames, Type type, Type otherType)
        {
            if ((otherType != null) && ((!type.IsSubclassOf(otherType) && !otherType.IsSubclassOf(type)) && !typeNames.Contains(otherType.FullName)))
            {
                typeNames.Add(otherType.FullName);
            }
        }

        internal static void AddTypesForInherit(StringCollection typeNames, Type type, Type otherType)
        {
            if ((otherType != null) && !typeNames.Contains(otherType.FullName))
            {
                typeNames.Add(otherType.FullName);
            }
        }

        internal void candelete()
        {
            GetAfferentCoupling(new ArrayList());
        }

        internal static int CouplingBetweenObjects(Type type)
        {
            StringCollection strings = TypeDependancies(type, false);
            StringCollection strings2 = new StringCollection();
            int num = 0;
            foreach (string str in strings)
            {
                if (!str.StartsWith("System."))
                {
                    int index = str.IndexOf("[");
                    if (index == -1)
                    {
                        num++;
                    }
                    else
                    {
                        string str2 = str.Substring(0, index);
                        if (!strings.Contains(str2) && !strings2.Contains(str2))
                        {
                            strings2.Add(str2);
                            num++;
                        }
                    }
                }
            }
            return num;
        }

        internal static int CyclomaticComplexity(Reflector.Disassembler.MethodBody methodBody)
        {
            int num = 1;
            foreach (Instruction instruction in methodBody.GetInstructions())
            {
                if (((instruction.Code.Name.StartsWith("b") && (instruction.Code.Name != "br")) && ((instruction.Code.Name != "br.s") && (instruction.Code.Name != "break"))) && (instruction.Code.Name != "box"))
                {
                    num++;
                }
                else if (instruction.Code.Name == "switch")
                {
                    StringCollection strings = new StringCollection();
                    string str = "";
                    int[] operand = (int[])instruction.Operand;
                    for (int i = 0; i < operand.Length; i++)
                    {
                        str = operand[i].ToString();
                        if (!strings.Contains(str))
                        {
                            strings.Add(str);
                            num++;
                        }
                    }
                }
            }
            return (num + methodBody.GetExceptions().Length);
        }

        public static StringCollection FilteredTypeDependencies(Type type)
        {
            if (Assembly.GetCallingAssembly().GetCustomAttributes(typeof(CommonIterator), false).Length == 0)
            {
                return new StringCollection();
            }
            StringCollection strings = TypeDependancies(type, true);
            StringCollection strings2 = new StringCollection();
            foreach (string str in strings)
            {
                int index = str.IndexOf("[");
                if (index != -1)
                {
                    string str2 = str.Substring(0, index);
                    if (!strings2.Contains(str2))
                    {
                        strings2.Add(str2);
                    }
                }
                else if (!strings2.Contains(str))
                {
                    strings2.Add(str);
                }
            }
            return strings2;
        }

        internal static float GetAfferentCoupling(ArrayList modules)
        {
            int num = 0;
            foreach (Type type in MultiAssemblyMetrics.tmap.Keys)
            {
                if (!modules.Contains(type.Module) && TypeDependanciesForAC(modules, type))
                {
                    num++;
                }
            }
            return (float)num;
        }

        internal static float GetEfferentCoupling(ArrayList modules, MetricRequest request)
        {
            int num = 0;
            foreach (object obj2 in modules)
            {
                Module module = (Module)obj2;
                foreach (Type type in module.GetTypes())
                {
                    if ((!type.IsClass || request.CanUseType(type)) && TypeDependanciesForEC(modules, type))
                    {
                        num++;
                    }
                }
            }
            return (float)num;
        }

        internal static bool IsTestFixture(Type type)
        {
            if (type.GetCustomAttributes(true).Length > 0)
            {
                foreach (object obj2 in type.GetCustomAttributes(true))
                {
                    if ((obj2.ToString() == "NUnit.Framework.TestFixtureAttribute") || (obj2.ToString() == "csUnit.TestFixtureAttribute"))
                    {
                        return true;
                    }
                    Type type2 = obj2.GetType();
                    while ((type2 != typeof(Attribute)) && (type2 != typeof(object)))
                    {
                        type2 = type2.BaseType;
                        if (type2.FullName == "MbUnit.Core.Framework.TestFixturePatternAttribute")
                        {
                            return true;
                        }
                    }
                }
                Type type3 = type.BaseType;
                if (type3 == typeof(object))
                {
                    return false;
                }
                return IsTestFixture(type3);
            }
            Type baseType = type.BaseType;
            if (baseType == typeof(object))
            {
                return false;
            }
            return IsTestFixture(baseType);
        }

        internal static int LackOfCohesionOfMethods(Type type)
        {
            if (type.IsAbstract)
            {
                return -1;
            }
            if (type.BaseType != typeof(object))
            {
                return -1;
            }
            FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            Hashtable hashtable = new Hashtable();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            Hashtable hashtable2 = new Hashtable();
            foreach (PropertyInfo info in properties)
            {
                MethodBase getMethod;
                if (info.CanRead)
                {
                    getMethod = info.GetGetMethod(true);
                    hashtable2[getMethod] = 0x37;
                }
                if (info.CanWrite)
                {
                    getMethod = info.GetSetMethod(true);
                    hashtable2[getMethod] = 0x37;
                }
            }
            foreach (FieldInfo info2 in fields)
            {
                hashtable[info2.FieldHandle.Value] = new ArrayList();
            }
            int num = 0;
            MethodBase[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            Hashtable hashtable3 = new Hashtable();
            bool flag = false;
            foreach (MethodBase base3 in methods)
            {
                Reflector.Disassembler.MethodBody methodBody;
                ArrayList list = new ArrayList();
                bool flag2 = false;
                try
                {
                    methodBody = ModuleMetrics.reader.GetMethodBody(base3);
                }
                catch (ModuleReaderException)
                {
                    methodBody = null;
                }
                if (methodBody != null)
                {
                    if (!hashtable2.ContainsKey((MethodInfo)base3))
                    {
                        num++;
                    }
                    else
                    {
                        flag2 = true;
                    }
                    foreach (Instruction instruction in methodBody.GetInstructions())
                    {
                        if (instruction.Code.Name.StartsWith("ldfld") || instruction.Code.Name.StartsWith("stfld"))
                        {
                            FieldInfo operand = (FieldInfo)instruction.Operand;
                            if (hashtable.ContainsKey(operand.FieldHandle.Value) && !((ArrayList)hashtable[operand.FieldHandle.Value]).Contains(base3))
                            {
                                if (!flag2)
                                {
                                    ((ArrayList)hashtable[operand.FieldHandle.Value]).Add(base3);
                                }
                                list.Add(operand.FieldHandle.Value);
                                flag = true;
                            }
                        }
                    }
                    hashtable3[base3] = list;
                }
            }
            int num2 = 0;
            while (flag)
            {
                num2++;
                flag = false;
                foreach (MethodBase base4 in methods)
                {
                    Reflector.Disassembler.MethodBody body2;
                    bool flag3 = false;
                    try
                    {
                        body2 = ModuleMetrics.reader.GetMethodBody(base4);
                    }
                    catch (ModuleReaderException)
                    {
                        body2 = null;
                    }
                    if (body2 != null)
                    {
                        if (hashtable2.ContainsKey((MethodInfo)base4))
                        {
                            flag3 = true;
                        }
                        foreach (Instruction instruction2 in body2.GetInstructions())
                        {
                            if ((instruction2.Code.Name == "call") || (instruction2.Code.Name == "callvirt"))
                            {
                                MemberInfo info4 = (MemberInfo)instruction2.Operand;
                                if (info4.DeclaringType == type)
                                {
                                    MethodBase key = (MethodBase)info4;
                                    if (hashtable3.ContainsKey(key))
                                    {
                                        ArrayList list2 = (ArrayList)hashtable3[key];
                                        foreach (object obj2 in list2)
                                        {
                                            IntPtr ptr = (IntPtr)obj2;
                                            if (!((ArrayList)hashtable[ptr]).Contains(base4))
                                            {
                                                if (!flag3)
                                                {
                                                    ((ArrayList)hashtable[ptr]).Add(base4);
                                                    flag = true;
                                                }
                                                else if (num2 < 3)
                                                {
                                                    flag = true;
                                                }
                                                if (hashtable3.ContainsKey(base4))
                                                {
                                                    if (!(base4.MethodHandle.Value == key.MethodHandle.Value))
                                                    {
                                                        ((ArrayList)hashtable3[base4]).Add(ptr);
                                                    }
                                                }
                                                else
                                                {
                                                    ArrayList list3 = new ArrayList();
                                                    list3.Add(ptr);
                                                    hashtable3[base4] = list3;
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
            int num3 = 0;
            foreach (object obj3 in hashtable.Keys)
            {
                num3 += ((ArrayList)hashtable[obj3]).Count;
            }
            if (num == 0)
            {
                return -1;
            }
            if (hashtable.Count == 0)
            {
                return 100;
            }
            return ((100 * ((hashtable.Count * num) - num3)) / (hashtable.Count * num));
        }

        internal static int MethodSize(Reflector.Disassembler.MethodBody methodBody)
        {
            return methodBody.GetInstructions().Length;
        }

        internal static void ProcessImp(Type type, MethodBase methodBase, StringCollection typeNames, ModuleReader mreader)
        {
            Reflector.Disassembler.MethodBody methodBody;
            try
            {
                methodBody = mreader.GetMethodBody(methodBase);
            }
            catch (ModuleReaderException)
            {
                methodBody = null;
            }
            if (methodBody != null)
            {
                foreach (ExceptionHandler handler in methodBody.GetExceptions())
                {
                    AddTypes(typeNames, type, handler.CatchType);
                }
                foreach (ParameterInfo info in methodBase.GetParameters())
                {
                    AddTypes(typeNames, type, info.ParameterType);
                }
                foreach (Type type2 in methodBody.GetLocals())
                {
                    AddTypes(typeNames, type, type2);
                }
                foreach (Instruction instruction in methodBody.GetInstructions())
                {
                    if (typeReferences.Contains(instruction.Code.Name))
                    {
                        AddTypes(typeNames, type, (Type)instruction.Operand);
                    }
                    else if (instruction.Code.Name == "newobj")
                    {
                        MethodBase operand = (MethodBase)instruction.Operand;
                        if (operand.IsConstructor)
                        {
                            AddTypes(typeNames, type, operand.DeclaringType);
                        }
                    }
                    else if ((instruction.Code.Name == "call") && ((MethodBase)instruction.Operand).IsStatic)
                    {
                        AddTypes(typeNames, type, ((MemberInfo)instruction.Operand).DeclaringType);
                    }
                }
            }
        }

        internal static bool ProcessImpForAC(ArrayList smodules, ArrayList modules, Type type, MethodBase methodBase, ModuleReader mreader)
        {
            Reflector.Disassembler.MethodBody methodBody;
            try
            {
                methodBody = mreader.GetMethodBody(methodBase);
            }
            catch (ModuleReaderException)
            {
                methodBody = null;
            }
            if (methodBody != null)
            {
                string name = ((Module)modules[0]).Name;
                foreach (ExceptionHandler handler in methodBody.GetExceptions())
                {
                    if ((((handler != null) && (handler.CatchType != null)) && smodules.Contains(handler.CatchType.Module.Name)) && ((handler.CatchType.Namespace == null) || !handler.CatchType.Namespace.StartsWith("System")))
                    {
                        return true;
                    }
                }
                foreach (ParameterInfo info in methodBase.GetParameters())
                {
                    if (smodules.Contains(info.ParameterType.Module.Name) && ((info.ParameterType.Namespace == null) || !info.ParameterType.Namespace.StartsWith("System")))
                    {
                        return true;
                    }
                }
                foreach (Type type2 in methodBody.GetLocals())
                {
                    if (smodules.Contains(type2.Module.Name) && ((type2.Namespace == null) || !type2.Namespace.StartsWith("System")))
                    {
                        return true;
                    }
                }
                foreach (Instruction instruction in methodBody.GetInstructions())
                {
                    if (typeReferences.Contains(instruction.Code.Name))
                    {
                        if (smodules.Contains(((Type)instruction.Operand).Module.Name) && ((((Type)instruction.Operand).Namespace == null) || !((Type)instruction.Operand).Namespace.StartsWith("System")))
                        {
                            return true;
                        }
                    }
                    else if (instruction.Code.Name == "newobj")
                    {
                        MethodBase operand = (MethodBase)instruction.Operand;
                        if ((operand.IsConstructor && smodules.Contains(operand.DeclaringType.Module.Name)) && ((operand.DeclaringType.Namespace == null) || !operand.DeclaringType.Namespace.StartsWith("System")))
                        {
                            return true;
                        }
                    }
                    else if ((((instruction.Code.Name == "call") && ((MethodBase)instruction.Operand).IsStatic) && smodules.Contains(((MemberInfo)instruction.Operand).DeclaringType.Module.Name)) && ((((MemberInfo)instruction.Operand).DeclaringType.Namespace == null) || !((MemberInfo)instruction.Operand).DeclaringType.Namespace.StartsWith("System")))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal static bool ProcessImpForEC(ArrayList modules, Type type, MethodBase methodBase, ModuleReader mreader)
        {
            Reflector.Disassembler.MethodBody methodBody;
            try
            {
                methodBody = mreader.GetMethodBody(methodBase);
            }
            catch (ModuleReaderException)
            {
                methodBody = null;
            }
            if (methodBody != null)
            {
                foreach (ExceptionHandler handler in methodBody.GetExceptions())
                {
                    if ((((handler != null) && (handler.CatchType != null)) && !modules.Contains(handler.CatchType.Module)) && ((handler.CatchType.Namespace == null) || !handler.CatchType.Namespace.StartsWith("System")))
                    {
                        return true;
                    }
                }
                foreach (ParameterInfo info in methodBase.GetParameters())
                {
                    if (!modules.Contains(info.ParameterType.Module) && ((info.ParameterType.Namespace == null) || !info.ParameterType.Namespace.StartsWith("System")))
                    {
                        return true;
                    }
                }
                foreach (Type type2 in methodBody.GetLocals())
                {
                    if (!modules.Contains(type2.Module) && ((type2.Namespace == null) || !type2.Namespace.StartsWith("System")))
                    {
                        return true;
                    }
                }
                foreach (Instruction instruction in methodBody.GetInstructions())
                {
                    if (typeReferences.Contains(instruction.Code.Name))
                    {
                        if (!modules.Contains(((Type)instruction.Operand).Module) && ((((Type)instruction.Operand).Namespace == null) || !((Type)instruction.Operand).Namespace.StartsWith("System")))
                        {
                            return true;
                        }
                    }
                    else if (instruction.Code.Name == "newobj")
                    {
                        MethodBase operand = (MethodBase)instruction.Operand;
                        if (!operand.IsConstructor)
                        {
                            goto Label_02CF;
                        }
                        if (!modules.Contains(operand.DeclaringType.Module) && ((operand.DeclaringType.Namespace == null) || !operand.DeclaringType.Namespace.StartsWith("System")))
                        {
                            return true;
                        }
                    }
                    if ((((instruction.Operand != null) && instruction.Operand.GetType().IsSubclassOf(typeof(MethodBase))) && (((MethodBase)instruction.Operand).IsStatic && !modules.Contains(((MemberInfo)instruction.Operand).DeclaringType.Module))) && ((((MemberInfo)instruction.Operand).DeclaringType.Namespace == null) || !((MemberInfo)instruction.Operand).DeclaringType.Namespace.StartsWith("System")))
                    {
                        return true;
                    }
                Label_02CF: ;
                }
            }
            return false;
        }

        internal static int ProcessImpForRFC(Type type, MethodBase methodBase, StringCollection methNames)
        {
            Reflector.Disassembler.MethodBody methodBody;
            try
            {
                methodBody = ModuleMetrics.reader.GetMethodBody(methodBase);
            }
            catch (ModuleReaderException)
            {
                methodBody = null;
            }
            if (methodBody == null)
            {
                return 0;
            }
            foreach (Instruction instruction in methodBody.GetInstructions())
            {
                if ((instruction.Code.Name == "call") || (instruction.Code.Name == "callvirt"))
                {
                    string str = "";
                    MemberInfo operand = (MemberInfo)instruction.Operand;
                    if (operand.DeclaringType != type)
                    {
                        if (operand.DeclaringType != null)
                        {
                            str = str + operand.DeclaringType + "::";
                        }
                        str = str + operand;
                        if (!methNames.Contains(str))
                        {
                            methNames.Add(str);
                        }
                    }
                }
            }
            return 1;
        }

        internal static int ResponseForClass(Type type)
        {
            int num2;
            StringCollection methNames = new StringCollection();
            int num = 0;
            ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            for (num2 = 0; num2 < constructors.Length; num2++)
            {
                MethodBase methodBase = constructors[num2];
                num += ProcessImpForRFC(type, methodBase, methNames);
            }
            MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            for (num2 = 0; num2 < methods.Length; num2++)
            {
                MethodBase base3 = methods[num2];
                num += ProcessImpForRFC(type, base3, methNames);
            }
            return (methNames.Count + num);
        }

        internal static float SumSubordinates(int position, int checkpoint, ArrayList resultSet)
        {
            float num = 0f;
            float num2 = 0f;
            for (int i = checkpoint; i < resultSet.Count; i++)
            {
                num2 = ((MetricProvider)resultSet[i]).computedMetrics[position];
                if (num2 != -1f)
                {
                    num += num2;
                }
            }
            return num;
        }

        internal static float SumSubordinates(int position, int checkpoint, ArrayList resultSet, CodeElement cElement)
        {
            float num = 0f;
            float num2 = 0f;
            for (int i = checkpoint; i < resultSet.Count; i++)
            {
                if (((MetricProvider)resultSet[i]).codeElement == cElement)
                {
                    num2 = ((MetricProvider)resultSet[i]).computedMetrics[position];
                    if (num2 != -1f)
                    {
                        num += num2;
                    }
                }
            }
            return num;
        }

        internal static StringCollection TypeDependancies(Type type, bool includeInherited)
        {
            int num;
            StringCollection typeNames = new StringCollection();
            ModuleReader mreader = (ModuleReader)ModuleMetrics.readermap[type];
            Type[] interfaces = type.GetInterfaces();
            for (num = 0; num < interfaces.Length; num++)
            {
                Type otherType = interfaces[num];
                AddTypes(typeNames, type, otherType);
            }
            if ((includeInherited && type.IsClass) && (type.BaseType != typeof(object)))
            {
                AddTypesForInherit(typeNames, type, type.BaseType);
            }
            FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            for (num = 0; num < fields.Length; num++)
            {
                FieldInfo info = fields[num];
                AddTypes(typeNames, type, info.FieldType);
            }
            ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            for (num = 0; num < constructors.Length; num++)
            {
                MethodBase methodBase = constructors[num];
                ProcessImp(type, methodBase, typeNames, mreader);
            }
            MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            for (num = 0; num < methods.Length; num++)
            {
                MethodBase base3 = methods[num];
                ProcessImp(type, base3, typeNames, mreader);
                AddTypes(typeNames, type, ((MethodInfo)base3).ReturnType);
            }
            return typeNames;
        }

        internal static bool TypeDependanciesForAC(ArrayList modules, Type type)
        {
            int num;
            ModuleReader mreader = (ModuleReader)MultiAssemblyMetrics.tmap[type];
            ArrayList smodules = new ArrayList();
            foreach (object obj2 in modules)
            {
                smodules.Add(((Module)obj2).Name);
            }
            string name = ((Module)modules[0]).Name;
            Type[] interfaces = type.GetInterfaces();
            for (num = 0; num < interfaces.Length; num++)
            {
                Type type2 = interfaces[num];
                if (smodules.Contains(type2.Module.Name) && ((type2.Namespace == null) || !type2.Namespace.StartsWith("System")))
                {
                    return true;
                }
            }
            if (((type.IsClass && (type.BaseType != typeof(object))) && smodules.Contains(type.BaseType.Module.Name)) && ((type.BaseType.Namespace == null) || !type.BaseType.Namespace.StartsWith("System")))
            {
                return true;
            }
            FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            for (num = 0; num < fields.Length; num++)
            {
                FieldInfo info = fields[num];
                if (smodules.Contains(info.FieldType.Module.Name) && ((info.FieldType.Namespace == null) || !info.FieldType.Namespace.StartsWith("System")))
                {
                    return true;
                }
            }
            ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            for (num = 0; num < constructors.Length; num++)
            {
                MethodBase methodBase = constructors[num];
                if (ProcessImpForAC(smodules, modules, type, methodBase, mreader))
                {
                    return true;
                }
            }
            MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            for (num = 0; num < methods.Length; num++)
            {
                MethodBase base3 = methods[num];
                if (ProcessImpForAC(smodules, modules, type, base3, mreader))
                {
                    return true;
                }
                if (smodules.Contains(((MethodInfo)base3).ReturnType.Module.Name) && ((((MethodInfo)base3).ReturnType.Namespace == null) || !((MethodInfo)base3).ReturnType.Namespace.StartsWith("System")))
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool TypeDependanciesForEC(ArrayList modules, Type type)
        {
            int num;
            ModuleReader mreader = (ModuleReader)ModuleMetrics.readermap[type];
            Type[] interfaces = type.GetInterfaces();
            for (num = 0; num < interfaces.Length; num++)
            {
                Type type2 = interfaces[num];
                if (!modules.Contains(type2.Module) && ((type2.Namespace == null) || !type2.Namespace.StartsWith("System")))
                {
                    return true;
                }
            }
            if (((type.IsClass && (type.BaseType != typeof(object))) && !modules.Contains(type.BaseType.Module)) && ((type.BaseType.Namespace == null) || !type.BaseType.Namespace.StartsWith("System")))
            {
                return true;
            }
            FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            for (num = 0; num < fields.Length; num++)
            {
                FieldInfo info = fields[num];
                if (!modules.Contains(info.FieldType.Module) && ((info.FieldType.Namespace == null) || !info.FieldType.Namespace.StartsWith("System")))
                {
                    return true;
                }
            }
            ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            for (num = 0; num < constructors.Length; num++)
            {
                MethodBase methodBase = constructors[num];
                if (ProcessImpForEC(modules, type, methodBase, mreader))
                {
                    return true;
                }
            }
            MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            for (num = 0; num < methods.Length; num++)
            {
                MethodBase base3 = methods[num];
                if (ProcessImpForEC(modules, type, base3, mreader))
                {
                    return true;
                }
                if (!modules.Contains(((MethodInfo)base3).ReturnType.Module) && ((((MethodInfo)base3).ReturnType.Namespace == null) || !((MethodInfo)base3).ReturnType.Namespace.StartsWith("System")))
                {
                    return true;
                }
            }
            return false;
        }
    }


}