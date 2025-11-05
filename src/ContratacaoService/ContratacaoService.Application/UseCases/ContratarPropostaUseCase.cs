using ContratacaoService.Application.DTOs;
using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Ports;

namespace ContratacaoService.Application.UseCases;

/// <summary>
/// Caso de uso para contratar uma proposta aprovada
/// </summary>
public class ContratarPropostaUseCase
{
    private readonly IContratacaoRepository _contratacaoRepository;
    private readonly IPropostaServiceClient _propostaServiceClient;

    public ContratarPropostaUseCase(
        IContratacaoRepository contratacaoRepository,
        IPropostaServiceClient propostaServiceClient)
    {
        _contratacaoRepository = contratacaoRepository ?? 
            throw new ArgumentNullException(nameof(contratacaoRepository));
        _propostaServiceClient = propostaServiceClient ?? 
            throw new ArgumentNullException(nameof(propostaServiceClient));
    }

    public async Task<ContratacaoResponse> ExecutarAsync(
        ContratarPropostaRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        // Verifica se já existe uma contratação para esta proposta
        var contratacaoExistente = await _contratacaoRepository
            .ExisteContratacaoParaPropostaAsync(request.PropostaId, cancellationToken);

        if (contratacaoExistente)
            throw new InvalidOperationException(
                $"Já existe uma contratação para a proposta {request.PropostaId}");

        // Busca a proposta no PropostaService
        var proposta = await _propostaServiceClient
            .ObterPropostaAsync(request.PropostaId, cancellationToken);

        if (proposta == null)
            throw new InvalidOperationException(
                $"Proposta {request.PropostaId} não encontrada no sistema");

        // Verifica se a proposta está aprovada (Status = 2)
        if (proposta.Status != 2)
            throw new InvalidOperationException(
                "Apenas propostas aprovadas podem ser contratadas");

        // Cria a contratação
        var contratacao = Contratacao.Criar(
            proposta.Id,
            request.DataVigenciaInicio,
            request.DataVigenciaFim,
            proposta.ValorPremio);

        var contratacaoCriada = await _contratacaoRepository
            .CriarAsync(contratacao, cancellationToken);

        return MapearParaResponse(contratacaoCriada);
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

