using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection.Emit;
using System.Reflection;
using System.Collections;
using System.IO;
using System.Globalization;

namespace Reflector.Disassembler
{
    public sealed class ModuleReader
    {
        // Fields
        private MetaData metaData = null;
        private OpCode[] multiByteOpCodes = new OpCode[0x100];
        private BinaryReader reader = null;
        private OpCode[] singleByteOpCodes = new OpCode[0x100];

        // Methods
        public ModuleReader(Module moduleInstance, IAssemblyProvider assemblyProvider)
        {
            if (moduleInstance == null)
            {
                throw new ArgumentNullException("moduleInstance");
            }
            this.reader = new BinaryReader(moduleInstance.FullyQualifiedName);
            this.metaData = new MetaData(this.reader, moduleInstance, assemblyProvider);
            foreach (FieldInfo info in typeof(OpCodes).GetFields())
            {
                if (info.FieldType == typeof(OpCode))
                {
                    OpCode code = (OpCode)info.GetValue(null);
                    ushort num2 = (ushort)code.Value;
                    if (num2 >= 0x100)
                    {
                        if ((num2 & 0xff00) != 0xfe00)
                        {
                            throw new ModuleReaderException("Invalid OpCode.");
                        }
                        this.multiByteOpCodes[num2 & 0xff] = code;
                    }
                    else
                    {
                        this.singleByteOpCodes[(int)num2] = code;
                    }
                }
            }
        }

        private Instruction DecodeInstruction(BinaryReader reader, MethodBase methodBase)
        {
            int position = reader.Position;
            OpCode nop = OpCodes.Nop;
            object operand = null;
            byte[] operandData = null;
            ushort num2 = reader.ReadByte();
            if (num2 != 0xfe)
            {
                nop = this.singleByteOpCodes[(int)num2];
            }
            else
            {
                num2 = reader.ReadByte();
                nop = this.multiByteOpCodes[(int)num2];
                num2 = (ushort)(num2 | 0xfe00);
            }
            int count = reader.Position;
            switch (nop.OperandType)
            {
                case OperandType.InlineBrTarget:
                    {
                        int num5 = reader.ReadInt32();
                        operand = reader.Position + num5;
                        break;
                    }
                case OperandType.InlineField:
                case OperandType.InlineMethod:
                case OperandType.InlineSig:
                case OperandType.InlineTok:
                case OperandType.InlineType:
                    operand = reader.ReadInt32();
                    break;

                case OperandType.InlineI:
                    operand = reader.ReadInt32();
                    break;

                case OperandType.InlineI8:
                    operand = reader.ReadInt64();
                    break;

                case OperandType.InlineNone:
                    break;

                case OperandType.InlinePhi:
                    throw new ModuleReaderException("Unknown operand type 'InlinePhi'.");

                case OperandType.InlineR:
                    operand = reader.ReadDouble();
                    break;

                case OperandType.InlineString:
                    operand = this.metaData.GetUserString(reader.ReadInt32() & 0xffffff);
                    break;

                case OperandType.InlineSwitch:
                    {
                        int num6 = reader.ReadInt32();
                        int[] numArray = new int[num6];
                        for (int i = 0; i < num6; i++)
                        {
                            numArray[i] = reader.ReadInt32();
                        }
                        int[] numArray2 = new int[num6];
                        for (int j = 0; j < num6; j++)
                        {
                            numArray2[j] = reader.Position + numArray[j];
                        }
                        operand = numArray2;
                        break;
                    }
                case OperandType.InlineVar:
                    operand = reader.ReadUInt16();
                    break;

                case OperandType.ShortInlineBrTarget:
                    {
                        sbyte num4 = reader.ReadSByte();
                        operand = reader.Position + num4;
                        break;
                    }
                case OperandType.ShortInlineI:
                    operand = reader.ReadSByte();
                    break;

                case OperandType.ShortInlineR:
                    operand = reader.ReadSingle();
                    break;

                case OperandType.ShortInlineVar:
                    operand = reader.ReadByte();
                    break;

                default:
                    throw new ModuleReaderException("Unknown operand type.");
            }
            count = reader.Position - count;
            if (count != 0)
            {
                reader.Position -= count;
                operandData = reader.ReadBytes(count);
            }
            switch (num2)
            {
                case 0xfe09:
                case 0xfe0a:
                case 14:
                case 15:
                case 0xfe0b:
                case 0x10:
                    {
                        int index = (int)operand;
                        if (!methodBase.IsStatic && (index == 0))
                        {
                            operand = new ThisParameterInfo(methodBase.DeclaringType);
                        }
                        else
                        {
                            if (!methodBase.IsStatic)
                            {
                                index--;
                            }
                            ParameterInfo[] parameters = methodBase.GetParameters();
                            if ((index < 0) || ((parameters != null) && (index >= parameters.Length)))
                            {
                                throw new ModuleReaderException("Argument index out of range.");
                            }
                            operand = parameters[index];
                        }
                        break;
                    }
            }
            return new Instruction(this, position, nop, operand, operandData);
        }

        internal Type[] DecodeLocalVariableSignature(int token)
        {
            return this.metaData.DecodeLocalVariableSignature(token);
        }

        public MethodBody GetMethodBody(MethodBase methodBase)
        {
            if (this.metaData == null)
            {
                throw new ArgumentNullException("metaData");
            }
            if (this.reader == null)
            {
                throw new NullReferenceException("reader");
            }
            switch ((methodBase.GetMethodImplementationFlags() & MethodImplAttributes.CodeTypeMask))
            {
                case MethodImplAttributes.Native:
                case MethodImplAttributes.CodeTypeMask:
                case MethodImplAttributes.OPTIL:
                    return null;
            }
            uint methodRva = this.metaData.GetMethodRva(methodBase);
            if (methodRva == 0)
            {
                return null;
            }
            MethodBodyReader methodBodyReader = this.metaData.GetMethodBodyReader(methodRva);
            IList list = new ArrayList();
            if (methodBodyReader.Code.Length != 0)
            {
                BinaryReader reader = new BinaryReader(methodBodyReader.Code);
                while (reader.Position < reader.Length)
                {
                    list.Add(this.DecodeInstruction(reader, methodBase));
                }
            }
            ExceptionHandler[] exceptions = null;
            Clause[] clauses = methodBodyReader.Clauses;
            if (clauses != null)
            {
                exceptions = new ExceptionHandler[clauses.Length];
                for (int i = 0; i < clauses.Length; i++)
                {
                    exceptions[i] = null;
                    int tryOffset = clauses[i].TryOffset;
                    int tryLength = clauses[i].TryLength;
                    int handlerOffset = clauses[i].HandlerOffset;
                    int handlerLength = clauses[i].HandlerLength;
                    switch ((clauses[i].Flags & 7))
                    {
                        case 0:
                            exceptions[i] = new ExceptionHandler(ExceptionHandlerType.Catch, tryOffset, tryLength, handlerOffset, handlerLength, this.metaData.GetType(clauses[i].Value));
                            break;

                        case 1:
                            exceptions[i] = new ExceptionHandler(ExceptionHandlerType.Filter, tryOffset, tryLength, handlerOffset, handlerLength, clauses[i].Value);
                            break;

                        case 2:
                            exceptions[i] = new ExceptionHandler(ExceptionHandlerType.Finally, tryOffset, tryLength, handlerOffset, handlerLength);
                            break;

                        case 4:
                            exceptions[i] = new ExceptionHandler(ExceptionHandlerType.Fault, tryOffset, tryLength, handlerOffset, handlerLength);
                            break;

                        default:
                            throw new ModuleReaderException("Unknown exception handler type.");
                    }
                }
            }
            Instruction[] array = new Instruction[list.Count];
            list.CopyTo(array, 0);
            return new MethodBody(this, methodBodyReader.Code.Length, (int)methodBodyReader.MaxStack, methodBodyReader.LocalVarSigToken, exceptions, array);
        }

        public AssemblyName[] GetReferencedAssemblies()
        {
            AssemblyReferenceRow[] assemblyReference = this.metaData.Tables.AssemblyReference;
            AssemblyName[] nameArray = new AssemblyName[assemblyReference.Length];
            for (int i = 0; i < assemblyReference.Length; i++)
            {
                nameArray[i] = this.metaData.GetAssemblyReference(0x23000000 | (i + 1));
            }
            return nameArray;
        }

        public MemberInfo[] GetReferencedMembers()
        {
            ArrayList list = new ArrayList();
            for (int i = 0; i < this.metaData.Tables.MemberReference.Length; i++)
            {
                try
                {
                    MemberInfo info = this.metaData.ResolveToken(0xa000000 | (i + 1));
                    list.Add(info);
                }
                catch (FileNotFoundException)
                {
                }
                catch (TypeLoadException)
                {
                }
                catch (ModuleReaderException)
                {
                }
            }
            for (int j = 0; j < this.metaData.Tables.TypeReference.Length; j++)
            {
                try
                {
                    MemberInfo info2 = this.metaData.ResolveToken(0x1000000 | (j + 1));
                    list.Add(info2);
                }
                catch (FileNotFoundException)
                {
                }
                catch (TypeLoadException)
                {
                }
                catch (ModuleReaderException)
                {
                }
            }
            MemberInfo[] array = new MemberInfo[list.Count];
            list.CopyTo(array, 0);
            return array;
        }

        public string[] GetReferencedModules()
        {
            ModuleReferenceRow[] moduleReference = this.metaData.Tables.ModuleReference;
            FileRow[] file = this.metaData.Tables.File;
            ArrayList list = new ArrayList();
            for (int i = 0; i < moduleReference.Length; i++)
            {
                string str = this.metaData.GetString(moduleReference[i].Name);
                list.Add(str);
                for (int j = 0; j < file.Length; j++)
                {
                    if ((file[j].Flags == 0) && (this.metaData.GetString(file[j].Name).ToLower(CultureInfo.InvariantCulture) == str.ToLower(CultureInfo.InvariantCulture)))
                    {
                        list.Remove(str);
                    }
                }
            }
            string[] array = new string[list.Count];
            list.CopyTo(array, 0);
            return array;
        }

        internal object ResolveToken(int token)
        {
            return this.metaData.ResolveToken(token);
        }

        // Properties
        public Module Module
        {
            get
            {
                return this.metaData.Module;
            }
        }

        // Nested Types
        [StructLayout(LayoutKind.Sequential)]
        private struct AssemblyReferenceRow
        {
            public ushort MajorVersion;
            public ushort MinorVersion;
            public ushort BuildNumber;
            public ushort RevisionNumber;
            public uint Flags;
            public int PublicKeyOrToken;
            public int Name;
            public int Culture;
            public int HashValue;
            public AssemblyReferenceRow(ModuleReader.ITableReader reader)
            {
                this.MajorVersion = reader.ReadUInt16();
                this.MinorVersion = reader.ReadUInt16();
                this.BuildNumber = reader.ReadUInt16();
                this.RevisionNumber = reader.ReadUInt16();
                this.Flags = reader.ReadUInt32();
                this.PublicKeyOrToken = reader.ReadByteArray();
                this.Name = reader.ReadString();
                this.Culture = reader.ReadString();
                this.HashValue = reader.ReadByteArray();
            }
        }

        private sealed class BinaryReader
        {
            // Fields
            private byte[] buffer;
            private int position;

            // Methods
            public BinaryReader(string fileName)
            {
                Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                this.position = 0;
                this.buffer = new byte[(uint)stream.Length];
                stream.Read(this.buffer, 0, (int)stream.Length);
                stream.Close();
            }

            public BinaryReader(byte[] buffer)
            {
                this.position = 0;
                this.buffer = buffer;
            }

            public bool ReadBoolean()
            {
                return (this.buffer[this.position++] != 0);
            }

            public byte ReadByte()
            {
                return this.buffer[this.position++];
            }

            public byte[] ReadBytes(int count)
            {
                byte[] destinationArray = new byte[count];
                Array.Copy(this.buffer, this.position, destinationArray, 0, count);
                this.position += count;
                return destinationArray;
            }

