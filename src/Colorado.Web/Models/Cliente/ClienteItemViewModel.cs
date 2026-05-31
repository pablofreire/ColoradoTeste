namespace Colorado.Web.Models.Cliente;

public class ClienteItemViewModel
{
    public int Codigo { get; set; }
    public string RazaoSocial { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public string TipoPessoaDescricao { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string CidadeUf { get; set; } = string.Empty;
}
