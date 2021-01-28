using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSE.Clientes.API.Application.Commands;
using NSE.Core.Mediator;

namespace NSE.Clientes.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IMediatorHandler, MediatorHandler>();

            // Estamos resolvendo da seguinte maneira
            // O RegistrarClienteCommand que vai ser entregue via IRequestHandler que vai retornar um ValidationResult vai ser manipulado pelo ClienteCommandHandler
            services.AddScoped<IRequestHandler<RegistrarClienteCommand, ValidationResult>, ClienteCommandHandler>();
        }
    }
}
