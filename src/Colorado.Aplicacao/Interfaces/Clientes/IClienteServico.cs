using Colorado.Aplicacao.Dtos.Clientes;
using Colorado.Aplicacao.Dtos;
using Colorado.Dominio.Paginacao;

namespace Colorado.Aplicacao.Interfaces.Clientes;

public interface IClienteServico
{
    Task<ResultadoPaginado<ClienteDto>> ListarAsync(string? nome, string? documento,
        int pagina, int tamanhoPagina);

    Task<ClienteDto?> ObterPorIdAsync(int codigoCliente);

    Task<ClienteDto> CriarAsync(SalvarClienteDto dto, string usuario);

    Task AtualizarAsync(int codigoCliente, SalvarClienteDto dto, string usuario);

    Task RemoverAsync(int codigoCliente);
}
