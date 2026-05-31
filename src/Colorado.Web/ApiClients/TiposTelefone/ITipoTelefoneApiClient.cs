using Colorado.Aplicacao.Dtos.TiposTelefone;

namespace Colorado.Web.ApiClients.TiposTelefone;

public interface ITipoTelefoneApiClient
{
    Task<ResultadoApi<IEnumerable<TipoTelefoneDto>>> ListarAsync();
    Task<ResultadoApi<TipoTelefoneDto>> ObterPorIdAsync(int codigo);
    Task<ResultadoApi<TipoTelefoneDto>> CriarAsync(SalvarTipoTelefoneDto dto);
    Task<ResultadoApi<object>> AtualizarAsync(int codigo, SalvarTipoTelefoneDto dto);
    Task<ResultadoApi<object>> RemoverAsync(int codigo);
}
