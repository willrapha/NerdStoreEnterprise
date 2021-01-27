using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace NSE.WebAPI.Core.Identidade
{
    public static class JwtConfig
    {
        public static IServiceCollection AddJwtConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            // Configurações JWT
            var appSettingsSection = configuration.GetSection("AppSettings"); // pegamos o nó AppSettings do nosso arquivo appsettings.json
            services.Configure<AppSettings>(appSettingsSection); // pedimos para que a classe AppSettings represente a seção AppSettings do nosso arquivo appsettings.json

            var appSettings = appSettingsSection.Get<AppSettings>(); // Obtemos a classe
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            // Vamos utilizar o JWT na Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Challenge - desafio de como apresentar e credenciar o usuario internamente
            }).AddJwtBearer(bearerOptions => // JWT
            {
                bearerOptions.RequireHttpsMetadata = true;
                bearerOptions.SaveToken = true; // Token sera guardado na instancia
                bearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true, // Validamos o emissor com base na assinatura
                    IssuerSigningKey = new SymmetricSecurityKey(key), // Chave
                    ValidateIssuer = true, // Validar o Emissor
                    ValidateAudience = true, // Validar Dominio
                    ValidAudience = appSettings.ValidoEm,
                    ValidIssuer = appSettings.Emissor
                };
            });

            return services;
        }

        public static IApplicationBuilder UseAuthConfiguration(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}
