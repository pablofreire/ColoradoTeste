namespace Colorado.Web.Models.TipoTelefone;

public class TipoTelefoneListaViewModel
{
    public List<TipoTelefoneItemViewModel> Itens { get; set; } = new();
}

public class TipoTelefoneItemViewModel
{
    public int Codigo { get; set; }
    public string Descricao { get; set; } = string.Empty;
}
