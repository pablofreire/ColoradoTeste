using Colorado.Aplicacao.Interfaces.Clientes;
using Colorado.Aplicacao.Interfaces.TiposTelefone;
using Colorado.Aplicacao.Servicos.Clientes;
using Colorado.Aplicacao.Servicos.TiposTelefone;
using Colorado.Aplicacao.Validacoes.Clientes;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Colorado.Aplicacao.DependencyInjection;

public static class AplicacaoServiceCollectionExtensions
{
    public static IServiceCollection AdicionarAplicacao(this IServiceCollection servicos)
    {
        servicos.AddScoped<IClienteServico, ClienteServico>();
        servicos.AddScoped<ITipoTelefoneServico, TipoTelefoneServico>();

        servicos.AddValidatorsFromAssemblyContaining<SalvarClienteValidator>();

        return servicos;
    }
}
