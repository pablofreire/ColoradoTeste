using Colorado.Dominio.Interfaces.Clientes;
using Colorado.Dominio.Interfaces.TiposTelefone;
using Colorado.Infraestrutura.Repositorios.Clientes;
using Colorado.Infraestrutura.Repositorios.TiposTelefone;
using Colorado.Dominio.Interfaces;
using Colorado.Infraestrutura.Contexto;
using Colorado.Infraestrutura.Repositorios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Colorado.Infraestrutura.DependencyInjection;

public static class InfraestruturaServiceCollectionExtensions
{
    // Centraliza o registro de DbContext, repositorios e unit of work. A camada
    // de apresentacao (API) chama este metodo. O banco e sempre SQL Server.
    public static IServiceCollection AdicionarInfraestrutura(this IServiceCollection servicos,
        string connectionString)
    {
        servicos.AddDbContext<ColoradoDbContext>(opcoes => opcoes.UseSqlServer(connectionString));

        servicos.AddScoped<IClienteRepositorio, ClienteRepositorio>();
        servicos.AddScoped<ITipoTelefoneRepositorio, TipoTelefoneRepositorio>();
        servicos.AddScoped<IUnitOfWork, UnitOfWork>();

        return servicos;
    }
}
