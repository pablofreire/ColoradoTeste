using Colorado.Dominio.Entidades;
using Colorado.Dominio.Excecoes;
using FluentAssertions;
using Xunit;

namespace Colorado.Dominio.Testes;

public class TipoTelefoneTests
{
    [Fact]
    public void Construtor_Descricao_PreservaCaixaEAplicaTrim()
    {
        var tipo = new TipoTelefone("  Residencial  ", "teste");

        tipo.DescricaoTipoTelefone.Should().Be("Residencial");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Construtor_DescricaoVazia_LancaDominioException(string descricao)
    {
        var acao = () => new TipoTelefone(descricao, "teste");

        acao.Should().Throw<DominioException>();
    }

    [Fact]
    public void AtualizarDescricao_AlteraValor()
    {
        var tipo = new TipoTelefone("Residencial", "teste");

        tipo.AtualizarDescricao("Comercial");

        tipo.DescricaoTipoTelefone.Should().Be("Comercial");
    }
}
