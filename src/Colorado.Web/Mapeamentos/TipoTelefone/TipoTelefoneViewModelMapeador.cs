using Colorado.Aplicacao.Dtos;
using Colorado.Web.Models.TipoTelefone;

namespace Colorado.Web.Mapeamentos.TipoTelefone;

public static class TipoTelefoneViewModelMapeador
{
    public static TipoTelefoneListaViewModel ParaLista(IEnumerable<TipoTelefoneDto> tipos)
    {
        return new TipoTelefoneListaViewModel
        {
            Itens = tipos
                .Select(t => new TipoTelefoneItemViewModel { Codigo = t.CodigoTipoTelefone, Descricao = t.DescricaoTipoTelefone })
                .ToList()
        };
    }

    public static SalvarTipoTelefoneViewModel ParaFormulario(TipoTelefoneDto dto)
    {
        return new SalvarTipoTelefoneViewModel
        {
            Codigo = dto.CodigoTipoTelefone,
            Descricao = dto.DescricaoTipoTelefone
        };
    }

    public static SalvarTipoTelefoneDto ParaDto(SalvarTipoTelefoneViewModel vm)
    {
        return new SalvarTipoTelefoneDto { DescricaoTipoTelefone = vm.Descricao };
    }
}
