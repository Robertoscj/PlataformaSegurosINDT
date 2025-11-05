using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using ContratacaoService.Domain.Ports;

namespace ContratacaoService.Infrastructure.Messaging;

/// <summary>
/// Adaptador para consumo de mensagens via AWS SQS
/// </summary>
public class SqsMessageConsumer : IMessageConsumer
{
    private readonly IAmazonSQS _sqsClient;
    private readonly ILogger<SqsMessageConsumer> _logger;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _consumingTask;

    public SqsMessageConsumer(
        IAmazonSQS sqsClient,
        ILogger<SqsMessageConsumer> logger)
    {
        _sqsClient = sqsClient;
        _logger = logger;
    }

    public async Task StartConsumingAsync<T>(
        string queueUrl,
        Func<T, Task> handler,
        CancellationToken cancellationToken = default) where T : class
    {
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        
        _consumingTask = Task.Run(async () =>
        {
            _logger.LogInformation("Iniciando consumo de mensagens da fila: {QueueUrl}", queueUrl);

            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    var request = new ReceiveMessageRequest
                    {
                        QueueUrl = queueUrl,
                        MaxNumberOfMessages = 10,
                        WaitTimeSeconds = 20, // Long polling
                        MessageAttributeNames = new List<string> { "All" }
                    };

                    var response = await _sqsClient.ReceiveMessageAsync(request, _cancellationTokenSource.Token);

                    foreach (var message in response.Messages)
                    {
                        try
                        {
                            // Extrair mensagem do SNS se necessário
                            var messageBody = ExtractMessageBody(message.Body);
                            
                            var deserializedMessage = JsonSerializer.Deserialize<T>(messageBody, new JsonSerializerOptions
                            {
                                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                PropertyNameCaseInsensitive = true
                            });

                            if (deserializedMessage != null)
                            {
                                await handler(deserializedMessage);

                                // Deletar mensagem após processamento bem-sucedido
                                await _sqsClient.DeleteMessageAsync(queueUrl, message.ReceiptHandle, _cancellationTokenSource.Token);

                                _logger.LogInformation(
                                    "Mensagem processada e deletada com sucesso. MessageId: {MessageId}",
                                    message.MessageId);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex,
                                "Erro ao processar mensagem. MessageId: {MessageId}",
                                message.MessageId);
                            // Mensagem não será deletada e voltará para a fila após o timeout de visibilidade
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao receber mensagens da fila: {QueueUrl}", queueUrl);
                    await Task.Delay(5000, _cancellationTokenSource.Token); // Retry delay
                }
            }

            _logger.LogInformation("Consumo de mensagens finalizado para a fila: {QueueUrl}", queueUrl);
        }, _cancellationTokenSource.Token);

        await Task.CompletedTask;
    }

    public async Task StopConsumingAsync()
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            
            if (_consumingTask != null)
            {
                await _consumingTask;
            }
        }
    }

    private string ExtractMessageBody(string messageBody)
    {
        try
        {
            // Tentar deserializar como mensagem SNS
            using var doc = JsonDocument.Parse(messageBody);
            if (doc.RootElement.TryGetProperty("Message", out var messageProperty))
            {
                return messageProperty.GetString() ?? messageBody;
            }
        }
        catch
        {
            // Se falhar, retornar o corpo original (mensagem direta do SQS)
        }

        return messageBody;
    }
}

