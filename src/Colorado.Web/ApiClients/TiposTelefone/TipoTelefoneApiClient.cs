using Colorado.Aplicacao.Dtos.TiposTelefone;

namespace Colorado.Web.ApiClients.TiposTelefone;

// Conhece apenas as rotas de tipos de telefone; o transporte fica no IApiHttpCliente.
public class TipoTelefoneApiClient : ITipoTelefoneApiClient
{
    private const string Rota = "api/tipos-telefone";

    private readonly IApiHttpCliente _api;

    public TipoTelefoneApiClient(IApiHttpCliente api)
    {
        _api = api;
    }

    public Task<ResultadoApi<IEnumerable<TipoTelefoneDto>>> ListarAsync()
        => _api.GetAsync<IEnumerable<TipoTelefoneDto>>(Rota);

    public Task<ResultadoApi<TipoTelefoneDto>> ObterPorIdAsync(int codigo)
        => _api.GetAsync<TipoTelefoneDto>($"{Rota}/{codigo}");

    public Task<ResultadoApi<TipoTelefoneDto>> CriarAsync(SalvarTipoTelefoneDto dto)
        => _api.PostAsync<TipoTelefoneDto>(Rota, dto);

    public Task<ResultadoApi<object>> AtualizarAsync(int codigo, SalvarTipoTelefoneDto dto)
        => _api.PutAsync($"{Rota}/{codigo}", dto);

    public Task<ResultadoApi<object>> RemoverAsync(int codigo)
        => _api.DeleteAsync($"{Rota}/{codigo}");
}
