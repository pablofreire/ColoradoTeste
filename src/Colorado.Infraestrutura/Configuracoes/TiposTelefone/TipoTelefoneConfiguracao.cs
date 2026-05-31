using Colorado.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Colorado.Infraestrutura.Configuracoes.TiposTelefone;

public class TipoTelefoneConfiguracao : IEntityTypeConfiguration<TipoTelefone>
{
    public void Configure(EntityTypeBuilder<TipoTelefone> builder)
    {
        builder.ToTable("TiposTelefone");

        builder.HasKey(t => t.CodigoTipoTelefone);
        builder.Property(t => t.CodigoTipoTelefone).ValueGeneratedOnAdd();

        builder.Property(t => t.DescricaoTipoTelefone).IsRequired().HasMaxLength(50);
        builder.Property(t => t.DataInsercao).IsRequired();
        builder.Property(t => t.UsuarioInsercao).HasMaxLength(50);

        builder.HasIndex(t => t.DescricaoTipoTelefone).IsUnique();

        Semear(builder);
    }

    // Tipos pedidos no enunciado. Data fixa para o seed nao gerar diff a cada migration.
    private static void Semear(EntityTypeBuilder<TipoTelefone> builder)
    {
        var dataSeed = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        builder.HasData(
            new { CodigoTipoTelefone = 1, DescricaoTipoTelefone = "RESIDENCIAL", DataInsercao = dataSeed, UsuarioInsercao = "SISTEMA" },
            new { CodigoTipoTelefone = 2, DescricaoTipoTelefone = "COMERCIAL", DataInsercao = dataSeed, UsuarioInsercao = "SISTEMA" },
            new { CodigoTipoTelefone = 3, DescricaoTipoTelefone = "WHATSAPP", DataInsercao = dataSeed, UsuarioInsercao = "SISTEMA" },
            new { CodigoTipoTelefone = 4, DescricaoTipoTelefone = "CELULAR", DataInsercao = dataSeed, UsuarioInsercao = "SISTEMA" });
    }
}
