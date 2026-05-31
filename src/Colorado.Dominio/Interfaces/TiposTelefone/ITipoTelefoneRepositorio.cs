using Colorado.Dominio.Entidades;

namespace Colorado.Dominio.Interfaces.TiposTelefone;

public interface ITipoTelefoneRepositorio : IRepositorio<TipoTelefone>
{
    Task<bool> ExisteDescricaoAsync(string descricao, int? ignorarCodigo = null);

    Task<bool> PossuiTelefonesVinculadosAsync(int codigoTipoTelefone);
}
