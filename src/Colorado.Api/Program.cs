using System.Reflection;
using Colorado.Aplicacao.DependencyInjection;
using Colorado.Api.Middleware;
using Colorado.Api.Models;
using Colorado.Infraestrutura.Contexto;
using Colorado.Infraestrutura.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

const string politicaCors = "PermitirTudo";
var provider = builder.Configuration["DatabaseProvider"] ?? "Sqlite";
var connectionString = builder.Configuration.GetConnectionString("ConexaoPadrao")
    ?? "Data Source=colorado.db";

builder.Services.AdicionarInfraestrutura(provider, connectionString);
builder.Services.AdicionarAplicacao();

builder.Services.AddControllers();

// Erros de ModelState saem no mesmo envelope das demais respostas.
builder.Services.Configure<ApiBehaviorOptions>(opcoes =>
{
    opcoes.InvalidModelStateResponseFactory = contexto =>
    {
        var erros = contexto.ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .Where(m => !string.IsNullOrWhiteSpace(m))
            .ToList();

        return new BadRequestObjectResult(RespostaApi.Falha("Dados invalidos.", erros));
    };
});

builder.Services.AddCors(opcoes => opcoes.AddPolicy(politicaCors, politica =>
    politica.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opcoes =>
{
    opcoes.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Colorado API",
        Version = "v1",
        Description = "API RESTful de clientes e tipos de telefone - desafio Grupo Colorado."
    });

    var arquivoXml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var caminhoXml = Path.Combine(AppContext.BaseDirectory, arquivoXml);
    if (File.Exists(caminhoXml))
        opcoes.IncludeXmlComments(caminhoXml);
});

var app = builder.Build();

// Prepara o banco no startup (migrations no SQL Server, schema no SQLite) para subir pronto.
using (var escopo = app.Services.CreateScope())
{
    var contexto = escopo.ServiceProvider.GetRequiredService<ColoradoDbContext>();
    InicializadorBanco.Inicializar(contexto, provider);
}

app.UseMiddleware<MiddlewareTratamentoExcecao>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(politicaCors);

app.MapControllers();

app.Run();
