using Dapper;
using NSE.Pedidos.API.Application.DTO;
using NSE.Pedidos.Domain.Pedidos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NSE.Pedidos.API.Application.Queries
{
    public interface IPedidoQueries
    {
        Task<PedidoDTO> ObterUltimoPedido(Guid clienteId);
        Task<IEnumerable<PedidoDTO>> ObterListaPorClienteId(Guid clienteId);
    }

    // Queries - Exibir dados 
    public class PedidoQueries : IPedidoQueries
    {
        private readonly IPedidoRepository _pedidoRepository; // Trabalha com o negocio

        public PedidoQueries(IPedidoRepository pedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
        }

        public async Task<PedidoDTO> ObterUltimoPedido(Guid clienteId)
        {
            const string sql = @"SELECT 
                                P.ID AS 'Produto', P.CODIGO, P.VOUCHERUTILIZADO, P.DESCONTO, P.VALORTOTAL, P.PEDIDOSTATUS,
                                P.LOGRADOURO, P.NUMERO, P.BAIRRO, P.CEP, P.COMPLEMENTO, P.CIDADE, P.ESTADO,
                                PIT.ID AS 'ProdutoItemId', PIT.PRODUTONOME, PIT.QUANTIDADE, PIT.PRODUTOIMAGEM, PIT.VALORUNITARIO
                                FROM PEDIDOS P
                                INNER JOIN PEDIDOITEMS PIT ON P.ID = PIT.PEDIDOID
                                WHERE P.CLIENTEID = @clientId
                                AND P.DATACADASTRO between DATEADD(minute, -3, GETDATE()) and DATEADD(minute, 0, GETDATE())
                                AND P.PEDIDOSTATUS = 1
                                ORDER BY P.DATACADASTRO DESC";

            // Mapeamento Dapper mais de uma entidade
            var pedido = await _pedidoRepository.ObterConexao()
                .QueryAsync<PedidoDTO, PedidoItemDTO, EnderecoDTO, PedidoDTO>(sql, (p, pi, e) => {
                    p.PedidoItems.Add(pi);
                    p.Endereco = e;
                    return p;
                }, new { clienteId }, splitOn:"");

            return MapearPedido();
        }

        public async Task<IEnumerable<PedidoDTO>> ObterListaPorClienteId(Guid clienteId)
        {
            var pedidos = await _pedidoRepository.ObterListaPorClienteId(clienteId);

            return pedidos.Select(PedidoDTO.ParaPedidoDTO);
        }

        private PedidoDTO MapearPedido()
        {
            return new PedidoDTO();
        }
    }
}
