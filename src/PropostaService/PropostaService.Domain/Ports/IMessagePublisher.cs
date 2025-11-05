namespace PropostaService.Domain.Ports;

/// <summary>
/// Porta para publicação de mensagens/eventos (Hexagonal Architecture)
/// </summary>
public interface IMessagePublisher
{
    /// <summary>
    /// Publica uma mensagem/evento
    /// </summary>
    Task PublishAsync<T>(T message, string topic, CancellationToken cancellationToken = default) where T : class;
}

