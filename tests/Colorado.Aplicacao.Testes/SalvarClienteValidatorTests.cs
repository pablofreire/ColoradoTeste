using Colorado.Aplicacao.Dtos;
using Colorado.Aplicacao.Validacoes;
using FluentAssertions;
using Xunit;

namespace Colorado.Aplicacao.Testes;

public class SalvarClienteValidatorTests
{
    private readonly SalvarClienteValidator _validator = new();

    private static SalvarClienteDto ClienteValido() => new()
    {
        RazaoSocial = "Acme Comercio LTDA",
        NomeFantasia = "Acme",
        TipoPessoa = "J",
        Documento = "11222333000181",
        Uf = "SP"
    };

    [Fact]
    public void Validar_ClienteCompleto_Valido()
    {
        _validator.Validate(ClienteValido()).IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validar_RazaoSocialVazia_Invalido()
    {
        var dto = ClienteValido();
        dto.RazaoSocial = "";

        _validator.Validate(dto).IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validar_TipoPessoaInvalido_Invalido()
    {
        var dto = ClienteValido();
        dto.TipoPessoa = "X";

        _validator.Validate(dto).IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validar_CpfInvalidoParaPessoaFisica_Invalido()
    {
        var dto = ClienteValido();
        dto.TipoPessoa = "F";
        dto.Documento = "12345678900";

        _validator.Validate(dto).IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validar_CnpjInvalidoParaPessoaJuridica_Invalido()
    {
        var dto = ClienteValido();
        dto.Documento = "11222333000100";

        _validator.Validate(dto).IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validar_TelefoneComNumeroCurto_Invalido()
    {
        var dto = ClienteValido();
        dto.Telefones.Add(new SalvarTelefoneDto { NumeroTelefone = "123", CodigoTipoTelefone = 1 });

        _validator.Validate(dto).IsValid.Should().BeFalse();
    }
}
