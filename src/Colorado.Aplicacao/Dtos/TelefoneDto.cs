namespace Colorado.Aplicacao.Dtos;

public class TelefoneDto
{
    public string NumeroTelefone { get; set; } = string.Empty;
    public int CodigoTipoTelefone { get; set; }
    public string DescricaoTipoTelefone { get; set; } = string.Empty;
    public string Operadora { get; set; } = string.Empty;
    public bool Ativo { get; set; }
}

public class SalvarTelefoneDto
{
    public string NumeroTelefone { get; set; } = string.Empty;
    public int CodigoTipoTelefone { get; set; }
    public string Operadora { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
}
