namespace Colorado.Aplicacao.Dtos.Clientes;

public class SalvarTelefoneDto
{
    public string NumeroTelefone { get; set; } = string.Empty;
    public int CodigoTipoTelefone { get; set; }
    public string Operadora { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
}
