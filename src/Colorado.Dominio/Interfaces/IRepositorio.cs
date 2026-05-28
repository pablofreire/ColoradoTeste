namespace Colorado.Dominio.Interfaces;

public interface IRepositorio<T> where T : class
{
    Task<T?> ObterPorIdAsync(params object[] chave);
    Task<IEnumerable<T>> ListarAsync();
    Task AdicionarAsync(T entidade);
    void Atualizar(T entidade);
    void Remover(T entidade);
}
