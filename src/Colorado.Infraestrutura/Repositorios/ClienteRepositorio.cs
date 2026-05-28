using Colorado.Dominio.Entidades;
using Colorado.Dominio.Interfaces;
using Colorado.Dominio.Paginacao;
using Colorado.Dominio.Validacoes;
using Colorado.Infraestrutura.Contexto;
using Microsoft.EntityFrameworkCore;

namespace Colorado.Infraestrutura.Repositorios;

public class ClienteRepositorio : RepositorioBase<Cliente>, IClienteRepositorio
{
    public ClienteRepositorio(ColoradoDbContext contexto) : base(contexto)
    {
    }

    public async Task<ResultadoPaginado<Cliente>> ListarPaginadoAsync(string? nome,
        string? documento, int pagina, int tamanhoPagina)
    {
        var consulta = _dbSet.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(nome))
        {
            var termo = nome.Trim().ToLower();
            consulta = consulta.Where(c => c.RazaoSocial.ToLower().Contains(termo)
                || c.NomeFantasia.ToLower().Contains(termo));
        }

        if (!string.IsNullOrWhiteSpace(documento))
        {
            var doc = Documento.ApenasNumeros(documento);
            consulta = consulta.Where(c => c.Documento.Contains(doc));
        }

        var total = await consulta.CountAsync();

        var itens = await consulta
            .OrderBy(c => c.RazaoSocial)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return new ResultadoPaginado<Cliente>(itens, total, pagina, tamanhoPagina);
    }

    public async Task<Cliente?> ObterComTelefonesAsync(int codigoCliente)
    {
        return await _dbSet
            .Include(c => c.Telefones)
            .ThenInclude(t => t.TipoTelefone)
            .FirstOrDefaultAsync(c => c.CodigoCliente == codigoCliente);
    }

    public async Task<bool> ExisteDocumentoAsync(string documento, int? ignorarCodigoCliente = null)
    {
        var doc = Documento.ApenasNumeros(documento);

        return await _dbSet.AnyAsync(c => c.Documento == doc
            && (ignorarCodigoCliente == null || c.CodigoCliente != ignorarCodigoCliente));
    }
}
