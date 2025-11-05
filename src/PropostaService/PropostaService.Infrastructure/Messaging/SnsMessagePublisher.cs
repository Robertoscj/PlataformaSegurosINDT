using System.Text.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Logging;
using PropostaService.Domain.Ports;

namespace PropostaService.Infrastructure.Messaging;

/// <summary>
/// Adaptador para publicação de mensagens via AWS SNS
/// </summary>
public class SnsMessagePublisher : IMessagePublisher
{
    private readonly IAmazonSimpleNotificationService _snsClient;
    private readonly ILogger<SnsMessagePublisher> _logger;

    public SnsMessagePublisher(
        IAmazonSimpleNotificationService snsClient,
        ILogger<SnsMessagePublisher> logger)
    {
        _snsClient = snsClient;
        _logger = logger;
    }

    public async Task PublishAsync<T>(T message, string topicArn, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var messageJson = JsonSerializer.Serialize(message, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var request = new PublishRequest
            {
                TopicArn = topicArn,
                Message = messageJson,
                MessageAttributes = new Dictionary<string, MessageAttributeValue>
                {
                    {
                        "MessageType",
                        new MessageAttributeValue
                        {
                            DataType = "String",
                            StringValue = typeof(T).Name
                        }
                    }
                }
            };

            var response = await _snsClient.PublishAsync(request, cancellationToken);

            _logger.LogInformation(
                "Mensagem publicada com sucesso. MessageId: {MessageId}, Topic: {TopicArn}",
                response.MessageId,
                topicArn);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erro ao publicar mensagem no SNS. Topic: {TopicArn}, MessageType: {MessageType}",
                topicArn,
                typeof(T).Name);
            throw;
        }
    }
}

