using Colorado.Aplicacao.Dtos;
using Colorado.Dominio.Entidades;

namespace Colorado.Aplicacao.Mapeamentos;

public static class TipoTelefoneMapeador
{
    public static TipoTelefoneDto ParaDto(TipoTelefone tipo)
    {
        return new TipoTelefoneDto
        {
            CodigoTipoTelefone = tipo.CodigoTipoTelefone,
            DescricaoTipoTelefone = tipo.DescricaoTipoTelefone,
            DataInsercao = tipo.DataInsercao,
            UsuarioInsercao = tipo.UsuarioInsercao
        };
    }

    public static IEnumerable<TipoTelefoneDto> ParaDtos(IEnumerable<TipoTelefone> tipos)
    {
        return tipos.Select(ParaDto);
    }
}
