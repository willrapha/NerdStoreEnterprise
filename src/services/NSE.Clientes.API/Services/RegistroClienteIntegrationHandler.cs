using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSE.Clientes.API.Application.Commands;
using NSE.Core.Mediator;
using NSE.Core.Messages.Integration;
using NSE.MessageBus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NSE.Clientes.API.Services
{
    // Manipulador de integração, que trabalha como um BackgroundService
    // BackgroundService - feature do aspnet core, funciona a parte de um request, trabalha em paralelo ao pipeline do aspnetcore - similar ao hangfire
    // tem escopo singleton, devido a isso todas as injeções de dependencia nessa classe precisam tbm ser singleton
    public class RegistroClienteIntegrationHandler : BackgroundService
    {
        // Nosso proprio Bus
        private readonly IMessageBus _bus;
        private readonly IServiceProvider _serviceProvider; // Utilizado no Startup.cs para injetar nossos serviços

        public RegistroClienteIntegrationHandler(IServiceProvider serviceProvider, IMessageBus bus)
        {
            _serviceProvider = serviceProvider;
            _bus = bus;
        }

        // Ao executar o projeto ele chamara primeiro esse metodo para criar o Bus porque esse metodo é um 'BackgroundService', o Bus ficara disponivel
        // o tempo inteiro esperando uma requisição
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // UsuarioRegistradoIntegrationEvent - classe que estamos esperando
            // ResponseMessage - tipo de resposta
            // request - retorno da classe que estamos esperando
            var sucesso = _bus.RespondAsync<UsuarioRegistradoIntegrationEvent, ResponseMessage>(async request =>
                await RegistrarCliente(request));

            return Task.CompletedTask;
        }

        private async Task<ResponseMessage> RegistrarCliente(UsuarioRegistradoIntegrationEvent message)
        {
            var clienteCommand = new RegistrarClienteCommand(message.Id, message.Nome, message.Email, message.Cpf);
            ValidationResult sucesso;

            // CreateScope - criamos um escopo dentro desse metodo RegistrarCliente com base no escopo que estamos trabalhando que é um singleton devido o BackgroundService
            using (var scope = _serviceProvider.CreateScope())
            {
                // Service Locator - Não é recomendada pra todos os casos, diferente da injeção de dependencia. Pegamos o container de injeção de dependencia
                // criamos um escopo e vamos buscar dentro dele com base na interface uma instancia do que precisamos, similar a injeção no construtor, porem dessa forma 
                // atrapalhamos testes enfim não é indicado, porém nesse cenario é requirido
                // Resolvendo o servico IMediatorHandler que é Scoped dentro da classe de escopo Singleton
                var mediator = scope.ServiceProvider.GetRequiredService<IMediatorHandler>();
                sucesso = await mediator.EnviarComando(clienteCommand);
            }

            return new ResponseMessage(sucesso);
        }
    }
}
