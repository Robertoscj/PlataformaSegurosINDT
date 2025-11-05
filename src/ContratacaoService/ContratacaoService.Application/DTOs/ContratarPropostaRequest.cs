namespace ContratacaoService.Application.DTOs;

public record ContratarPropostaRequest(
    Guid PropostaId,
    DateTime DataVigenciaInicio,
    DateTime DataVigenciaFim
);

