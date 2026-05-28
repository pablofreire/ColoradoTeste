using Colorado.Infraestrutura.Contexto;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Colorado.Infraestrutura.Testes;

// Base para testes de repositorio usando SQLite in-memory real (respeita chaves, FKs e
// traducao de queries), com schema e seed criados via EnsureCreated. Cada classe de teste
// recebe um banco isolado.
public abstract class TesteComBanco : IDisposable
{
    private readonly SqliteConnection _conexao;
    protected readonly ColoradoDbContext Contexto;

    protected TesteComBanco()
    {
        _conexao = new SqliteConnection("DataSource=:memory:");
        _conexao.Open();

        var opcoes = new DbContextOptionsBuilder<ColoradoDbContext>()
            .UseSqlite(_conexao)
            .Options;

        Contexto = new ColoradoDbContext(opcoes);
        Contexto.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Contexto.Dispose();
        _conexao.Dispose();
    }
}
