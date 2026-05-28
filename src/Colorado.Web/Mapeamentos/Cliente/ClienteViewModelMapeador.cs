using Colorado.Aplicacao.Dtos;
using Colorado.Dominio.Paginacao;
using Colorado.Web.Models.Cliente;

namespace Colorado.Web.Mapeamentos.Cliente;

// Conversao entre os DTOs da Aplicacao e os ViewModels da tela (manual, sem AutoMapper).
public static class ClienteViewModelMapeador
{
    public static ClienteListaViewModel ParaLista(ResultadoPaginado<ClienteDto> resultado,
        string? nome, string? documento)
    {
        return new ClienteListaViewModel
        {
            Nome = nome,
            Documento = documento,
            Pagina = resultado.Pagina,
            TotalPaginas = resultado.TotalPaginas,
            TotalRegistros = resultado.TotalRegistros,
            Itens = resultado.Itens.Select(ParaItem).ToList()
        };
    }

    public static ClienteItemViewModel ParaItem(ClienteDto dto)
    {
        return new ClienteItemViewModel
        {
            Codigo = dto.CodigoCliente,
            RazaoSocial = dto.RazaoSocial,
            NomeFantasia = dto.NomeFantasia,
            TipoPessoaDescricao = dto.TipoPessoa == "F" ? "Fisica" : "Juridica",
            Documento = dto.Documento,
            CidadeUf = string.IsNullOrWhiteSpace(dto.Cidade) ? dto.Uf : $"{dto.Cidade}/{dto.Uf}"
        };
    }

    public static SalvarClienteViewModel ParaFormulario(ClienteDto dto)
    {
        return new SalvarClienteViewModel
        {
            Codigo = dto.CodigoCliente,
            RazaoSocial = dto.RazaoSocial,
            NomeFantasia = dto.NomeFantasia,
            TipoPessoa = dto.TipoPessoa,
            Documento = dto.Documento,
            Endereco = dto.Endereco,
            Complemento = dto.Complemento,
            Bairro = dto.Bairro,
            Cidade = dto.Cidade,
            Cep = dto.Cep,
            Uf = dto.Uf,
            Telefones = dto.Telefones.Select(t => new SalvarTelefoneViewModel
            {
                NumeroTelefone = t.NumeroTelefone,
                CodigoTipoTelefone = t.CodigoTipoTelefone,
                Operadora = t.Operadora,
                Ativo = t.Ativo
            }).ToList()
        };
    }

    public static SalvarClienteDto ParaDto(SalvarClienteViewModel vm)
    {
        return new SalvarClienteDto
        {
            RazaoSocial = vm.RazaoSocial,
            NomeFantasia = vm.NomeFantasia,
            TipoPessoa = vm.TipoPessoa,
            Documento = vm.Documento,
            Endereco = vm.Endereco,
            Complemento = vm.Complemento,
            Bairro = vm.Bairro,
            Cidade = vm.Cidade,
            Cep = vm.Cep,
            Uf = vm.Uf,
            Telefones = (vm.Telefones ?? new()).Select(t => new SalvarTelefoneDto
            {
                NumeroTelefone = t.NumeroTelefone,
                CodigoTipoTelefone = t.CodigoTipoTelefone,
                Operadora = t.Operadora,
                Ativo = t.Ativo
            }).ToList()
        };
    }

    public static List<TipoTelefoneOpcaoViewModel> ParaOpcoes(IEnumerable<TipoTelefoneDto> tipos)
    {
        return tipos
            .Select(t => new TipoTelefoneOpcaoViewModel { Codigo = t.CodigoTipoTelefone, Descricao = t.DescricaoTipoTelefone })
            .ToList();
    }
}
