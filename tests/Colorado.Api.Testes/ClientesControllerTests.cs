using Colorado.Aplicacao.Dtos;
using Colorado.Aplicacao.Interfaces;
using Colorado.Api.Controllers;
using Colorado.Dominio.Paginacao;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace Colorado.Api.Testes;

public class ClientesControllerTests
{
    private readonly IClienteServico _servico = Substitute.For<IClienteServico>();
    private readonly ClientesController _controller;

    public ClientesControllerTests()
    {
        _controller = new ClientesController(_servico);
    }

    [Fact]
    public async Task ObterPorId_Inexistente_RetornaNotFound()
    {
        _servico.ObterPorIdAsync(1).Returns((ClienteDto?)null);

        var resultado = await _controller.ObterPorId(1);

        resultado.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task ObterPorId_Existente_RetornaOk()
    {
        _servico.ObterPorIdAsync(1).Returns(new ClienteDto { CodigoCliente = 1 });

        var resultado = await _controller.ObterPorId(1);

        resultado.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Listar_RetornaOk()
    {
        _servico.ListarAsync(null, null, 1, 10)
            .Returns(new ResultadoPaginado<ClienteDto>(Array.Empty<ClienteDto>(), 0, 1, 10));

        var resultado = await _controller.Listar(null, null, 1, 10);

        resultado.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Criar_RetornaCreated()
    {
        _servico.CriarAsync(Arg.Any<SalvarClienteDto>(), Arg.Any<string>())
            .Returns(new ClienteDto { CodigoCliente = 5 });

        var resultado = await _controller.Criar(new SalvarClienteDto());

        resultado.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task Remover_RetornaNoContent()
    {
        var resultado = await _controller.Remover(1);

        resultado.Should().BeOfType<NoContentResult>();
        await _servico.Received(1).RemoverAsync(1);
    }
}
