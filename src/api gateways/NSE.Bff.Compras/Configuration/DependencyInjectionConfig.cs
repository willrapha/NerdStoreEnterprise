using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NSE.WebAPI.Core.Usuario;

namespace NSE.Bff.Compras.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // Precisamos registrar devido a classe AspNetUser receber a injeção dele
            services.AddScoped<IAspNetUser, AspNetUser>();
        }
    }
}
