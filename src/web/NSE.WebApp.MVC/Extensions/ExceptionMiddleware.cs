using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;

namespace NSE.WebApp.MVC.Extensions
{
    // Middleware para pegar nossas exceptions pegas no Service.cs
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (CustomHttpRequestException ex)
            {

                HandleRequestExceptionAsync(httpContext, ex);
            }
        }

        private static void HandleRequestExceptionAsync(HttpContext context, CustomHttpRequestException httpRequestException)
        {
            if(httpRequestException.StatusCode == HttpStatusCode.Unauthorized)
            {
                // context.Request.Path - da onde estavamos vindo
                context.Response.Redirect($"/login?ReturnUrl={context.Request.Path}");
                return;
            }

            // Resposta do request vai receber o status code da exception
            context.Response.StatusCode = httpRequestException.StatusCode.GetHashCode();
        }
    }
}
