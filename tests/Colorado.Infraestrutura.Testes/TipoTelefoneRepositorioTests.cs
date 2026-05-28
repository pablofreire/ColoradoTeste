using Colorado.Dominio.Entidades;
using Colorado.Dominio.Enums;
using Colorado.Infraestrutura.Repositorios;
using FluentAssertions;
using Xunit;

namespace Colorado.Infraestrutura.Testes;

public class TipoTelefoneRepositorioTests : TesteComBanco
{
    private readonly TipoTelefoneRepositorio _repo;
    private readonly ClienteRepositorio _clienteRepo;
    private readonly UnitOfWork _uow;

    public TipoTelefoneRepositorioTests()
    {
        _repo = new TipoTelefoneRepositorio(Contexto);
        _clienteRepo = new ClienteRepositorio(Contexto);
        _uow = new UnitOfWork(Contexto, _clienteRepo, _repo);
    }

    [Theory]
    [InlineData("RESIDENCIAL", true)]
    [InlineData("residencial", true)]
    [InlineData("FAX", false)]
    public async Task ExisteDescricaoAsync_ConsideraSeedCaseInsensitive(string descricao, bool esperado)
    {
        (await _repo.ExisteDescricaoAsync(descricao)).Should().Be(esperado);
    }

    [Fact]
    public async Task PossuiTelefonesVinculadosAsync_TipoEmUso_RetornaTrue()
    {
        var cliente = new Cliente("Acme", "Acme", TipoPessoa.Juridica, "11222333000181",
            "", "", "", "", "", "SP", "teste");
        cliente.AdicionarTelefone(new Telefone("1133334444", 2, "Tim", true, "teste"));
        await _clienteRepo.AdicionarAsync(cliente);
        await _uow.CommitAsync();

        (await _repo.PossuiTelefonesVinculadosAsync(2)).Should().BeTrue();
        (await _repo.PossuiTelefonesVinculadosAsync(4)).Should().BeFalse();
    }
}
