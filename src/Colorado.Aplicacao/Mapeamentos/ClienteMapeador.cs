using Colorado.Aplicacao.Dtos;
using Colorado.Dominio.Entidades;
using Colorado.Dominio.Enums;
using Colorado.Dominio.Excecoes;

namespace Colorado.Aplicacao.Mapeamentos;

public static class ClienteMapeador
{
    public static ClienteDto ParaDto(Cliente cliente)
    {
        return new ClienteDto
        {
            CodigoCliente = cliente.CodigoCliente,
            RazaoSocial = cliente.RazaoSocial,
            NomeFantasia = cliente.NomeFantasia,
            TipoPessoa = cliente.TipoPessoa == TipoPessoa.Fisica ? "F" : "J",
            Documento = cliente.Documento,
            Endereco = cliente.Endereco,
            Complemento = cliente.Complemento,
            Bairro = cliente.Bairro,
            Cidade = cliente.Cidade,
            Cep = cliente.Cep,
            Uf = cliente.Uf,
            DataInsercao = cliente.DataInsercao,
            UsuarioInsercao = cliente.UsuarioInsercao,
            Telefones = cliente.Telefones.Select(TelefoneMapeador.ParaDto).ToList()
        };
    }

    public static TipoPessoa ConverterTipoPessoa(string tipoPessoa)
    {
        return (tipoPessoa?.Trim().ToUpper()) switch
        {
            "F" => TipoPessoa.Fisica,
            "J" => TipoPessoa.Juridica,
            _ => throw new DominioException("Tipo de pessoa invalido. Use 'F' (fisica) ou 'J' (juridica).")
        };
    }
}
