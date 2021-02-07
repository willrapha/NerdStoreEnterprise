using Microsoft.Extensions.DependencyInjection;
using System;

namespace NSE.MessageBus
{
    public static class DependecyInjectionExtensions
    {
        public static IServiceCollection AddMessageBus(this IServiceCollection services, string connection)
        {
            if (string.IsNullOrEmpty(connection)) throw new ArgumentNullException();

            // Singleton devido o IBus
            // MessageBus não recebe nada no construtor por isso nao precisamos resolver injeção de dependencia apenas a string de conexão
            services.AddSingleton<IMessageBus>(new MessageBus(connection));

            return services;
        }
    }
}
