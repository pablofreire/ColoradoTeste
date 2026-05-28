using Colorado.Dominio.Excecoes;
using Colorado.Dominio.Validacoes;

namespace Colorado.Dominio.Entidades;

public class Telefone
{
    #region Propriedades

    public int CodigoCliente { get; private set; }
    public string NumeroTelefone { get; private set; } = string.Empty;
    public int CodigoTipoTelefone { get; private set; }
    public string Operadora { get; private set; } = string.Empty;
    public bool Ativo { get; private set; }
    public DateTime DataInsercao { get; private set; }
    public string UsuarioInsercao { get; private set; } = string.Empty;

    public Cliente? Cliente { get; private set; }
    public TipoTelefone? TipoTelefone { get; private set; }

    #endregion

    #region Construtores

    protected Telefone() { }

    public Telefone(string numeroTelefone, int codigoTipoTelefone, string operadora,
        bool ativo, string usuarioInsercao)
    {
        var numero = Documento.ApenasNumeros(numeroTelefone);

        if (string.IsNullOrWhiteSpace(numero))
            throw new DominioException("O numero do telefone e obrigatorio.");

        if (numero.Length < 8 || numero.Length > 11)
            throw new DominioException("O numero do telefone deve ter entre 8 e 11 digitos.");

        if (codigoTipoTelefone <= 0)
            throw new DominioException("O tipo de telefone e obrigatorio.");

        NumeroTelefone = numero;
        CodigoTipoTelefone = codigoTipoTelefone;
        Operadora = operadora ?? string.Empty;
        Ativo = ativo;
        DataInsercao = DateTime.Now;
        UsuarioInsercao = usuarioInsercao;
    }

    #endregion

    #region Comportamento

    internal void VincularAoCliente(int codigoCliente)
    {
        CodigoCliente = codigoCliente;
    }

    #endregion
}
