using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Colorado.Infraestrutura.Contexto;

// Usada apenas em tempo de design pelo dotnet-ef (migrations). As migrations sao geradas
// para SQL Server (provider primario). Em runtime o contexto e configurado via injecao de
// dependencia nas camadas de apresentacao, respeitando a configuracao DatabaseProvider.
public class ColoradoDbContextFactory : IDesignTimeDbContextFactory<ColoradoDbContext>
{
    public ColoradoDbContext CreateDbContext(string[] args)
    {
        var opcoes = new DbContextOptionsBuilder<ColoradoDbContext>()
            .UseSqlServer("Server=localhost,1433;Database=ColoradoDb;User Id=sa;Password=Colorado@123;TrustServerCertificate=True;")
            .Options;

        return new ColoradoDbContext(opcoes);
    }
}
