using Colorado.Aplicacao.Dtos;
using Colorado.Aplicacao.Dtos.Clientes;
using Colorado.Aplicacao.Dtos.TiposTelefone;
using Colorado.Aplicacao.Excecoes;
using Colorado.Aplicacao.Servicos;
using Colorado.Aplicacao.Servicos.Clientes;
using Colorado.Aplicacao.Servicos.TiposTelefone;
using Colorado.Aplicacao.Validacoes;
using Colorado.Aplicacao.Validacoes.Clientes;
using Colorado.Aplicacao.Validacoes.TiposTelefone;
using Colorado.Dominio.Entidades;
using Colorado.Dominio.Excecoes;
using Colorado.Dominio.Interfaces;
using Colorado.Dominio.Interfaces.Clientes;
using Colorado.Dominio.Interfaces.TiposTelefone;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Colorado.Aplicacao.Testes;

public class TipoTelefoneServicoTests
{
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly ITipoTelefoneRepositorio _tipos = Substitute.For<ITipoTelefoneRepositorio>();
    private readonly TipoTelefoneServico _servico;

    public TipoTelefoneServicoTests()
    {
        _uow.TiposTelefone.Returns(_tipos);
        _servico = new TipoTelefoneServico(_uow, new SalvarTipoTelefoneValidator());
    }

    private static SalvarTipoTelefoneDto Dto(string descricao = "FAX") => new() { DescricaoTipoTelefone = descricao, Usuario = "teste" };

    [Fact]
    public async Task CriarAsync_DescricaoVazia_LancaValidacaoException()
    {
        var acao = () => _servico.CriarAsync(Dto(""), "teste");

        await acao.Should().ThrowAsync<ValidacaoException>();
    }

    [Fact]
    public async Task CriarAsync_DescricaoDuplicada_LancaDominioException()
    {
        _tipos.ExisteDescricaoAsync(Arg.Any<string>(), Arg.Any<int?>()).Returns(true);

        var acao = () => _servico.CriarAsync(Dto(), "teste");

        await acao.Should().ThrowAsync<DominioException>();
    }

    [Fact]
    public async Task CriarAsync_Valido_AdicionaECommita()
    {
        _tipos.ExisteDescricaoAsync(Arg.Any<string>(), Arg.Any<int?>()).Returns(false);

        await _servico.CriarAsync(Dto(), "teste");

        await _tipos.Received(1).AdicionarAsync(Arg.Any<TipoTelefone>());
        await _uow.Received(1).CommitAsync();
    }

    [Fact]
    public async Task RemoverAsync_TipoInexistente_LancaNaoEncontrado()
    {
        _tipos.ObterPorIdAsync(Arg.Any<object[]>()).Returns((TipoTelefone?)null);

        var acao = () => _servico.RemoverAsync(99);

        await acao.Should().ThrowAsync<NaoEncontradoException>();
    }

    [Fact]
    public async Task RemoverAsync_TipoEmUso_LancaDominioException()
    {
        _tipos.ObterPorIdAsync(Arg.Any<object[]>()).Returns(new TipoTelefone("COMERCIAL", "teste"));
        _tipos.PossuiTelefonesVinculadosAsync(Arg.Any<int>()).Returns(true);

        var acao = () => _servico.RemoverAsync(2);

        await acao.Should().ThrowAsync<DominioException>();
        await _uow.DidNotReceive().CommitAsync();
    }

    [Fact]
    public async Task RemoverAsync_TipoLivre_RemoveECommita()
    {
        _tipos.ObterPorIdAsync(Arg.Any<object[]>()).Returns(new TipoTelefone("FAX", "teste"));
        _tipos.PossuiTelefonesVinculadosAsync(Arg.Any<int>()).Returns(false);

        await _servico.RemoverAsync(5);

        _tipos.Received(1).Remover(Arg.Any<TipoTelefone>());
        await _uow.Received(1).CommitAsync();
    }
}
