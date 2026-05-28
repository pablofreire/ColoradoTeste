namespace Colorado.Web.Models;

// Formato unico das respostas AJAX consumidas pelo JavaScript das telas.
public class RespostaJson
{
    public bool Sucesso { get; set; }
    public string? Mensagem { get; set; }
    public object? Dados { get; set; }
}
