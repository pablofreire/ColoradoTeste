using Colorado.Dominio.Interfaces.Clientes;
using Colorado.Dominio.Interfaces.TiposTelefone;

namespace Colorado.Dominio.Interfaces;

public interface IUnitOfWork
{
    IClienteRepositorio Clientes { get; }
    ITipoTelefoneRepositorio TiposTelefone { get; }

    Task<int> CommitAsync();
}
