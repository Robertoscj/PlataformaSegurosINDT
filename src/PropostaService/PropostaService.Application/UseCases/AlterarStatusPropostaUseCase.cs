using PropostaService.Application.DTOs;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Enums;
using PropostaService.Domain.Events;
using PropostaService.Domain.Ports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PropostaService.Application.UseCases;

/// <summary>
/// Caso de uso para alterar o status de uma proposta
/// </summary>
public class AlterarStatusPropostaUseCase
{
    private readonly IPropostaRepository _repository;
    private readonly IMessagePublisher _messagePublisher;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AlterarStatusPropostaUseCase> _logger;

    public AlterarStatusPropostaUseCase(
        IPropostaRepository repository,
        IMessagePublisher messagePublisher,
        IConfiguration configuration,
        ILogger<AlterarStatusPropostaUseCase> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _messagePublisher = messagePublisher ?? throw new ArgumentNullException(nameof(messagePublisher));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PropostaResponse> ExecutarAsync(
        Guid id,
        StatusProposta novoStatus,
        CancellationToken cancellationToken = default)
    {
        var proposta = await _repository.ObterPorIdAsync(id, cancellationToken);

        if (proposta == null)
            throw new InvalidOperationException($"Proposta com ID {id} não encontrada");

        var statusAnterior = proposta.Status;
        proposta.AlterarStatus(novoStatus);

        await _repository.AtualizarAsync(proposta, cancellationToken);

        // Publicar evento se proposta foi aprovada
        if (novoStatus == StatusProposta.Aprovada && statusAnterior != StatusProposta.Aprovada)
        {
            await PublicarEventoPropostaAprovadaAsync(proposta, cancellationToken);
        }

        return MapearParaResponse(proposta);
    }

    private async Task PublicarEventoPropostaAprovadaAsync(Proposta proposta, CancellationToken cancellationToken)
    {
        try
        {
            var evento = new PropostaAprovadaEvent
            {
                PropostaId = proposta.Id,
                NomeCliente = proposta.NomeCliente,
                CpfCliente = proposta.CpfCliente.Numero,
                TipoSeguro = proposta.TipoSeguro,
                ValorCobertura = proposta.ValorCobertura.Valor,
                ValorPremio = proposta.ValorPremio.Valor,
                DataAprovacao = proposta.DataAtualizacao ?? DateTime.UtcNow
            };

            var topicArn = _configuration["AWS:SNS:PropostaAprovadaTopic"] ?? string.Empty;
            
            await _messagePublisher.PublishAsync(evento, topicArn, cancellationToken);
            
            _logger.LogInformation(
                "Evento PropostaAprovada publicado com sucesso. PropostaId: {PropostaId}",
                proposta.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erro ao publicar evento PropostaAprovada. PropostaId: {PropostaId}",
                proposta.Id);
            // Não propaga a exceção para não impedir a conclusão do caso de uso
        }
    }

    private static PropostaResponse MapearParaResponse(Proposta proposta)
    {
        return new PropostaResponse(
            proposta.Id,
            proposta.NomeCliente,
            proposta.CpfCliente.Numero,
            proposta.TipoSeguro,
            proposta.ValorCobertura.Valor,
            proposta.ValorPremio.Valor,
            proposta.Status,
            proposta.DataCriacao,
            proposta.DataAtualizacao
        );
    }
}

