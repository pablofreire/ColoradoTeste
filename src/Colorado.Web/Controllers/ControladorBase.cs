using Colorado.Aplicacao.Excecoes;
using Colorado.Dominio.Excecoes;
using Colorado.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Colorado.Web.Controllers;

public abstract class ControladorBase : Controller
{
    // Embrulha as chamadas de servico das acoes AJAX no envelope RespostaJson e
    // converte excecoes de negocio em status HTTP, evitando try/catch repetido.
    protected async Task<JsonResult> ExecutarAsync(Func<Task<object?>> acao, string? mensagemSucesso = null)
    {
        try
        {
            var dados = await acao();
            return Json(new RespostaJson { Sucesso = true, Dados = dados, Mensagem = mensagemSucesso });
        }
        catch (ValidacaoException ex)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return Json(new RespostaJson { Sucesso = false, Mensagem = string.Join(" ", ex.Erros) });
        }
        catch (DominioException ex)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return Json(new RespostaJson { Sucesso = false, Mensagem = ex.Message });
        }
        catch (NaoEncontradoException ex)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return Json(new RespostaJson { Sucesso = false, Mensagem = ex.Message });
        }
    }
}
