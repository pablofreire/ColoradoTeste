namespace Colorado.Web.ApiClients;

// Espelha o envelope devolvido pela API (sucesso, mensagem, dados, erros) e serve tanto como
// alvo de desserializacao quanto como resultado retornado aos controllers da Web.
public class ResultadoApi<T>
{
    public bool Sucesso { get; set; }
    public T? Dados { get; set; }
    public string? Mensagem { get; set; }
    public List<string> Erros { get; set; } = new();
}
