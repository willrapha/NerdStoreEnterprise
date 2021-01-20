using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NSE.WebApp.MVC.Extensions;

namespace NSE.WebApp.MVC.Configuration
{
    public static class WebAppConfig
    {
        public static void AddMvcConfiguration(this IServiceCollection services)
        {
            services.AddControllersWithViews();
        }

        public static void UseMvcConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
                
            //}

            // Middleware ja existente que pega as exceções realiza um log, resetar o request path e reexecuta o request apontando pra essa pagina
            // trataremos apenas o 500 aqui por se tratar de algum erro no servidor que nao sabemos por isso jogaremos uma msg generica
            app.UseExceptionHandler("/erro/500");

            // Para os erros que ja conhecemos (401,403,404,450 ...) utilizaremos esse Middleware disponibilizado pelo aspnet core, 
            // que verifica quando a um problema com um status code de erro dentro desse response redirecionaremos para essa rota
            app.UseStatusCodePagesWithRedirects("erro/{0}");
            app.UseHsts();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityConfiguration(); // Identity precisa estar entre o UseRouting e UseEndpoints

            app.UseMiddleware<ExceptionMiddleware>(); // Nosso Middleware, tudo que vai acontecer de erro vai passar por aqui, evitando colocar diversos try cath na aplicação

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
