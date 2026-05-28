using System.ComponentModel.DataAnnotations;

namespace Colorado.Aplicacao.Dtos;

public class ClienteDto
{
    public int CodigoCliente { get; set; }
    public string RazaoSocial { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public string TipoPessoa { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Complemento { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public string Uf { get; set; } = string.Empty;
    public DateTime DataInsercao { get; set; }
    public string UsuarioInsercao { get; set; } = string.Empty;

    public List<TelefoneDto> Telefones { get; set; } = new();
}

public class SalvarClienteDto
{
    [Required(ErrorMessage = "A razao social e obrigatoria.")]
    [StringLength(200)]
    public string RazaoSocial { get; set; } = string.Empty;

    [Required(ErrorMessage = "O nome fantasia e obrigatorio.")]
    [StringLength(200)]
    public string NomeFantasia { get; set; } = string.Empty;

    [Required(ErrorMessage = "O tipo de pessoa e obrigatorio.")]
    public string TipoPessoa { get; set; } = string.Empty;

    [Required(ErrorMessage = "O documento e obrigatorio.")]
    [StringLength(20)]
    public string Documento { get; set; } = string.Empty;

    public string Endereco { get; set; } = string.Empty;
    public string Complemento { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;

    [StringLength(9)]
    public string Cep { get; set; } = string.Empty;

    [Required(ErrorMessage = "A UF e obrigatoria.")]
    [StringLength(2, MinimumLength = 2)]
    public string Uf { get; set; } = string.Empty;

    public List<SalvarTelefoneDto> Telefones { get; set; } = new();
}
