using Colorado.Aplicacao.Mapeamentos.TiposTelefone;
using Colorado.Aplicacao.Interfaces.TiposTelefone;
using Colorado.Aplicacao.Dtos.TiposTelefone;
using Colorado.Aplicacao.Dtos;
using Colorado.Aplicacao.Excecoes;
using Colorado.Aplicacao.Interfaces;
using Colorado.Aplicacao.Mapeamentos;
using Colorado.Dominio.Entidades;
using Colorado.Dominio.Excecoes;
using Colorado.Dominio.Interfaces;
using FluentValidation;

namespace Colorado.Aplicacao.Servicos.TiposTelefone;

public class TipoTelefoneServico : ITipoTelefoneServico
{
    private readonly IUnitOfWork _uow;
    private readonly IValidator<SalvarTipoTelefoneDto> _validator;

    public TipoTelefoneServico(IUnitOfWork uow, IValidator<SalvarTipoTelefoneDto> validator)
    {
        _uow = uow;
        _validator = validator;
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
        await ValidarAsync(dto, incluirCriacao: true);

        if (await _uow.TiposTelefone.ExisteDescricaoAsync(dto.DescricaoTipoTelefone))
            throw new DominioException("Ja existe um tipo de telefone com esta descricao.");

        var tipo = new TipoTelefone(dto.DescricaoTipoTelefone, usuario);

        await _uow.TiposTelefone.AdicionarAsync(tipo);
        await _uow.CommitAsync();

        return TipoTelefoneMapeador.ParaDto(tipo);
    }

    public async Task AtualizarAsync(int codigoTipoTelefone, SalvarTipoTelefoneDto dto)
    {
        await ValidarAsync(dto);

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

    private async Task ValidarAsync(SalvarTipoTelefoneDto dto, bool incluirCriacao = false)
    {
        var resultado = incluirCriacao
            ? await _validator.ValidateAsync(dto, opcoes =>
            {
                opcoes.IncludeRulesNotInRuleSet();
                opcoes.IncludeRuleSets("Criar");
            })
            : await _validator.ValidateAsync(dto);

        if (!resultado.IsValid)
            throw new ValidacaoException(resultado.Errors.Select(e => e.ErrorMessage));
    }
}
