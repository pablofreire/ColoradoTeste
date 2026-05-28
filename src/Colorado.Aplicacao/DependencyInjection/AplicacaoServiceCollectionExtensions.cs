using Colorado.Aplicacao.Interfaces;
using Colorado.Aplicacao.Servicos;
using Microsoft.Extensions.DependencyInjection;

namespace Colorado.Aplicacao.DependencyInjection;

public static class AplicacaoServiceCollectionExtensions
{
    public static IServiceCollection AdicionarAplicacao(this IServiceCollection servicos)
    {
        servicos.AddScoped<IClienteServico, ClienteServico>();
        servicos.AddScoped<ITipoTelefoneServico, TipoTelefoneServico>();

        return servicos;
    }
}
