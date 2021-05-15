using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSE.Core.Utils;
using NSE.MessageBus;

namespace NSE.Pedidos.API.Configuration
{
    public static class MessageBusConfig
    {
        public static void AddMessageBusConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            // AddHostedService - o ciclo da injeção de dependencia funciona como Singleton - trabalha como um só no pipeline do aspnet
            // E uma vez que temos um instancia de um objeto singleton nao podemos injetar nada que seja diferente de singleton nessa classe
            // MessageBus - Bus
            //services.AddMessageBus(configuration.GetMessageQueueConnection("MessageBus")) 
            //    .AddHostedService<RegistroClienteIntegrationHandler>(); // Nosso HostedService esta ligado ao nosso MessageBus
        }
    }
}
