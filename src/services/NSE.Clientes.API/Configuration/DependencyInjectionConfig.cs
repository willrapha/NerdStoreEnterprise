using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSE.Clientes.API.Application.Commands;
using NSE.Clientes.API.Application.Events;
using NSE.Clientes.API.Data;
using NSE.Clientes.API.Data.Repository;
using NSE.Clientes.API.Models;
using NSE.Clientes.API.Services;
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

            // evento e seu manipulador semelhante ao comando 
            services.AddScoped<INotificationHandler<ClienteRegistradoEvent>, ClienteEventHandler>();

            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<ClientesContext>();

            // AddHostedService - o ciclo da injeção de dependencia funciona como Singleton - trabalha como um só no pipeline do aspnet
            // E uma vez que temos um instancia de um objeto singleton nao podemos injetar nada que seja diferente de singleton nessa classe
            services.AddHostedService<RegistroClienteIntegrationHandler>();

        }
    }
}
