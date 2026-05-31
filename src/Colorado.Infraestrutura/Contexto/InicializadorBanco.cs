using Microsoft.EntityFrameworkCore;

namespace Colorado.Infraestrutura.Contexto;

public static class InicializadorBanco
{
    // Aplica as migrations do SQL Server (versionado); o seed via HasData entra junto. O retry
    // cobre o caso do banco subir depois da aplicacao no docker-compose.
    public static void Inicializar(ColoradoDbContext contexto)
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
}
