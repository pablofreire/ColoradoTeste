namespace Colorado.Web.Models.TipoTelefone;

public class SalvarTipoTelefoneViewModel
{
    public int? Codigo { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;

    public bool EhEdicao => Codigo.HasValue;
}
