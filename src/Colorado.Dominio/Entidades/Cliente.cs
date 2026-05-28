using Colorado.Dominio.Enums;
using Colorado.Dominio.Excecoes;
using Colorado.Dominio.Validacoes;

namespace Colorado.Dominio.Entidades;

public class Cliente
{
    #region Propriedades

    public int CodigoCliente { get; private set; }
    public string RazaoSocial { get; private set; } = string.Empty;
    public string NomeFantasia { get; private set; } = string.Empty;
    public TipoPessoa TipoPessoa { get; private set; }
    public string Documento { get; private set; } = string.Empty;
    public string Endereco { get; private set; } = string.Empty;
    public string Complemento { get; private set; } = string.Empty;
    public string Bairro { get; private set; } = string.Empty;
    public string Cidade { get; private set; } = string.Empty;
    public string Cep { get; private set; } = string.Empty;
    public string Uf { get; private set; } = string.Empty;
    public DateTime DataInsercao { get; private set; }
    public string UsuarioInsercao { get; private set; } = string.Empty;

    private readonly List<Telefone> _telefones = new();
    public IReadOnlyCollection<Telefone> Telefones => _telefones.AsReadOnly();

    #endregion

    #region Construtores

    protected Cliente() { }

    public Cliente(string razaoSocial, string nomeFantasia, TipoPessoa tipoPessoa,
        string documento, string endereco, string complemento, string bairro,
        string cidade, string cep, string uf, string usuarioInsercao)
    {
        Validar(razaoSocial, nomeFantasia, tipoPessoa, documento, uf);

        RazaoSocial = razaoSocial.Trim();
        NomeFantasia = nomeFantasia.Trim();
        TipoPessoa = tipoPessoa;
        Documento = Validacoes.Documento.ApenasNumeros(documento);
        Endereco = endereco?.Trim() ?? string.Empty;
        Complemento = complemento?.Trim() ?? string.Empty;
        Bairro = bairro?.Trim() ?? string.Empty;
        Cidade = cidade?.Trim() ?? string.Empty;
        Cep = Validacoes.Documento.ApenasNumeros(cep);
        Uf = (uf ?? string.Empty).Trim().ToUpper();
        DataInsercao = DateTime.Now;
        UsuarioInsercao = usuarioInsercao;
    }

    #endregion

    #region Comportamento

    public void AtualizarDados(string razaoSocial, string nomeFantasia, TipoPessoa tipoPessoa,
        string documento, string endereco, string complemento, string bairro,
        string cidade, string cep, string uf)
    {
        Validar(razaoSocial, nomeFantasia, tipoPessoa, documento, uf);

        RazaoSocial = razaoSocial.Trim();
        NomeFantasia = nomeFantasia.Trim();
        TipoPessoa = tipoPessoa;
        Documento = Validacoes.Documento.ApenasNumeros(documento);
        Endereco = endereco?.Trim() ?? string.Empty;
        Complemento = complemento?.Trim() ?? string.Empty;
        Bairro = bairro?.Trim() ?? string.Empty;
        Cidade = cidade?.Trim() ?? string.Empty;
        Cep = Validacoes.Documento.ApenasNumeros(cep);
        Uf = (uf ?? string.Empty).Trim().ToUpper();
    }

    public void AdicionarTelefone(Telefone telefone)
    {
        if (telefone is null)
            throw new DominioException("Telefone invalido.");

        if (_telefones.Any(t => t.NumeroTelefone == telefone.NumeroTelefone))
            throw new DominioException($"O telefone {telefone.NumeroTelefone} ja esta cadastrado para este cliente.");

        telefone.VincularAoCliente(CodigoCliente);
        _telefones.Add(telefone);
    }

    public void LimparTelefones()
    {
        _telefones.Clear();
    }

    private static void Validar(string razaoSocial, string nomeFantasia,
        TipoPessoa tipoPessoa, string documento, string uf)
    {
        if (string.IsNullOrWhiteSpace(razaoSocial))
            throw new DominioException("A razao social e obrigatoria.");

        if (string.IsNullOrWhiteSpace(nomeFantasia))
            throw new DominioException("O nome fantasia e obrigatorio.");

        if (string.IsNullOrWhiteSpace(uf) || uf.Trim().Length != 2)
            throw new DominioException("A UF deve ter 2 caracteres.");

        var numero = Validacoes.Documento.ApenasNumeros(documento);

        if (tipoPessoa == TipoPessoa.Fisica && !Validacoes.Documento.EhCpfValido(numero))
            throw new DominioException("CPF invalido.");

        if (tipoPessoa == TipoPessoa.Juridica && !Validacoes.Documento.EhCnpjValido(numero))
            throw new DominioException("CNPJ invalido.");
    }

    #endregion
}
