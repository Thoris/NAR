using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;

namespace Reflector.Disassembler
{
    public sealed class Instruction
    {
        // Fields
        private OpCode code;
        private ModuleReader moduleReader;
        private int offset;
        private object operand;
        private byte[] operandData;

        // Methods
        internal Instruction(ModuleReader moduleReader, int offset, OpCode code, object operand, byte[] operandData)
        {
            this.moduleReader = moduleReader;
            this.offset = offset;
            this.code = code;
            this.operand = operand;
            this.operandData = operandData;
        }

        public byte[] GetOperandData()
        {
            return this.operandData;
        }

        // Properties
        public OpCode Code
        {
            get
            {
                return this.code;
            }
        }

        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        public object Operand
        {
            get
            {
                switch (this.code.OperandType)
                {
                    case OperandType.InlineSig:
                    case OperandType.InlineTok:
                    case OperandType.InlineType:
                    case OperandType.InlineField:
                    case OperandType.InlineMethod:
                        return this.moduleReader.ResolveToken((int)this.operand);
                }
                return this.operand;
            }
        }
    }


}
