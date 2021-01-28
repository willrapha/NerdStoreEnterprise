using FluentValidation.Results;
using System;

namespace NSE.Core.Messages
{
    public abstract class Command : Message
    {
        public DateTime Timestamp { get; private set; }
        public ValidationResult ValidationResult { get; set; }

        protected Command()
        {
            Timestamp = DateTime.Now; // Data que foi criado o comando
        }

        // virtual - podemos dar o override mais não somos obrigado
        public virtual bool EhValido()
        {
            throw new NotImplementedException();
        }
    }
}
