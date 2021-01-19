using NSE.WebApp.MVC.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NSE.WebApp.MVC.Services
{
    public abstract class Service
    {
        protected bool TratarErrosResponse(HttpResponseMessage response)
        {
            switch (response.StatusCode.GetHashCode())
            {
                case 401:
                case 403:
                case 404:
                case 450:
                    throw new CustomHttpRequestException(response.StatusCode);

                case 400:
                    return false;
            }

            // EnsureSuccessStatusCode - garante que retornou um codigo de sucesso caso não é estourado uma Exception
            response.EnsureSuccessStatusCode();

            return true;
        }
    }
}
