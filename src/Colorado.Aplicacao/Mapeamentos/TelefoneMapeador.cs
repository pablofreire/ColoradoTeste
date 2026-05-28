using Colorado.Aplicacao.Dtos;
using Colorado.Dominio.Entidades;

namespace Colorado.Aplicacao.Mapeamentos;

public static class TelefoneMapeador
{
    public static TelefoneDto ParaDto(Telefone telefone)
    {
        return new TelefoneDto
        {
            NumeroTelefone = telefone.NumeroTelefone,
            CodigoTipoTelefone = telefone.CodigoTipoTelefone,
            DescricaoTipoTelefone = telefone.TipoTelefone?.DescricaoTipoTelefone ?? string.Empty,
            Operadora = telefone.Operadora,
            Ativo = telefone.Ativo
        };
    }
}
