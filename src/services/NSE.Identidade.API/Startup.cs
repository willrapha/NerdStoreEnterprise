using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NSE.Identidade.API.Data;
using NSE.Identidade.API.Extensions;

namespace NSE.Identidade.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configuração de contexto identity
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Configuração Identity
            services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders(); // Não é o JWT, token para identificar email entre outros

            // *****Configurações JWT*****
            var appSettingsSection = Configuration.GetSection("AppSettings"); // pegamos o nó AppSettings do nosso arquivo appsettings.json
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
            // *****Configurações JWT*****

            // Antigo AddMvc, suporte ao WebApi
            services.AddControllers();

            // Swagger
            services.AddSwaggerGen( c => {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "NerdSotre Enterprise Identity API",
                    Description = "Esta API faz parte do curso ASP.NET Core Enterprise Applications.",
                    Contact = new OpenApiContact() { Name = "Willian Raphael", Email = "willian.mattos@gmail.com" },
                    License = new OpenApiLicense() { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT")}
                });
            }); 
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"); // Endpoint para acessarmos o swagger
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // Percorre todas as classes que herdam de controller e criar os endpoints para cada um
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
