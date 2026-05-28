using Colorado.Aplicacao.Dtos;
using Colorado.Aplicacao.Excecoes;
using Colorado.Aplicacao.Servicos;
using Colorado.Aplicacao.Validacoes;
using Colorado.Dominio.Entidades;
using Colorado.Dominio.Enums;
using Colorado.Dominio.Excecoes;
using Colorado.Dominio.Interfaces;
using Colorado.Dominio.Paginacao;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Colorado.Aplicacao.Testes;

public class ClienteServicoTests
{
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly IClienteRepositorio _clientes = Substitute.For<IClienteRepositorio>();
    private readonly ITipoTelefoneRepositorio _tipos = Substitute.For<ITipoTelefoneRepositorio>();
    private readonly ClienteServico _servico;

    public ClienteServicoTests()
    {
        _uow.Clientes.Returns(_clientes);
        _uow.TiposTelefone.Returns(_tipos);
        // Usa o validator real para exercitar a integracao validacao + servico.
        _servico = new ClienteServico(_uow, new SalvarClienteValidator());
    }

    private static SalvarClienteDto ClienteValido() => new()
    {
        RazaoSocial = "Acme Comercio LTDA",
        NomeFantasia = "Acme",
        TipoPessoa = "J",
        Documento = "11222333000181",
        Uf = "SP"
    };

    private static Cliente ClienteEntidade() => new(
        "Acme Comercio LTDA", "Acme", TipoPessoa.Juridica, "11222333000181",
        "", "", "", "", "", "SP", "teste");

    [Fact]
    public async Task CriarAsync_DadosInvalidos_LancaValidacaoException()
    {
        var dto = ClienteValido();
        dto.RazaoSocial = "";

        var acao = () => _servico.CriarAsync(dto, "teste");

        await acao.Should().ThrowAsync<ValidacaoException>();
        await _clientes.DidNotReceive().AdicionarAsync(Arg.Any<Cliente>());
    }

    [Fact]
    public async Task CriarAsync_DocumentoDuplicado_LancaDominioException()
    {
        _clientes.ExisteDocumentoAsync(Arg.Any<string>(), Arg.Any<int?>()).Returns(true);

        var acao = () => _servico.CriarAsync(ClienteValido(), "teste");

        await acao.Should().ThrowAsync<DominioException>();
    }

    [Fact]
    public async Task CriarAsync_DadosValidos_AdicionaECommita()
    {
        _clientes.ExisteDocumentoAsync(Arg.Any<string>(), Arg.Any<int?>()).Returns(false);
        _clientes.ObterComTelefonesAsync(Arg.Any<int>()).Returns(ClienteEntidade());

        await _servico.CriarAsync(ClienteValido(), "teste");

        await _clientes.Received(1).AdicionarAsync(Arg.Any<Cliente>());
        await _uow.Received(1).CommitAsync();
    }

    [Fact]
    public async Task ObterPorIdAsync_Inexistente_RetornaNull()
    {
        _clientes.ObterComTelefonesAsync(Arg.Any<int>()).Returns((Cliente?)null);

        var resultado = await _servico.ObterPorIdAsync(1);

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ObterPorIdAsync_Existente_RetornaDto()
    {
        _clientes.ObterComTelefonesAsync(1).Returns(ClienteEntidade());

        var resultado = await _servico.ObterPorIdAsync(1);

        resultado.Should().NotBeNull();
        resultado!.RazaoSocial.Should().Be("Acme Comercio LTDA");
    }

    [Fact]
    public async Task AtualizarAsync_Inexistente_LancaNaoEncontrado()
    {
        _clientes.ObterComTelefonesAsync(Arg.Any<int>()).Returns((Cliente?)null);

        var acao = () => _servico.AtualizarAsync(99, ClienteValido(), "teste");

        await acao.Should().ThrowAsync<NaoEncontradoException>();
    }

    [Fact]
    public async Task AtualizarAsync_Existente_AtualizaECommita()
    {
        _clientes.ObterComTelefonesAsync(1).Returns(ClienteEntidade());
        _clientes.ExisteDocumentoAsync(Arg.Any<string>(), Arg.Any<int?>()).Returns(false);

        await _servico.AtualizarAsync(1, ClienteValido(), "teste");

        _clientes.Received(1).Atualizar(Arg.Any<Cliente>());
        await _uow.Received(1).CommitAsync();
    }

    [Fact]
    public async Task RemoverAsync_Inexistente_LancaNaoEncontrado()
    {
        _clientes.ObterPorIdAsync(Arg.Any<object[]>()).Returns((Cliente?)null);

        var acao = () => _servico.RemoverAsync(99);

        await acao.Should().ThrowAsync<NaoEncontradoException>();
    }

    [Fact]
    public async Task RemoverAsync_Existente_RemoveECommita()
    {
        _clientes.ObterPorIdAsync(Arg.Any<object[]>()).Returns(ClienteEntidade());

        await _servico.RemoverAsync(1);

        _clientes.Received(1).Remover(Arg.Any<Cliente>());
        await _uow.Received(1).CommitAsync();
    }

    [Fact]
    public async Task ListarAsync_RetornaResultadoMapeado()
    {
        var pagina = new ResultadoPaginado<Cliente>(new[] { ClienteEntidade() }, 1, 1, 10);
        _clientes.ListarPaginadoAsync(null, null, 1, 10).Returns(pagina);

        var resultado = await _servico.ListarAsync(null, null, 1, 10);

        resultado.TotalRegistros.Should().Be(1);
        resultado.Itens.Should().ContainSingle();
    }
}
