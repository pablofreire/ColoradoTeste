using Colorado.Dominio.Validacoes;
using FluentAssertions;
using Xunit;

namespace Colorado.Testes.Unitarios.Dominio;

public class DocumentoTests
{
    [Theory]
    [InlineData("529.982.247-25")]
    [InlineData("52998224725")]
    public void EhCpfValido_CpfCorreto_RetornaTrue(string cpf)
    {
        Documento.EhCpfValido(cpf).Should().BeTrue();
    }

    [Theory]
    [InlineData("111.111.111-11")]
    [InlineData("123")]
    [InlineData("52998224724")]
    public void EhCpfValido_CpfIncorreto_RetornaFalse(string cpf)
    {
        Documento.EhCpfValido(cpf).Should().BeFalse();
    }

    [Theory]
    [InlineData("11.222.333/0001-81")]
    [InlineData("11222333000181")]
    public void EhCnpjValido_CnpjCorreto_RetornaTrue(string cnpj)
    {
        Documento.EhCnpjValido(cnpj).Should().BeTrue();
    }

    [Theory]
    [InlineData("11.222.333/0001-00")]
    [InlineData("00000000000000")]
    [InlineData("123")]
    public void EhCnpjValido_CnpjIncorreto_RetornaFalse(string cnpj)
    {
        Documento.EhCnpjValido(cnpj).Should().BeFalse();
    }

    [Fact]
    public void ApenasNumeros_RemoveCaracteresNaoNumericos()
    {
        Documento.ApenasNumeros("11.222.333/0001-81").Should().Be("11222333000181");
    }
}
