namespace PropostaService.Application.DTOs;

public record CriarPropostaRequest(
    string NomeCliente,
    string CpfCliente,
    string TipoSeguro,
    decimal ValorCobertura,
    decimal ValorPremio
);

