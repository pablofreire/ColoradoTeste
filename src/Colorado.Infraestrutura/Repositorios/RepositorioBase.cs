using Colorado.Dominio.Interfaces;
using Colorado.Infraestrutura.Contexto;
using Microsoft.EntityFrameworkCore;

namespace Colorado.Infraestrutura.Repositorios;

public abstract class RepositorioBase<T> : IRepositorio<T> where T : class
{
    protected readonly ColoradoDbContext _contexto;
    protected readonly DbSet<T> _dbSet;

    protected RepositorioBase(ColoradoDbContext contexto)
    {
        _contexto = contexto;
        _dbSet = contexto.Set<T>();
    }

    public virtual async Task<T?> ObterPorIdAsync(params object[] chave)
    {
        return await _dbSet.FindAsync(chave);
    }

    public virtual async Task<IEnumerable<T>> ListarAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public virtual async Task AdicionarAsync(T entidade)
    {
        await _dbSet.AddAsync(entidade);
    }

    public virtual void Atualizar(T entidade)
    {
        _dbSet.Update(entidade);
    }

    public virtual void Remover(T entidade)
    {
        _dbSet.Remove(entidade);
    }
}
