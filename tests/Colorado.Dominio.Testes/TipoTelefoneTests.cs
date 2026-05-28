using Colorado.Dominio.Entidades;
using Colorado.Dominio.Excecoes;
using FluentAssertions;
using Xunit;

namespace Colorado.Dominio.Testes;

public class TipoTelefoneTests
{
    [Fact]
    public void Construtor_Descricao_GuardaEmMaiusculas()
    {
        var tipo = new TipoTelefone("residencial", "teste");

        tipo.DescricaoTipoTelefone.Should().Be("RESIDENCIAL");
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
        var tipo = new TipoTelefone("RESIDENCIAL", "teste");

        tipo.AtualizarDescricao("comercial");

        tipo.DescricaoTipoTelefone.Should().Be("COMERCIAL");
    }
}
