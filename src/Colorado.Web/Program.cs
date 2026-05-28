using Colorado.Aplicacao.DependencyInjection;
using Colorado.Infraestrutura.Contexto;
using Colorado.Infraestrutura.DependencyInjection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var provider = builder.Configuration["DatabaseProvider"] ?? "Sqlite";
var connectionString = builder.Configuration.GetConnectionString("ConexaoPadrao")
    ?? "Data Source=colorado.db";

// A camada Web consome Aplicacao/Infraestrutura diretamente, sem passar pela API.
builder.Services.AdicionarInfraestrutura(provider, connectionString);
builder.Services.AdicionarAplicacao();

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var escopo = app.Services.CreateScope())
{
    var contexto = escopo.ServiceProvider.GetRequiredService<ColoradoDbContext>();
    InicializadorBanco.Inicializar(contexto, provider);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
