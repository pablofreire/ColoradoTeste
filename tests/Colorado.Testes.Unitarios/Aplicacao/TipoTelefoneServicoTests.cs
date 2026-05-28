using Colorado.Aplicacao.Excecoes;
using Colorado.Aplicacao.Servicos;
using Colorado.Aplicacao.Validacoes;
using Colorado.Dominio.Entidades;
using Colorado.Dominio.Excecoes;
using Colorado.Dominio.Interfaces;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Colorado.Testes.Unitarios.Aplicacao;

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
}