            public char ReadChar()
            {
                return (char)this.ReadUInt16();
            }

            public double ReadDouble()
            {
                //MemoryStream input = new MemoryStream(this.ReadBytes(8));
                //BinaryReader reader = new BinaryReader(input);
                BinaryReader reader = new BinaryReader(this.ReadBytes(8));
                double num = reader.ReadDouble();
                //reader.Close();
                //input.Close();
                return num;
            }

            public short ReadInt16()
            {
                return (short)(this.buffer[this.position++] | (this.buffer[this.position++] << 8));
            }

            public int ReadInt32()
            {
                return (((this.buffer[this.position++] | (this.buffer[this.position++] << 8)) | (this.buffer[this.position++] << 0x10)) | (this.buffer[this.position++] << 0x18));
            }

            public long ReadInt64()
            {
                return (long)this.ReadUInt64();
            }

            public sbyte ReadSByte()
            {
                return (sbyte)this.buffer[this.position++];
            }

            public float ReadSingle()
            {
                //MemoryStream input = new MemoryStream(this.ReadBytes(4));
                //BinaryReader reader = new BinaryReader(input);
                BinaryReader reader = new BinaryReader(this.ReadBytes (4));
                float num = reader.ReadSingle();
                //reader.Close();
                //input.Close();
                return num;
            }

            public string ReadStringUtf8()
            {
                byte num3;
                int position = this.position;
                int num2 = 0;
            Label_000E:
                num3 = this.buffer[this.position++];
                if (num3 != 0)
                {
                    if ((num3 & 0x80) == 0)
                    {
                        num2++;
                        goto Label_000E;
                    }
                    byte num4 = this.buffer[this.position++];
                    if (num4 != 0)
                    {
                        if ((num3 & 0x20) != 0)
                        {
                            uint num6;
                            byte num5 = this.buffer[this.position++];
                            if (num5 == 0)
                            {
                                goto Label_010A;
                            }
                            if ((num3 & 0x10) == 0)
                            {
                                num6 = (uint)((((num3 & 15) << 12) | ((num4 & 0x3f) << 6)) | (num5 & 0x3f));
                            }
                            else
                            {
                                byte num7 = this.buffer[this.position++];
                                if (num7 == 0)
                                {
                                    num2++;
                                    goto Label_010A;
                                }
                                num6 = (uint)(((((num3 & 7) << 0x12) | ((num4 & 0x3f) << 12)) | ((num5 & 0x3f) << 6)) | (num7 & 0x3f));
                            }
                            if ((num6 & 0xffff0000) != 0)
                            {
                                num2++;
                            }
                        }
                        num2++;
                        goto Label_000E;
                    }
                }
            Label_010A:
                this.position = position;
                int num8 = 0;
                char[] chArray = new char[num2];
                while (true)
                {
                    byte num9 = this.buffer[this.position++];
                    if (num9 == 0)
                    {
                        break;
                    }
                    if ((num9 & 0x80) == 0)
                    {
                        chArray[num8++] = (char)num9;
                    }
                    else
                    {
                        char ch;
                        byte num10 = this.buffer[this.position++];
                        if (num10 == 0)
                        {
                            chArray[num8++] = (char)num9;
                            break;
                        }
                        if ((num9 & 0x20) == 0)
                        {
                            ch = (char)(((num9 & 0x1f) << 6) | (num10 & 0x3f));
                        }
                        else
                        {
                            uint num12;
                            byte num11 = this.buffer[this.position++];
                            if (num11 == 0)
                            {
                                chArray[num8++] = (char)((num9 << 8) | num10);
                                break;
                            }
                            if ((num9 & 0x10) == 0)
                            {
                                num12 = (uint)((((num9 & 15) << 12) | ((num10 & 0x3f) << 6)) | (num11 & 0x3f));
                            }
                            else
                            {
                                byte num13 = this.buffer[this.position++];
                                if (num13 == 0)
                                {
                                    chArray[num8++] = (char)((num9 << 8) | num10);
                                    chArray[num8++] = (char)num11;
                                    break;
                                }
                                num12 = (uint)(((((num9 & 7) << 0x12) | ((num10 & 0x3f) << 12)) | ((num11 & 0x3f) << 6)) | (num13 & 0x3f));
                            }
                            if ((num12 & 0xffff0000) == 0)
                            {
                                ch = (char)num12;
                            }
                            else
                            {
                                chArray[num8++] = (char)((num12 >> 10) | 0xd800);
                                ch = (char)((num12 & 0x3ff) | 0xdc00);
                            }
                        }
                        chArray[num8++] = ch;
                    }
                }
                return new string(chArray);
            }

            public ushort ReadUInt16()
            {
                return (ushort)(this.buffer[this.position++] | (this.buffer[this.position++] << 8));
            }

            public uint ReadUInt32()
            {
                return (uint)(((this.buffer[this.position++] | (this.buffer[this.position++] << 8)) | (this.buffer[this.position++] << 0x10)) | (this.buffer[this.position++] << 0x18));
            }

            public ulong ReadUInt64()
            {
                ulong num = this.ReadUInt32();
                ulong num2 = this.ReadUInt32();
                return (num | (num2 << 0x20));
            }

            // Properties
            public int Length
            {
                get
                {
                    return this.buffer.Length;
                }
            }

