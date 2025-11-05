namespace ContratacaoService.Domain.Models;


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

