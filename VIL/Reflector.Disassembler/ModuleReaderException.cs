using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Reflector.Disassembler
{
    [Serializable]
    public class ModuleReaderException : Exception
    {
        // Methods
        public ModuleReaderException()
        {
        }

        public ModuleReaderException(string message)
            : base(message)
        {
        }

        protected ModuleReaderException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ModuleReaderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
