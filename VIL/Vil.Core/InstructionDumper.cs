using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Reflector.Disassembler;
using System.Collections;
using System.Reflection.Emit;

namespace Vil.Core
{
    public class InstructionDumper
    {
        // Fields
        public static int[] OpCodeStackDelta = new int[0x5e9];
        private ModuleReader reader;
        internal int[] stacks = new int[0x7d0];

        // Methods
        static InstructionDumper()
        {
            for (int i = 0; i < 0x5e9; i++)
            {
                OpCodeStackDelta[i] = -20;
            }
            OpCodeStackDelta[0] = 0;
            OpCodeStackDelta[2] = 1;
            OpCodeStackDelta[3] = 1;
            OpCodeStackDelta[4] = 1;
            OpCodeStackDelta[5] = 1;
            OpCodeStackDelta[6] = 1;
            OpCodeStackDelta[7] = 1;
            OpCodeStackDelta[8] = 1;
            OpCodeStackDelta[9] = 1;
            OpCodeStackDelta[10] = -1;
            OpCodeStackDelta[11] = -1;
            OpCodeStackDelta[12] = -1;
            OpCodeStackDelta[13] = -1;
            OpCodeStackDelta[14] = 1;
            OpCodeStackDelta[15] = 1;
            OpCodeStackDelta[0x10] = -1;
            OpCodeStackDelta[0x11] = 1;
            OpCodeStackDelta[0x12] = 1;
            OpCodeStackDelta[0x13] = -1;
            OpCodeStackDelta[20] = 1;
            OpCodeStackDelta[0x15] = 1;
            OpCodeStackDelta[0x16] = 1;
            OpCodeStackDelta[0x17] = 1;
            OpCodeStackDelta[0x18] = 1;
            OpCodeStackDelta[0x19] = 1;
            OpCodeStackDelta[0x1a] = 1;
            OpCodeStackDelta[0x1b] = 1;
            OpCodeStackDelta[0x1c] = 1;
            OpCodeStackDelta[0x1d] = 1;
            OpCodeStackDelta[30] = 1;
            OpCodeStackDelta[0x1f] = 1;
            OpCodeStackDelta[0x20] = 1;
            OpCodeStackDelta[0x21] = 1;
            OpCodeStackDelta[0x22] = 1;
            OpCodeStackDelta[0x23] = 1;
            OpCodeStackDelta[0x25] = 1;
            OpCodeStackDelta[0x26] = -1;
            OpCodeStackDelta[40] = 20;
            OpCodeStackDelta[0x2a] = 20;
            OpCodeStackDelta[0x2b] = 0;
            OpCodeStackDelta[0x2c] = -1;
            OpCodeStackDelta[0x2d] = -1;
            OpCodeStackDelta[0x2e] = -2;
            OpCodeStackDelta[0x2f] = -2;
            OpCodeStackDelta[0x30] = -2;
            OpCodeStackDelta[0x31] = -2;
            OpCodeStackDelta[50] = -2;
            OpCodeStackDelta[0x33] = -2;
            OpCodeStackDelta[0x34] = -2;
            OpCodeStackDelta[0x35] = -2;
            OpCodeStackDelta[0x36] = -2;
            OpCodeStackDelta[0x37] = -2;
            OpCodeStackDelta[0x38] = 0;
            OpCodeStackDelta[0x39] = -1;
            OpCodeStackDelta[0x3a] = -1;
            OpCodeStackDelta[0x3b] = -2;
            OpCodeStackDelta[60] = -2;
            OpCodeStackDelta[0x3d] = -2;
            OpCodeStackDelta[0x3e] = -2;
            OpCodeStackDelta[0x3f] = -2;
            OpCodeStackDelta[0x40] = -2;
            OpCodeStackDelta[0x42] = -2;
            OpCodeStackDelta[0x43] = -2;
            OpCodeStackDelta[0x44] = -2;
            OpCodeStackDelta[0x45] = -1;
            OpCodeStackDelta[70] = 0;
            OpCodeStackDelta[0x47] = 0;
            OpCodeStackDelta[0x48] = 0;
            OpCodeStackDelta[0x49] = 0;
            OpCodeStackDelta[0x4a] = 0;
            OpCodeStackDelta[0x4b] = 0;
            OpCodeStackDelta[0x4c] = 0;
            OpCodeStackDelta[0x4d] = 0;
            OpCodeStackDelta[0x4e] = 0;
            OpCodeStackDelta[0x4f] = 0;
            OpCodeStackDelta[80] = 0;
            OpCodeStackDelta[0x51] = -2;
            OpCodeStackDelta[0x52] = -2;
            OpCodeStackDelta[0x53] = -2;
            OpCodeStackDelta[0x54] = -2;
            OpCodeStackDelta[0x55] = -2;
            OpCodeStackDelta[0x56] = -2;
            OpCodeStackDelta[0x57] = -2;
            OpCodeStackDelta[0x58] = -1;
            OpCodeStackDelta[0x59] = -1;
            OpCodeStackDelta[90] = -1;
            OpCodeStackDelta[0x5b] = -1;
            OpCodeStackDelta[0x5c] = -1;
            OpCodeStackDelta[0x5d] = -1;
            OpCodeStackDelta[0x5e] = -1;
            OpCodeStackDelta[0x5f] = -1;
            OpCodeStackDelta[0x60] = -1;
            OpCodeStackDelta[0x61] = -1;
            OpCodeStackDelta[0x62] = -1;
            OpCodeStackDelta[0x63] = -1;
            OpCodeStackDelta[100] = -1;
            OpCodeStackDelta[0x65] = 0;
            OpCodeStackDelta[0x66] = 0;
            OpCodeStackDelta[0x67] = 0;
            OpCodeStackDelta[0x68] = 0;
            OpCodeStackDelta[0x69] = 0;
            OpCodeStackDelta[0x6a] = 0;
            OpCodeStackDelta[0x6b] = 0;
            OpCodeStackDelta[0x6c] = 0;
            OpCodeStackDelta[0x6d] = 0;
            OpCodeStackDelta[110] = 0;
            OpCodeStackDelta[0x6f] = 20;
            OpCodeStackDelta[0x71] = 0;
            OpCodeStackDelta[0x72] = 1;
            OpCodeStackDelta[0x73] = 20;
            OpCodeStackDelta[0x74] = 0;
            OpCodeStackDelta[0x75] = 0;
            OpCodeStackDelta[0x76] = 0;
            OpCodeStackDelta[0x79] = 0;
            OpCodeStackDelta[0x7a] = -1;
            OpCodeStackDelta[0x7b] = 0;
            OpCodeStackDelta[0x7c] = 0;
            OpCodeStackDelta[0x7d] = -2;
            OpCodeStackDelta[0x7e] = 1;
            OpCodeStackDelta[0x7f] = 1;
            OpCodeStackDelta[0x80] = -1;
            OpCodeStackDelta[0x81] = -2;
            OpCodeStackDelta[0x85] = 0;
            OpCodeStackDelta[140] = 0;
            OpCodeStackDelta[0x8d] = 0;
            OpCodeStackDelta[0x8e] = 0;
            OpCodeStackDelta[0x8f] = -1;
            OpCodeStackDelta[0x90] = -1;
            OpCodeStackDelta[0x91] = -1;
            OpCodeStackDelta[0x92] = -1;
            OpCodeStackDelta[0x93] = -1;
            OpCodeStackDelta[0x94] = -1;
            OpCodeStackDelta[0x95] = -1;
            OpCodeStackDelta[150] = -1;
            OpCodeStackDelta[0x98] = -1;
            OpCodeStackDelta[0x99] = -1;
            OpCodeStackDelta[0x9a] = -1;
            OpCodeStackDelta[0x9c] = -3;
            OpCodeStackDelta[0x9d] = -3;
            OpCodeStackDelta[0x9e] = -3;
            OpCodeStackDelta[0x9f] = -3;
            OpCodeStackDelta[160] = -3;
            OpCodeStackDelta[0xa1] = -3;
            OpCodeStackDelta[0xa2] = -3;
            OpCodeStackDelta[0xb3] = 0;
            OpCodeStackDelta[180] = 0;
            OpCodeStackDelta[0xb5] = 0;
            OpCodeStackDelta[0xb6] = 0;
            OpCodeStackDelta[0xb7] = 0;
            OpCodeStackDelta[0xb8] = 0;
            OpCodeStackDelta[0xd0] = 1;
            OpCodeStackDelta[0xd1] = 0;
            OpCodeStackDelta[210] = 0;
            OpCodeStackDelta[0xd3] = 0;
            OpCodeStackDelta[0xd4] = 0;
            OpCodeStackDelta[0xd6] = -1;
            OpCodeStackDelta[0xd7] = -1;
            OpCodeStackDelta[0xd8] = -1;
            OpCodeStackDelta[0xd9] = -1;
            OpCodeStackDelta[0xda] = -1;
            OpCodeStackDelta[0xdb] = -1;
            OpCodeStackDelta[220] = 0;
            OpCodeStackDelta[0xdd] = 0;
            OpCodeStackDelta[0xde] = 0;
            OpCodeStackDelta[0xdf] = -2;
            OpCodeStackDelta[0xe0] = 0;
            OpCodeStackDelta[0x5cb] = 0;
            OpCodeStackDelta[0x5cc] = 1;
            OpCodeStackDelta[0x5ce] = 0;
            OpCodeStackDelta[0x5d3] = -1;
            OpCodeStackDelta[0x5d5] = 20;
            OpCodeStackDelta[0x5d7] = -1;
            OpCodeStackDelta[0x5d9] = 0;
            OpCodeStackDelta[0x5db] = 1;
            OpCodeStackDelta[0x5de] = 1;
            OpCodeStackDelta[0x5df] = 1;
            OpCodeStackDelta[0x5e1] = 0;
            OpCodeStackDelta[0x5e2] = 1;
            OpCodeStackDelta[0x5e3] = -1;
            OpCodeStackDelta[0x5e4] = -1;
            OpCodeStackDelta[0x5e5] = -1;
            OpCodeStackDelta[0x5e6] = -1;
            OpCodeStackDelta[0x5e7] = -1;
            OpCodeStackDelta[0x5e8] = 1;
        }

