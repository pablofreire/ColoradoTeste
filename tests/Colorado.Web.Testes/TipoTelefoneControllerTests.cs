using Colorado.Aplicacao.Dtos;
using Colorado.Aplicacao.Interfaces;
using Colorado.Web.Controllers;
using Colorado.Web.Models;
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
    public async Task Listar_RetornaJsonComSucesso()
    {
        _servico.ListarAsync().Returns(new List<TipoTelefoneDto>());

        var resultado = await _controller.Listar();

        var corpo = resultado.Value.Should().BeOfType<RespostaJson>().Subject;
        corpo.Sucesso.Should().BeTrue();
    }

    [Fact]
    public async Task Criar_RetornaJsonComSucesso()
    {
        _servico.CriarAsync(Arg.Any<SalvarTipoTelefoneDto>(), Arg.Any<string>())
            .Returns(new TipoTelefoneDto { CodigoTipoTelefone = 1, DescricaoTipoTelefone = "FAX" });

        var resultado = await _controller.Criar(new SalvarTipoTelefoneDto());

        var corpo = resultado.Value.Should().BeOfType<RespostaJson>().Subject;
        corpo.Sucesso.Should().BeTrue();
    }
}
