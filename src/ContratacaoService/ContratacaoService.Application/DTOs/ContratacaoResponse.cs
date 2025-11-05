namespace ContratacaoService.Application.DTOs;

public record ContratacaoResponse(
    Guid Id,
    Guid PropostaId,
    string NumeroApolice,
    DateTime DataContratacao,
    DateTime DataVigenciaInicio,
    DateTime DataVigenciaFim,
    decimal ValorPremio
);

