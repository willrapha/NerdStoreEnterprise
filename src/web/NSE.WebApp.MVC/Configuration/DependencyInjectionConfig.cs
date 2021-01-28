using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSE.WebApp.MVC.Extensions;
using NSE.WebApp.MVC.Services;
using NSE.WebApp.MVC.Services.Handlers;
using Polly;
using Polly.Extensions.Http;
using System;

namespace NSE.WebApp.MVC.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Injeçao para ser resolvido nosso Handler
            // Estamos utilizando Transient porque ja estamos trabalhando no modo Scoped do request esse cara vai ser chamado um instancia de cada vez
            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

            // HttpClient atraves do meu HttpClientFactory
            services.AddHttpClient<IAutenticacaoService, AutenticacaoService>();

            //services.AddHttpClient<ICatalogoService, CatalogoService>()
            //    .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>(); // HttpClientAuthorizationDelegatinhHandler - Handler que criamos para manipular o request

            #region Polly Especializada
            //var retryWaitPolicy = HttpPolicyExtensions
            //    .HandleTransientHttpError()
            //    .WaitAndRetryAsync(new[]
            //    {
            //        TimeSpan.FromSeconds(1),
            //        TimeSpan.FromSeconds(5),
            //        TimeSpan.FromSeconds(10)
            //    }, (outcome, timespan, retryCount, context) => // onRetry, a cada tentativa ele faz determinada ação no nosso caso exibe ao console
            //    { 
            //        Console.ForegroundColor = ConsoleColor.Blue;
            //        Console.WriteLine($"Tentando pela {retryCount} vez!");
            //        Console.ForegroundColor = ConsoleColor.White;
            //    });

            //services.AddHttpClient<ICatalogoService, CatalogoService>()
            //    .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
            //    .AddPolicyHandler(retryWaitPolicy);
            #endregion

            #region Polly Simples
            //services.AddHttpClient<ICatalogoService, CatalogoService>()
            //    .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>() 
            //    .AddTransientHttpErrorPolicy(   // Polly - Politica para erro HTTP
            //        p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));
            #endregion

            #region Refit
            // Registrando dependencia Refit
            //services.AddHttpClient("Refit", 
            //options => 
            //{
            //    options.BaseAddress = new Uri(configuration.GetSection("CatalogoUrl").Value);
            //})
            //.AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
            //.AddTypedClient(Refit.RestService.For<ICatalogoServiceRefit>);
            #endregion

            // Singleton - porque utilizamos para a aplicação toda
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<IUser, AspNetUser>();
        }
    }
}
