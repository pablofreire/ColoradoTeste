using Colorado.Dominio.Validacoes;
using FluentAssertions;
using Xunit;

namespace Colorado.Dominio.Testes;

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
    [InlineData("11.222.333/0001-81")]   // CNPJ numerico antigo (continua valido)
    [InlineData("11222333000181")]
    [InlineData("12.ABC.345/01DE-35")]   // CNPJ alfanumerico (Receita, jul/2026)
    [InlineData("12ABC34501DE35")]
    public void EhCnpjValido_CnpjCorreto_RetornaTrue(string cnpj)
    {
        Documento.EhCnpjValido(cnpj).Should().BeTrue();
    }

    [Theory]
    [InlineData("11.222.333/0001-00")]
    [InlineData("00000000000000")]
    [InlineData("123")]
    [InlineData("12.ABC.345/01DE-34")]   // alfanumerico com DV errado
    [InlineData("12ABC34501DEAB")]       // DV nao numerico
    public void EhCnpjValido_CnpjIncorreto_RetornaFalse(string cnpj)
    {
        Documento.EhCnpjValido(cnpj).Should().BeFalse();
    }

    [Fact]
    public void ApenasNumeros_RemoveCaracteresNaoNumericos()
    {
        Documento.ApenasNumeros("11.222.333/0001-81").Should().Be("11222333000181");
    }

    [Fact]
    public void Normalizar_PreservaAlfanumericoMaiusculoSemPontuacao()
    {
        Documento.Normalizar("12.abc.345/01de-35").Should().Be("12ABC34501DE35");
    }
}
