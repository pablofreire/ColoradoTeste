using System.Text.Json;
using Colorado.Api.Models;
using Colorado.Aplicacao.Excecoes;
using Colorado.Dominio.Excecoes;

namespace Colorado.Api.Middleware;

// Captura excecoes nao tratadas e devolve sempre o envelope RespostaApi com o
// status HTTP adequado: regra de negocio -> 400, recurso ausente -> 404, resto -> 500.
public class MiddlewareTratamentoExcecao
{
    private readonly RequestDelegate _proximo;
    private readonly ILogger<MiddlewareTratamentoExcecao> _logger;

    public MiddlewareTratamentoExcecao(RequestDelegate proximo, ILogger<MiddlewareTratamentoExcecao> logger)
    {
        _proximo = proximo;
        _logger = logger;
    }

    public async Task Invoke(HttpContext contexto)
    {
        try
        {
            await _proximo(contexto);
        }
        catch (ValidacaoException ex)
        {
            await Responder(contexto, StatusCodes.Status400BadRequest, ex.Message, ex.Erros);
        }
        catch (DominioException ex)
        {
            await Responder(contexto, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (NaoEncontradoException ex)
        {
            await Responder(contexto, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro nao tratado ao processar {Caminho}", contexto.Request.Path);
            await Responder(contexto, StatusCodes.Status500InternalServerError,
                "Ocorreu um erro inesperado ao processar a requisicao.");
        }
    }

    private static async Task Responder(HttpContext contexto, int statusCode, string mensagem,
        IEnumerable<string>? erros = null)
    {
        contexto.Response.ContentType = "application/json";
        contexto.Response.StatusCode = statusCode;

        var corpo = RespostaApi.Falha(mensagem, erros);
        var opcoes = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        await contexto.Response.WriteAsync(JsonSerializer.Serialize(corpo, opcoes));
    }
}
