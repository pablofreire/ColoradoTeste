using Colorado.Aplicacao.Dtos;
using Colorado.Aplicacao.Excecoes;
using Colorado.Aplicacao.Interfaces;
using Colorado.Aplicacao.Mapeamentos;
using Colorado.Dominio.Entidades;
using Colorado.Dominio.Excecoes;
using Colorado.Dominio.Interfaces;

namespace Colorado.Aplicacao.Servicos;

public class TipoTelefoneServico : ITipoTelefoneServico
{
    private readonly IUnitOfWork _uow;

    public TipoTelefoneServico(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<TipoTelefoneDto>> ListarAsync()
    {
        var tipos = await _uow.TiposTelefone.ListarAsync();
        return TipoTelefoneMapeador.ParaDtos(tipos.OrderBy(t => t.DescricaoTipoTelefone));
    }

    public async Task<TipoTelefoneDto?> ObterPorIdAsync(int codigoTipoTelefone)
    {
        var tipo = await _uow.TiposTelefone.ObterPorIdAsync(codigoTipoTelefone);
        return tipo is null ? null : TipoTelefoneMapeador.ParaDto(tipo);
    }

    public async Task<TipoTelefoneDto> CriarAsync(SalvarTipoTelefoneDto dto, string usuario)
    {
        if (await _uow.TiposTelefone.ExisteDescricaoAsync(dto.DescricaoTipoTelefone))
            throw new DominioException("Ja existe um tipo de telefone com esta descricao.");

        var tipo = new TipoTelefone(dto.DescricaoTipoTelefone, usuario);

        await _uow.TiposTelefone.AdicionarAsync(tipo);
        await _uow.CommitAsync();

        return TipoTelefoneMapeador.ParaDto(tipo);
    }

    public async Task AtualizarAsync(int codigoTipoTelefone, SalvarTipoTelefoneDto dto)
    {
        var tipo = await _uow.TiposTelefone.ObterPorIdAsync(codigoTipoTelefone);
        if (tipo is null)
            throw new NaoEncontradoException($"Tipo de telefone {codigoTipoTelefone} nao encontrado.");

        if (await _uow.TiposTelefone.ExisteDescricaoAsync(dto.DescricaoTipoTelefone, codigoTipoTelefone))
            throw new DominioException("Ja existe outro tipo de telefone com esta descricao.");

        tipo.AtualizarDescricao(dto.DescricaoTipoTelefone);

        _uow.TiposTelefone.Atualizar(tipo);
        await _uow.CommitAsync();
    }

    public async Task RemoverAsync(int codigoTipoTelefone)
    {
        var tipo = await _uow.TiposTelefone.ObterPorIdAsync(codigoTipoTelefone);
        if (tipo is null)
            throw new NaoEncontradoException($"Tipo de telefone {codigoTipoTelefone} nao encontrado.");

        if (await _uow.TiposTelefone.PossuiTelefonesVinculadosAsync(codigoTipoTelefone))
            throw new DominioException("Nao e possivel remover: existem telefones usando este tipo.");

        _uow.TiposTelefone.Remover(tipo);
        await _uow.CommitAsync();
    }
}
