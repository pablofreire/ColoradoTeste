using Colorado.Aplicacao.Dtos;
using Colorado.Aplicacao.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Colorado.Web.Controllers;

public class ClienteController : ControladorBase
{
    private const string UsuarioWeb = "web";

    private readonly IClienteServico _clienteServico;
    private readonly ITipoTelefoneServico _tipoTelefoneServico;

    public ClienteController(IClienteServico clienteServico, ITipoTelefoneServico tipoTelefoneServico)
    {
        _clienteServico = clienteServico;
        _tipoTelefoneServico = tipoTelefoneServico;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public Task<JsonResult> Listar(string? nome, string? documento, int pagina = 1, int tamanhoPagina = 10)
    {
        return ExecutarAsync(async () =>
            await _clienteServico.ListarAsync(nome, documento, pagina, tamanhoPagina));
    }

    [HttpGet]
    public Task<JsonResult> Obter(int codigo)
    {
        return ExecutarAsync(async () =>
        {
            var cliente = await _clienteServico.ObterPorIdAsync(codigo);
            return cliente ?? throw new Aplicacao.Excecoes.NaoEncontradoException($"Cliente {codigo} nao encontrado.");
        });
    }

    // Tipos de telefone alimentam o combo do formulario de telefones.
    [HttpGet]
    public Task<JsonResult> TiposTelefone()
    {
        return ExecutarAsync(async () => await _tipoTelefoneServico.ListarAsync());
    }

    [HttpPost]
    public Task<JsonResult> Criar([FromBody] SalvarClienteDto dto)
    {
        return ExecutarAsync(async () => await _clienteServico.CriarAsync(dto, UsuarioWeb),
            "Cliente salvo com sucesso.");
    }

    [HttpPost]
    public Task<JsonResult> Atualizar(int codigo, [FromBody] SalvarClienteDto dto)
    {
        return ExecutarAsync(async () =>
        {
            await _clienteServico.AtualizarAsync(codigo, dto, UsuarioWeb);
            return (object?)null;
        }, "Cliente atualizado com sucesso.");
    }

    [HttpPost]
    public Task<JsonResult> Excluir(int codigo)
    {
        return ExecutarAsync(async () =>
        {
            await _clienteServico.RemoverAsync(codigo);
            return (object?)null;
        }, "Cliente removido com sucesso.");
    }
}
