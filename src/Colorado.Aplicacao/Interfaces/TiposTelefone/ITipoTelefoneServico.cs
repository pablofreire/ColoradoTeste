using Colorado.Aplicacao.Dtos.TiposTelefone;
using Colorado.Aplicacao.Dtos;

namespace Colorado.Aplicacao.Interfaces.TiposTelefone;

public interface ITipoTelefoneServico
{
    Task<IEnumerable<TipoTelefoneDto>> ListarAsync();

    Task<TipoTelefoneDto?> ObterPorIdAsync(int codigoTipoTelefone);

    Task<TipoTelefoneDto> CriarAsync(SalvarTipoTelefoneDto dto, string usuario);

    Task AtualizarAsync(int codigoTipoTelefone, SalvarTipoTelefoneDto dto);

    Task RemoverAsync(int codigoTipoTelefone);
}
