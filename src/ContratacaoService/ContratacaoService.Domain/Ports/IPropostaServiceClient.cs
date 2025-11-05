using ContratacaoService.Domain.Models;

namespace ContratacaoService.Domain.Ports;

/// <summary>
/// Port (interface) para comunicação com o PropostaService
/// Define o contrato para o adapter que fará as chamadas HTTP
/// </summary>
public interface IPropostaServiceClient
{
    Task<PropostaDto?> ObterPropostaAsync(Guid propostaId, CancellationToken cancellationToken = default);
}

