using Microsoft.EntityFrameworkCore;

namespace PropostaService.Infrastructure.Database;

/// <summary>
/// Interface para abstração de provedores de banco de dados
/// Permite trocar facilmente entre PostgreSQL, SQL Server, MySQL, etc.
/// </summary>
public interface IDatabaseProvider
{
    /// <summary>
    /// Nome do provedor (PostgreSQL, SqlServer, MySql, etc.)
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// Configura o DbContext com o provedor específico
    /// </summary>
    void Configure(DbContextOptionsBuilder options, string connectionString);

    /// <summary>
    /// Verifica se o provedor está disponível
    /// </summary>
    bool IsAvailable();

    /// <summary>
    /// Retorna o tipo SQL específico para decimal/money
    /// </summary>
    string GetDecimalColumnType();

    /// <summary>
    /// Retorna o tipo SQL específico para strings longas
    /// </summary>
    string GetTextColumnType();
}

