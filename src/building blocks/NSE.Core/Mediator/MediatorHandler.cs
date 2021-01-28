using FluentValidation.Results;
using MediatR;
using NSE.Core.Messages;
using System.Threading.Tasks;

namespace NSE.Core.Mediator
{
    // Abastracao concreta para o Mediator
    public class MediatorHandler : IMediatorHandler
    {
        private readonly IMediator _mediador;
        public MediatorHandler(IMediator mediador)
        {
            _mediador = mediador;
        }

        public async Task<ValidationResult> EnviarComando<T>(T comando) where T : Command
        {
            return await _mediador.Send(comando);
        }

        public async Task PublicarEvento<T>(T evento) where T : Event
        {
            await _mediador.Publish(evento);
        }
    }
}
