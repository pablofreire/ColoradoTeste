namespace Colorado.Aplicacao.Dtos.Clientes;

public class TelefoneDto
{
    public string NumeroTelefone { get; set; } = string.Empty;
    public int CodigoTipoTelefone { get; set; }
    public string DescricaoTipoTelefone { get; set; } = string.Empty;
    public string Operadora { get; set; } = string.Empty;
    public bool Ativo { get; set; }
}
