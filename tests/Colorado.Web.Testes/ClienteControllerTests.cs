using Colorado.Aplicacao.Dtos.Clientes;
using Colorado.Aplicacao.Dtos.TiposTelefone;
using Colorado.Dominio.Paginacao;
using Colorado.Web.ApiClients;
using Colorado.Web.ApiClients.Clientes;
using Colorado.Web.ApiClients.TiposTelefone;
using Colorado.Web.Controllers;
using Colorado.Web.Models;
using Colorado.Web.Models.Cliente;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace Colorado.Web.Testes;

public class ClienteControllerTests
{
    private readonly IClienteApiClient _clienteApi = Substitute.For<IClienteApiClient>();
    private readonly ITipoTelefoneApiClient _tipoApi = Substitute.For<ITipoTelefoneApiClient>();
    private readonly ClienteController _controller;

    public ClienteControllerTests()
    {
        _tipoApi.ListarAsync().Returns(new ResultadoApi<IEnumerable<TipoTelefoneDto>>
        {
            Sucesso = true,
            Dados = new List<TipoTelefoneDto>()
        });

        _controller = new ClienteController(_clienteApi, _tipoApi)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
    }

    private static SalvarClienteViewModel FormularioValido() => new()
    {
        RazaoSocial = "Acme",
        NomeFantasia = "Acme",
        TipoPessoa = "J",
        Documento = "11222333000181",
        Uf = "SP"
    };

    [Fact]
    public void Index_RetornaView()
    {
        _controller.Index().Should().BeOfType<ViewResult>();
    }

    [Fact]
    public async Task Listar_RetornaPartialDaTabela()
    {
        _clienteApi.ListarAsync(null, null, 1, 10).Returns(new ResultadoApi<ResultadoPaginado<ClienteDto>>
        {
            Sucesso = true,
            Dados = new ResultadoPaginado<ClienteDto>(Array.Empty<ClienteDto>(), 0, 1, 10)
        });

        var resultado = await _controller.Listar(null, null, 1);

        var partial = resultado.Should().BeOfType<PartialViewResult>().Subject;
        partial.ViewName.Should().Be("_TabelaClientes");
        partial.Model.Should().BeOfType<ClienteListaViewModel>();
    }

    [Fact]
    public async Task Formulario_Novo_RetornaPartialComViewModelVazio()
    {
        var resultado = await _controller.Formulario(null);

        var partial = resultado.Should().BeOfType<PartialViewResult>().Subject;
        partial.ViewName.Should().Be("_FormularioCliente");
        partial.Model.Should().BeOfType<SalvarClienteViewModel>().Which.EhEdicao.Should().BeFalse();
    }

    [Fact]
    public async Task Salvar_Valido_RetornaJsonSucesso()
    {
        _clienteApi.CriarAsync(Arg.Any<SalvarClienteDto>())
            .Returns(new ResultadoApi<ClienteDto> { Sucesso = true });

        var resultado = await _controller.Salvar(FormularioValido());

        var json = resultado.Should().BeOfType<JsonResult>().Subject;
        json.Value.Should().BeOfType<RespostaJson>().Which.Sucesso.Should().BeTrue();
        await _clienteApi.Received(1).CriarAsync(Arg.Any<SalvarClienteDto>());
    }

    [Fact]
    public async Task Salvar_Invalido_RetornaJsonComErros()
    {
        _clienteApi.CriarAsync(Arg.Any<SalvarClienteDto>()).Returns(new ResultadoApi<ClienteDto>
        {
            Sucesso = false,
            Erros = new List<string> { "Documento invalido." }
        });

        var resultado = await _controller.Salvar(FormularioValido());

        var json = resultado.Should().BeOfType<JsonResult>().Which.Value
            .Should().BeOfType<RespostaJson>().Subject;
        json.Sucesso.Should().BeFalse();
        json.Erros.Should().Contain("Documento invalido.");
        _controller.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Excluir_RetornaJsonSucesso()
    {
        _clienteApi.RemoverAsync(1).Returns(new ResultadoApi<object> { Sucesso = true });

        var resultado = await _controller.Excluir(1);

        resultado.Should().BeOfType<JsonResult>().Which.Value
            .Should().BeOfType<RespostaJson>().Which.Sucesso.Should().BeTrue();
        await _clienteApi.Received(1).RemoverAsync(1);
    }
}
