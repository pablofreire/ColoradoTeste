using Colorado.Dominio.Entidades;
using Colorado.Dominio.Enums;
using Colorado.Dominio.Excecoes;
using FluentAssertions;
using Xunit;

namespace Colorado.Dominio.Testes;

public class ClienteTests
{
    private static Cliente NovoClienteValido() => new(
        "Acme Comercio LTDA", "Acme", TipoPessoa.Juridica, "11.222.333/0001-81",
        "Rua A", "Sala 1", "Centro", "Sao Paulo", "01001-000", "sp", "teste");

    [Fact]
    public void Construtor_DadosValidos_NormalizaDocumentoCepEUf()
    {
        var cliente = NovoClienteValido();

        cliente.Documento.Should().Be("11222333000181");
        cliente.Cep.Should().Be("01001000");
        cliente.Uf.Should().Be("SP");
        cliente.DataInsercao.Should().NotBe(default);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Construtor_RazaoSocialVazia_LancaDominioException(string razao)
    {
        var acao = () => new Cliente(razao, "Acme", TipoPessoa.Juridica, "11222333000181",
            "", "", "", "", "", "SP", "teste");

        acao.Should().Throw<DominioException>();
    }

    [Fact]
    public void Construtor_CnpjAlfanumerico_PreservaLetras()
    {
        var cliente = new Cliente("Acme", "Acme", TipoPessoa.Juridica, "12.ABC.345/01DE-35",
            "", "", "", "", "", "SP", "teste");

        cliente.Documento.Should().Be("12ABC34501DE35");
    }

    [Fact]
    public void Construtor_CnpjInvalidoParaJuridica_LancaDominioException()
    {
        var acao = () => new Cliente("Acme", "Acme", TipoPessoa.Juridica, "11222333000100",
            "", "", "", "", "", "SP", "teste");

        acao.Should().Throw<DominioException>();
    }

    [Fact]
    public void Construtor_UfComTamanhoInvalido_LancaDominioException()
    {
        var acao = () => new Cliente("Acme", "Acme", TipoPessoa.Juridica, "11222333000181",
            "", "", "", "", "", "SAO", "teste");

        acao.Should().Throw<DominioException>();
    }

    [Fact]
    public void AdicionarTelefone_NumeroDuplicado_LancaDominioException()
    {
        var cliente = NovoClienteValido();
        cliente.AdicionarTelefone(new Telefone("11999998888", 1, "Vivo", true, "teste"));

        var acao = () => cliente.AdicionarTelefone(new Telefone("11999998888", 2, "Tim", true, "teste"));

        acao.Should().Throw<DominioException>();
    }

    [Fact]
    public void AdicionarTelefone_NumeroNovo_IncluiNaColecao()
    {
        var cliente = NovoClienteValido();

        cliente.AdicionarTelefone(new Telefone("11999998888", 1, "Vivo", true, "teste"));

        cliente.Telefones.Should().HaveCount(1);
    }

    [Fact]
    public void LimparTelefones_RemoveTodos()
    {
        var cliente = NovoClienteValido();
        cliente.AdicionarTelefone(new Telefone("11999998888", 1, "Vivo", true, "teste"));

        cliente.LimparTelefones();

        cliente.Telefones.Should().BeEmpty();
    }

    [Fact]
    public void AtualizarDados_AlteraCamposEMantemDocumentoNormalizado()
    {
        var cliente = NovoClienteValido();

        cliente.AtualizarDados("Nova Razao", "Novo Fantasia", TipoPessoa.Juridica,
            "11.444.777/0001-61", "Rua B", "", "Bairro", "Cidade", "20000-000", "rj");

        cliente.RazaoSocial.Should().Be("Nova Razao");
        cliente.Documento.Should().Be("11444777000161");
        cliente.Uf.Should().Be("RJ");
    }
}
