using Colorado.Web.ApiClients;
using Colorado.Web.ApiClients.Clientes;
using Colorado.Web.ApiClients.TiposTelefone;

var builder = WebApplication.CreateBuilder(args);

// A camada Web e um frontend puro: consome a API REST por HTTP, sem acesso direto ao banco.
var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
    ?? throw new InvalidOperationException("Configuracao 'ApiBaseUrl' nao definida.");

builder.Services.AddHttpClient<IApiHttpCliente, ApiHttpCliente>(cliente =>
    cliente.BaseAddress = new Uri(apiBaseUrl));

builder.Services.AddScoped<IClienteApiClient, ClienteApiClient>();
builder.Services.AddScoped<ITipoTelefoneApiClient, TipoTelefoneApiClient>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

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
