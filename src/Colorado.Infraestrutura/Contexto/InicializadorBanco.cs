using Microsoft.EntityFrameworkCore;

namespace Colorado.Infraestrutura.Contexto;

public static class InicializadorBanco
{
    // SQL Server aplica migrations (versionado). SQLite cria o schema pelo modelo. Em ambos
    // o seed via HasData entra junto. O retry cobre o caso do banco subir depois da aplicacao
    // no docker-compose.
    public static void Inicializar(ColoradoDbContext contexto, string provider)
    {
        if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
        {
            var tentativas = 0;
            while (true)
            {
                try
                {
                    contexto.Database.Migrate();
                    break;
                }
                catch when (++tentativas < 10)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                }
            }
        }
        else
        {
            contexto.Database.EnsureCreated();
        }
    }
}
