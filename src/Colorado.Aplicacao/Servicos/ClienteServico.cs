using Colorado.Aplicacao.Dtos;
using Colorado.Aplicacao.Excecoes;
using Colorado.Aplicacao.Interfaces;
using Colorado.Aplicacao.Mapeamentos;
using Colorado.Dominio.Entidades;
using Colorado.Dominio.Excecoes;
using Colorado.Dominio.Interfaces;
using Colorado.Dominio.Paginacao;

namespace Colorado.Aplicacao.Servicos;

public class ClienteServico : IClienteServico
{
    private readonly IUnitOfWork _uow;

    public ClienteServico(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<ResultadoPaginado<ClienteDto>> ListarAsync(string? nome, string? documento,
        int pagina, int tamanhoPagina)
    {
        if (pagina < 1) pagina = 1;
        if (tamanhoPagina < 1) tamanhoPagina = 10;

        var resultado = await _uow.Clientes.ListarPaginadoAsync(nome, documento, pagina, tamanhoPagina);
        var itens = resultado.Itens.Select(ClienteMapeador.ParaDto);

        return new ResultadoPaginado<ClienteDto>(itens, resultado.TotalRegistros, pagina, tamanhoPagina);
    }

    public async Task<ClienteDto?> ObterPorIdAsync(int codigoCliente)
    {
        var cliente = await _uow.Clientes.ObterComTelefonesAsync(codigoCliente);
        return cliente is null ? null : ClienteMapeador.ParaDto(cliente);
    }

    public async Task<ClienteDto> CriarAsync(SalvarClienteDto dto, string usuario)
    {
        var tipoPessoa = ClienteMapeador.ConverterTipoPessoa(dto.TipoPessoa);

        if (await _uow.Clientes.ExisteDocumentoAsync(dto.Documento))
            throw new DominioException("Ja existe um cliente cadastrado com este documento.");

        var cliente = new Cliente(dto.RazaoSocial, dto.NomeFantasia, tipoPessoa, dto.Documento,
            dto.Endereco, dto.Complemento, dto.Bairro, dto.Cidade, dto.Cep, dto.Uf, usuario);

        await AdicionarTelefonesAsync(cliente, dto.Telefones, usuario);

        await _uow.Clientes.AdicionarAsync(cliente);
        await _uow.CommitAsync();

        var criado = await _uow.Clientes.ObterComTelefonesAsync(cliente.CodigoCliente);
        return ClienteMapeador.ParaDto(criado!);
    }

    public async Task AtualizarAsync(int codigoCliente, SalvarClienteDto dto, string usuario)
    {
        var cliente = await _uow.Clientes.ObterComTelefonesAsync(codigoCliente);
        if (cliente is null)
            throw new NaoEncontradoException($"Cliente {codigoCliente} nao encontrado.");

        var tipoPessoa = ClienteMapeador.ConverterTipoPessoa(dto.TipoPessoa);

        if (await _uow.Clientes.ExisteDocumentoAsync(dto.Documento, codigoCliente))
            throw new DominioException("Ja existe outro cliente cadastrado com este documento.");

        cliente.AtualizarDados(dto.RazaoSocial, dto.NomeFantasia, tipoPessoa, dto.Documento,
            dto.Endereco, dto.Complemento, dto.Bairro, dto.Cidade, dto.Cep, dto.Uf);

        // Estrategia simples: recriar os telefones a cada atualizacao do cliente.
        cliente.LimparTelefones();
        await AdicionarTelefonesAsync(cliente, dto.Telefones, usuario);

        _uow.Clientes.Atualizar(cliente);
        await _uow.CommitAsync();
    }

    public async Task RemoverAsync(int codigoCliente)
    {
        var cliente = await _uow.Clientes.ObterPorIdAsync(codigoCliente);
        if (cliente is null)
            throw new NaoEncontradoException($"Cliente {codigoCliente} nao encontrado.");

        _uow.Clientes.Remover(cliente);
        await _uow.CommitAsync();
    }

    private async Task AdicionarTelefonesAsync(Cliente cliente, List<SalvarTelefoneDto> telefones, string usuario)
    {
        foreach (var item in telefones)
        {
            var tipo = await _uow.TiposTelefone.ObterPorIdAsync(item.CodigoTipoTelefone);
            if (tipo is null)
                throw new DominioException($"Tipo de telefone {item.CodigoTipoTelefone} nao existe.");

            var telefone = new Telefone(item.NumeroTelefone, item.CodigoTipoTelefone,
                item.Operadora, item.Ativo, usuario);

            cliente.AdicionarTelefone(telefone);
        }
    }
}
