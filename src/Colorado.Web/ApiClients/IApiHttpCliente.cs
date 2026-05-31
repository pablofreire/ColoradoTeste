namespace Colorado.Web.ApiClients;

// Transporte HTTP generico para consumir a API. Concentra serializacao, desserializacao do
// envelope e o mapeamento de falhas (HTTP/erro de rede) em um unico ponto, evitando repetir
// esse encanamento em cada cliente de entidade.
public interface IApiHttpCliente
{
    Task<ResultadoApi<T>> GetAsync<T>(string rota);
    Task<ResultadoApi<T>> PostAsync<T>(string rota, object corpo);
    Task<ResultadoApi<object>> PutAsync(string rota, object corpo);
    Task<ResultadoApi<object>> DeleteAsync(string rota);
}
