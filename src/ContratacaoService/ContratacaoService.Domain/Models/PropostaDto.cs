namespace ContratacaoService.Domain.Models;

/// <summary>
/// DTO que representa os dados de uma proposta vinda do PropostaService
/// </summary>
public record PropostaDto(
    Guid Id,
    string NomeCliente,
    string CpfCliente,
    string TipoSeguro,
    decimal ValorCobertura,
    decimal ValorPremio,
    int Status,
    DateTime DataCriacao
);

