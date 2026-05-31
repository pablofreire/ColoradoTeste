using Colorado.Dominio.Entidades;
using Colorado.Dominio.Enums;
using Colorado.Infraestrutura.Repositorios;
using Colorado.Infraestrutura.Repositorios.Clientes;
using Colorado.Infraestrutura.Repositorios.TiposTelefone;
using FluentAssertions;
using Xunit;

namespace Colorado.Infraestrutura.Testes;

public class ClienteRepositorioTests : TesteComBanco
{
    private readonly ClienteRepositorio _repo;
    private readonly UnitOfWork _uow;

    public ClienteRepositorioTests()
    {
        _repo = new ClienteRepositorio(Contexto);
        _uow = new UnitOfWork(Contexto, _repo, new TipoTelefoneRepositorio(Contexto));
    }

    private static Cliente NovoCliente(string razao = "Acme Comercio LTDA", string doc = "11222333000181")
        => new(razao, "Acme", TipoPessoa.Juridica, doc, "", "", "", "Sao Paulo", "", "SP", "teste");

    [Fact]
    public void Seed_CriaOsQuatroTiposDeTelefone()
    {
        Contexto.TiposTelefone.Count().Should().Be(4);
    }

    [Fact]
    public async Task ListarPaginadoAsync_FiltroPorNome_CaseInsensitive()
    {
        await _repo.AdicionarAsync(NovoCliente());
        await _uow.CommitAsync();

        var resultado = await _repo.ListarPaginadoAsync("acme", null, 1, 10);

        resultado.TotalRegistros.Should().Be(1);
        resultado.Itens.Should().ContainSingle();
    }

    [Fact]
    public async Task ListarPaginadoAsync_SemCorrespondencia_RetornaVazio()
    {
        await _repo.AdicionarAsync(NovoCliente());
        await _uow.CommitAsync();

        var resultado = await _repo.ListarPaginadoAsync("inexistente", null, 1, 10);

        resultado.TotalRegistros.Should().Be(0);
    }

    [Fact]
    public async Task ObterComTelefonesAsync_CarregaTelefonesETipo()
    {
        var cliente = NovoCliente();
        cliente.AdicionarTelefone(new Telefone("11999998888", 3, "Vivo", true, "teste"));
        await _repo.AdicionarAsync(cliente);
        await _uow.CommitAsync();

        var carregado = await _repo.ObterComTelefonesAsync(cliente.CodigoCliente);

        carregado.Should().NotBeNull();
        carregado!.Telefones.Should().ContainSingle();
        carregado.Telefones.First().TipoTelefone!.DescricaoTipoTelefone.Should().Be("WHATSAPP");
    }

    [Fact]
    public async Task ExisteDocumentoAsync_DocumentoCadastrado_RetornaTrue()
    {
        await _repo.AdicionarAsync(NovoCliente());
        await _uow.CommitAsync();

        (await _repo.ExisteDocumentoAsync("11.222.333/0001-81")).Should().BeTrue();
        (await _repo.ExisteDocumentoAsync("11444777000161")).Should().BeFalse();
    }

    [Fact]
    public async Task ExisteDocumentoAsync_IgnorandoProprioCodigo_RetornaFalse()
    {
        var cliente = NovoCliente();
        await _repo.AdicionarAsync(cliente);
        await _uow.CommitAsync();

        var existe = await _repo.ExisteDocumentoAsync("11222333000181", cliente.CodigoCliente);

        existe.Should().BeFalse();
    }
}
