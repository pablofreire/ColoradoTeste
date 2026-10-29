using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Colorado.Infraestrutura.Contexto;

// Usada apenas em tempo de design pelo dotnet-ef (migrations). As migrations sao geradas
// para SQL Server (provider primario). Em runtime o contexto e configurado via injecao de
// dependencia nas camadas de apresentacao, respeitando a configuracao DatabaseProvider.
// A connection string vem da variavel de ambiente ConnectionStrings__ConexaoPadrao; sem ela,
// "migrations add" funciona (nao conecta), mas "database update" exige a variavel.
public class ColoradoDbContextFactory : IDesignTimeDbContextFactory<ColoradoDbContext>
{
    private const string ConexaoSemCredenciais =
        "Server=localhost,1434;Database=ColoradoDb;Integrated Security=false;TrustServerCertificate=True;";

    public ColoradoDbContext CreateDbContext(string[] args)
    {
        var conexao = Environment.GetEnvironmentVariable("ConnectionStrings__ConexaoPadrao")
            ?? ConexaoSemCredenciais;

        var opcoes = new DbContextOptionsBuilder<ColoradoDbContext>()
            .UseSqlServer(conexao)
            .Options;

        return new ColoradoDbContext(opcoes);
    }
}
