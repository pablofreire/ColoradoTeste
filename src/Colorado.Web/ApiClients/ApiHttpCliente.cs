using System.Text;
using System.Text.Json;

namespace Colorado.Web.ApiClients;

// Implementacao do transporte HTTP. A API serializa em camelCase, por isso a desserializacao
// e case-insensitive. Respostas 2xx sem corpo (204 de PUT/DELETE) viram sucesso; respostas de
// erro tem o envelope (mensagem/erros) extraido para o controller traduzir em ModelState.
public class ApiHttpCliente : IApiHttpCliente
{
    private static readonly JsonSerializerOptions OpcoesJson = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly HttpClient _http;

    public ApiHttpCliente(HttpClient http)
    {
        _http = http;
    }

    public Task<ResultadoApi<T>> GetAsync<T>(string rota)
        => EnviarAsync<T>(new HttpRequestMessage(HttpMethod.Get, rota));

    public Task<ResultadoApi<T>> PostAsync<T>(string rota, object corpo)
        => EnviarAsync<T>(ComCorpo(HttpMethod.Post, rota, corpo));

    public Task<ResultadoApi<object>> PutAsync(string rota, object corpo)
        => EnviarAsync<object>(ComCorpo(HttpMethod.Put, rota, corpo));

    public Task<ResultadoApi<object>> DeleteAsync(string rota)
        => EnviarAsync<object>(new HttpRequestMessage(HttpMethod.Delete, rota));

    private static HttpRequestMessage ComCorpo(HttpMethod metodo, string rota, object corpo)
    {
        return new HttpRequestMessage(metodo, rota)
        {
            Content = new StringContent(JsonSerializer.Serialize(corpo, OpcoesJson), Encoding.UTF8, "application/json")
        };
    }

    private async Task<ResultadoApi<T>> EnviarAsync<T>(HttpRequestMessage requisicao)
    {
        HttpResponseMessage resposta;
        try
        {
            resposta = await _http.SendAsync(requisicao);
        }
        catch (Exception ex)
        {
            return Falha<T>($"Falha ao comunicar com a API: {ex.Message}");
        }

        var conteudo = await resposta.Content.ReadAsStringAsync();

        if (resposta.IsSuccessStatusCode)
        {
            if (string.IsNullOrWhiteSpace(conteudo))
                return new ResultadoApi<T> { Sucesso = true };

            try
            {
                return JsonSerializer.Deserialize<ResultadoApi<T>>(conteudo, OpcoesJson)
                    ?? new ResultadoApi<T> { Sucesso = true };
            }
            catch (JsonException)
            {
                return new ResultadoApi<T> { Sucesso = true };
            }
        }

        try
        {
            var erro = JsonSerializer.Deserialize<ResultadoApi<object>>(conteudo, OpcoesJson);
            if (erro is not null)
                return new ResultadoApi<T> { Sucesso = false, Mensagem = erro.Mensagem, Erros = erro.Erros ?? new List<string>() };
        }
        catch (JsonException)
        {
            // corpo de erro nao e JSON: cai no retorno generico abaixo.
        }

        return Falha<T>($"Erro {(int)resposta.StatusCode} ao processar a requisicao.");
    }

    private static ResultadoApi<T> Falha<T>(string mensagem)
        => new() { Sucesso = false, Mensagem = mensagem };
}
