using Colorado.Dominio.Interfaces;
using Colorado.Infraestrutura.Contexto;
using Colorado.Infraestrutura.Repositorios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Colorado.Infraestrutura.DependencyInjection;

public static class InfraestruturaServiceCollectionExtensions
{
    // Centraliza o registro de DbContext, repositorios e unit of work. As duas
    // camadas de apresentacao (API e Web) chamam este mesmo metodo. O provider e
    // escolhido por configuracao: SQLite no dev local, SQL Server no Docker.
    public static IServiceCollection AdicionarInfraestrutura(this IServiceCollection servicos,
        string provider, string connectionString)
    {
        servicos.AddDbContext<ColoradoDbContext>(opcoes =>
        {
            if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
                opcoes.UseSqlServer(connectionString);
            else
                opcoes.UseSqlite(connectionString);
        });

        servicos.AddScoped<IClienteRepositorio, ClienteRepositorio>();
        servicos.AddScoped<ITipoTelefoneRepositorio, TipoTelefoneRepositorio>();
        servicos.AddScoped<IUnitOfWork, UnitOfWork>();

        return servicos;
    }
}
