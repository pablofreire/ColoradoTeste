namespace Colorado.Web.Models.Cliente;

public class SalvarClienteViewModel
{
    public int? Codigo { get; set; }
    public string RazaoSocial { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public string TipoPessoa { get; set; } = "J";
    public string Documento { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Complemento { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public string Uf { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;

    public List<SalvarTelefoneViewModel> Telefones { get; set; } = new();

    // Preenchido pelo controller para alimentar o combo de tipos no formulario.
    public List<TipoTelefoneOpcaoViewModel> TiposDisponiveis { get; set; } = new();

    public bool EhEdicao => Codigo.HasValue;
}
