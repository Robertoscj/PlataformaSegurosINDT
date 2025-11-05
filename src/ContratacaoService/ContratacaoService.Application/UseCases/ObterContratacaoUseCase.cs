using ContratacaoService.Application.DTOs;
using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Ports;

namespace ContratacaoService.Application.UseCases;

/// <summary>
/// Caso de uso para obter uma contratação específica
/// </summary>
public class ObterContratacaoUseCase
{
    private readonly IContratacaoRepository _repository;

    public ObterContratacaoUseCase(IContratacaoRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<ContratacaoResponse?> ExecutarAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var contratacao = await _repository.ObterPorIdAsync(id, cancellationToken);

        return contratacao == null ? null : MapearParaResponse(contratacao);
    }

    private static ContratacaoResponse MapearParaResponse(Contratacao contratacao)
    {
        return new ContratacaoResponse(
            contratacao.Id,
            contratacao.PropostaId,
            contratacao.NumeroApolice,
            contratacao.DataContratacao,
            contratacao.DataVigenciaInicio,
            contratacao.DataVigenciaFim,
            contratacao.ValorPremio
        );
    }
}

