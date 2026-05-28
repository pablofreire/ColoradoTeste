using Colorado.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Colorado.Infraestrutura.Contexto;

public class ColoradoDbContext : DbContext
{
    public ColoradoDbContext(DbContextOptions<ColoradoDbContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Telefone> Telefones => Set<Telefone>();
    public DbSet<TipoTelefone> TiposTelefone => Set<TipoTelefone>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ColoradoDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
