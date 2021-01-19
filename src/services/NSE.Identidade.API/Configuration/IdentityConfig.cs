using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NSE.Identidade.API.Data;
using NSE.Identidade.API.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSE.Identidade.API.Configuration
{
    public static class IdentityConfig
    {
        public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuração de contexto identity
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Configuração Identity
            services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddErrorDescriber<IdentityMensagensPortugues>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders(); // Não é o JWT, token para identificar email entre outros

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

        public static IApplicationBuilder UseIdentityConfiguration(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}
