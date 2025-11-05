using ContratacaoService.Domain.Models;

namespace ContratacaoService.Domain.Ports;

public interface IPropostaServiceClient
{
    Task<PropostaDto?> ObterPropostaAsync(Guid propostaId, CancellationToken cancellationToken = default);
}

