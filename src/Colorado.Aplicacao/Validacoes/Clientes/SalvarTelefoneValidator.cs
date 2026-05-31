using Colorado.Aplicacao.Dtos.Clientes;
using Colorado.Aplicacao.Dtos;
using Colorado.Dominio.Validacoes;
using FluentValidation;

namespace Colorado.Aplicacao.Validacoes.Clientes;

public class SalvarTelefoneValidator : AbstractValidator<SalvarTelefoneDto>
{
    public SalvarTelefoneValidator()
    {
        RuleFor(t => t.NumeroTelefone)
            .NotEmpty().WithMessage("O numero do telefone e obrigatorio.")
            .Must(numero => Documento.ApenasNumeros(numero).Length is >= 8 and <= 11)
            .WithMessage("O numero do telefone deve ter entre 8 e 11 digitos.");

        RuleFor(t => t.CodigoTipoTelefone)
            .GreaterThan(0).WithMessage("O tipo de telefone e obrigatorio.");

        RuleFor(t => t.Operadora)
            .MaximumLength(50).WithMessage("A operadora deve ter no maximo 50 caracteres.");
    }
}
