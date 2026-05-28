using Colorado.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Colorado.Infraestrutura.Configuracoes;

public class TelefoneConfiguracao : IEntityTypeConfiguration<Telefone>
{
    public void Configure(EntityTypeBuilder<Telefone> builder)
    {
        builder.ToTable("Telefones");

        // Chave composta: um numero e unico dentro de um mesmo cliente.
        builder.HasKey(t => new { t.CodigoCliente, t.NumeroTelefone });

        builder.Property(t => t.NumeroTelefone).IsRequired().HasMaxLength(20);
        builder.Property(t => t.Operadora).HasMaxLength(50);
        builder.Property(t => t.Ativo).IsRequired();
        builder.Property(t => t.DataInsercao).IsRequired();
        builder.Property(t => t.UsuarioInsercao).HasMaxLength(50);

        builder.HasOne(t => t.TipoTelefone)
            .WithMany(tt => tt.Telefones)
            .HasForeignKey(t => t.CodigoTipoTelefone)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
