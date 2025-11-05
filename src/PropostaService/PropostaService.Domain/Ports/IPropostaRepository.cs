using PropostaService.Domain.Entities;
using PropostaService.Domain.Enums;

namespace PropostaService.Domain.Ports;

/// <summary>
/// Port (interface) para o repositório de propostas
/// Define o contrato que deve ser implementado pelos adapters
/// </summary>
public interface IPropostaRepository
{
    Task<Proposta> CriarAsync(Proposta proposta, CancellationToken cancellationToken = default);
    Task<Proposta?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Proposta>> ListarAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Proposta>> ListarPorStatusAsync(StatusProposta status, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Proposta proposta, CancellationToken cancellationToken = default);
    Task<bool> ExisteAsync(Guid id, CancellationToken cancellationToken = default);
}

