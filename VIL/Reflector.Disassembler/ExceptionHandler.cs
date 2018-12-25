using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reflector.Disassembler
{
    public class ExceptionHandler
    {
        // Fields
        private Type catchType;
        private int filterOffset;
        private int handlerLength;
        private int handlerOffset;
        private int tryLength;
        private int tryOffset;
        private ExceptionHandlerType type;

        // Methods
        internal ExceptionHandler(ExceptionHandlerType type, int tryOffset, int tryLength, int handlerOffset, int handlerLength)
        {
            this.type = type;
            this.tryOffset = tryOffset;
            this.tryLength = tryLength;
            this.handlerOffset = handlerOffset;
            this.handlerLength = handlerLength;

        }
        internal ExceptionHandler(ExceptionHandlerType type, int tryOffset, int tryLength, int handlerOffset, int handlerLength, int filterOffset)
            : this(type, tryOffset, tryLength, handlerOffset, handlerLength)
        {
            this.filterOffset = filterOffset;
        }

        internal ExceptionHandler(ExceptionHandlerType type, int tryOffset, int tryLength, int handlerOffset, int handlerLength, Type catchType)
            : this(type, tryOffset, tryLength, handlerOffset, handlerLength)
        {
            this.catchType = catchType;
        }

 


        // Properties
        public Type CatchType { get { return catchType ;} }
        public int FilterOffset { get { return filterOffset; } }
        public int HandlerLength { get { return handlerLength; } }
        public int HandlerOffset { get { return handlerOffset; } }
        public int TryLength { get { return tryLength; } }
        public int TryOffset { get { return tryOffset; } }
        public ExceptionHandlerType Type { get { return type; } }

    }
}
