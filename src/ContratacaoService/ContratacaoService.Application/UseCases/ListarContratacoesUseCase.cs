using ContratacaoService.Application.DTOs;
using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Ports;

namespace ContratacaoService.Application.UseCases;

/// <summary>
/// Caso de uso para listar contratações
/// </summary>
public class ListarContratacoesUseCase
{
    private readonly IContratacaoRepository _repository;

    public ListarContratacoesUseCase(IContratacaoRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IEnumerable<ContratacaoResponse>> ExecutarAsync(
        CancellationToken cancellationToken = default)
    {
        var contratacoes = await _repository.ListarAsync(cancellationToken);

        return contratacoes.Select(MapearParaResponse);
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

