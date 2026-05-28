namespace Colorado.Dominio.Interfaces;

public interface IUnitOfWork
{
    IClienteRepositorio Clientes { get; }
    ITipoTelefoneRepositorio TiposTelefone { get; }

    Task<int> CommitAsync();
}
