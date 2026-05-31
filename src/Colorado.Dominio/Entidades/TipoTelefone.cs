using Colorado.Dominio.Excecoes;

namespace Colorado.Dominio.Entidades;

public class TipoTelefone
{
    #region Propriedades

    public int CodigoTipoTelefone { get; private set; }
    public string DescricaoTipoTelefone { get; private set; } = string.Empty;
    public DateTime DataInsercao { get; private set; }
    public string UsuarioInsercao { get; private set; } = string.Empty;

    public ICollection<Telefone> Telefones { get; private set; } = new List<Telefone>();

    #endregion

    #region Construtores

    protected TipoTelefone() { }

    public TipoTelefone(string descricao, string usuarioInsercao)
    {
        DefinirDescricao(descricao);
        DataInsercao = DateTime.Now;
        UsuarioInsercao = usuarioInsercao;
    }

    #endregion

    #region Comportamento

    public void AtualizarDescricao(string descricao)
    {
        DefinirDescricao(descricao);
    }

    private void DefinirDescricao(string descricao)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new DominioException("A descricao do tipo de telefone e obrigatoria.");

        if (descricao.Length > 50)
            throw new DominioException("A descricao do tipo de telefone deve ter no maximo 50 caracteres.");

        DescricaoTipoTelefone = descricao.Trim();
    }

    #endregion
}
