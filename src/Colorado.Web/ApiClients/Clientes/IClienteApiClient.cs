using Colorado.Aplicacao.Dtos.Clientes;
using Colorado.Dominio.Paginacao;

namespace Colorado.Web.ApiClients.Clientes;

public interface IClienteApiClient
{
    Task<ResultadoApi<ResultadoPaginado<ClienteDto>>> ListarAsync(string? nome, string? documento,
        int pagina, int tamanhoPagina);
    Task<ResultadoApi<ClienteDto>> ObterPorIdAsync(int codigo);
    Task<ResultadoApi<ClienteDto>> CriarAsync(SalvarClienteDto dto);
    Task<ResultadoApi<object>> AtualizarAsync(int codigo, SalvarClienteDto dto);
    Task<ResultadoApi<object>> RemoverAsync(int codigo);
}
