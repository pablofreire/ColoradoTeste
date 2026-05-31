using Colorado.Dominio.Entidades;
using Colorado.Dominio.Paginacao;

namespace Colorado.Dominio.Interfaces.Clientes;

public interface IClienteRepositorio : IRepositorio<Cliente>
{
    Task<ResultadoPaginado<Cliente>> ListarPaginadoAsync(string? nome, string? documento,
        int pagina, int tamanhoPagina);

    Task<Cliente?> ObterComTelefonesAsync(int codigoCliente);

    Task<bool> ExisteDocumentoAsync(string documento, int? ignorarCodigoCliente = null);
}
