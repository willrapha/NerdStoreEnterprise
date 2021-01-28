using FluentValidation.Results;
using MediatR;
using System;

namespace NSE.Core.Messages
{
    // IRequest - necessario para que os comandos sejam interpretados pelo MediatR
    // O MediatR irá retornar um objeto de validação em nosso caso o 'ValidationResult'
    public abstract class Command : Message, IRequest<ValidationResult>
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
