namespace Colorado.Aplicacao.Excecoes;

// Falha de validacao de input (FluentValidation). O middleware da API e o controlador base
// do Web traduzem para HTTP 400, expondo a lista de erros.
public class ValidacaoException : Exception
{
    public IReadOnlyCollection<string> Erros { get; }

    public ValidacaoException(IEnumerable<string> erros)
        : base("Dados invalidos.")
    {
        Erros = erros.ToList();
    }
}
