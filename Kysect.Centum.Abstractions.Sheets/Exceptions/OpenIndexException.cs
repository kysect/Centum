using System;
using System.Runtime.Serialization;

namespace Kysect.Centum.Sheets.Exceptions
{
    [Serializable]
    public class OpenIndexException : Exception
    {
        public OpenIndexException() { }
        protected OpenIndexException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public OpenIndexException(string? message) : base(message) { }
        public OpenIndexException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}