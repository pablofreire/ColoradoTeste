using Colorado.Aplicacao.Dtos;
using Colorado.Aplicacao.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Colorado.Web.Controllers;

public class TipoTelefoneController : ControladorBase
{
    private const string UsuarioWeb = "web";

    private readonly ITipoTelefoneServico _servico;

    public TipoTelefoneController(ITipoTelefoneServico servico)
    {
        _servico = servico;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public Task<JsonResult> Listar()
    {
        return ExecutarAsync(async () => await _servico.ListarAsync());
    }

    [HttpGet]
    public Task<JsonResult> Obter(int codigo)
    {
        return ExecutarAsync(async () =>
        {
            var tipo = await _servico.ObterPorIdAsync(codigo);
            return tipo ?? throw new Aplicacao.Excecoes.NaoEncontradoException($"Tipo de telefone {codigo} nao encontrado.");
        });
    }

    [HttpPost]
    public Task<JsonResult> Criar([FromBody] SalvarTipoTelefoneDto dto)
    {
        return ExecutarAsync(async () => await _servico.CriarAsync(dto, UsuarioWeb),
            "Tipo de telefone salvo com sucesso.");
    }

    [HttpPost]
    public Task<JsonResult> Atualizar(int codigo, [FromBody] SalvarTipoTelefoneDto dto)
    {
        return ExecutarAsync(async () =>
        {
            await _servico.AtualizarAsync(codigo, dto);
            return (object?)null;
        }, "Tipo de telefone atualizado com sucesso.");
    }

    [HttpPost]
    public Task<JsonResult> Excluir(int codigo)
    {
        return ExecutarAsync(async () =>
        {
            await _servico.RemoverAsync(codigo);
            return (object?)null;
        }, "Tipo de telefone removido com sucesso.");
    }
}
