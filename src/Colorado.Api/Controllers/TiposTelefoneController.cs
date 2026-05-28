using Colorado.Aplicacao.Dtos;
using Colorado.Aplicacao.Interfaces;
using Colorado.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Colorado.Api.Controllers;

[ApiController]
[Route("api/tipos-telefone")]
[Produces("application/json")]
public class TiposTelefoneController : ControllerBase
{
    private const string UsuarioApi = "api";

    private readonly ITipoTelefoneServico _servico;

    public TiposTelefoneController(ITipoTelefoneServico servico)
    {
        _servico = servico;
    }

    /// <summary>Lista todos os tipos de telefone cadastrados.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar()
    {
        var tipos = await _servico.ListarAsync();
        return Ok(RespostaApi<IEnumerable<TipoTelefoneDto>>.Ok(tipos));
    }

    /// <summary>Obtem um tipo de telefone pelo codigo.</summary>
    [HttpGet("{codigo:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int codigo)
    {
        var tipo = await _servico.ObterPorIdAsync(codigo);

        if (tipo is null)
            return NotFound(RespostaApi.Falha($"Tipo de telefone {codigo} nao encontrado."));

        return Ok(RespostaApi<TipoTelefoneDto>.Ok(tipo));
    }

    /// <summary>Cria um novo tipo de telefone.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] SalvarTipoTelefoneDto dto)
    {
        var criado = await _servico.CriarAsync(dto, UsuarioApi);

        return CreatedAtAction(nameof(ObterPorId), new { codigo = criado.CodigoTipoTelefone },
            RespostaApi<TipoTelefoneDto>.Ok(criado, "Tipo de telefone criado com sucesso."));
    }

    /// <summary>Atualiza a descricao de um tipo de telefone.</summary>
    [HttpPut("{codigo:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int codigo, [FromBody] SalvarTipoTelefoneDto dto)
    {
        await _servico.AtualizarAsync(codigo, dto);
        return NoContent();
    }

    /// <summary>Remove um tipo de telefone, desde que nao esteja em uso.</summary>
    [HttpDelete("{codigo:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int codigo)
    {
        await _servico.RemoverAsync(codigo);
        return NoContent();
    }
}
