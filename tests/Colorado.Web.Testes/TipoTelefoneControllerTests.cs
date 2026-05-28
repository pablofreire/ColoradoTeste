using Colorado.Aplicacao.Dtos;
using Colorado.Aplicacao.Excecoes;
using Colorado.Aplicacao.Interfaces;
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
    private readonly ITipoTelefoneServico _servico = Substitute.For<ITipoTelefoneServico>();
    private readonly TipoTelefoneController _controller;

    public TipoTelefoneControllerTests()
    {
        _controller = new TipoTelefoneController(_servico)
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
        _servico.ListarAsync().Returns(new List<TipoTelefoneDto>());

        var resultado = await _controller.Listar();

        var partial = resultado.Should().BeOfType<PartialViewResult>().Subject;
        partial.ViewName.Should().Be("_TabelaTiposTelefone");
        partial.Model.Should().BeOfType<TipoTelefoneListaViewModel>();
    }

    [Fact]
    public async Task Salvar_Valido_RetornaJsonSucesso()
    {
        var vm = new SalvarTipoTelefoneViewModel { Descricao = "FAX" };

        var resultado = await _controller.Salvar(vm);

        resultado.Should().BeOfType<JsonResult>().Which.Value
            .Should().BeOfType<RespostaJson>().Which.Sucesso.Should().BeTrue();
        await _servico.Received(1).CriarAsync(Arg.Any<SalvarTipoTelefoneDto>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Salvar_Duplicado_RetornaPartialComModelStateInvalido()
    {
        _servico
            .When(s => s.CriarAsync(Arg.Any<SalvarTipoTelefoneDto>(), Arg.Any<string>()))
            .Do(_ => throw new Colorado.Dominio.Excecoes.DominioException("Ja existe."));

        var resultado = await _controller.Salvar(new SalvarTipoTelefoneViewModel { Descricao = "RESIDENCIAL" });

        resultado.Should().BeOfType<PartialViewResult>().Which.ViewName.Should().Be("_FormularioTipoTelefone");
        _controller.ModelState.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Excluir_RetornaJsonSucesso()
    {
        var resultado = await _controller.Excluir(1);

        resultado.Should().BeOfType<JsonResult>().Which.Value
            .Should().BeOfType<RespostaJson>().Which.Sucesso.Should().BeTrue();
        await _servico.Received(1).RemoverAsync(1);
    }
}
