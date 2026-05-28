using Colorado.Aplicacao.Dtos;
using Colorado.Dominio.Validacoes;
using FluentValidation;

namespace Colorado.Aplicacao.Validacoes;

public class SalvarClienteValidator : AbstractValidator<SalvarClienteDto>
{
    public SalvarClienteValidator()
    {
        RuleFor(c => c.RazaoSocial)
            .NotEmpty().WithMessage("A razao social e obrigatoria.")
            .MaximumLength(200).WithMessage("A razao social deve ter no maximo 200 caracteres.");

        RuleFor(c => c.NomeFantasia)
            .NotEmpty().WithMessage("O nome fantasia e obrigatorio.")
            .MaximumLength(200).WithMessage("O nome fantasia deve ter no maximo 200 caracteres.");

        RuleFor(c => c.TipoPessoa)
            .Must(tipo => tipo == "F" || tipo == "J")
            .WithMessage("O tipo de pessoa deve ser 'F' (fisica) ou 'J' (juridica).");

        // CPF para pessoa fisica, CNPJ para juridica - reaproveita o validador de dominio.
        RuleFor(c => c.Documento)
            .NotEmpty().WithMessage("O documento e obrigatorio.")
            .Must((dto, documento) => DocumentoValidoParaTipo(dto.TipoPessoa, documento))
            .WithMessage("Documento invalido para o tipo de pessoa informado.");

        RuleFor(c => c.Uf)
            .NotEmpty().WithMessage("A UF e obrigatoria.")
            .Length(2).WithMessage("A UF deve ter 2 caracteres.");

        RuleFor(c => c.Cep)
            .MaximumLength(9).WithMessage("O CEP deve ter no maximo 9 caracteres.");

        RuleForEach(c => c.Telefones).SetValidator(new SalvarTelefoneValidator());
    }

    private static bool DocumentoValidoParaTipo(string tipoPessoa, string documento)
    {
        return tipoPessoa switch
        {
            "F" => Documento.EhCpfValido(documento),
            "J" => Documento.EhCnpjValido(documento),
            _ => false
        };
    }
}
