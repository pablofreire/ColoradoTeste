using Colorado.Aplicacao.Dtos;
using Colorado.Aplicacao.Interfaces;
using Colorado.Web.Controllers;
using Colorado.Web.Models;
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
        _controller = new ClienteController(_clienteServico, _tipoServico)
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
        _clienteServico.ListarAsync(null, null, 1, 10)
            .Returns(new ResultadoPaginado<ClienteDto>(Array.Empty<ClienteDto>(), 0, 1, 10));

        var resultado = await _controller.Listar(null, null, 1, 10);

        var corpo = resultado.Value.Should().BeOfType<RespostaJson>().Subject;
        corpo.Sucesso.Should().BeTrue();
    }

    [Fact]
    public async Task Criar_RetornaJsonComSucesso()
    {
        _clienteServico.CriarAsync(Arg.Any<SalvarClienteDto>(), Arg.Any<string>())
            .Returns(new ClienteDto { CodigoCliente = 1 });

        var resultado = await _controller.Criar(new SalvarClienteDto());

        var corpo = resultado.Value.Should().BeOfType<RespostaJson>().Subject;
        corpo.Sucesso.Should().BeTrue();
    }
}
