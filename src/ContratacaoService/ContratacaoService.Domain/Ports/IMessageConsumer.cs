namespace ContratacaoService.Domain.Ports;

/// <summary>
/// Porta para consumo de mensagens/eventos (Hexagonal Architecture)
/// </summary>
public interface IMessageConsumer
{
    /// <summary>
    /// Inicia o consumo de mensagens de uma fila
    /// </summary>
    Task StartConsumingAsync<T>(string queueUrl, Func<T, Task> handler, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Para o consumo de mensagens
    /// </summary>
    Task StopConsumingAsync();
}