        internal int[] ComputeStack(Reflector.Disassembler.MethodBody methodBody, MethodBase methodBase, bool isConstructor)
        {
            if (methodBody == null)
            {
                return new int[0];
            }
            ExceptionHandler[] exceptions = methodBody.GetExceptions();
            Hashtable hashtable = new Hashtable();
            bool flag = false;
            int[] numArray = new int[methodBody.GetInstructions().Length];
            int[] numArray2 = new int[methodBody.GetInstructions().Length];
            int index = 0;
            numArray[index] = 0;
            bool flag2 = false;
            Hashtable hashtable2 = new Hashtable();
            if (exceptions != null)
            {
                foreach (ExceptionHandler handler in exceptions)
                {
                    if (handler.Type != ExceptionHandlerType.Finally)
                    {
                        hashtable[handler.HandlerOffset] = 1;
                    }
                    else
                    {
                        hashtable[handler.HandlerOffset] = 0;
                    }
                    if (handler.Type == ExceptionHandlerType.Filter)
                    {
                        hashtable[handler.FilterOffset] = 1;
                    }
                }
            }
            foreach (Instruction instruction in methodBody.GetInstructions())
            {
                int num2 = 0;
                if (hashtable.ContainsKey(instruction.Offset))
                {
                    numArray[index] = (int)hashtable[instruction.Offset];
                }
                else if (flag)
                {
                    numArray[index] = -100;
                    hashtable2[instruction.Offset] = 1;
                    flag2 = true;
                }
                else if (index > 0)
                {
                    numArray[index] = numArray[index - 1] + numArray2[index - 1];
                }
                OpCode code = instruction.Code;
                object operand = instruction.Operand;
                instruction.GetOperandData();
                if (((code.Value == 0x45) && (operand != null)) && (code.OperandType == OperandType.InlineSwitch))
                {
                    int[] numArray3 = (int[])operand;
                    for (int i = 0; i < numArray3.Length; i++)
                    {
                        hashtable[numArray3[i]] = 0;
                    }
                }
                if ((code.Value == 0xdd) || (code.Value == 0xde))
                {
                    num2 = -1 * numArray[index];
                }
                else if (((code.Value == 40) || (code.Value == 0x2a)) || (((code.Value == 0x73) || (code.Value == 0x6f)) || (code.Value == -493)))
                {
                    if (((code.Value == 40) || (code.Value == 0x6f)) || (code.Value == 0x73))
                    {
                        MemberInfo info = (MemberInfo)operand;
                        int num4 = 0;
                        string str = info.ToString().ToLower();
                        if (code.Value == 0x73)
                        {
                            num4++;
                        }
                        else
                        {
                            if (info.MemberType == MemberTypes.Constructor)
                            {
                                if (!((ConstructorInfo)info).IsStatic)
                                {
                                    num4--;
                                }
                            }
                            else if (!((MethodInfo)info).IsStatic)
                            {
                                num4--;
                            }
                            if (!str.StartsWith("void "))
                            {
                                num4++;
                            }
                        }
                        str = str.Substring(str.IndexOf("(") + 1, (str.IndexOf(")") - str.IndexOf("(")) - 1);
                        if (str.Length != 0)
                        {
                            num4 -= str.Split(new char[] { ',' }).Length;
                        }
                        num2 = num4;
                    }
                    else if (((code.Value == 0x2a) && !isConstructor) && (((MethodInfo)methodBase).ReturnType != typeof(void)))
                    {
                        num2 = -1;
                    }
                }
                else if (code.Value >= 0)
                {
                    num2 = OpCodeStackDelta[code.Value];
                }
                else
                {
                    num2 = OpCodeStackDelta[0x3e8 - code.Value];
                }
                if ((operand != null) && ((code.OperandType == OperandType.ShortInlineBrTarget) || (code.OperandType == OperandType.InlineBrTarget)))
                {
                    int num5 = (int)operand;
                    if (((numArray[index] >= 0) || (code.Value == 0xdd)) || (code.Value == 0xde))
                    {
                        hashtable[num5] = numArray[index] + num2;
                    }
                }
                if (((code.Value == 0x38) || (code.Value == 0x2b)) || ((code.Value == 0xdd) || (code.Value == 0xde)))
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
                numArray2[index] = num2;
                index++;
            }
            bool flag3 = false;
            int num6 = 0;
            bool flag4 = false;
            if (flag2)
            {
                foreach (Instruction instruction2 in methodBody.GetInstructions())
                {
                    OpCode code2 = instruction2.Code;
                    object obj3 = instruction2.Operand;
                    instruction2.GetOperandData();
                    if (hashtable2.ContainsKey(instruction2.Offset))
                    {
                        flag4 = true;
                        if (hashtable.ContainsKey(instruction2.Offset))
                        {
                            hashtable2.Remove(instruction2.Offset);
                            numArray[num6] = (int)hashtable[instruction2.Offset];
                        }
                        else
                        {
                            numArray[num6] = -50;
                            flag3 = true;
                        }
                    }
                    else if ((numArray[num6] < -10) && flag4)
                    {
                        if (hashtable.ContainsKey(instruction2.Offset))
                        {
                            numArray[num6] = (int)hashtable[instruction2.Offset];
                        }
                        else
                        {
                            numArray[num6] = numArray[num6 - 1] + numArray2[num6 - 1];
                        }
                    }
                    else
                    {
                        flag4 = false;
                    }
                    if ((obj3 != null) && ((code2.OperandType == OperandType.ShortInlineBrTarget) || (code2.OperandType == OperandType.InlineBrTarget)))
                    {
                        int num7 = (int)obj3;
                        if (numArray[num6] >= 0)
                        {
                            hashtable[num7] = numArray[num6] + numArray2[num6];
                        }
                    }
                    num6++;
                }
                if (flag3)
                {
                    flag3 = false;
                    flag4 = false;
                    num6 = 0;
                    foreach (Instruction instruction3 in methodBody.GetInstructions())
                    {
                        OpCode code3 = instruction3.Code;
                        object obj4 = instruction3.Operand;
                        instruction3.GetOperandData();
                        if (hashtable2.ContainsKey(instruction3.Offset))
                        {
                            flag4 = true;
                            if (hashtable.ContainsKey(instruction3.Offset))
                            {
                                hashtable2.Remove(instruction3.Offset);
                                numArray[num6] = (int)hashtable[instruction3.Offset];
                            }
                            else
                            {
                                numArray[num6] = -50;
                                flag3 = true;
                            }
                        }
                        else if ((numArray[num6] < -10) && flag4)
                        {
                            if (hashtable.ContainsKey(instruction3.Offset))
                            {
                                numArray[num6] = (int)hashtable[instruction3.Offset];
                            }
                            else
                            {
                                numArray[num6] = numArray[num6 - 1] + numArray2[num6 - 1];
                            }
                        }
                        else
                        {
                            flag4 = false;
                        }
                        if ((obj4 != null) && ((code3.OperandType == OperandType.ShortInlineBrTarget) || (code3.OperandType == OperandType.InlineBrTarget)))
                        {
                            int num8 = (int)obj4;
                            if (numArray[num6] >= 0)
                            {
                                hashtable[num8] = numArray[num6] + numArray2[num6];
                            }
                        }
                        num6++;
                    }
                }
                if (!flag3)
                {
                    return numArray;
                }
                flag3 = false;
                flag4 = false;
                num6 = 0;
                foreach (Instruction instruction4 in methodBody.GetInstructions())
                {
                    OpCode code4 = instruction4.Code;
                    object obj5 = instruction4.Operand;
                    instruction4.GetOperandData();
                    if (hashtable2.ContainsKey(instruction4.Offset))
                    {
                        flag4 = true;
                        if (hashtable.ContainsKey(instruction4.Offset))
                        {
                            hashtable2.Remove(instruction4.Offset);
                            numArray[num6] = (int)hashtable[instruction4.Offset];
                        }
                        else
                        {
                            numArray[num6] = -50;
                            flag3 = true;
                        }
                    }
                    else if ((numArray[num6] < -10) && flag4)
                    {
                        if (hashtable.ContainsKey(instruction4.Offset))
                        {
                            numArray[num6] = (int)hashtable[instruction4.Offset];
                        }
                        else
                        {
                            numArray[num6] = numArray[num6 - 1] + numArray2[num6 - 1];
                        }
                    }
                    else
                    {
                        flag4 = false;
                    }
                    if ((obj5 != null) && ((code4.OperandType == OperandType.ShortInlineBrTarget) || (code4.OperandType == OperandType.InlineBrTarget)))
                    {
                        int num9 = (int)obj5;
                        if (numArray[num6] >= 0)
                        {
                            hashtable[num9] = numArray[num6] + numArray2[num6];
                        }
                    }
                    num6++;
                }
            }
            return numArray;
        }

        public int[] ComputeStack(ModuleReader reader, MethodBase methodBase, bool isConstructor)
        {
            Reflector.Disassembler.MethodBody methodBody = reader.GetMethodBody(methodBase);
            return this.ComputeStack(methodBody, methodBase, isConstructor);
        }

        public void ProcessAssembly(Assembly a)
        {
            foreach (Module module in a.GetModules())
            {
                this.reader = new ModuleReader(module, new AssemblyProvider());
                foreach (Type type in module.GetTypes())
                {
                    Console.WriteLine(type.ToString());
                    MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    for (int i = 0; i < methods.Length; i++)
                    {
                        MethodInfo info1 = methods[i];
                    }
                }
            }
        }

        public void ProcessAssemblyStacks(Assembly a)
        {
            foreach (Module module in a.GetModules())
            {
                this.reader = new ModuleReader(module, new AssemblyProvider());
                foreach (Type type in module.GetTypes())
                {
                    Console.WriteLine(type.ToString());
                    MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    for (int i = 0; i < methods.Length; i++)
                    {
                        MethodBase methodBase = methods[i];
                        this.ComputeStack(this.reader, methodBase, false);
                    }
                }
            }
        }
    }



}
