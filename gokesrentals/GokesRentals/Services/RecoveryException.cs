using System;
using System.Runtime.Serialization;

namespace GokesRentals.Services
{
    [Serializable]
    internal class RecoveryException : Exception
    {
        public RecoveryException()
        {
        }

        public RecoveryException(string message) : base(message)
        {
        }

        public RecoveryException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RecoveryException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}