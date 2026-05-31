using Colorado.Aplicacao.Dtos.Clientes;
using Colorado.Aplicacao.Interfaces.Clientes;
using Colorado.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Colorado.Api.Controllers;

[ApiController]
[Route("api/clientes")]
[Produces("application/json")]
public class ClientesController : ControllerBase
{
    private readonly IClienteServico _servico;

    public ClientesController(IClienteServico servico)
    {
        _servico = servico;
    }

    /// <summary>Lista clientes de forma paginada, com filtro opcional por nome e documento.</summary>
    /// <param name="nome">Texto a procurar na razao social ou nome fantasia.</param>
    /// <param name="documento">CPF ou CNPJ (ou parte) a procurar.</param>
    /// <param name="pagina">Numero da pagina (inicia em 1).</param>
    /// <param name="tamanhoPagina">Quantidade de registros por pagina.</param>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] string? nome, [FromQuery] string? documento,
        [FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 10)
    {
        var resultado = await _servico.ListarAsync(nome, documento, pagina, tamanhoPagina);
        return Ok(RespostaApi<object>.Ok(resultado));
    }

    /// <summary>Obtem um cliente pelo codigo, incluindo seus telefones.</summary>
    [HttpGet("{codigo:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int codigo)
    {
        var cliente = await _servico.ObterPorIdAsync(codigo);

        if (cliente is null)
            return NotFound(RespostaApi<object>.Falha($"Cliente {codigo} nao encontrado."));

        return Ok(RespostaApi<ClienteDto>.Ok(cliente));
    }

    /// <summary>Cria um novo cliente.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] SalvarClienteDto dto)
    {
        var criado = await _servico.CriarAsync(dto, dto.Usuario);

        return CreatedAtAction(nameof(ObterPorId), new { codigo = criado.CodigoCliente },
            RespostaApi<ClienteDto>.Ok(criado, "Cliente criado com sucesso."));
    }

    /// <summary>Atualiza os dados de um cliente existente.</summary>
    [HttpPut("{codigo:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int codigo, [FromBody] SalvarClienteDto dto)
    {
        await _servico.AtualizarAsync(codigo, dto, dto.Usuario);
        return NoContent();
    }

    /// <summary>Remove um cliente e seus telefones.</summary>
    [HttpDelete("{codigo:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int codigo)
    {
        await _servico.RemoverAsync(codigo);
        return NoContent();
    }
}
