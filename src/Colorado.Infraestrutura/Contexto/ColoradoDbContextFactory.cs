using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Colorado.Infraestrutura.Contexto;

// Usada apenas em tempo de design pelo dotnet-ef (migrations). Em runtime o
// contexto e configurado via injecao de dependencia nas camadas de apresentacao.
public class ColoradoDbContextFactory : IDesignTimeDbContextFactory<ColoradoDbContext>
{
    public ColoradoDbContext CreateDbContext(string[] args)
    {
        var opcoes = new DbContextOptionsBuilder<ColoradoDbContext>()
            .UseSqlite("Data Source=colorado.db")
            .Options;

        return new ColoradoDbContext(opcoes);
    }
}
