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
