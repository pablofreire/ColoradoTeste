namespace Colorado.Dominio.Excecoes;

// Usada para violacoes de regra de negocio. O middleware da API traduz para HTTP 400.
public class DominioException : Exception
{
    public DominioException(string mensagem) : base(mensagem)
    {
    }
}
