using Colorado.Aplicacao.Dtos;
using Colorado.Aplicacao.Interfaces;
using Colorado.Api.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace Colorado.Api.Testes;

public class TiposTelefoneControllerTests
{
    private readonly ITipoTelefoneServico _servico = Substitute.For<ITipoTelefoneServico>();
    private readonly TiposTelefoneController _controller;

    public TiposTelefoneControllerTests()
    {
        _controller = new TiposTelefoneController(_servico);
    }

    [Fact]
    public async Task ObterPorId_Inexistente_RetornaNotFound()
    {
        _servico.ObterPorIdAsync(1).Returns((TipoTelefoneDto?)null);

        var resultado = await _controller.ObterPorId(1);

        resultado.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Criar_RetornaCreated()
    {
        _servico.CriarAsync(Arg.Any<SalvarTipoTelefoneDto>(), Arg.Any<string>())
            .Returns(new TipoTelefoneDto { CodigoTipoTelefone = 5, DescricaoTipoTelefone = "FAX" });

        var resultado = await _controller.Criar(new SalvarTipoTelefoneDto());

        resultado.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task Listar_RetornaOk()
    {
        _servico.ListarAsync().Returns(new List<TipoTelefoneDto>());

        var resultado = await _controller.Listar();

        resultado.Should().BeOfType<OkObjectResult>();
    }
}
