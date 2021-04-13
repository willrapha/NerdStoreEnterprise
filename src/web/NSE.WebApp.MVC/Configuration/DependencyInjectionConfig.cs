using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSE.WebAPI.Core.Usuario;
using NSE.WebApp.MVC.Extensions;
using NSE.WebApp.MVC.Services;
using NSE.WebApp.MVC.Services.Handlers;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using System;
using System.Net.Http;

namespace NSE.WebApp.MVC.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Injecao para nosso annotation
            // Ele irá se resolver quando tiver um atributo 'Cpf' sendo utilizado
            services.AddSingleton<IValidationAttributeAdapterProvider, CpfValidationAttributeAdapterProvider>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // Singleton - porque utilizamos para a aplicação toda
            services.AddScoped<IAspNetUser, AspNetUser>();

            #region HttpServices
            // Injeçao para ser resolvido nosso Handler
            // Estamos utilizando Transient porque ja estamos trabalhando no modo Scoped do request esse cara vai ser chamado um instancia de cada vez
            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

            // HttpClient atraves do meu HttpClientFactory
            services.AddHttpClient<IAutenticacaoService, AutenticacaoService>()
                .AddPolicyHandler(PollyExtensions.EsperarTentar())
                .AddTransientHttpErrorPolicy(
                    p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

            services.AddHttpClient<ICatalogoService, CatalogoService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>() // HttpClientAuthorizationDelegatinhHandler - Handler que criamos para manipular o request
                .AddPolicyHandler(PollyExtensions.EsperarTentar())
                .AddTransientHttpErrorPolicy(
                    p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

            services.AddHttpClient<ICarrinhoService, CarrinhoService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AddPolicyHandler(PollyExtensions.EsperarTentar())
                .AddTransientHttpErrorPolicy(
                    p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));
            #endregion

            #region Polly com Circuit Breaker
            // Circuit Breaker - encerra a conexão após n tentativas
            //services.AddHttpClient<ICatalogoService, CatalogoService>()
            //    .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
            //    .AddPolicyHandler(PollyExtensions.EsperarTentar())
            //    .AddTransientHttpErrorPolicy(
            //        // 5 - numero de tentativas
            //        // TimeSpan.FromSeconds(30) - vezes sem responder
            //        // Independente do usuario se tiver 5 exceptions consecutivas na aplicação ja irá entrar em Circuit Breaker
            //        p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));
            #endregion

            #region Polly Especializada
            //services.AddHttpClient<ICatalogoService, CatalogoService>()
            //    .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
            //    .AddPolicyHandler(PollyExtensions.EsperarTentar());
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
        }
    }

    public static class PollyExtensions
    {
        public static AsyncRetryPolicy<HttpResponseMessage> EsperarTentar()
        {
            var retryWaitPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10)
                }, (outcome, timespan, retryCount, context) => // onRetry, a cada tentativa ele faz determinada ação no nosso caso exibe ao console
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"Tentando pela {retryCount} vez!");
                    Console.ForegroundColor = ConsoleColor.White;
                });

            return retryWaitPolicy;
        }
    }
}
