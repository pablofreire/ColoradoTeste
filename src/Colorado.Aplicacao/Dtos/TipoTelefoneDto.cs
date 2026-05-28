using System.ComponentModel.DataAnnotations;

namespace Colorado.Aplicacao.Dtos;

public class TipoTelefoneDto
{
    public int CodigoTipoTelefone { get; set; }
    public string DescricaoTipoTelefone { get; set; } = string.Empty;
    public DateTime DataInsercao { get; set; }
    public string UsuarioInsercao { get; set; } = string.Empty;
}

public class SalvarTipoTelefoneDto
{
    [Required(ErrorMessage = "A descricao e obrigatoria.")]
    [StringLength(50)]
    public string DescricaoTipoTelefone { get; set; } = string.Empty;
}
