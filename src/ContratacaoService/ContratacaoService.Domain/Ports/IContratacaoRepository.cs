using ContratacaoService.Domain.Entities;

namespace ContratacaoService.Domain.Ports;

/// <summary>
/// Port (interface) para o repositório de contratações
/// </summary>
public interface IContratacaoRepository
{
    Task<Contratacao> CriarAsync(Contratacao contratacao, CancellationToken cancellationToken = default);
    Task<Contratacao?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Contratacao?> ObterPorPropostaIdAsync(Guid propostaId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Contratacao>> ListarAsync(CancellationToken cancellationToken = default);
    Task<bool> ExisteContratacaoParaPropostaAsync(Guid propostaId, CancellationToken cancellationToken = default);
}

