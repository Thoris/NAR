using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reflector.Disassembler
{
    public sealed class MethodBody
    {
        // Fields
        private int codeSize;
        private ExceptionHandler[] exceptions;
        private Instruction[] instructions;
        private int localVarSigToken;
        private int maxStack;
        private ModuleReader moduleReader;

        // Methods
        internal MethodBody(ModuleReader moduleReader, int codeSize, int maxStack, int localVarSigToken, ExceptionHandler[] exceptions, Instruction[] instructions)
        {
            this.moduleReader = moduleReader;
            this.codeSize = codeSize;
            this.maxStack = maxStack;
            this.localVarSigToken = localVarSigToken;
            this.exceptions = exceptions;
            this.instructions = instructions;
        }

        public ExceptionHandler[] GetExceptions()
        {
            return this.exceptions;
        }

        public Instruction[] GetInstructions()
        {
            return this.instructions;
        }

        public Type[] GetLocals()
        {
            return this.moduleReader.DecodeLocalVariableSignature(this.localVarSigToken);
        }

        // Properties
        public int CodeSize
        {
            get
            {
                return this.codeSize;
            }
        }

        public int MaxStack
        {
            get
            {
                return this.maxStack;
            }
        }
    }

 

}
