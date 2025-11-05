using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ContratacaoService.Domain.Entities;

namespace ContratacaoService.Infrastructure.Persistence.Configurations;

public class ContratacaoConfiguration : IEntityTypeConfiguration<Contratacao>
{
    public void Configure(EntityTypeBuilder<Contratacao> builder)
    {
        builder.ToTable("Contratacoes");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.PropostaId)
            .IsRequired();

        builder.Property(c => c.NumeroApolice)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.DataContratacao)
            .IsRequired();

        builder.Property(c => c.DataVigenciaInicio)
            .IsRequired();

        builder.Property(c => c.DataVigenciaFim)
            .IsRequired();

        builder.Property(c => c.ValorPremio)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        // Índices
        builder.HasIndex(c => c.PropostaId)
            .IsUnique();

        builder.HasIndex(c => c.NumeroApolice)
            .IsUnique();

        builder.HasIndex(c => c.DataContratacao);
    }
}

