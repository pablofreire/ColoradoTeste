namespace Colorado.Api.Models;

// Envelope padrao das respostas da API. Mantem o mesmo formato em sucesso e erro.
public class RespostaApi<T>
{
    public bool Sucesso { get; set; }
    public string? Mensagem { get; set; }
    public T? Dados { get; set; }
    public IEnumerable<string>? Erros { get; set; }

    public static RespostaApi<T> Ok(T dados, string? mensagem = null)
    {
        return new RespostaApi<T> { Sucesso = true, Dados = dados, Mensagem = mensagem };
    }
}

public static class RespostaApi
{
    public static RespostaApi<object> Falha(string mensagem, IEnumerable<string>? erros = null)
    {
        return new RespostaApi<object> { Sucesso = false, Mensagem = mensagem, Erros = erros };
    }
}
