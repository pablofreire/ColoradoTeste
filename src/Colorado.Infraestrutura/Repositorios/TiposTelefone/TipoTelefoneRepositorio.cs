using Colorado.Dominio.Interfaces.TiposTelefone;
using Colorado.Dominio.Entidades;
using Colorado.Dominio.Interfaces;
using Colorado.Infraestrutura.Contexto;
using Microsoft.EntityFrameworkCore;

namespace Colorado.Infraestrutura.Repositorios.TiposTelefone;

public class TipoTelefoneRepositorio : RepositorioBase<TipoTelefone>, ITipoTelefoneRepositorio
{
    public TipoTelefoneRepositorio(ColoradoDbContext contexto) : base(contexto)
    {
    }

    public async Task<bool> ExisteDescricaoAsync(string descricao, int? ignorarCodigo = null)
    {
        var desc = descricao.Trim().ToUpper();

        return await _dbSet.AnyAsync(t => t.DescricaoTipoTelefone == desc
            && (ignorarCodigo == null || t.CodigoTipoTelefone != ignorarCodigo));
    }

    public async Task<bool> PossuiTelefonesVinculadosAsync(int codigoTipoTelefone)
    {
        return await _contexto.Telefones
            .AnyAsync(t => t.CodigoTipoTelefone == codigoTipoTelefone);
    }
}
