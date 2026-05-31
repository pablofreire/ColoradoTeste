using Colorado.Aplicacao.Mapeamentos;
using Colorado.Aplicacao.Mapeamentos.Clientes;
using Colorado.Aplicacao.Mapeamentos.TiposTelefone;
using Colorado.Dominio.Entidades;
using Colorado.Dominio.Enums;
using Colorado.Dominio.Excecoes;
using FluentAssertions;
using Xunit;

namespace Colorado.Aplicacao.Testes;

public class MapeadorTests
{
    [Fact]
    public void ClienteMapeador_ParaDto_MapeiaCamposETipoPessoa()
    {
        var cliente = new Cliente("Acme Comercio LTDA", "Acme", TipoPessoa.Juridica,
            "11222333000181", "Rua A", "Sala 1", "Centro", "Sao Paulo", "01001000", "SP", "teste");
        cliente.AdicionarTelefone(new Telefone("11999998888", 3, "Vivo", true, "teste"));

        var dto = ClienteMapeador.ParaDto(cliente);

        dto.RazaoSocial.Should().Be("Acme Comercio LTDA");
        dto.TipoPessoa.Should().Be("J");
        dto.Documento.Should().Be("11222333000181");
        dto.Telefones.Should().HaveCount(1);
        dto.Telefones[0].NumeroTelefone.Should().Be("11999998888");
    }

    [Theory]
    [InlineData("F", TipoPessoa.Fisica)]
    [InlineData("J", TipoPessoa.Juridica)]
    public void ClienteMapeador_ConverterTipoPessoa_Valido(string entrada, TipoPessoa esperado)
    {
        ClienteMapeador.ConverterTipoPessoa(entrada).Should().Be(esperado);
    }

    [Fact]
    public void ClienteMapeador_ConverterTipoPessoa_Invalido_Lanca()
    {
        var acao = () => ClienteMapeador.ConverterTipoPessoa("X");

        acao.Should().Throw<DominioException>();
    }

    [Fact]
    public void TipoTelefoneMapeador_ParaDto_MapeiaCampos()
    {
        var tipo = new TipoTelefone("WHATSAPP", "teste");

        var dto = TipoTelefoneMapeador.ParaDto(tipo);

        dto.DescricaoTipoTelefone.Should().Be("WHATSAPP");
        dto.UsuarioInsercao.Should().Be("teste");
    }
}
