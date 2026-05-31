using Colorado.Aplicacao.Dtos.Clientes;
using Colorado.Dominio.Paginacao;

namespace Colorado.Web.ApiClients.Clientes;

// Conhece apenas as rotas de clientes; todo o transporte fica no IApiHttpCliente.
public class ClienteApiClient : IClienteApiClient
{
    private const string Rota = "api/clientes";

    private readonly IApiHttpCliente _api;

    public ClienteApiClient(IApiHttpCliente api)
    {
        _api = api;
    }

    public Task<ResultadoApi<ResultadoPaginado<ClienteDto>>> ListarAsync(string? nome, string? documento,
        int pagina, int tamanhoPagina)
    {
        var query = $"?pagina={pagina}&tamanhoPagina={tamanhoPagina}";
        if (!string.IsNullOrWhiteSpace(nome))
            query += $"&nome={Uri.EscapeDataString(nome)}";
        if (!string.IsNullOrWhiteSpace(documento))
            query += $"&documento={Uri.EscapeDataString(documento)}";

        return _api.GetAsync<ResultadoPaginado<ClienteDto>>($"{Rota}{query}");
    }

    public Task<ResultadoApi<ClienteDto>> ObterPorIdAsync(int codigo)
        => _api.GetAsync<ClienteDto>($"{Rota}/{codigo}");

    public Task<ResultadoApi<ClienteDto>> CriarAsync(SalvarClienteDto dto)
        => _api.PostAsync<ClienteDto>(Rota, dto);

    public Task<ResultadoApi<object>> AtualizarAsync(int codigo, SalvarClienteDto dto)
        => _api.PutAsync($"{Rota}/{codigo}", dto);

    public Task<ResultadoApi<object>> RemoverAsync(int codigo)
        => _api.DeleteAsync($"{Rota}/{codigo}");
}
