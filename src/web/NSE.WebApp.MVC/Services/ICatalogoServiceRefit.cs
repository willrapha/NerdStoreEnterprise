using NSE.WebApp.MVC.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSE.WebApp.MVC.Services
{
    // Refit para consumo de API
    public interface ICatalogoServiceRefit
    {
        [Get("/api/catalogo/produtos/")]
        Task<IEnumerable<ProdutoViewModel>> ObterTodos();
        [Get("/api/catalogo/produtos/{id}")]
        Task<ProdutoViewModel> ObterPorId(Guid id);
    }
}
