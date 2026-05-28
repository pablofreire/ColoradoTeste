namespace Colorado.Dominio.Paginacao;

public class ResultadoPaginado<T>
{
    public IEnumerable<T> Itens { get; }
    public int TotalRegistros { get; }
    public int Pagina { get; }
    public int TamanhoPagina { get; }

    public int TotalPaginas => TamanhoPagina > 0
        ? (int)Math.Ceiling(TotalRegistros / (double)TamanhoPagina)
        : 0;

    public ResultadoPaginado(IEnumerable<T> itens, int totalRegistros, int pagina, int tamanhoPagina)
    {
        Itens = itens;
        TotalRegistros = totalRegistros;
        Pagina = pagina;
        TamanhoPagina = tamanhoPagina;
    }
}
