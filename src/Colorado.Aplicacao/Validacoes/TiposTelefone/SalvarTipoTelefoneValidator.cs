using Colorado.Aplicacao.Dtos.TiposTelefone;
using Colorado.Aplicacao.Dtos;
using FluentValidation;

namespace Colorado.Aplicacao.Validacoes.TiposTelefone;

public class SalvarTipoTelefoneValidator : AbstractValidator<SalvarTipoTelefoneDto>
{
    public SalvarTipoTelefoneValidator()
    {
        RuleFor(t => t.DescricaoTipoTelefone)
            .NotEmpty().WithMessage("A descricao e obrigatoria.")
            .MaximumLength(50).WithMessage("A descricao deve ter no maximo 50 caracteres.");

        // Usuario (quem cadastrou) so faz sentido na criacao; a atualizacao nao o altera.
        RuleSet("Criar", () =>
        {
            RuleFor(t => t.Usuario)
                .NotEmpty().WithMessage("O usuario e obrigatorio.")
                .MaximumLength(50).WithMessage("O usuario deve ter no maximo 50 caracteres.");
        });
    }
}
