using Colorado.Aplicacao.Dtos;
using FluentValidation;

namespace Colorado.Aplicacao.Validacoes;

public class SalvarTipoTelefoneValidator : AbstractValidator<SalvarTipoTelefoneDto>
{
    public SalvarTipoTelefoneValidator()
    {
        RuleFor(t => t.DescricaoTipoTelefone)
            .NotEmpty().WithMessage("A descricao e obrigatoria.")
            .MaximumLength(50).WithMessage("A descricao deve ter no maximo 50 caracteres.");
    }
}
