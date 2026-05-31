using Colorado.Dominio.Entidades;
using Colorado.Dominio.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Colorado.Infraestrutura.Configuracoes.Clientes;

public class ClienteConfiguracao : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");

        builder.HasKey(c => c.CodigoCliente);
        builder.Property(c => c.CodigoCliente).ValueGeneratedOnAdd();

        builder.Property(c => c.RazaoSocial).IsRequired().HasMaxLength(200);
        builder.Property(c => c.NomeFantasia).IsRequired().HasMaxLength(200);

        // TipoPessoa e gravado como 'F'/'J' (char) conforme o modelo do desafio.
        builder.Property(c => c.TipoPessoa)
            .IsRequired()
            .HasMaxLength(1)
            .HasConversion(
                v => v == TipoPessoa.Fisica ? "F" : "J",
                v => v == "F" ? TipoPessoa.Fisica : TipoPessoa.Juridica);

        builder.Property(c => c.Documento).IsRequired().HasMaxLength(20);
        builder.Property(c => c.Endereco).HasMaxLength(200);
        builder.Property(c => c.Complemento).HasMaxLength(100);
        builder.Property(c => c.Bairro).HasMaxLength(100);
        builder.Property(c => c.Cidade).HasMaxLength(100);
        builder.Property(c => c.Cep).HasMaxLength(9);
        builder.Property(c => c.Uf).HasMaxLength(2);
        builder.Property(c => c.DataInsercao).IsRequired();
        builder.Property(c => c.UsuarioInsercao).HasMaxLength(50);

        builder.HasIndex(c => c.Documento).IsUnique();

        builder.HasMany(c => c.Telefones)
            .WithOne(t => t.Cliente!)
            .HasForeignKey(t => t.CodigoCliente)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata
            .FindNavigation(nameof(Cliente.Telefones))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
