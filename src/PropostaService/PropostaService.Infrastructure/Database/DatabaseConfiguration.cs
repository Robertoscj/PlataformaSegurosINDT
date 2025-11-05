namespace PropostaService.Infrastructure.Database;

/// <summary>
/// Configuração de banco de dados
/// </summary>
public class DatabaseConfiguration
{
    /// <summary>
    /// Provedor do banco de dados (PostgreSQL, SqlServer, MySql, InMemory)
    /// </summary>
    public string Provider { get; set; } = "PostgreSQL";

    /// <summary>
    /// String de conexão
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Habilitar migrations automáticas
    /// </summary>
    public bool EnableAutoMigration { get; set; } = false;

    /// <summary>
    /// Habilitar logs detalhados do SQL
    /// </summary>
    public bool EnableSensitiveDataLogging { get; set; } = false;

    /// <summary>
    /// Timeout de comando em segundos
    /// </summary>
    public int CommandTimeout { get; set; } = 30;

    /// <summary>
    /// Número máximo de tentativas em caso de falha
    /// </summary>
    public int MaxRetryCount { get; set; } = 3;

    /// <summary>
    /// Validar configuração
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Provider))
            throw new InvalidOperationException("Provider do banco de dados não configurado");

        if (string.IsNullOrWhiteSpace(ConnectionString) && Provider.ToLower() != "inmemory")
            throw new InvalidOperationException("Connection string não configurada");

        if (CommandTimeout <= 0)
            throw new InvalidOperationException("CommandTimeout deve ser maior que zero");

        if (MaxRetryCount < 0)
            throw new InvalidOperationException("MaxRetryCount deve ser maior ou igual a zero");
    }
}

