using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropostaService.Domain.Entities;
using PropostaService.Domain.ValueObjects;

namespace PropostaService.Infrastructure.Persistence.Configurations;

public class PropostaConfiguration : IEntityTypeConfiguration<Proposta>
{
    public void Configure(EntityTypeBuilder<Proposta> builder)
    {
        builder.ToTable("Propostas");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.NomeCliente)
            .IsRequired()
            .HasMaxLength(200);


        builder.Property(p => p.CpfCliente)
            .HasConversion(
                cpf => cpf.Numero,
                valor => Cpf.Criar(valor))
            .IsRequired()
            .HasMaxLength(11)
            .HasColumnName("CpfCliente");

        builder.Property(p => p.TipoSeguro)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.ValorCobertura)
            .HasConversion(
                valor => valor.Valor,
                v => ValorMonetario.Criar(v))
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasColumnName("ValorCobertura");

        builder.Property(p => p.ValorPremio)
            .HasConversion(
                valor => valor.Valor,
                v => ValorMonetario.Criar(v))
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasColumnName("ValorPremio");

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.DataCriacao)
            .IsRequired();

        builder.Property(p => p.DataAtualizacao);

        // Índices
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.CpfCliente);
        builder.HasIndex(p => p.DataCriacao);
    }
}

