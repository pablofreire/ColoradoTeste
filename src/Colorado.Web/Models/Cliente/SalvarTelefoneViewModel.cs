namespace Colorado.Web.Models.Cliente;

public class SalvarTelefoneViewModel
{
    public string NumeroTelefone { get; set; } = string.Empty;
    public int CodigoTipoTelefone { get; set; }
    public string Operadora { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
}
