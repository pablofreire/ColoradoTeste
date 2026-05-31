using Colorado.Dominio.Interfaces.Clientes;
using Colorado.Dominio.Interfaces.TiposTelefone;
using Colorado.Dominio.Interfaces;
using Colorado.Infraestrutura.Contexto;

namespace Colorado.Infraestrutura.Repositorios;

public class UnitOfWork : IUnitOfWork
{
    private readonly ColoradoDbContext _contexto;

    public IClienteRepositorio Clientes { get; }
    public ITipoTelefoneRepositorio TiposTelefone { get; }

    public UnitOfWork(ColoradoDbContext contexto, IClienteRepositorio clientes,
        ITipoTelefoneRepositorio tiposTelefone)
    {
        _contexto = contexto;
        Clientes = clientes;
        TiposTelefone = tiposTelefone;
    }

    public Task<int> CommitAsync()
    {
        return _contexto.SaveChangesAsync();
    }
}
