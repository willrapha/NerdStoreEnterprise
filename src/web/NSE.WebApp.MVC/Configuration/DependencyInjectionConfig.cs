using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NSE.WebApp.MVC.Extensions;
using NSE.WebApp.MVC.Services;
using NSE.WebApp.MVC.Services.Handlers;

namespace NSE.WebApp.MVC.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            // Injeçao para ser resolvido nosso Handler
            // Estamos utilizando Transient porque ja estamos trabalhando no modo Scoped do request esse cara vai ser chamado um instancia de cada vez
            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

            // HttpClient atraves do meu HttpClientFactory
            services.AddHttpClient<IAutenticacaoService, AutenticacaoService>();

            services.AddHttpClient<ICatalogoService, CatalogoService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>(); // HttpClientAuthorizationDelegatinhHandler - Handler que criamos para manipular o request

            // Singleton - porque utilizamos para a aplicação toda
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<IUser, AspNetUser>();
        }
    }
}
