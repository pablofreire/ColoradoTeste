using Colorado.Aplicacao.Dtos.TiposTelefone;
using Colorado.Web.ApiClients;
using Colorado.Web.ApiClients.TiposTelefone;
using Colorado.Web.Controllers;
using Colorado.Web.Models;
using Colorado.Web.Models.TipoTelefone;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace Colorado.Web.Testes;

public class TipoTelefoneControllerTests
{
    private readonly ITipoTelefoneApiClient _tipoApi = Substitute.For<ITipoTelefoneApiClient>();
    private readonly TipoTelefoneController _controller;

    public TipoTelefoneControllerTests()
    {
        _controller = new TipoTelefoneController(_tipoApi)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
    }

    [Fact]
    public void Index_RetornaView()
    {
        _controller.Index().Should().BeOfType<ViewResult>();
    }

    [Fact]
    public async Task Listar_RetornaPartialDaTabela()
    {
        _tipoApi.ListarAsync().Returns(new ResultadoApi<IEnumerable<TipoTelefoneDto>>
        {
            Sucesso = true,
            Dados = new List<TipoTelefoneDto>()
        });

        var resultado = await _controller.Listar();

        var partial = resultado.Should().BeOfType<PartialViewResult>().Subject;
        partial.ViewName.Should().Be("_TabelaTiposTelefone");
        partial.Model.Should().BeOfType<TipoTelefoneListaViewModel>();
    }

    [Fact]
    public async Task Salvar_Valido_RetornaJsonSucesso()
    {
        _tipoApi.CriarAsync(Arg.Any<SalvarTipoTelefoneDto>())
            .Returns(new ResultadoApi<TipoTelefoneDto> { Sucesso = true });

        var resultado = await _controller.Salvar(new SalvarTipoTelefoneViewModel { Descricao = "FAX" });

        resultado.Should().BeOfType<JsonResult>().Which.Value
            .Should().BeOfType<RespostaJson>().Which.Sucesso.Should().BeTrue();
        await _tipoApi.Received(1).CriarAsync(Arg.Any<SalvarTipoTelefoneDto>());
    }

    [Fact]
    public async Task Salvar_Duplicado_RetornaJsonComErros()
    {
        _tipoApi.CriarAsync(Arg.Any<SalvarTipoTelefoneDto>()).Returns(new ResultadoApi<TipoTelefoneDto>
        {
            Sucesso = false,
            Erros = new List<string> { "Ja existe." }
        });

        var resultado = await _controller.Salvar(new SalvarTipoTelefoneViewModel { Descricao = "Residencial" });

        var json = resultado.Should().BeOfType<JsonResult>().Which.Value
            .Should().BeOfType<RespostaJson>().Subject;
        json.Sucesso.Should().BeFalse();
        json.Erros.Should().Contain("Ja existe.");
        _controller.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Excluir_RetornaJsonSucesso()
    {
        _tipoApi.RemoverAsync(1).Returns(new ResultadoApi<object> { Sucesso = true });

        var resultado = await _controller.Excluir(1);

        resultado.Should().BeOfType<JsonResult>().Which.Value
            .Should().BeOfType<RespostaJson>().Which.Sucesso.Should().BeTrue();
        await _tipoApi.Received(1).RemoverAsync(1);
    }
}
