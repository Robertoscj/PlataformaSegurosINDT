using ContratacaoService.Domain.Events;
using ContratacaoService.Domain.Ports;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace ContratacaoService.API.BackgroundServices;

/// <summary>
/// Serviço em background para consumir eventos de proposta aprovada
/// </summary>
public class PropostaAprovadaConsumerService : BackgroundService
{
    private readonly IMessageConsumer _messageConsumer;
    private readonly ILogger<PropostaAprovadaConsumerService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public PropostaAprovadaConsumerService(
        IMessageConsumer messageConsumer,
        ILogger<PropostaAprovadaConsumerService> logger,
        IConfiguration configuration,
        IServiceProvider serviceProvider)
    {
        _messageConsumer = messageConsumer;
        _logger = logger;
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Serviço de consumo de eventos PropostaAprovada iniciado");

        var queueUrl = _configuration["AWS:SQS:PropostaAprovadaQueue"] ?? string.Empty;

        if (string.IsNullOrEmpty(queueUrl))
        {
            _logger.LogWarning("URL da fila SQS não configurada. Consumidor não será iniciado.");
            return;
        }

        try
        {
            await _messageConsumer.StartConsumingAsync<PropostaAprovadaEvent>(
                queueUrl,
                ProcessarPropostaAprovadaAsync,
                stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico no serviço de consumo de eventos");
        }
    }

    private async Task ProcessarPropostaAprovadaAsync(PropostaAprovadaEvent evento)
    {
        _logger.LogInformation(
            "Recebido evento PropostaAprovada. PropostaId: {PropostaId}, Cliente: {NomeCliente}",
            evento.PropostaId,
            evento.NomeCliente);

        try
        {
            // Aqui você pode:
            // 1. Criar automaticamente uma contratação
            // 2. Enviar notificação
            // 3. Processar regras de negócio adicionais
            
            _logger.LogInformation(
                "Evento PropostaAprovada processado com sucesso. PropostaId: {PropostaId}",
                evento.PropostaId);
            
            // Exemplo: Log dos dados
            _logger.LogDebug(
                "Dados da proposta aprovada - Tipo: {TipoSeguro}, Valor Cobertura: {ValorCobertura}, Prêmio: {ValorPremio}",
                evento.TipoSeguro,
                evento.ValorCobertura,
                evento.ValorPremio);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erro ao processar evento PropostaAprovada. PropostaId: {PropostaId}",
                evento.PropostaId);
            throw; // Relança para que a mensagem volte para a fila
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Parando serviço de consumo de eventos PropostaAprovada");
        await _messageConsumer.StopConsumingAsync();
        await base.StopAsync(cancellationToken);
    }
}

