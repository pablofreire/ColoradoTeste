using Colorado.Aplicacao.Dtos;
using Colorado.Aplicacao.Excecoes;
using Colorado.Aplicacao.Interfaces;
using Colorado.Web.Controllers;
using Colorado.Web.Models;
using Colorado.Web.Models.Cliente;
using Colorado.Dominio.Paginacao;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace Colorado.Web.Testes;

public class ClienteControllerTests
{
    private readonly IClienteServico _clienteServico = Substitute.For<IClienteServico>();
    private readonly ITipoTelefoneServico _tipoServico = Substitute.For<ITipoTelefoneServico>();
    private readonly ClienteController _controller;

    public ClienteControllerTests()
    {
        _tipoServico.ListarAsync().Returns(new List<TipoTelefoneDto>());
        _controller = new ClienteController(_clienteServico, _tipoServico)
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
        _clienteServico.ListarAsync(null, null, 1, 10)
            .Returns(new ResultadoPaginado<ClienteDto>(Array.Empty<ClienteDto>(), 0, 1, 10));

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
        var resultado = await _controller.Salvar(FormularioValido());

        var json = resultado.Should().BeOfType<JsonResult>().Subject;
        json.Value.Should().BeOfType<RespostaJson>().Which.Sucesso.Should().BeTrue();
        await _clienteServico.Received(1).CriarAsync(Arg.Any<SalvarClienteDto>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Salvar_Invalido_RetornaPartialComModelStateInvalido()
    {
        _clienteServico
            .When(s => s.CriarAsync(Arg.Any<SalvarClienteDto>(), Arg.Any<string>()))
            .Do(_ => throw new ValidacaoException(new[] { "Documento invalido." }));

        var resultado = await _controller.Salvar(FormularioValido());

        resultado.Should().BeOfType<PartialViewResult>().Which.ViewName.Should().Be("_FormularioCliente");
        _controller.ModelState.IsValid.Should().BeFalse();
        _controller.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Excluir_RetornaJsonSucesso()
    {
        var resultado = await _controller.Excluir(1);

        resultado.Should().BeOfType<JsonResult>().Which.Value
            .Should().BeOfType<RespostaJson>().Which.Sucesso.Should().BeTrue();
        await _clienteServico.Received(1).RemoverAsync(1);
    }
}
