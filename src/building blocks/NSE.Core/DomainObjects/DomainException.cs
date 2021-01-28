using System;

namespace NSE.Core.DomainObjects
{
    // Exception Custom para Dominio
    public class DomainException : Exception
    {
        public DomainException()
        { }

        public DomainException(string message) : base(message)
        { }

        public DomainException(string message, Exception innerException) : base (message, innerException)
        { }
    }
}
