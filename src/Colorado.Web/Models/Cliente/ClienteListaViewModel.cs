namespace Colorado.Web.Models.Cliente;

public class ClienteListaViewModel
{
    public string? Nome { get; set; }
    public string? Documento { get; set; }

    public List<ClienteItemViewModel> Itens { get; set; } = new();
    public int Pagina { get; set; }
    public int TotalPaginas { get; set; }
    public int TotalRegistros { get; set; }
}

public class ClienteItemViewModel
{
    public int Codigo { get; set; }
    public string RazaoSocial { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public string TipoPessoaDescricao { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string CidadeUf { get; set; } = string.Empty;
}
