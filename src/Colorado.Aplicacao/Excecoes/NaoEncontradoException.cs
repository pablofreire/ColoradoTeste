namespace Colorado.Aplicacao.Excecoes;

// Sinaliza que um recurso solicitado nao existe. O middleware traduz para HTTP 404.
public class NaoEncontradoException : Exception
{
    public NaoEncontradoException(string mensagem) : base(mensagem)
    {
    }
}
