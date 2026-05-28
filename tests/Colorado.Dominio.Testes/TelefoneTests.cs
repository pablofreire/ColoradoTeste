using Colorado.Dominio.Entidades;
using Colorado.Dominio.Excecoes;
using FluentAssertions;
using Xunit;

namespace Colorado.Dominio.Testes;

public class TelefoneTests
{
    [Fact]
    public void Construtor_DadosValidos_GuardaSomenteDigitos()
    {
        var telefone = new Telefone("(11) 99999-8888", 3, "Vivo", true, "teste");

        telefone.NumeroTelefone.Should().Be("11999998888");
        telefone.CodigoTipoTelefone.Should().Be(3);
        telefone.Ativo.Should().BeTrue();
    }

    [Theory]
    [InlineData("123")]
    [InlineData("")]
    public void Construtor_NumeroForaDoTamanho_LancaDominioException(string numero)
    {
        var acao = () => new Telefone(numero, 1, "Vivo", true, "teste");

        acao.Should().Throw<DominioException>();
    }

    [Fact]
    public void Construtor_TipoTelefoneInvalido_LancaDominioException()
    {
        var acao = () => new Telefone("11999998888", 0, "Vivo", true, "teste");

        acao.Should().Throw<DominioException>();
    }
}
