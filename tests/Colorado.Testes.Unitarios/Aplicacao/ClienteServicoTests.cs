using Colorado.Aplicacao.Dtos;
using Colorado.Aplicacao.Excecoes;
using Colorado.Aplicacao.Servicos;
using Colorado.Aplicacao.Validacoes;
using Colorado.Dominio.Entidades;
using Colorado.Dominio.Enums;
using Colorado.Dominio.Excecoes;
using Colorado.Dominio.Interfaces;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Colorado.Testes.Unitarios.Aplicacao;

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
        _clientes.ObterComTelefonesAsync(Arg.Any<int>()).Returns(new Cliente(
            "Acme Comercio LTDA", "Acme", TipoPessoa.Juridica, "11222333000181",
            "", "", "", "", "", "SP", "teste"));

        await _servico.CriarAsync(ClienteValido(), "teste");

        await _clientes.Received(1).AdicionarAsync(Arg.Any<Cliente>());
        await _uow.Received(1).CommitAsync();
    }
}