            public int Position
            {
                get
                {
                    return this.position;
                }
                set
                {
                    this.position = value;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Clause
        {
            public int Flags;
            public int TryOffset;
            public int TryLength;
            public int HandlerOffset;
            public int HandlerLength;
            public int Value;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FieldPointerRow
        {
            public int Field;
            public FieldPointerRow(ModuleReader.ITableReader reader)
            {
                this.Field = reader.ReadToken(4);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FieldRow
        {
            public ushort Flags;
            public int Name;
            public int Signature;
            public FieldRow(ModuleReader.ITableReader reader)
            {
                this.Flags = reader.ReadUInt16();
                this.Name = reader.ReadString();
                this.Signature = reader.ReadByteArray();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FileRow
        {
            public uint Flags;
            public int Name;
            public int HashValue;
            public FileRow(ModuleReader.ITableReader reader)
            {
                this.Flags = reader.ReadUInt32();
                this.Name = reader.ReadString();
                this.HashValue = reader.ReadByteArray();
            }
        }

        private interface ITableReader
        {
            // Methods
            int ReadByteArray();
            int ReadCodedToken(int type);
            int ReadGuid();
            int ReadString();
            int ReadToken(int type);
            ushort ReadUInt16();
            uint ReadUInt32();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MemberReferenceRow
        {
            public int DeclaringType;
            public int Name;
            public int Signature;
            public MemberReferenceRow(ModuleReader.ITableReader reader)
            {
                this.DeclaringType = reader.ReadCodedToken(0x47);
                this.Name = reader.ReadString();
                this.Signature = reader.ReadByteArray();
            }
        }

        private sealed class MetaData
        {
            // Fields
            private IAssemblyProvider assemblyProvider;
            private Assembly[] assemblyReferenceTable = null;
            private StreamHeader blobHeap;
            private bool debugTokenEncoding = false;
            private FieldInfo[] fieldDeclarationTable = null;
            private StreamHeader guidHeap;
            private int metaDataOffset;
            private IDictionary methodBaseToMethodDef = new Hashtable();
            private MethodBase[] methodDeclarationTable = null;
            private Module module;
            private int[] nestedTypeToEnclosingType = null;
            private ModuleReader.BinaryReader reader;
            private SectionHeaderTable sections;
            private StreamHeader stringHeap;
            private ModuleReader.TableManager tables = null;
            private Type[] typeDeclarationTable = null;
            private IDictionary typeNameDictionary = null;
            private Type[] typeReferenceTable = null;
            private Type[] typeSpecificationTable = null;
            private StreamHeader userStringHeap;

            // Methods
            public MetaData(ModuleReader.BinaryReader reader, Module moduleInstance, IAssemblyProvider assemblyProvider)
            {
                this.reader = reader;
                this.module = moduleInstance;
                this.assemblyProvider = assemblyProvider;
                DosHeader header = new DosHeader(reader);
                CoffHeader header2 = new CoffHeader(reader);
                PeHeader header3 = new PeHeader(reader);
                OptionalNtHeader header4 = new OptionalNtHeader(reader);
                DataDirectory directory = new DataDirectory(reader);
                DataDirectory directory2 = new DataDirectory(reader);
                DataDirectory directory3 = new DataDirectory(reader);
                DataDirectory directory4 = new DataDirectory(reader);
                DataDirectory directory5 = new DataDirectory(reader);
                DataDirectory directory6 = new DataDirectory(reader);
                DataDirectory directory7 = new DataDirectory(reader);
                DataDirectory directory8 = new DataDirectory(reader);
                DataDirectory directory9 = new DataDirectory(reader);
                DataDirectory directory10 = new DataDirectory(reader);
                DataDirectory directory11 = new DataDirectory(reader);
                DataDirectory directory12 = new DataDirectory(reader);
                DataDirectory directory13 = new DataDirectory(reader);
                DataDirectory directory14 = new DataDirectory(reader);
                DataDirectory directory15 = new DataDirectory(reader);
                DataDirectory directory16 = new DataDirectory(reader);
                for (int i = 0; i < (header4.NumberOfDataDirectories - 0x10); i++)
                {
                    new DataDirectory(reader);
                }
                this.sections = new SectionHeaderTable();
                for (int j = 0; j < header2.NumberOfSections; j++)
                {
                    SectionHeader header5 = new SectionHeader(reader);
                    this.sections[header5.Name] = header5;
                }
                reader.Position = this.sections.RvaToVa(directory15.Rva);
                CliHeader header6 = new CliHeader(reader);
                this.metaDataOffset = (header6.MetaData.Size == 0) ? -1 : this.sections.RvaToVa(header6.MetaData.Rva);
                this.reader.Position = this.metaDataOffset;
                MetaDataHeader header7 = new MetaDataHeader(reader);
                foreach (StreamHeader header8 in header7.StreamHeaders)
                {
                    switch (header8.Name)
                    {
                        case "#-":
                        case "#~":
                            if (this.tables != null)
                            {
                                throw new ModuleReaderException("Multiple table heaps.");
                            }
                            reader.Position = this.metaDataOffset + header8.Offset;
                            this.tables = new ModuleReader.TableManager(reader);
                            this.debugTokenEncoding = header8.Name == "#-";
                            break;

                        case "#Strings":
                            if (this.stringHeap != null)
                            {
                                throw new ModuleReaderException("Multiple #Strings heaps.");
                            }
                            this.stringHeap = header8;
                            break;

                        case "#US":
                            if (this.userStringHeap != null)
                            {
                                throw new ModuleReaderException("Multiple #US heaps.");
                            }
                            this.userStringHeap = header8;
                            break;

                        case "#GUID":
                            if (this.guidHeap != null)
                            {
                                throw new ModuleReaderException("Multiple #GUID heaps.");
                            }
                            this.guidHeap = header8;
                            break;

                        case "#Blob":
                            if (this.blobHeap != null)
                            {
                                throw new ModuleReaderException("Multiple #Blob heaps.");
                            }
                            this.blobHeap = header8;
                            break;

                        default:
                            throw new ModuleReaderException("Invalid meta-data stream.");
                    }
                }
            }

            private bool CheckFieldInfoSignature(FieldInfo fieldInfo, ModuleReader.BinaryReader reader)
            {
                if (this.DecodeInt32(reader) != 6)
                {
                    throw new ModuleReaderException("Invalid field signature.");
                }
                Type type = this.DecodeTypeSignature(reader);
                if (type == null)
                {
                    throw new ModuleReaderException("Invalid field signature.");
                }
                return (type == fieldInfo.FieldType);
            }

            private bool CheckMemberInfoSignature(MemberInfo memberInfo, byte[] signature)
            {
                bool flag = false;
                ModuleReader.BinaryReader reader = new ModuleReader.BinaryReader(signature);
                FieldInfo fieldInfo = memberInfo as FieldInfo;
                if (fieldInfo != null)
                {
                    flag = this.CheckFieldInfoSignature(fieldInfo, reader);
                }
                MethodBase methodBase = memberInfo as MethodBase;
                if (methodBase != null)
                {
                    flag = this.CheckMethodBaseSignature(methodBase, reader);
                }
                return flag;
            }

            private bool CheckMethodBaseSignature(MethodBase methodBase, ModuleReader.BinaryReader reader)
            {
                byte num = reader.ReadByte();
                int num2 = this.DecodeInt32(reader);
                ParameterInfo[] parameters = null;
                if (methodBase != null)
                {
                    int num3 = num & 240;
                    int num4 = num & 15;
                    if (num4 > 5)
                    {
                        throw new ModuleReaderException("Invalid method signature.");
                    }
                    CallingConventions callingConvention = methodBase.CallingConvention;
                    if ((num3 == 0x40) && ((callingConvention & CallingConventions.ExplicitThis) == 0))
                    {
                        return false;
                    }
                    if ((num3 == 0x20) && ((callingConvention & CallingConventions.HasThis) == 0))
                    {
                        return false;
                    }
                    if ((num4 == 5) && ((callingConvention & CallingConventions.Any) != CallingConventions.VarArgs))
                    {
                        return false;
                    }
                    if ((num4 == 0) && ((callingConvention & CallingConventions.Any) != CallingConventions.Standard))
                    {
                        return false;
                    }
                    parameters = methodBase.GetParameters();
                    if (num2 != parameters.Length)
                    {
                        return false;
                    }
                }
                Type type = this.DecodeTypeSignature(reader);
                MethodInfo info = methodBase as MethodInfo;
                if ((info != null) && !TypeInformation.Compare(info.ReturnType, type))
                {
                    return false;
                }
                for (int i = 0; i < num2; i++)
                {
                    if (parameters != null)
                    {
                        Type type2 = this.DecodeTypeSignature(reader);
                        if (!TypeInformation.Compare(parameters[i].ParameterType, type2))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            private int DecodeInt32(ModuleReader.BinaryReader reader)
            {
                int num = reader.ReadByte();
                if ((num & 0x80) == 0)
                {
                    return num;
                }
                if ((num & 0xc0) == 0x80)
                {
                    return (((num & 0x3f) << 8) | reader.ReadByte());
                }
                return (((((num & 0x3f) << 0x18) | (reader.ReadByte() << 0x10)) | (reader.ReadByte() << 8)) | reader.ReadByte());
            }

            public Type[] DecodeLocalVariableSignature(int token)
            {
                if (token == 0)
                {
                    return new Type[0];
                }
                int num = token >> 0x18;
                if (num != 0x11)
                {
                    throw new ModuleReaderException("Invalid signature token.");
                }
                int index = (token & 0xffffff) - 1;
                ModuleReader.BinaryReader reader = new ModuleReader.BinaryReader(this.GetByteArray(this.tables.Signature[index].Signature));
                if (this.DecodeInt32(reader) != 7)
                {
                    throw new ModuleReaderException("Invalid local variable signature.");
                }
                int num4 = this.DecodeInt32(reader);
                Type[] typeArray = new Type[num4];
                for (int i = 0; i < num4; i++)
                {
                    typeArray[i] = this.DecodeTypeSignature(reader);
                }
                return typeArray;
            }

            private int DecodeSignatureToken(ModuleReader.BinaryReader reader)
            {
                int num = this.DecodeInt32(reader);
                int[] numArray = ModuleReader.TableType.CodedTokenList(0x40);
                if (numArray == null)
                {
                    return -1;
                }
                int num2 = numArray[num & 3];
                int num3 = (num >> 2) - 1;
                if (num3 < 0)
                {
                    return -1;
                }
                return ((num2 << 0x18) | (num3 + 1));
            }

            private int DecodeToken(int token)
            {
                if (this.debugTokenEncoding)
                {
                    if (token == -1)
                    {
                        return -1;
                    }
                    int num = token >> 0x18;
                    int index = (token & 0xffffff) - 1;
                    if (index == -1)
                    {
                        return -1;
                    }
                    switch (num)
                    {
                        case 4:
                            if (this.tables.FieldPointer.Length != 0)
                            {
                                return this.tables.FieldPointer[index].Field;
                            }
                            return token;

                        case 5:
                            return token;

                        case 6:
                            if (this.tables.MethodPointer.Length != 0)
                            {
                                return this.tables.MethodPointer[index].Method;
                            }
                            return token;
                    }
                }
                return token;
            }

            private Type DecodeTypeSignature(ModuleReader.BinaryReader reader)
            {
                switch (((ElementType)this.DecodeInt32(reader)))
                {
                    case ElementType.Void:
                        return typeof(void);

                    case ElementType.Boolean:
                        return typeof(bool);

                    case ElementType.Char:
                        return typeof(char);

                    case ElementType.Int8:
                        return typeof(sbyte);

                    case ElementType.UInt8:
                        return typeof(byte);

                    case ElementType.Int16:
                        return typeof(short);

                    case ElementType.UInt16:
                        return typeof(ushort);

                    case ElementType.Int32:
                        return typeof(int);

                    case ElementType.UInt32:
                        return typeof(uint);

                    case ElementType.Int64:
                        return typeof(long);

                    case ElementType.UInt64:
                        return typeof(ulong);

                    case ElementType.Single:
                        return typeof(float);

                    case ElementType.Double:
                        return typeof(double);

                    case ElementType.String:
                        return typeof(string);

                    case ElementType.Pointer:
                        {
                            Type type2 = this.DecodeTypeSignature(reader);
                            if (type2 == null)
                            {
                                throw new ModuleReaderException("Unable to decode type of 'Ptr' signature.");
                            }
                            return type2.Assembly.GetType(type2.FullName + "*");
                        }
                    case ElementType.ByReference:
                        {
                            Type type3 = this.DecodeTypeSignature(reader);
                            if (type3 == null)
                            {
                                throw new ModuleReaderException("Unable to decode type of 'ByRef' signature.");
                            }
                            Type type = type3.Assembly.GetType(type3.FullName + "&");
                            if (type == null)
                            {
                                throw new ModuleReaderException("Unable to find type of 'ByRef' signature.");
                            }
                            return type;
                        }
                    case ElementType.ValueType:
                    case ElementType.Class:
                        {
                            int token = this.DecodeSignatureToken(reader);
                            if (token == -1)
                            {
                                throw new ModuleReaderException("Unable to decode value type or class signature.");
                            }
                            return this.GetType(token);
                        }
                    case ElementType.Array:
                        {
                            Type type5 = this.DecodeTypeSignature(reader);
                            if (type5 == null)
                            {
                                throw new ModuleReaderException("Unable to decode type of array.");
                            }
                            int num2 = this.DecodeInt32(reader);
                            int num3 = this.DecodeInt32(reader);
                            int[] numArray = new int[num3];
                            for (int i = 0; i < num3; i++)
                            {
                                numArray[i] = this.DecodeInt32(reader);
                            }
                            int num5 = this.DecodeInt32(reader);
                            int[] numArray2 = new int[num5];
                            for (int j = 0; j < num5; j++)
                            {
                                numArray2[j] = this.DecodeInt32(reader);
                            }
                            StringBuilder builder = new StringBuilder();
                            builder.Append(type5.FullName);
                            if (num2 > 0)
                            {
                                builder.Append("[");
                                for (int k = 0; k < num2; k++)
                                {
                                    bool flag = (k < num5) && (numArray2[k] > 0);
                                    bool flag2 = k < num3;
                                    if (flag && flag2)
                                    {
                                        builder.Append(numArray2[k] + ".." + ((numArray2[k] + numArray[k]) - 1));
                                    }
                                    if (flag && !flag2)
                                    {
                                        builder.Append(numArray2[k] + "...");
                                    }
                                    if (!flag && flag2)
                                    {
                                        builder.Append((int)(numArray[k] - 1));
                                    }
                                    if (k < (num2 - 1))
                                    {
                                        builder.Append(",");
                                    }
                                }
                                builder.Append("]");
                            }
                            Type type6 = type5.Assembly.GetType(builder.ToString());
                            if (type6 == null)
                            {
                                throw new ModuleReaderException("Unable to find array type in assembly.");
                            }
                            return type6;
                        }
                    case ElementType.TypedReference:
                        return typeof(TypedReference);

                    case ElementType.IntPtr:
                        return typeof(IntPtr);

                    case ElementType.UIntPtr:
                        return typeof(UIntPtr);

                    case ElementType.FunctionPointer:
                        this.CheckMethodBaseSignature(null, reader);
                        return typeof(IntPtr);

                    case ElementType.Object:
                        return typeof(object);

                    case ElementType.SzArray:
                        {
                            Type type7 = this.DecodeTypeSignature(reader);
                            if (type7 == null)
                            {
                                throw new ModuleReaderException("Unable to decode type of single-dimensional array signature.");
                            }
                            return type7.Assembly.GetType(type7.FullName + "[]");
                        }
                    case ElementType.RequiredModifier:
                    case ElementType.OptionalModifier:
                        {
                            int num8 = this.DecodeSignatureToken(reader);
                            return this.DecodeTypeSignature(reader);
                        }
                    case ElementType.Sentinel:
                        return this.DecodeTypeSignature(reader);

                    case ElementType.Pinned:
                        return this.DecodeTypeSignature(reader);
                }
                throw new ModuleReaderException("Unknown element type.");
            }

            private int EncodeToken(int token)
            {
                if (this.debugTokenEncoding)
                {
                    if (token == -1)
                    {
                        return -1;
                    }
                    switch ((token >> 0x18))
                    {
                        case 4:
                            {
                                ModuleReader.FieldPointerRow[] fieldPointer = this.tables.FieldPointer;
                                for (int i = 0; i < fieldPointer.Length; i++)
                                {
                                    if (fieldPointer[i].Field == token)
                                    {
                                        return (0x4000000 | (i + 1));
                                    }
                                }
                                return token;
                            }
                        case 5:
                            return token;

                        case 6:
                            {
                                ModuleReader.MethodPointerRow[] methodPointer = this.tables.MethodPointer;
                                for (int j = 0; j < methodPointer.Length; j++)
                                {
                                    if (methodPointer[j].Method == token)
                                    {
                                        return (0x6000000 | (j + 1));
                                    }
                                }
                                return token;
                            }
                    }
                }
                return token;
            }

            private Assembly GetAssembly(int token)
            {
                if (this.assemblyReferenceTable == null)
                {
                    this.assemblyReferenceTable = new Assembly[this.tables.AssemblyReference.Length];
                }
                int num = token >> 0x18;
                if (num != 0x23)
                {
                    throw new ModuleReaderException("Invalid assembly reference token.");
                }
                int index = (token & 0xffffff) - 1;
                if (this.assemblyReferenceTable[index] == null)
                {
                    AssemblyName assemblyReference = this.GetAssemblyReference(token);
                    if (this.assemblyProvider == null)
                    {
                        throw new NullReferenceException("assemblyProvider");
                    }
                    this.assemblyReferenceTable[index] = this.assemblyProvider.Load(assemblyReference.FullName);
                }
                return this.assemblyReferenceTable[index];
            }

            public AssemblyName GetAssemblyReference(int token)
            {
                int num = token >> 0x18;
                if (num != 0x23)
                {
                    throw new ModuleReaderException("Invalid assembly reference token.");
                }
                int index = (token & 0xffffff) - 1;
                ModuleReader.AssemblyReferenceRow row = this.Tables.AssemblyReference[index];
                AssemblyName name = new AssemblyName();
                name.Name = this.GetString(row.Name);
                name.Version = new Version((int)row.MajorVersion, (int)row.MinorVersion, (int)row.BuildNumber, (int)row.RevisionNumber);
                string str = this.GetString(row.Culture);
                name.CultureInfo = (str.Length == 0) ? CultureInfo.InvariantCulture : new CultureInfo(str);
                byte[] byteArray = this.GetByteArray(row.PublicKeyOrToken);
                name.SetPublicKeyToken(byteArray);
                return name;
            }

            public byte[] GetByteArray(int offset)
            {
                if (this.blobHeap == null)
                {
                    throw new ModuleReaderException("#Blob heap not found.");
                }
                if (offset >= this.blobHeap.Size)
                {
                    throw new ModuleReaderException("Invalid blob heap offset.");
                }
                this.reader.Position = (this.metaDataOffset + this.blobHeap.Offset) + offset;
                int count = this.DecodeInt32(this.reader);
                return this.reader.ReadBytes(count);
            }

            private int GetDeclaringTypeToken(int token)
            {
                if (this.nestedTypeToEnclosingType == null)
                {
                    this.nestedTypeToEnclosingType = new int[this.tables.Type.Length];
                    foreach (ModuleReader.NestedClassRow row in this.tables.NestedClass)
                    {
                        this.nestedTypeToEnclosingType[(row.Nested & 0xffffff) - 1] = row.Enclosing;
                    }
                }
                int index = (token & 0xffffff) - 1;
                return this.nestedTypeToEnclosingType[index];
            }

            private FieldInfo GetFieldDeclaration(int token)
            {
                if (this.fieldDeclarationTable == null)
                {
                    this.fieldDeclarationTable = new FieldInfo[this.tables.Field.Length];
                }
                int num = token >> 0x18;
                int index = (token & 0xffffff) - 1;
                if (this.fieldDeclarationTable[index] == null)
                {
                    ModuleReader.FieldRow row = this.tables.Field[index];
                    Type declaringType = this.TypeFromMemberDefToken(token, false);
                    MemberInfo info = this.GetMemberInfo(declaringType, this.GetString(row.Name), this.GetByteArray(row.Signature));
                    this.fieldDeclarationTable[index] = info as FieldInfo;
                }
                return this.fieldDeclarationTable[index];
            }

            public Guid GetGuid(int index)
            {
                if (index == 0)
                {
                    return Guid.Empty;
                }
                if (this.guidHeap == null)
                {
                    throw new ModuleReaderException("#GUID heap not found.");
                }
                int num = (index - 1) << 4;
                if (num >= this.guidHeap.Size)
                {
                    throw new ModuleReaderException("Invalid #GUID heap index.");
                }
                this.reader.Position = (this.metaDataOffset + this.guidHeap.Offset) + num;
                return new Guid(this.reader.ReadBytes(0x10));
            }

            private MemberInfo GetMemberInfo(Type declaringType, string name, byte[] signature)
            {
                MemberInfo info = this.GetMemberInfoRecursive(declaringType, name, signature);
                if (info != null)
                {
                    return info;
                }
                string str = name;
                if (declaringType != null)
                {
                    str = declaringType.FullName + "." + str;
                }
                throw new ModuleReaderException("Member '" + str + "' cannot be resolved.");
            }

            private MemberInfo GetMemberInfoRecursive(Type declaringType, string name, byte[] signature)
            {
                if (((signature != null) && (signature.Length > 0)) && (signature[0] == 6))
                {
                    if (declaringType != null)
                    {
                        return declaringType.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    }
                    return this.module.GetField(name);
                }
                if (declaringType != null)
                {
                    MemberInfo[] member = declaringType.GetMember(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    for (int i = 0; i < member.Length; i++)
                    {
                        MethodBase memberInfo = member[i] as MethodBase;
                        if ((memberInfo != null) && this.CheckMemberInfoSignature(memberInfo, signature))
                        {
                            return memberInfo;
                        }
                    }
                }
                else
                {
                    foreach (MethodInfo info in this.module.GetMethods())
                    {
                        if ((name == info.Name) && this.CheckMemberInfoSignature(info, signature))
                        {
                            return info;
                        }
                    }
                }
                if ((declaringType != null) && (declaringType.BaseType != null))
                {
                    return this.GetMemberInfoRecursive(declaringType.BaseType, name, signature);
                }
                return null;
            }

            private MemberInfo GetMemberReference(int token)
            {
                int num = token >> 0x18;
                int index = (token & 0xffffff) - 1;
                ModuleReader.MemberReferenceRow row = this.tables.MemberReference[index];
                int declaringType = row.DeclaringType;
                switch ((declaringType >> 0x18))
                {
                    case 0:
                        throw new ModuleReaderException("Module definition is not supported in member references.");

                    case 1:
                    case 2:
                    case 0x1b:
                        {
                            Type type = this.GetType(declaringType);
                            return this.GetMemberInfo(type, this.GetString(row.Name), this.GetByteArray(row.Signature));
                        }
                    case 6:
                        throw new ModuleReaderException("Method definition is not supported in member references.");
                }
                throw new ModuleReaderException("Unknown parent in member reference.");
            }

            public ModuleReader.MethodBodyReader GetMethodBodyReader(uint rva)
            {
                this.reader.Position = this.sections.RvaToVa(rva);
                return new ModuleReader.MethodBodyReader(this.reader);
            }

            private MethodBase GetMethodDeclaration(int token)
            {
                if (this.methodDeclarationTable == null)
                {
                    this.methodDeclarationTable = new MethodBase[this.tables.Method.Length];
                }
                int num = token >> 0x18;
                if (num != 6)
                {
                    throw new ModuleReaderException("Invalid method token.");
                }
                int index = (token & 0xffffff) - 1;
                if (this.methodDeclarationTable[index] == null)
                {
                    ModuleReader.MethodRow row = this.tables.Method[index];
                    Type declaringType = this.TypeFromMemberDefToken(token, true);
                    MemberInfo info = this.GetMemberInfo(declaringType, this.GetString(row.Name), this.GetByteArray(row.Signature));
                    this.methodDeclarationTable[index] = info as MethodBase;
                }
                return this.methodDeclarationTable[index];
            }

            public uint GetMethodRva(MethodBase methodBase)
            {
                int methodToken = this.GetMethodToken(methodBase);
                int index = (this.DecodeToken(methodToken) & 0xffffff) - 1;
                return this.tables.Method[index].Rva;
            }

            private int GetMethodToken(MethodBase methodBase)
            {
                if (this.methodBaseToMethodDef.Contains(methodBase))
                {
                    return (int)this.methodBaseToMethodDef[methodBase];
                }
                Type declaringType = methodBase.DeclaringType;
                int num = (declaringType != null) ? this.GetTypeToken(declaringType) : 0x2000001;
                int index = (num & 0xffffff) - 1;
                ModuleReader.TypeRow row = this.tables.Type[index];
                int methods = row.Methods;
                int num4 = 0x6000000 | (this.tables.Method.Length + 1);
                int num5 = 0x2000000 | (this.tables.Type.Length + 1);
                int num6 = num + 1;
                if (num6 < num5)
                {
                    int num7 = (num6 & 0xffffff) - 1;
                    ModuleReader.TypeRow row2 = this.tables.Type[num7];
                    num4 = row2.Methods;
                }
                for (int i = methods; i < num4; i++)
                {
                    int num9 = (this.DecodeToken(i) & 0xffffff) - 1;
                    ModuleReader.MethodRow row3 = this.tables.Method[num9];
                    if ((methodBase.Name == this.GetString(row3.Name)) && this.CheckMemberInfoSignature(methodBase, this.GetByteArray(row3.Signature)))
                    {
                        this.methodBaseToMethodDef[methodBase] = i;
                        return i;
                    }
                }
                throw new ModuleReaderException("Unable to find token for method.");
            }

            private Module GetModuleReference(int token)
            {
                int num = token >> 0x18;
                int index = (token & 0xffffff) - 1;
                if (num != 0x1a)
                {
                    throw new ModuleReaderException("Invalid module reference token.");
                }
                ModuleReader.ModuleReferenceRow row = this.tables.ModuleReference[index];
                string name = this.GetString(row.Name);
                Module module = this.Module.Assembly.GetModule(name);
                if (module == null)
                {
                    throw new ModuleReaderException("Unable to load module '" + name + "'.");
                }
                return module;
            }

            public string GetString(int offset)
            {
                if (this.stringHeap == null)
                {
                    throw new ModuleReaderException("#Strings heap not found.");
                }
                if (offset >= this.stringHeap.Size)
                {
                    throw new ModuleReaderException("Invalid #Strings heap index.");
                }
                this.reader.Position = (this.metaDataOffset + this.stringHeap.Offset) + offset;
                return this.reader.ReadStringUtf8();
            }

            public Type GetType(int token)
            {
                if (token == 0x2000001)
                {
                    return null;
                }
                switch ((token >> 0x18))
                {
                    case 1:
                        return this.GetTypeReference(token);

                    case 2:
                        return this.GetTypeDeclaration(token);

                    case 0x1b:
                        return this.GetTypeSpecification(token);
                }
                throw new ModuleReaderException("Type is not a definition, reference or specification.");
            }

            private Type GetTypeDeclaration(int token)
            {
                if (this.typeDeclarationTable == null)
                {
                    this.typeDeclarationTable = new Type[this.tables.Type.Length];
                }
                int num = token >> 0x18;
                int index = (token & 0xffffff) - 1;
                if (index == -1)
                {
                    return null;
                }
                if (this.typeDeclarationTable[index] == null)
                {
                    ModuleReader.TypeRow row = this.tables.Type[index];
                    int declaringTypeToken = this.GetDeclaringTypeToken(token);
                    Type type = (declaringTypeToken == 0) ? null : this.GetTypeDeclaration(declaringTypeToken);
                    Type nestedType = null;
                    string name = this.GetString(row.Name);
                    if (type != null)
                    {
                        nestedType = type.GetNestedType(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                    }
                    else
                    {
                        string str2 = this.GetString(row.Namespace);
                        if (str2.Length != 0)
                        {
                            str2 = str2 + ".";
                        }
                        nestedType = this.module.GetType(str2 + name);
                    }
                    this.typeDeclarationTable[index] = nestedType;
                }
                return this.typeDeclarationTable[index];
            }

            private Type GetTypeReference(int token)
            {
                if (this.typeReferenceTable == null)
                {
                    this.typeReferenceTable = new Type[this.tables.TypeReference.Length];
                }
                int num = token >> 0x18;
                int index = (token & 0xffffff) - 1;
                if (this.typeReferenceTable[index] == null)
                {
                    ModuleReader.TypeReferenceRow row = this.tables.TypeReference[index];
                    switch ((row.ResolutionScope >> 0x18))
                    {
                        case 0:
                            throw new ModuleReaderException("Type references scoped in module definitions are not supported.");

                        case 1:
                            {
                                Type typeReference = this.GetTypeReference(row.ResolutionScope);
                                if (typeReference == null)
                                {
                                    throw new ModuleReaderException("Unable to resolve declaring type of type reference.");
                                }
                                this.typeReferenceTable[index] = typeReference.GetNestedType(this.GetString(row.Name), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                                break;
                            }
                        case 0x1a:
                            {
                                Module moduleReference = this.GetModuleReference(row.ResolutionScope);
                                string str = this.GetString(row.Namespace);
                                if (str.Length != 0)
                                {
                                    str = str + ".";
                                }
                                this.typeReferenceTable[index] = moduleReference.GetType(str + this.GetString(row.Name));
                                break;
                            }
                        case 0x23:
                            {
                                Assembly assembly = this.GetAssembly(row.ResolutionScope);
                                if (assembly == null)
                                {
                                    throw new ModuleReaderException("Unable to find assembly reference.");
                                }
                                string str2 = this.GetString(row.Namespace);
                                if (str2.Length != 0)
                                {
                                    str2 = str2 + ".";
                                }
                                this.typeReferenceTable[index] = assembly.GetType(str2 + this.GetString(row.Name));
                                break;
                            }
                        default:
                            throw new ModuleReaderException("Unkown type reference resolution scope.");
                    }
                }
                return this.typeReferenceTable[index];
            }

            private Type GetTypeSpecification(int token)
            {
                if (this.typeSpecificationTable == null)
                {
                    this.typeSpecificationTable = new Type[this.tables.TypeSpecification.Length];
                }
                int num = token >> 0x18;
                int index = (token & 0xffffff) - 1;
                if (this.typeSpecificationTable[index] == null)
                {
                    ModuleReader.TypeSpecificationRow row = this.tables.TypeSpecification[index];
                    ModuleReader.BinaryReader reader = new ModuleReader.BinaryReader(this.GetByteArray(row.Signature));
                    Type type = this.DecodeTypeSignature(reader);
                    if (type == null)
                    {
                        throw new ModuleReaderException("Unable to decode type specification.");
                    }
                    this.typeSpecificationTable[index] = type;
                }
                return this.typeSpecificationTable[index];
            }

            private int GetTypeToken(Type type)
            {
                if (this.typeNameDictionary == null)
                {
                    this.typeNameDictionary = new Hashtable();
                    ModuleReader.TypeRow[] rowArray = this.tables.Type;
                    for (int i = 0; i < rowArray.Length; i++)
                    {
                        string key = string.Empty;
                        int index = i;
                        if (((rowArray[index].Flags & 0x800) == 0) || (this.GetString(rowArray[index].Name) != "_Deleted"))
                        {
                            int declaringTypeToken = 0;
                            do
                            {
                                ModuleReader.TypeRow row = rowArray[index];
                                declaringTypeToken = this.GetDeclaringTypeToken(0x2000000 | (index + 1));
                                if (declaringTypeToken == 0)
                                {
                                    string str2 = this.GetString(row.Namespace);
                                    if (str2.Length != 0)
                                    {
                                        str2 = str2 + ".";
                                    }
                                    key = str2 + this.GetString(row.Name) + key;
                                }
                                else
                                {
                                    key = "+" + this.GetString(row.Name) + key;
                                    index = (declaringTypeToken & 0xffffff) - 1;
                                }
                            }
                            while (declaringTypeToken != 0);
                            int num4 = 0x2000000 | (i + 1);
                            this.typeNameDictionary.Add(key, num4);
                        }
                    }
                }
                return (int)this.typeNameDictionary[type.FullName];
            }

            public string GetUserString(int offset)
            {
                if (this.userStringHeap == null)
                {
                    throw new ModuleReaderException("#US heap not found.");
                }
                if (offset == 0)
                {
                    throw new ModuleReaderException("Invalid #US heap offset.");
                }
                this.reader.Position = (this.metaDataOffset + this.userStringHeap.Offset) + offset;
                int count = (this.DecodeInt32(this.reader) >> 1) << 1;
                if (count == 0)
                {
                    return string.Empty;
                }
                byte[] bytes = this.reader.ReadBytes(count);
                return new UnicodeEncoding().GetString(bytes);
            }

            public MemberInfo ResolveToken(int token)
            {
                int num = token >> 0x18;
                if (num <= 10)
                {
                    switch (num)
                    {
                        case 1:
                        case 2:
                            goto Label_003D;

                        case 3:
                        case 5:
                            goto Label_0069;

                        case 4:
                            return this.GetFieldDeclaration(token);

                        case 6:
                            return this.GetMethodDeclaration(token);

                        case 10:
                            return this.GetMemberReference(token);
                    }
                    goto Label_0069;
                }
                if (num == 0x11)
                {
                    return null;
                }
                if (num != 0x1b)
                {
                    goto Label_0069;
                }
            Label_003D:
                return this.GetType(token);
            Label_0069:
                throw new ModuleReaderException("Token " + token.ToString("X8", CultureInfo.InvariantCulture) + " cannot be reolved.");
            }

            private Type TypeFromMemberDefToken(int token, bool methodOrField)
            {
                token = this.EncodeToken(token);
                ModuleReader.TypeRow[] type = this.tables.Type;
                for (int i = 0; i < type.Length; i++)
                {
                    ModuleReader.TypeRow row = type[i];
                    if (token >= (methodOrField ? row.Methods : row.Fields))
                    {
                        int num2 = 0x2000000 | (i + 1);
                        if (i == (type.Length - 1))
                        {
                            return this.GetType(num2);
                        }
                        ModuleReader.TypeRow row2 = type[i + 1];
                        if (token < (methodOrField ? row2.Methods : row2.Fields))
                        {
                            return this.GetType(num2);
                        }
                    }
                }
                throw new ModuleReaderException("Type definition cannot be resolved.");
            }

            // Properties
            public Module Module
            {
                get
                {
                    return this.module;
                }
            }

            public ModuleReader.TableManager Tables
            {
                get
                {
                    return this.tables;
                }
            }

            // Nested Types
            private sealed class CliHeader
            {
                // Fields
                public ModuleReader.MetaData.DataDirectory CodeManagerTable;
                public uint EntryPointToken = 0;
                public ModuleReader.MetaData.DataDirectory ExportAddressTableJumps;
                public uint Flags = 1;
                public ushort MajorRuntimeVersion = 2;
                public ModuleReader.MetaData.DataDirectory ManagedNativeHeader;
                public ModuleReader.MetaData.DataDirectory MetaData;
                public ushort MinorRuntimeVersion = 0;
                public ModuleReader.MetaData.DataDirectory Resources;
                public uint Size = 0x48;
                public ModuleReader.MetaData.DataDirectory StrongNameSignature;
                public ModuleReader.MetaData.DataDirectory VTableFixups;

                // Methods
                public CliHeader(ModuleReader.BinaryReader reader)
                {
                    this.Size = reader.ReadUInt32();
                    this.MajorRuntimeVersion = reader.ReadUInt16();
                    this.MinorRuntimeVersion = reader.ReadUInt16();
                    this.MetaData = new ModuleReader.MetaData.DataDirectory(reader);
                    this.Flags = reader.ReadUInt32();
                    this.EntryPointToken = reader.ReadUInt32();
                    this.Resources = new ModuleReader.MetaData.DataDirectory(reader);
                    this.StrongNameSignature = new ModuleReader.MetaData.DataDirectory(reader);
                    this.CodeManagerTable = new ModuleReader.MetaData.DataDirectory(reader);
                    this.VTableFixups = new ModuleReader.MetaData.DataDirectory(reader);
                    this.ExportAddressTableJumps = new ModuleReader.MetaData.DataDirectory(reader);
                    this.ManagedNativeHeader = new ModuleReader.MetaData.DataDirectory(reader);
                }
            }

            private sealed class CoffHeader
            {
                // Fields
                public ushort Characteristics = 270;
                public ushort Machine = 0x14c;
                public ushort NumberOfSections = 0;
                public uint NumberOfSymbols = 0;
                public ushort OptionalHeaderSize = 0xe0;
                public uint PointerToSymbolTable = 0;
                public uint Signature = 0x4550;
                public uint TimeDateStamp = 0;

                // Methods
                public CoffHeader(ModuleReader.BinaryReader reader)
                {
                    this.Signature = reader.ReadUInt32();
                    this.Machine = reader.ReadUInt16();
                    this.NumberOfSections = reader.ReadUInt16();
                    this.TimeDateStamp = reader.ReadUInt32();
                    this.PointerToSymbolTable = reader.ReadUInt32();
                    this.NumberOfSymbols = reader.ReadUInt32();
                    this.OptionalHeaderSize = reader.ReadUInt16();
                    this.Characteristics = reader.ReadUInt16();
                    if ((this.Signature != 0x4550) || (this.Machine != 0x14c))
                    {
                        throw new ModuleReaderException("Invalid COFF header.");
                    }
                    if (this.OptionalHeaderSize != 0xe0)
                    {
                        throw new ModuleReaderException("Invalid optional header size in COFF header.");
                    }
                    if ((this.PointerToSymbolTable != 0) || (this.NumberOfSymbols != 0))
                    {
                        throw new ModuleReaderException("Invalid symbol table in COFF header.");
                    }
                }
            }

            private sealed class DataDirectory
            {
                // Fields
                public uint Rva;
                public int Size;

                // Methods
                public DataDirectory(ModuleReader.BinaryReader reader)
                {
                    this.Rva = reader.ReadUInt32();
                    this.Size = reader.ReadInt32();
                }
            }

            private sealed class DosHeader
            {
                // Methods
                public DosHeader(ModuleReader.BinaryReader reader)
                {
                    int position = reader.Position;
                    byte[] buffer = reader.ReadBytes(0x80);
                    if ((buffer[0] != 0x4d) || (buffer[1] != 90))
                    {
                        throw new ModuleReaderException("Invalid DOS header.");
                    }
                    int num2 = ((buffer[60] | (buffer[0x3d] << 8)) | (buffer[0x3e] << 0x10)) | (buffer[0x3f] << 0x18);
                    reader.Position = position + num2;
                }
            }

            private enum ElementType
            {
                Array = 20,
                Boolean = 2,
                ByReference = 0x10,
                Char = 3,
                Class = 0x12,
                Double = 13,
                End = 0,
                FunctionPointer = 0x1b,
                Int16 = 6,
                Int32 = 8,
                Int64 = 10,
                Int8 = 4,
                Internal = 0x21,
                IntPtr = 0x18,
                Object = 0x1c,
                OptionalModifier = 0x20,
                Pinned = 0x45,
                Pointer = 15,
                RequiredModifier = 0x1f,
                Sentinel = 0x41,
                Single = 12,
                String = 14,
                SzArray = 0x1d,
                TypedReference = 0x16,
                UInt16 = 7,
                UInt32 = 9,
                UInt64 = 11,
                UInt8 = 5,
                UIntPtr = 0x19,
                ValueType = 0x11,
                Void = 1
            }

            private sealed class MetaDataHeader
            {
                // Fields
                public int Flags;
                public ushort MajorVersion;
                public ushort MinorVersion;
                public uint Reserved;
                public uint Signature;
                public ModuleReader.MetaData.StreamHeader[] StreamHeaders;
                public string VersionString;

                // Methods
                public MetaDataHeader(ModuleReader.BinaryReader reader)
                {
                    this.Signature = reader.ReadUInt32();
                    this.MajorVersion = reader.ReadUInt16();
                    this.MinorVersion = reader.ReadUInt16();
                    this.Reserved = reader.ReadUInt32();
                    int count = reader.ReadInt32();
                    this.VersionString = new ASCIIEncoding().GetString(reader.ReadBytes(count)).Trim(new char[1]);
                    while ((count++ % 4) != 0)
                    {
                        reader.ReadByte();
                    }
                    this.Flags = reader.ReadUInt16();
                    if (this.Signature != 0x424a5342)
                    {
                        throw new ModuleReaderException("Invalid meta-data header.");
                    }
                    if ((this.MajorVersion != 1) || (this.MinorVersion != 1))
                    {
                        throw new ModuleReaderException("Invalid meta-data version.");
                    }
                    this.StreamHeaders = new ModuleReader.MetaData.StreamHeader[(int)reader.ReadUInt16()];
                    for (int i = 0; i < this.StreamHeaders.Length; i++)
                    {
                        this.StreamHeaders[i] = new ModuleReader.MetaData.StreamHeader(reader);
                    }
                }
            }

            private sealed class OptionalNtHeader
            {
                // Fields
                public ushort DllCharacteristics = 0;
                public uint FileAlignment = 0x200;
                public uint FileChecksum = 0;
                public uint HeaderSize = 0;
                public uint HeapCommitSize = 0x1000;
                public uint HeapReserveSize = 0x100000;
                public uint ImageBase = 0x400000;
                public uint ImageSize = 0;
                public uint LoaderFlags = 0;
                public ushort MajorImageVersion = 0;
                public ushort MajorOperatingSystemVersion = 4;
                public ushort MajorSubsystemVersion = 4;
                public ushort MinorImageVersion = 0;
                public ushort MinorOperatingSystemVersion = 0;
                public ushort MinorSubsystemVersion = 0;
                public uint NumberOfDataDirectories = 0x10;
                public uint Reserved;
                public uint SectionAlignment = 0x2000;
                public uint StackCommitSize = 0x1000;
                public uint StackReserveSize = 0x100000;
                public ushort SubSystem = 0;

                // Methods
                public OptionalNtHeader(ModuleReader.BinaryReader reader)
                {
                    this.ImageBase = reader.ReadUInt32();
                    this.SectionAlignment = reader.ReadUInt32();
                    this.FileAlignment = reader.ReadUInt32();
                    this.MajorOperatingSystemVersion = reader.ReadUInt16();
                    this.MinorOperatingSystemVersion = reader.ReadUInt16();
                    this.MajorImageVersion = reader.ReadUInt16();
                    this.MinorImageVersion = reader.ReadUInt16();
                    this.MajorSubsystemVersion = reader.ReadUInt16();
                    this.MinorSubsystemVersion = reader.ReadUInt16();
                    this.Reserved = reader.ReadUInt32();
                    this.ImageSize = reader.ReadUInt32();
                    this.HeaderSize = reader.ReadUInt32();
                    this.FileChecksum = reader.ReadUInt32();
                    this.SubSystem = reader.ReadUInt16();
                    this.DllCharacteristics = reader.ReadUInt16();
                    this.StackReserveSize = reader.ReadUInt32();
                    this.StackCommitSize = reader.ReadUInt32();
                    this.HeapReserveSize = reader.ReadUInt32();
                    this.HeapCommitSize = reader.ReadUInt32();
                    this.LoaderFlags = reader.ReadUInt32();
                    this.NumberOfDataDirectories = reader.ReadUInt32();
                    if ((this.FileAlignment != 0x200) && (this.FileAlignment != 0x1000))
                    {
                        throw new ModuleReaderException("Invalid file alignment in NT header.");
                    }
                    if (this.NumberOfDataDirectories < 0x10)
                    {
                        throw new ModuleReaderException("Invalid number of data directories in NT header.");
                    }
                }
            }

            private sealed class PeHeader
            {
                // Fields
                public int BaseOfCode = 0x2000;
                public int BaseOfData = 0;
                public int CodeSize = 0;
                public int EntryPointRva = 0;
                public int InitializedDataSize = 0x200;
                public ushort Magic = 0x10b;
                public byte MajorLinkerVersion = 6;
                public byte MinorLinkerVersion = 0;
                public int UninitializedDataSize = 0;

                // Methods
                public PeHeader(ModuleReader.BinaryReader reader)
                {
                    this.Magic = reader.ReadUInt16();
                    if (((this.Magic != 0x10b) || (this.MajorLinkerVersion < 6)) || (this.MinorLinkerVersion != 0))
                    {
                        throw new ModuleReaderException("Invalid PE header.");
                    }
                    this.MajorLinkerVersion = reader.ReadByte();
                    this.MinorLinkerVersion = reader.ReadByte();
                    this.CodeSize = reader.ReadInt32();
                    this.InitializedDataSize = reader.ReadInt32();
                    this.UninitializedDataSize = reader.ReadInt32();
                    this.EntryPointRva = reader.ReadInt32();
                    this.BaseOfCode = reader.ReadInt32();
                    this.BaseOfData = reader.ReadInt32();
                }
            }

            private sealed class SectionHeader
            {
                // Fields
                public int Characteristics;
                public string Name;
                public ushort NumberOfLineNumbers;
                public ushort NumberOfRelocations;
                public int PointerToLineNumbers;
                public int PointerToRawData;
                public int PointerToRelocations;
                public int SizeOfRawData;
                public int VirtualAddress;
                public int VirtualSize;

                // Methods
                public SectionHeader(ModuleReader.BinaryReader reader)
                {
                    this.Name = new ASCIIEncoding().GetString(reader.ReadBytes(8)).Trim(new char[1]);
                    this.VirtualSize = reader.ReadInt32();
                    this.VirtualAddress = reader.ReadInt32();
                    this.SizeOfRawData = reader.ReadInt32();
                    this.PointerToRawData = reader.ReadInt32();
                    this.PointerToRelocations = reader.ReadInt32();
                    this.PointerToLineNumbers = reader.ReadInt32();
                    this.NumberOfRelocations = reader.ReadUInt16();
                    this.NumberOfLineNumbers = reader.ReadUInt16();
                    this.Characteristics = reader.ReadInt32();
                }
            }

            private sealed class SectionHeaderTable
            {
                // Fields
                private IDictionary table = new Hashtable();

                // Methods
                public int RvaToVa(uint rva)
                {
                    IDictionaryEnumerator enumerator = this.table.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        ModuleReader.MetaData.SectionHeader header = (ModuleReader.MetaData.SectionHeader)enumerator.Value;
                        if ((rva >= header.VirtualAddress) && (rva < (header.VirtualAddress + header.SizeOfRawData)))
                        {
                            return (header.PointerToRawData + (((int)rva) - header.VirtualAddress));
                        }
                    }
                    throw new ModuleReaderException("Invalid RVA address '0x" + rva.ToString("x8", CultureInfo.InvariantCulture) + "'.");
                }

                // Properties
                public ModuleReader.MetaData.SectionHeader this[string name]
                {
                    get
                    {
                        return (ModuleReader.MetaData.SectionHeader)this.table[name];
                    }
                    set
                    {
                        this.table[name] = value;
                    }
                }
            }

            private sealed class StreamHeader
            {
                // Fields
                public string Name;
                public int Offset;
                public int Size;

                // Methods
                public StreamHeader(ModuleReader.BinaryReader reader)
                {
                    this.Offset = reader.ReadInt32();
                    this.Size = reader.ReadInt32();
                    byte[] bytes = new byte[0x40];
                    int num = 0;
                    byte num2 = 0;
                    while ((num2 = reader.ReadByte()) != 0)
                    {
                        bytes[num++] = num2;
                    }
                    num++;
                    int count = ((num % 4) != 0) ? (4 - (num % 4)) : 0;
                    reader.ReadBytes(count);
                    this.Name = new UTF8Encoding().GetString(bytes).Trim(new char[1]);
                }
            }

            private sealed class TypeInformation
            {
                // Methods
                public static bool Compare(Type t1, Type t2)
                {
                    if (t1 != t2)
                    {
                        if ((t1 == null) || (t2 == null))
                        {
                            return false;
                        }
                        if (t1.Name != t2.Name)
                        {
                            return false;
                        }
                        if (t1.FullName != t2.FullName)
                        {
                            return false;
                        }
                        if (t1.Assembly.FullName != t2.Assembly.FullName)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
        }

        private sealed class MethodBodyReader
        {
            // Fields
            private byte[] code = new byte[0];
            private int localVarSigToken = 0;
            private ushort maxStack = 0;
            private Section section = null;

            // Methods
            public MethodBodyReader(ModuleReader.BinaryReader reader)
            {
                byte num = reader.ReadByte();
                if ((num & 3) == 2)
                {
                    this.maxStack = 8;
                    int count = num >> 2;
                    this.code = reader.ReadBytes(count);
                }
                else
                {
                    if ((num & 3) != 3)
                    {
                        throw new ModuleReaderException("Unknown method header format.");
                    }
                    byte num3 = reader.ReadByte();
                    byte num4 = (byte)(num3 >> 4);
                    ushort num5 = (ushort)(num | ((num3 & 15) << 8));
                    this.maxStack = reader.ReadUInt16();
                    int num6 = reader.ReadInt32();
                    this.localVarSigToken = reader.ReadInt32();
                    this.code = reader.ReadBytes(num6);
                    if ((num5 & 8) != 0)
                    {
                        this.section = new Section(reader);
                    }
                }
            }

            // Properties
            public ModuleReader.Clause[] Clauses
            {
                get
                {
                    ArrayList list = new ArrayList();
                    for (Section section = this.section; section != null; section = section.Next)
                    {
                        list.AddRange(section.Clauses);
                    }
                    ModuleReader.Clause[] array = new ModuleReader.Clause[list.Count];
                    list.CopyTo(array, 0);
                    return array;
                }
            }

            public byte[] Code
            {
                get
                {
                    return this.code;
                }
            }

            public int LocalVarSigToken
            {
                get
                {
                    return this.localVarSigToken;
                }
            }

            public ushort MaxStack
            {
                get
                {
                    return this.maxStack;
                }
            }

            // Nested Types
            [StructLayout(LayoutKind.Sequential, Size = 1)]
            private struct Flags
            {
                public const ushort InitLocals = 0x10;
                public const ushort MoreSects = 8;
            }

            [StructLayout(LayoutKind.Sequential, Size = 1)]
            private struct Format
            {
                public const byte Tiny = 2;
                public const byte Fat = 3;
                public const byte Mask = 3;
            }

            private sealed class Section
            {
                // Fields
                private ModuleReader.Clause[] clauses = new ModuleReader.Clause[0];
                private ModuleReader.MethodBodyReader.Section next = null;

                // Methods
                public Section(ModuleReader.BinaryReader reader)
                {
                    if ((reader.Position % 4) != 0)
                    {
                        reader.Position += 4 - (reader.Position % 4);
                    }
                    byte num = reader.ReadByte();
                    if ((num & 0x40) == 0)
                    {
                        byte num2 = reader.ReadByte();
                        reader.ReadBytes(2);
                        this.clauses = new ModuleReader.Clause[num2 / 12];
                        for (int i = 0; i < this.clauses.Length; i++)
                        {
                            this.clauses[i] = new ModuleReader.Clause();
                            this.clauses[i].Flags = reader.ReadInt16();
                            this.clauses[i].TryOffset = reader.ReadInt16();
                            this.clauses[i].TryLength = reader.ReadByte();
                            this.clauses[i].HandlerOffset = reader.ReadInt16();
                            this.clauses[i].HandlerLength = reader.ReadByte();
                            this.clauses[i].Value = reader.ReadInt32();
                        }
                    }
                    else
                    {
                        byte[] buffer = reader.ReadBytes(3);
                        int count = ((buffer[2] << 12) | (buffer[1] << 8)) | buffer[0];
                        if ((num & 1) == 0)
                        {
                            reader.ReadBytes(count);
                        }
                        else
                        {
                            this.clauses = new ModuleReader.Clause[count / 0x18];
                            for (int j = 0; j < this.clauses.Length; j++)
                            {
                                this.clauses[j] = new ModuleReader.Clause();
                                this.clauses[j].Flags = reader.ReadInt32();
                                this.clauses[j].TryOffset = reader.ReadInt32();
                                this.clauses[j].TryLength = reader.ReadInt32();
                                this.clauses[j].HandlerOffset = reader.ReadInt32();
                                this.clauses[j].HandlerLength = reader.ReadInt32();
                                this.clauses[j].Value = reader.ReadInt32();
                            }
                        }
                    }
                    if ((num & 0x80) != 0)
                    {
                        this.next = new ModuleReader.MethodBodyReader.Section(reader);
                    }
                }

                // Properties
                public ModuleReader.Clause[] Clauses
                {
                    get
                    {
                        return this.clauses;
                    }
                }

                public ModuleReader.MethodBodyReader.Section Next
                {
                    get
                    {
                        return this.next;
                    }
                }

                // Nested Types
                [StructLayout(LayoutKind.Sequential, Size = 1)]
                private struct Flags
                {
                    public const ushort EHTable = 1;
                    public const ushort OptILTable = 2;
                    public const ushort FatFormat = 0x40;
                    public const ushort MoreSects = 0x80;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MethodPointerRow
        {
            public int Method;
            public MethodPointerRow(ModuleReader.ITableReader reader)
            {
                this.Method = reader.ReadToken(6);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MethodRow
        {
            public uint Rva;
            public ushort ImplementationFlags;
            public ushort Flags;
            public int Name;
            public int Signature;
            public int Parameters;
            public MethodRow(ModuleReader.ITableReader reader)
            {
                this.Rva = reader.ReadUInt32();
                this.ImplementationFlags = reader.ReadUInt16();
                this.Flags = reader.ReadUInt16();
                this.Name = reader.ReadString();
                this.Signature = reader.ReadByteArray();
                this.Parameters = reader.ReadToken(8);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ModuleReferenceRow
        {
            public int Name;
            public ModuleReferenceRow(ModuleReader.ITableReader reader)
            {
                this.Name = reader.ReadString();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NestedClassRow
        {
            public int Nested;
            public int Enclosing;
            public NestedClassRow(ModuleReader.ITableReader reader)
            {
                this.Nested = reader.ReadToken(2);
                this.Enclosing = reader.ReadToken(2);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SignatureRow
        {
            public int Signature;
            public SignatureRow(ModuleReader.ITableReader reader)
            {
                this.Signature = reader.ReadByteArray();
            }
        }

        private sealed class TableManager : ModuleReader.ITableReader
        {
            // Fields
            private ModuleReader.AssemblyReferenceRow[] assemblyReferenceTable = null;
            private int byteArraySize;
            private static int[] codedTokenBits = new int[] { 
            0, 1, 1, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 
            4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 
            5
         };
            private ModuleReader.FieldPointerRow[] fieldPointerTable = null;
            private ModuleReader.FieldRow[] fieldTable = null;
            private ModuleReader.FileRow[] fileTable = null;
            private int guidSize;
            private ModuleReader.MemberReferenceRow[] memberReferenceTable = null;
            private ModuleReader.MethodPointerRow[] methodPointerTable = null;
            private ModuleReader.MethodRow[] methodTable = null;
            private ModuleReader.ModuleReferenceRow[] moduleReferenceTable = null;
            private ModuleReader.NestedClassRow[] nestedClassTable = null;
            private int[] positions = new int[0x40];
            private ModuleReader.BinaryReader reader;
            private int[] rows = new int[0x40];
            private ModuleReader.SignatureRow[] signatureTable = null;
            private int[] sizes = new int[0x60];
            private int stringSize;
            private ModuleReader.TypeReferenceRow[] typeReferenceTable = null;
            private ModuleReader.TypeSpecificationRow[] typeSpecificationTable = null;
            private ModuleReader.TypeRow[] typeTable = null;

            // Methods
            public TableManager(ModuleReader.BinaryReader reader)
            {
                this.reader = reader;
                TableHeader header = new TableHeader(reader);
                this.stringSize = ((header.HeapSizes & 1) == 0) ? 2 : 4;
                this.guidSize = ((header.HeapSizes & 2) == 0) ? 2 : 4;
                this.byteArraySize = ((header.HeapSizes & 4) == 0) ? 2 : 4;
                for (int i = 0; i < 0x40; i++)
                {
                    this.rows[i] = (((header.MaskValid >> i) & ((ulong)1L)) != 0L) ? this.reader.ReadInt32() : 0;
                }
                int position = this.reader.Position;
                this.positions[0] = position;
                position += this.rows[0] * ((((2 + this.SizeString()) + this.SizeGuid()) + this.SizeGuid()) + this.SizeGuid());
                this.positions[1] = position;
                position += this.rows[1] * ((this.SizeCodedToken(0x44) + this.SizeString()) + this.SizeString());
                this.positions[2] = position;
                position += this.rows[2] * (((((4 + this.SizeString()) + this.SizeString()) + this.SizeCodedToken(0x40)) + this.SizeToken(4)) + this.SizeToken(6));
                this.positions[3] = position;
                position += this.rows[3] * this.SizeToken(4);
                this.positions[4] = position;
                position += this.rows[4] * ((2 + this.SizeString()) + this.SizeByteArray());
                this.positions[5] = position;
                position += this.rows[5] * this.SizeToken(6);
                this.positions[6] = position;
                position += this.rows[6] * (((8 + this.SizeString()) + this.SizeByteArray()) + this.SizeToken(8));
                this.positions[7] = position;
                position += this.rows[7] * this.SizeToken(8);
                this.positions[8] = position;
                position += this.rows[8] * (4 + this.SizeString());
                this.positions[9] = position;
                position += this.rows[9] * (this.SizeToken(2) + this.SizeCodedToken(0x40));
                this.positions[10] = position;
                position += this.rows[10] * ((this.SizeCodedToken(0x47) + this.SizeString()) + this.SizeByteArray());
                this.positions[11] = position;
                position += this.rows[11] * ((2 + this.SizeCodedToken(0x41)) + this.SizeByteArray());
                this.positions[12] = position;
                position += this.rows[12] * ((this.SizeCodedToken(0x51) + this.SizeCodedToken(0x42)) + this.SizeByteArray());
                this.positions[13] = position;
                position += this.rows[13] * (this.SizeCodedToken(0x45) + this.SizeByteArray());
                this.positions[14] = position;
                position += this.rows[14] * ((2 + this.SizeCodedToken(70)) + this.SizeByteArray());
                this.positions[15] = position;
                position += this.rows[15] * (6 + this.SizeToken(2));
                this.positions[0x10] = position;
                position += this.rows[0x10] * (4 + this.SizeToken(4));
                this.positions[0x11] = position;
                position += this.rows[0x11] * this.SizeByteArray();
                this.positions[0x12] = position;
                position += this.rows[0x12] * (this.SizeToken(2) + this.SizeToken(20));
                this.positions[0x13] = position;
                position += this.rows[0x13] * this.SizeToken(20);
                this.positions[20] = position;
                position += this.rows[20] * ((2 + this.SizeString()) + this.SizeCodedToken(0x40));
                this.positions[0x15] = position;
                position += this.rows[0x15] * (this.SizeToken(2) + this.SizeToken(0x17));
                this.positions[0x16] = position;
                position += this.rows[0x16] * this.SizeToken(0x17);
                this.positions[0x17] = position;
                position += this.rows[0x17] * ((2 + this.SizeString()) + this.SizeByteArray());
                this.positions[0x18] = position;
                position += this.rows[0x18] * ((2 + this.SizeToken(6)) + this.SizeCodedToken(0x43));
                this.positions[0x19] = position;
                position += this.rows[0x19] * ((this.SizeToken(2) + this.SizeCodedToken(0x48)) + this.SizeCodedToken(0x48));
                this.positions[0x1a] = position;
                position += this.rows[0x1a] * this.SizeString();
                this.positions[0x1b] = position;
                position += this.rows[0x1b] * this.SizeByteArray();
                this.positions[0x1c] = position;
                position += this.rows[0x1c] * (((2 + this.SizeCodedToken(0x49)) + this.SizeString()) + this.SizeToken(0));
                this.positions[0x1d] = position;
                position += this.rows[0x1d] * (4 + this.SizeToken(4));
                this.positions[30] = position;
                position += this.rows[30] * 8;
                this.positions[0x1f] = position;
                position += this.rows[0x1f] * 4;
                this.positions[0x20] = position;
                position += this.rows[0x20] * (((0x10 + this.SizeByteArray()) + this.SizeString()) + this.SizeString());
                this.positions[0x21] = position;
                position += this.rows[0x21] * 4;
                this.positions[0x22] = position;
                position += this.rows[0x22] * 12;
                this.positions[0x23] = position;
                position += this.rows[0x23] * ((((12 + this.SizeByteArray()) + this.SizeString()) + this.SizeString()) + this.SizeByteArray());
                this.positions[0x24] = position;
                position += this.rows[0x24] * (4 + this.SizeToken(0x23));
                this.positions[0x25] = position;
                position += this.rows[0x25] * (12 + this.SizeToken(0x23));
                this.positions[0x26] = position;
                position += this.rows[0x26] * ((4 + this.SizeString()) + this.SizeByteArray());
                this.positions[0x27] = position;
                position += this.rows[0x27] * (((8 + this.SizeString()) + this.SizeString()) + this.SizeCodedToken(80));
                this.positions[40] = position;
                position += this.rows[40] * ((8 + this.SizeString()) + this.SizeCodedToken(80));
                this.positions[0x29] = position;
                position += this.rows[0x29] * (this.SizeToken(2) + this.SizeToken(2));
            }

            private int Initialize(int type)
            {
                this.reader.Position = this.positions[type];
                return this.rows[type];
            }

            public int ReadByteArray()
            {
                return ((this.byteArraySize == 4) ? this.reader.ReadInt32() : ((int)this.reader.ReadUInt16()));
            }

            public int ReadCodedToken(int tableType)
            {
                int num = (this.SizeCodedToken(tableType) == 4) ? this.reader.ReadInt32() : ((int)this.reader.ReadUInt16());
                if (num == 0)
                {
                    return 0;
                }
                int[] numArray = ModuleReader.TableType.CodedTokenList(tableType);
                int num2 = codedTokenBits[numArray.Length];
                int index = num & ~(((int)(-1)) << num2);
                int num4 = num >> num2;
                return ((numArray[index] << 0x18) | num4);
            }

            public int ReadGuid()
            {
                return ((this.guidSize == 4) ? this.reader.ReadInt32() : ((int)this.reader.ReadUInt16()));
            }

            public int ReadString()
            {
                return ((this.stringSize == 4) ? this.reader.ReadInt32() : ((int)this.reader.ReadUInt16()));
            }

            public int ReadToken(int tableType)
            {
                return ((tableType << 0x18) | ((this.rows[tableType] <= 0xffff) ? ((int)this.reader.ReadUInt16()) : this.reader.ReadInt32()));
            }

            public ushort ReadUInt16()
            {
                return this.reader.ReadUInt16();
            }

            public uint ReadUInt32()
            {
                return this.reader.ReadUInt32();
            }

            private int SizeByteArray()
            {
                return this.byteArraySize;
            }

            private int SizeCodedToken(int type)
            {
                if ((type < 0x40) || (type > 0x60))
                {
                    throw new ModuleReaderException("Invalid coded token.");
                }
                int num = this.sizes[type];
                if (num == 0)
                {
                    int[] numArray = ModuleReader.TableType.CodedTokenList(type);
                    int num2 = 0;
                    for (int i = 0; i < numArray.Length; i++)
                    {
                        if (numArray[i] != 0x70)
                        {
                            num2 = (num2 > this.rows[numArray[i]]) ? num2 : this.rows[numArray[i]];
                        }
                    }
                    num2 = num2 << codedTokenBits[numArray.Length];
                    this.sizes[type] = num = (num2 <= 0xffff) ? 2 : 4;
                }
                return num;
            }

            private int SizeGuid()
            {
                return this.guidSize;
            }

            private int SizeString()
            {
                return this.stringSize;
            }

            private int SizeToken(int type)
            {
                if (type >= 0x40)
                {
                    throw new ModuleReaderException("Invalid table token.");
                }
                int num = this.sizes[type];
                if (num == 0)
                {
                    num = (this.rows[type] <= 0xffff) ? 2 : 4;
                    this.sizes[type] = num;
                }
                return num;
            }

            // Properties
            public ModuleReader.AssemblyReferenceRow[] AssemblyReference
            {
                get
                {
                    if (this.assemblyReferenceTable == null)
                    {
                        int num = this.Initialize(0x23);
                        this.assemblyReferenceTable = new ModuleReader.AssemblyReferenceRow[num];
                        for (int i = 0; i < num; i++)
                        {
                            this.assemblyReferenceTable[i] = new ModuleReader.AssemblyReferenceRow(this);
                        }
                    }
                    return this.assemblyReferenceTable;
                }
            }

            public ModuleReader.FieldRow[] Field
            {
                get
                {
                    if (this.fieldTable == null)
                    {
                        int num = this.Initialize(4);
                        this.fieldTable = new ModuleReader.FieldRow[num];
                        for (int i = 0; i < num; i++)
                        {
                            this.fieldTable[i] = new ModuleReader.FieldRow(this);
                        }
                    }
                    return this.fieldTable;
                }
            }

            public ModuleReader.FieldPointerRow[] FieldPointer
            {
                get
                {
                    if (this.fieldPointerTable == null)
                    {
                        int num = this.Initialize(3);
                        this.fieldPointerTable = new ModuleReader.FieldPointerRow[num];
                        for (int i = 0; i < num; i++)
                        {
                            this.fieldPointerTable[i] = new ModuleReader.FieldPointerRow(this);
                        }
                    }
                    return this.fieldPointerTable;
                }
            }

            public ModuleReader.FileRow[] File
            {
                get
                {
                    if (this.fileTable == null)
                    {
                        int num = this.Initialize(0x26);
                        this.fileTable = new ModuleReader.FileRow[num];
                        for (int i = 0; i < num; i++)
                        {
                            this.fileTable[i] = new ModuleReader.FileRow(this);
                        }
                    }
                    return this.fileTable;
                }
            }

            public ModuleReader.MemberReferenceRow[] MemberReference
            {
                get
                {
                    if (this.memberReferenceTable == null)
                    {
                        int num = this.Initialize(10);
                        this.memberReferenceTable = new ModuleReader.MemberReferenceRow[num];
                        for (int i = 0; i < num; i++)
                        {
                            this.memberReferenceTable[i] = new ModuleReader.MemberReferenceRow(this);
                        }
                    }
                    return this.memberReferenceTable;
                }
            }

            public ModuleReader.MethodRow[] Method
            {
                get
                {
                    if (this.methodTable == null)
                    {
                        int num = this.Initialize(6);
                        this.methodTable = new ModuleReader.MethodRow[num];
                        for (int i = 0; i < num; i++)
                        {
                            this.methodTable[i] = new ModuleReader.MethodRow(this);
                        }
                    }
                    return this.methodTable;
                }
            }

            public ModuleReader.MethodPointerRow[] MethodPointer
            {
                get
                {
                    if (this.methodPointerTable == null)
                    {
                        int num = this.Initialize(5);
                        this.methodPointerTable = new ModuleReader.MethodPointerRow[num];
                        for (int i = 0; i < num; i++)
                        {
                            this.methodPointerTable[i] = new ModuleReader.MethodPointerRow(this);
                        }
                    }
                    return this.methodPointerTable;
                }
            }

            public ModuleReader.ModuleReferenceRow[] ModuleReference
            {
                get
                {
                    if (this.moduleReferenceTable == null)
                    {
                        int num = this.Initialize(0x1a);
                        this.moduleReferenceTable = new ModuleReader.ModuleReferenceRow[num];
                        for (int i = 0; i < num; i++)
                        {
                            this.moduleReferenceTable[i] = new ModuleReader.ModuleReferenceRow(this);
                        }
                    }
                    return this.moduleReferenceTable;
                }
            }

            public ModuleReader.NestedClassRow[] NestedClass
            {
                get
                {
                    if (this.nestedClassTable == null)
                    {
                        int num = this.Initialize(0x29);
                        this.nestedClassTable = new ModuleReader.NestedClassRow[num];
                        for (int i = 0; i < num; i++)
                        {
                            this.nestedClassTable[i] = new ModuleReader.NestedClassRow(this);
                        }
                    }
                    return this.nestedClassTable;
                }
            }

            public ModuleReader.SignatureRow[] Signature
            {
                get
                {
                    if (this.signatureTable == null)
                    {
                        int num = this.Initialize(0x11);
                        this.signatureTable = new ModuleReader.SignatureRow[num];
                        for (int i = 0; i < num; i++)
                        {
                            this.signatureTable[i] = new ModuleReader.SignatureRow(this);
                        }
                    }
                    return this.signatureTable;
                }
            }

            public ModuleReader.TypeRow[] Type
            {
                get
                {
                    if (this.typeTable == null)
                    {
                        int num = this.Initialize(2);
                        this.typeTable = new ModuleReader.TypeRow[num];
                        for (int i = 0; i < num; i++)
                        {
                            this.typeTable[i] = new ModuleReader.TypeRow(this);
                        }
                    }
                    return this.typeTable;
                }
            }

            public ModuleReader.TypeReferenceRow[] TypeReference
            {
                get
                {
                    if (this.typeReferenceTable == null)
                    {
                        int num = this.Initialize(1);
                        this.typeReferenceTable = new ModuleReader.TypeReferenceRow[num];
                        for (int i = 0; i < num; i++)
                        {
                            this.typeReferenceTable[i] = new ModuleReader.TypeReferenceRow(this);
                        }
                    }
                    return this.typeReferenceTable;
                }
            }

            public ModuleReader.TypeSpecificationRow[] TypeSpecification
            {
                get
                {
                    if (this.typeSpecificationTable == null)
                    {
                        int num = this.Initialize(0x1b);
                        this.typeSpecificationTable = new ModuleReader.TypeSpecificationRow[num];
                        for (int i = 0; i < num; i++)
                        {
                            this.typeSpecificationTable[i] = new ModuleReader.TypeSpecificationRow(this);
                        }
                    }
                    return this.typeSpecificationTable;
                }
            }

            // Nested Types
            private sealed class TableHeader
            {
                // Fields
                public byte HeapSizes;
                public byte MajorVersion;
                public ulong MaskSorted;
                public ulong MaskValid;
                public byte MinorVersion;
                public int Reserved;
                public byte RowId;

                // Methods
                public TableHeader(ModuleReader.BinaryReader reader)
                {
                    this.Reserved = reader.ReadInt32();
                    this.MajorVersion = reader.ReadByte();
                    this.MinorVersion = reader.ReadByte();
                    this.HeapSizes = reader.ReadByte();
                    this.RowId = reader.ReadByte();
                    this.MaskValid = reader.ReadUInt64();
                    this.MaskSorted = reader.ReadUInt64();
                    if ((this.MajorVersion != 1) || (this.MinorVersion != 0))
                    {
                        throw new ModuleReaderException("Invalid meta-data table header");
                    }
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, Size = 1)]
        private struct TableType
        {
            public const int Module = 0;
            public const int TypeReference = 1;
            public const int Type = 2;
            public const int FieldPointer = 3;
            public const int Field = 4;
            public const int MethodPointer = 5;
            public const int Method = 6;
            public const int ParameterPointer = 7;
            public const int Parameter = 8;
            public const int InterfaceImplementation = 9;
            public const int MemberReference = 10;
            public const int Constant = 11;
            public const int CustomAttribute = 12;
            public const int FieldMarshal = 13;
            public const int Permission = 14;
            public const int ClassLayout = 15;
            public const int FieldLayout = 0x10;
            public const int Signature = 0x11;
            public const int EventMap = 0x12;
            public const int EventPointer = 0x13;
            public const int Event = 20;
            public const int PropertyMap = 0x15;
            public const int PropertyPointer = 0x16;
            public const int Property = 0x17;
            public const int MethodSemantics = 0x18;
            public const int MethodImplementation = 0x19;
            public const int ModuleReference = 0x1a;
            public const int TypeSpecification = 0x1b;
            public const int ImplementationMap = 0x1c;
            public const int FieldRva = 0x1d;
            public const int EncodingLog = 30;
            public const int EncodingMap = 0x1f;
            public const int Assembly = 0x20;
            public const int AssemblyProcessor = 0x21;
            public const int AssemblyOperatingSystem = 0x22;
            public const int AssemblyReference = 0x23;
            public const int AssemblyReferenceProcessor = 0x24;
            public const int AssemblyReferenceOperatingSystem = 0x25;
            public const int File = 0x26;
            public const int ExportedType = 0x27;
            public const int ManifestResource = 40;
            public const int NestedClass = 0x29;
            public const int TypeTyPar = 0x2a;
            public const int MethodTyPar = 0x2b;
            public const int TypeOrTypeReference = 0x40;
            public const int HasConstant = 0x41;
            public const int CustomAttributeType = 0x42;
            public const int HasSemantic = 0x43;
            public const int ResolutionScope = 0x44;
            public const int HasFieldMarshal = 0x45;
            public const int HasDeclSecurity = 70;
            public const int MemberReferenceParent = 0x47;
            public const int MethodOrMethodReference = 0x48;
            public const int MemberForwarded = 0x49;
            public const int Implementation = 80;
            public const int HasCustomAttribute = 0x51;
            public const int UInt16 = 0x61;
            public const int UInt32 = 0x63;
            public const int String = 0x65;
            public const int Blob = 0x66;
            public const int Guid = 0x67;
            public const int UserString = 0x70;
            private static int[] typeOrTypeReference;
            private static int[] hasConstant;
            private static int[] customAttributeType;
            private static int[] hasSemantic;
            private static int[] resolutionScope;
            private static int[] hasFieldMarshal;
            private static int[] hasDeclSecurity;
            private static int[] memberReferenceParent;
            private static int[] methodOrMethodReference;
            private static int[] memberForwarded;
            private static int[] implementation;
            private static int[] hasCustomAttribute;
            public static int[] CodedTokenList(int type)
            {
                switch (type)
                {
                    case 0x40:
                        return typeOrTypeReference;

                    case 0x41:
                        return hasConstant;

                    case 0x42:
                        return customAttributeType;

                    case 0x43:
                        return hasSemantic;

                    case 0x44:
                        return resolutionScope;

                    case 0x45:
                        return hasFieldMarshal;

                    case 70:
                        return hasDeclSecurity;

                    case 0x47:
                        return memberReferenceParent;

                    case 0x48:
                        return methodOrMethodReference;

                    case 0x49:
                        return memberForwarded;

                    case 80:
                        return implementation;

                    case 0x51:
                        return hasCustomAttribute;
                }
                throw new ModuleReaderException("Invalid reference token.");
            }

            static TableType()
            {
                typeOrTypeReference = new int[] { 2, 1, 0x1b };
                hasConstant = new int[] { 4, 8, 0x17 };
                customAttributeType = new int[] { 1, 2, 6, 10, 0x70 };
                hasSemantic = new int[] { 20, 0x17 };
                resolutionScope = new int[] { 0, 0x1a, 0x23, 1 };
                hasFieldMarshal = new int[] { 4, 8 };
                hasDeclSecurity = new int[] { 2, 6, 0x20 };
                memberReferenceParent = new int[] { 2, 1, 0x1a, 6, 0x1b };
                methodOrMethodReference = new int[] { 6, 10 };
                memberForwarded = new int[] { 4, 6 };
                implementation = new int[] { 0x26, 0x23, 0x27 };
                hasCustomAttribute = new int[] { 
                6, 4, 1, 2, 8, 9, 10, 0, 14, 0x17, 20, 0x11, 0x1a, 0x1b, 0x20, 0x23, 
                0x26, 0x27, 40
             };
            }
        }

        private sealed class ThisParameterInfo : ParameterInfo
        {
            // Fields
            private Type parameterType;

            // Methods
            public ThisParameterInfo(Type parameterType)
            {
                this.parameterType = parameterType;
            }

            // Properties
            public override string Name
            {
                get
                {
                    return "0";
                }
            }

            public override Type ParameterType
            {
                get
                {
                    return this.parameterType;
                }
            }

            public override int Position
            {
                get
                {
                    return -1;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TypeReferenceRow
        {
            public int ResolutionScope;
            public int Name;
            public int Namespace;
            public TypeReferenceRow(ModuleReader.ITableReader reader)
            {
                this.ResolutionScope = reader.ReadCodedToken(0x44);
                this.Name = reader.ReadString();
                this.Namespace = reader.ReadString();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TypeRow
        {
            public uint Flags;
            public int Name;
            public int Namespace;
            public int Extends;
            public int Fields;
            public int Methods;
            public TypeRow(ModuleReader.ITableReader reader)
            {
                this.Flags = reader.ReadUInt32();
                this.Name = reader.ReadString();
                this.Namespace = reader.ReadString();
                this.Extends = reader.ReadCodedToken(0x40);
                this.Fields = reader.ReadToken(4);
                this.Methods = reader.ReadToken(6);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TypeSpecificationRow
        {
            public int Signature;
            public TypeSpecificationRow(ModuleReader.ITableReader reader)
            {
                this.Signature = reader.ReadByteArray();
            }
        }
    }


}
